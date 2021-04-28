using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.Support;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit._2.Walkers
{
    public class Empty { }
    class Nothing { }
    class TypeVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly ConcurrentIndexed<IVerifiableType, System.Type> typeCache;
        public readonly ConcurrentIndexed<IObjectDefiniton, System.Type> objectCache;
        private readonly ModuleBuilder moduleBuilder;

        private readonly ConcurrentLinkedList<Action> actions = new ConcurrentLinkedList<Action>();

        public TypeVisitor(ConcurrentIndexed<IVerifiableType, System.Type> typeCache, ConcurrentIndexed<IObjectDefiniton, System.Type> objectCache, ModuleBuilder moduleBuilder)
        {
            this.typeCache = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
            this.objectCache = objectCache ?? throw new ArgumentNullException(nameof(objectCache));
            this.moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
        }

        public void CreateTypeProperties() {
            foreach (var action in actions)
            {
                action();
            }
        }

        private void HandleLines(IEnumerable<ICodeElement> lines)
        {
            foreach (var line in lines)
            {
                line.Convert(this);
            } 
        }

        private Nothing HandleOp(IOperation operation) {
            foreach (var line in operation.Operands)
            {
                line.Convert(this);
            }

            HandleType(operation.Returns());
            return new Nothing();
        }

        private void HandleScope(IFinalizedScope scope)
        {
            foreach (var member in scope.Members) {
                member.Value.Value.Convert(this);
            }
        }

        private System.Type HandleType(IVerifiableType verifiableType)
        {
            return typeCache.GetOrAdd(verifiableType, () => InnerMapType(verifiableType));
        }

        private string GetTypeName() => "_" + Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "");

        private System.Type InnerMapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return typeof(double);
            }
            else if (verifiableType is IBooleanType)
            {
                return typeof(bool);
            }
            else if (verifiableType is IStringType)
            {
                return typeof(string);
            }
            else if (verifiableType is IBlockType)
            {
                throw new NotImplementedException();
                // ??
                //return typeof(Action);
            }
            else if (verifiableType is IEmptyType)
            {
                return typeof(Empty);
            }
            else if (verifiableType is IAnyType)
            {
                return typeof(object);
            }
            else if (verifiableType.SafeIs(out IInterfaceModuleType moduleType))
            {
                return JitInterface(moduleType.Members);
            }
            else if (verifiableType is IMethodType method)
            {
                var inputType =  typeCache.GetOrAdd(method.InputType, () => InnerMapType(method.InputType));
                var outputType = typeCache.GetOrAdd(method.OutputType, () => InnerMapType(method.OutputType));
                return typeof(Func<,>).MakeGenericType(inputType, outputType);
            }
            else if (verifiableType.SafeIs(out IReferanceType memberReferance))
            {
                throw new NotImplementedException();
                // I have to fresh up on what this means....
                // I think it is ref<T> 
                // used on the target of assignment 
                //return HandleType(memberReferance.MemberDefinition.Type);
            }
            else if (verifiableType is ITypeOr typeOr)
            {
                // we try to find the intersection of the types
                return MergeTypes(typeOr.Left, typeOr.Right, typeOr);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private System.Type JitInterface(IReadOnlyList<IMemberDefinition> members) {
            var res =  moduleBuilder.DefineType(GetTypeName(), TypeAttributes.Public | TypeAttributes.Interface);

            actions.Add(() =>
            {
                foreach (var member in members)
                {
                    res.DefineProperty(member.Key.CastTo<NameKey>().Name, PropertyAttributes.None, typeCache.GetOrThrow( member.Type), null);
                }
            });

            return res;
        }

        private System.Type MergeTypes(IVerifiableType left, IVerifiableType right,ITypeOr typeOr)
        {
            typeCache.GetOrAdd(left, () => InnerMapType(left)); ;
            typeCache.GetOrAdd(right, () => InnerMapType(right));

            //var leftType = InnerMapType(left); ;
            //var rightType = InnerMapType(right);

            //// if they are the same we are happy
            //if (leftType == rightType)
            //{
            //    return leftType;
            //}

            // if either is an any... then the or can't be anything interesting
            if (left.SafeIs(out IAnyType _) || right.SafeIs(out IAnyType _))
            {
                return typeof(object);
            }

            // if either is a primitive type... return empty?
            if (left.SafeIs(out IPrimitiveType _) || right.SafeIs(out IPrimitiveType _))
            {
                return typeof(object);
            }

            // if they are both methods 
            // we have re merge the method io
            if (typeOr.TryGetInput().Is(out var input) &&
                typeOr.TryGetReturn().Is(out var output))
            {
                return typeof(Func<,>).MakeGenericType(InnerMapType(input), InnerMapType(output));
            }

            if (left.SafeIs(out IInterfaceModuleType _) && right.SafeIs(out IInterfaceModuleType _)) {
                // JIT a interface
                return JitInterface(typeOr.Members);
            }

            // can it have members if both are neither an interface or a module
            //if (typeOr.Members.Any())
            //{
            //    return typeof(ITacObject);
            //}

            // if it is a method and something with members...
            if (HasMember(left) && right.TryGetInput().Is(out var _) && right.TryGetReturn().Is(out var _))
            {
                return typeof(object);
            }
            if (HasMember(right) && left.TryGetInput().Is(out var _) && left.TryGetReturn().Is(out var _))
            {
                return typeof(object);
            }

            throw new Exception("what case did I miis");
        }

        private static bool HasMember(IVerifiableType type) {
            if (type.SafeIs(out IInterfaceModuleType interfaceModuleType) && interfaceModuleType.Members.Any()) {
                return true;
            }
            if (type.SafeIs(out ITypeOr typeOr) && typeOr.Members.Any()) {
                return true;
            }
            return false;
        }

        public Nothing AddOperation(IAddOperation co) =>HandleOp(co);
        public Nothing AssignOperation(IAssignOperation co) => HandleOp(co);
        public Nothing ElseOperation(IElseOperation co) => HandleOp(co);
        public Nothing IfTrueOperation(IIfOperation co) => HandleOp(co);
        public Nothing LastCallOperation(ILastCallOperation co) => HandleOp(co);
        public Nothing LessThanOperation(ILessThanOperation co) => HandleOp(co);
        public Nothing MultiplyOperation(IMultiplyOperation co) => HandleOp(co);
        public Nothing NextCallOperation(INextCallOperation co) => HandleOp(co);
        public Nothing PathOperation(IPathOperation co) => HandleOp(co);
        public Nothing ReturnOperation(IReturnOperation co) => HandleOp(co);
        public Nothing SubtractOperation(ISubtractOperation co) => HandleOp(co);
        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation) {
            return HandleOp(tryAssignOperation);
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            HandleLines(codeElement.Body);
            HandleLines(codeElement.StaticInitailizers);
            HandleScope(codeElement.Scope);
            return new Nothing();
        }

        public Nothing ConstantBool(IConstantBool constantBool) { HandleType(constantBool.Returns()); return new Nothing(); }
        public Nothing ConstantNumber(IConstantNumber codeElement) { HandleType(codeElement.Returns()); return new Nothing(); }
        public Nothing ConstantString(IConstantString co) { HandleType(co.Returns()); return new Nothing(); }
        public Nothing EmptyInstance(IEmptyInstance co) { HandleType(co.Returns()); return new Nothing(); }

        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            HandleLines(entryPointDefinition.Body);
            HandleLines(entryPointDefinition.StaticInitailizers);
            HandleScope(entryPointDefinition.Scope);
            entryPointDefinition.ParameterDefinition.Convert(this);
            HandleType(entryPointDefinition.OutputType);
            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            HandleLines(codeElement.MethodBody);
            HandleLines(codeElement.StaticInitialzers);
            HandleScope(codeElement.IntermediateScope);
            HandleScope(codeElement.Scope);
            codeElement.ContextDefinition.Convert(this);
            codeElement.ParameterDefinition.Convert(this);
            HandleType(codeElement.OutputType);
            return new Nothing();
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            HandleType(codeElement.Type);
            return new Nothing();
        }

        public Nothing MemberReferance(IMemberReference codeElement)
        {
            codeElement.MemberDefinition.Convert(this);
            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition co)
        {
            HandleLines(co.Body);
            HandleLines(co.StaticInitailizers);
            HandleScope(co.Scope);
            co.ParameterDefinition.Convert(this);
            HandleType(co.InputType);
            HandleType(co.OutputType);
            HandleType(co.Returns());
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            HandleLines(codeElement.Assignments);
            HandleScope(codeElement.Scope);
            var interfactType = HandleType(codeElement.Returns());
            objectCache.GetOrAdd(codeElement, () =>
            {
                var myConcreteType = moduleBuilder.DefineType(GetTypeName(), TypeAttributes.Public | TypeAttributes.Interface);

                actions.Add(() =>
                {

                    myConcreteType.AddInterfaceImplementation(interfactType);

                    foreach (var propertyInfo in interfactType.GetProperties())
                    {
                        var field = myConcreteType.DefineField("_" + propertyInfo.Name.ToLower(), propertyInfo.PropertyType, FieldAttributes.Public);
                        var property = myConcreteType.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new System.Type[0]);

                        var getter = myConcreteType.DefineMethod("get_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, propertyInfo.PropertyType, new System.Type[0]);
                        var getGenerator = getter.GetILGenerator();
                        getGenerator.Emit(OpCodes.Ldarg_0);
                        getGenerator.Emit(OpCodes.Ldfld, field);
                        getGenerator.Emit(OpCodes.Ret);
                        property.SetGetMethod(getter);
                        myConcreteType.DefineMethodOverride(getter, propertyInfo.GetGetMethod());

                        var setter = myConcreteType.DefineMethod("set_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new System.Type[] { propertyInfo.PropertyType });
                        var setGenerator = setter.GetILGenerator();
                        setGenerator.Emit(OpCodes.Ldarg_0);
                        setGenerator.Emit(OpCodes.Ldarg_1);
                        setGenerator.Emit(OpCodes.Stfld, field);
                        setGenerator.Emit(OpCodes.Ret);
                        property.SetSetMethod(setter);
                        myConcreteType.DefineMethodOverride(setter, propertyInfo.GetSetMethod());
                    }
                });

                return myConcreteType;
            });
            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            HandleType(codeElement);
            return new Nothing();
        }

        public Nothing RootScope(IRootScope co)
        {
            HandleLines(co.Assignments);
            HandleScope(co.Scope);
            co.EntryPoint.Convert(this);
            return new Nothing();
        }
    }
}
