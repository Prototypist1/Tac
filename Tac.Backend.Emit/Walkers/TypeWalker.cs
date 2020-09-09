using Prototypist.Toolbox;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit.Walkers
{

    class Nothing { }
    class TypeWalker : IOpenBoxesContext<Nothing>
    {
        private readonly ModuleBuilder moduleBuilder;

        public readonly Dictionary<IVerifiableType, System.Type> typeCache = new Dictionary<IVerifiableType, System.Type>();
        private readonly LinkedList<Action> followUp = new LinkedList<Action>();

        public TypeWalker(ModuleBuilder moduleBuilder)
        {
            this.moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
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


        private System.Type InnerMapType(IVerifiableType verifiableType)
        {
            if (verifiableType is INumberType)
            {
                return typeof(double);
            }
            if (verifiableType is IBooleanType)
            {
                return typeof(bool);
            }
            if (verifiableType is IStringType)
            {
                return typeof(string);
            }
            if (verifiableType is IBlockType)
            {
                // ??
                return typeof(Action);
            }
            if (verifiableType is IEmptyType)
            {
                return typeof(Empty);
            }
            if (verifiableType is IAnyType)
            {
                return typeof(object);
            }
            if (verifiableType.SafeIs(out IInterfaceModuleType moduleType))
            {
                var myType = moduleBuilder.DefineType(new Guid().ToString("N"));

                followUp.AddLast(() => {
                    foreach (var member in moduleType.Members)
                    {

                        // duplicate code {BEAEC647-A435-4315-919B-D6CF353A8B27}
                        var type = InnerMapType(member.Type);

                        if (type.IsPrimitive)
                        {
                            myType.DefineField(member.Key.SafeCastTo(out NameKey _).Name, typeof(Ref<>).MakeGenericType(type), FieldAttributes.Public);
                        }
                        else
                        {
                            myType.DefineField(member.Key.SafeCastTo(out NameKey _).Name, typeof(Func<>).MakeGenericType(typeof(Ref<>).MakeGenericType(type)), FieldAttributes.Public);
                        }
                    }
                });

                return myType;
            }
            if (verifiableType is IMethodType method)
            {
                return typeof(Func<,>).MakeGenericType(
                    HandleType(method.InputType),
                    HandleType(method.OutputType)
                    );
            }
            if (verifiableType is IMemberReferance memberReferance)
            {
                // I have to fresh up on what this means....
                // I think it is ref<T> 
                // used on the target of assignment 
                return HandleType(memberReferance.MemberDefinition.Type);
            }
            if (verifiableType is ITypeOr typeOr)
            {
                // we try to find the intersection of the types
                return MergeTypes(typeOr.Left, typeOr.Right);
            }

            throw new NotImplementedException();
        }

        
        private System.Type MergeTypes(IVerifiableType left, IVerifiableType right)
        {

            var leftType = InnerMapType(left); ;
            var rightType = InnerMapType(right);

            // if they are the same we are happy'
            if (leftType == rightType)
            {
                return leftType;
            }

            // if either is a primitive type... return empty?
            if (left.SafeIs(out IPrimitiveType _) || right.SafeIs(out IPrimitiveType _))
            {
                return typeof(Empty);
            }

            // if they are both methods 
            // we have re merge the method io
            if (left.TryGetInput().Is(out var leftInput) &&
                right.TryGetInput().Is(out var rightInput) &&
                left.TryGetReturn().Is(out var leftReturn) &&
                right.TryGetReturn().Is(out var rightReturn))
            {

                return typeof(Func<,>).MakeGenericType(
                    MergeTypes(leftInput, rightInput),
                    MergeTypes(leftReturn, rightReturn));
            }

            if (left.SafeIs(out IInterfaceModuleType leftHasMembers) && right.SafeIs(out IInterfaceModuleType rightHasMembers)) { 
                var myType = moduleBuilder.DefineType(new Guid().ToString("N"));


                followUp.AddLast(() => {

                    foreach (var member in leftHasMembers.Members)
                    {
                        if (rightHasMembers.TryGetMember(member.Key).Is(out var memberType)) {

                            // duplicate code {BEAEC647-A435-4315-919B-D6CF353A8B27}
                            var type = InnerMapType(member.Type);

                            if (type.IsPrimitive)
                            {
                                myType.DefineField(member.Key.SafeCastTo(out NameKey _).Name, typeof(Ref<>).MakeGenericType(type), FieldAttributes.Public);
                            }
                            else
                            {
                                myType.DefineField(member.Key.SafeCastTo(out NameKey _).Name, typeof(Func<>).MakeGenericType(typeof(Ref<>).MakeGenericType(type)), FieldAttributes.Public);
                            }
                        }
                    }
                });

                return myType;
            }

            return typeof(Empty);
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
        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation) => HandleOp(tryAssignOperation);

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            HandleLines(codeElement.Body);
            HandleLines(codeElement.StaticInitailizers);
            HandleScope(codeElement.Scope);
            return new Nothing();
        }


        public Nothing ConstantBool(IConstantBool constantBool) =>new Nothing();
        public Nothing ConstantNumber(IConstantNumber codeElement) => new Nothing();
        public Nothing ConstantString(IConstantString co) => new Nothing();
        public Nothing EmptyInstance(IEmptyInstance co) => new Nothing();


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            HandleLines(entryPointDefinition.Body);
            HandleLines(entryPointDefinition.StaticInitailizers);
            HandleScope(entryPointDefinition.Scope);
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

        public Nothing MemberReferance(IMemberReferance codeElement)
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
            return new Nothing();
        }

        public Nothing ModuleDefinition(IModuleDefinition codeElement)
        {
            HandleScope(codeElement.Scope);
            codeElement.EntryPoint.Convert(this);
            HandleLines(codeElement.StaticInitialization);
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            HandleLines(codeElement.Assignments);
            HandleScope(codeElement.Scope);
            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            HandleType(codeElement);
            return new Nothing();
        }
    }
}
