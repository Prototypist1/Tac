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
//using Tac.Backend.Emit.Support;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit.Walkers
{

    // todo maybe a struct so it's not null-able
    public class Empty { }
    class Nothing { }

    //interface ITypeLookup {
    //    System.Type GetObject(IObjectDefiniton codeElement); 
    //    System.Type GetType(IVerifiableType verifiableType);
    //}


    public abstract class TypeTracker {
        protected string GetTypeName() => "_" + Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "");
        protected static bool HasMember(IVerifiableType type)
        {
            if (type.SafeIs(out IInterfaceModuleType interfaceModuleType) && interfaceModuleType.Members.Any())
            {
                return true;
            }
            if (type.SafeIs(out ITypeOr typeOr) && typeOr.Members.Any())
            {
                return true;
            }
            return false;
        }

        // TODO 
        // this makes test-name and test_name the same... 
        public static string ConvertName(string name)
        {
            return name.Replace("-", "_");
        }
    }

    public abstract class TypeTracker<T>: TypeTracker where T : System.Type {


        protected readonly ConcurrentIndexed<(System.Type, System.Type), IVerifiableType> funcCache;

        protected TypeTracker(ConcurrentIndexed<(System.Type, System.Type), IVerifiableType> funcCache)
        {
            this.funcCache = funcCache ?? throw new ArgumentNullException(nameof(funcCache));
        }

        public System.Type ResolvePossiblyPrimitive(IVerifiableType verifiableType)
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
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(moduleType));
            }
            else if (verifiableType is IMethodType method)
            {
                var inputType = ResolvePossiblyPrimitive(method.InputType);
                var outputType = ResolvePossiblyPrimitive(method.OutputType);
                funcCache.GetOrAdd((inputType, outputType), method);
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

        private System.Type MergeTypes(IVerifiableType left, IVerifiableType right, ITypeOr typeOr)
        {
            // this sure looks like it could be about 5 lines
            ResolvePossiblyPrimitive(left);
            ResolvePossiblyPrimitive(right);

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
                // we still git an interface
                // there will be no properties or anything so I used to jsut use object
                // but we need to be able to look up the IVerifiableType at runtime from a type
                // so we need tac types and CIL type 1-1
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(typeOr));
            }

            // if either is a primitive type... return empty?
            if (left.SafeIs(out IPrimitiveType _) || right.SafeIs(out IPrimitiveType _))
            {
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(typeOr));
            }

            // if they are both methods 
            // we have re merge the method io
            if (typeOr.TryGetInput().Is(out var input) &&
                typeOr.TryGetReturn().Is(out var output))
            {
                var myInput = ResolvePossiblyPrimitive(input);
                var myOutput = ResolvePossiblyPrimitive(output);
                funcCache.GetOrAdd((myInput, myOutput), typeOr);
                return typeof(Func<,>).MakeGenericType(myInput, myOutput);
            }

            if (left.SafeIs(out IInterfaceModuleType _) && right.SafeIs(out IInterfaceModuleType _))
            {
                // JIT a interface
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(typeOr));
            }

            // can it have members if both are neither an interface or a module
            //if (typeOr.Members.Any())
            //{
            //    return typeof(ITacObject);
            //}

            // if it is a method and something with members...
            if (HasMember(left) && right.TryGetInput().Is(out var _) && right.TryGetReturn().Is(out var _))
            {
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(typeOr));
            }
            if (HasMember(right) && left.TryGetInput().Is(out var _) && left.TryGetReturn().Is(out var _))
            {
                return ResolveNotPrimitive(OrType.Make<ITypeOr, IInterfaceModuleType>(typeOr));
            }

            throw new Exception("what case did I miis");
        }

        protected abstract T ResolveNotPrimitive(IOrType<ITypeOr, IInterfaceModuleType> key);

    }

    public class TypePassTypeTracker : TypeTracker<TypeBuilder>
    {
        private readonly ConcurrentIndexed<IVerifiableType, TypeBuilder> typeCache = new ConcurrentIndexed<IVerifiableType, TypeBuilder>();
        private readonly ConcurrentIndexed<IObjectDefiniton, TypeBuilder> objectCache = new ConcurrentIndexed<IObjectDefiniton, TypeBuilder>();
        private readonly List<DebuggableILGenerator> gens;
        private readonly ModuleBuilder moduleBuilder;
        private readonly ConcurrentLinkedList<Action> typeActions = new ConcurrentLinkedList<Action>();
        private readonly ConcurrentLinkedList<Action> objectActions = new ConcurrentLinkedList<Action>();

        public TypePassTypeTracker(ModuleBuilder value, List<DebuggableILGenerator> gens):base(new ConcurrentIndexed<(System.Type, System.Type), IVerifiableType>())
        {
            this.gens = gens?? throw new ArgumentNullException(nameof(gens));
            moduleBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }

        public System.Type IdempotentAddType(IVerifiableType verifiableType)
        {
            return ResolvePossiblyPrimitive(verifiableType);
        }

        internal TypeBuilder IdempotentAddObject(IObjectDefiniton codeElement)
        {
            var verifiedType = codeElement.Returns();

            return objectCache.GetOrAdd(codeElement, () =>
            {
                var interfactType = IdempotentAddType(verifiedType);
                var myConcreteType = moduleBuilder.DefineType(GetTypeName(), TypeAttributes.Public);


                objectActions.Add(() =>
                {

                    myConcreteType.AddInterfaceImplementation(interfactType);


                    foreach (var propertyInfo in interfactType.GetProperties())
                    {
                        var field = myConcreteType.DefineField("_" + propertyInfo.Name.ToLower(), propertyInfo.PropertyType, FieldAttributes.Public);
                        var property = myConcreteType.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new System.Type[0]);

                        if (propertyInfo.CanRead)
                        {
                            var getter = myConcreteType.DefineMethod(
                                "get_" + propertyInfo.Name,
                                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                                propertyInfo.PropertyType,
                                new System.Type[0]);
                            var getGenerator = new DebuggableILGenerator(getter.GetILGenerator(), "get_" + propertyInfo.Name + " of " + propertyInfo.DeclaringType.Name + " on " + myConcreteType.Name);
                            gens.Add(getGenerator);
                            getGenerator.Emit(OpCodes.Ldarg_0);
                            getGenerator.Emit(OpCodes.Ldfld, field);
                            getGenerator.Emit(OpCodes.Ret);
                            property.SetGetMethod(getter);
                            myConcreteType.DefineMethodOverride(getter, propertyInfo.GetGetMethod());
                        }
                        if (propertyInfo.CanWrite)
                        {
                            var setter = myConcreteType.DefineMethod(
                                "set_" + propertyInfo.Name,
                                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                                null,
                                new System.Type[] { propertyInfo.PropertyType });
                            var setGenerator = new DebuggableILGenerator(setter.GetILGenerator(), "set_" + propertyInfo.Name + " of " + propertyInfo.DeclaringType.Name + " on " + myConcreteType.Name);
                            gens.Add(setGenerator);
                            setGenerator.Emit(OpCodes.Ldarg_0);
                            setGenerator.Emit(OpCodes.Ldarg_1);
                            setGenerator.Emit(OpCodes.Stfld, field);
                            setGenerator.Emit(OpCodes.Ret);
                            property.SetSetMethod(setter);
                            myConcreteType.DefineMethodOverride(setter, propertyInfo.GetSetMethod());
                        }
                    }
                });

                return myConcreteType;
            });
        }

        protected override TypeBuilder ResolveNotPrimitive(IOrType<ITypeOr, IInterfaceModuleType> key)
        {
            return typeCache.GetOrAdd(key.SwitchReturns<IVerifiableType>(x => x, x => x), () =>
            {
                var res = moduleBuilder.DefineType(GetTypeName(), TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

                typeActions.Add(() =>
                {
                    foreach (var member in key.SwitchReturns(x => x.Members, x => x.Members))
                    {
                        var name = ConvertName(member.Key.CastTo<NameKey>().Name);
                        var type = IdempotentAddType(member.Type);
                        var property = res.DefineProperty(
                            name,
                            PropertyAttributes.None,
                            type,
                            null);

                        var getter = res.DefineMethod(
                                "get_" + name,
                                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract,
                                type,
                                new System.Type[0]);
                        property.SetGetMethod(getter);

                        var setter = res.DefineMethod(
                           "set_" + name,
                           MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract,
                           null,
                           new System.Type[] { type });
                        property.SetSetMethod(setter);
                    }
                });

                return res;
            });
        }


        internal AssemblerTypeTracker CreateTypesAndProperties()
        {
            ConcurrentIndexed<IVerifiableType, System.Type> nextTypeCache = new ConcurrentIndexed<IVerifiableType, System.Type>();
            ConcurrentIndexed<IObjectDefiniton, System.Type> nextObjectCache = new ConcurrentIndexed<IObjectDefiniton, System.Type>();

            // order is important here:
            // interfaces must be complete before objects can implementment, thus "type" before "objects"

            // we create all types in the main run
            // but we don't create properties until after so we know all the TypeBuilder objects exists
            // altho I am not sure I need to do that doing things lazy like might be enough...
            foreach (var action in typeActions)
            {
                action();
            }
            foreach (var type in typeCache)
            {
                nextTypeCache.AddOrThrow(type.Key, type.Value.CreateType());
            }
            //var nextDependencyType = dependenciesType.CreateType();
            foreach (var action in objectActions)
            {
                action();
            }
            foreach (var obj in objectCache)
            {
                nextObjectCache.AddOrThrow(obj.Key,obj.Value.CreateType());
            }
            return new AssemblerTypeTracker(nextTypeCache, nextObjectCache, funcCache);//, nextDependencyType);
        }


        //private TypeBuilder dependenciesType;
        internal void HandleDependencies(IProject<Assembly, object> project)
        {
            //dependenciesType = moduleBuilder.DefineType(AssemblerVisitor.GenerateName(), TypeAttributes.Public);

            foreach (var member in project.References)
            {
                IdempotentAddType(member.Scope);
            }

            //typeActions.Add(() =>
            //{
            //    foreach (var member in project.References)
            //    {
            //        dependenciesType.DefineField(TypeTracker.ConvertName(member.Key.SafeCastTo(out NameKey _).Name), IdempotentAddType(member.Scope), FieldAttributes.Public);
            //    }
            //});

        }
    }


    //public struct DependenciesType : IVerifiableType
    //{
    //    // this is a bit werid...
    //    // I don't think anything should reference this
    //    // or ever call it's members
    //    // 
    //    // I just need a key for typeCache
    //    // if this turns out to be a problem
    //    // I can create of object type

    //    public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IIsPossibly<IVerifiableType> TryGetInput()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IIsPossibly<(IVerifiableType type, Access access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IIsPossibly<IVerifiableType> TryGetReturn()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public class AssemblerTypeTracker: TypeTracker<System.Type>
    {
        private readonly ConcurrentIndexed<IVerifiableType, System.Type> typeCache = new ConcurrentIndexed<IVerifiableType, System.Type>();
        private readonly ConcurrentIndexed<IObjectDefiniton, System.Type> objectCache = new ConcurrentIndexed<IObjectDefiniton, System.Type>();
        //private readonly System.Type dependencyType;
        public readonly ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionCache = new ConcurrentIndexed<(System.Type, System.Type), TypeBuilder>();
        public readonly ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> methodConversionCache = new ConcurrentIndexed<(System.Type, System.Type), TypeBuilder>();

        //, System.Type nextDependencyType
        public AssemblerTypeTracker(ConcurrentIndexed<IVerifiableType, System.Type> typeCache, ConcurrentIndexed<IObjectDefiniton, System.Type> objectCache, ConcurrentIndexed<(System.Type, System.Type), IVerifiableType> funcCache) : base(funcCache)
        {
            this.typeCache = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
            this.objectCache = objectCache ?? throw new ArgumentNullException(nameof(objectCache));
            //this.dependencyType = nextDependencyType ?? throw new ArgumentNullException(nameof(nextDependencyType));
        }

        protected override System.Type ResolveNotPrimitive(IOrType<ITypeOr, IInterfaceModuleType> key)
        {
            return typeCache.GetOrThrow(key.SwitchReturns<IVerifiableType>(x => x, x => x));
        }
        public System.Type ResolveObject(IObjectDefiniton key)
        {
            return objectCache.GetOrThrow(key);
        }

        //public System.Type GetDependencyType() => dependencyType;
        //internal IVerifiableType lookUpType(System.Type implements)
        //{
        //    return typeCache.Single(x => x.Value == implements).Key;
        //}

        internal RunTimeTypeTracker RunTimeTypeTracker() {
            var runtimeTypeCache = new ConcurrentIndexed<System.Type, IVerifiableType>();
            foreach (var typePair in typeCache)
            {
                runtimeTypeCache.AddOrThrow(typePair.Value, typePair.Key);
            }
            foreach (var objectPair in objectCache)
            {
                runtimeTypeCache.AddOrThrow(objectPair.Value, objectPair.Key.Returns());
            }
            foreach (var pair in conversionCache)
            {
                // the runtimeTypeCache shouldn't cause any trouble
                // pair.Key.Item2 should all be root 
                runtimeTypeCache.AddOrThrow(pair.Value, runtimeTypeCache[pair.Key.Item2]);
            }
            var nextConversionCache = new ConcurrentIndexed<(System.Type, System.Type), System.Type>();
            foreach (var pait in conversionCache)
            {
                nextConversionCache.AddOrThrow(pait.Key, pait.Value);
            }
            var nextMethodConversionCache = new ConcurrentIndexed<(System.Type, System.Type), System.Type>();
            foreach (var pait in methodConversionCache)
            {
                nextMethodConversionCache.AddOrThrow(pait.Key, pait.Value);
            }
            return new RunTimeTypeTracker(runtimeTypeCache, nextConversionCache, nextMethodConversionCache, funcCache);
        }
    }

    public class RunTimeTypeTracker {
        private readonly ConcurrentIndexed<System.Type, IVerifiableType> cache;
        private readonly ConcurrentIndexed<(System.Type,System.Type), IVerifiableType> funcCache;
        public readonly ConcurrentIndexed<(System.Type, System.Type), System.Type> methodConversionCache;
        public readonly ConcurrentIndexed<(System.Type, System.Type), System.Type> conversionCache;

        public RunTimeTypeTracker(
            ConcurrentIndexed<System.Type, IVerifiableType> runtimeTypeCache, 
            ConcurrentIndexed<(System.Type, System.Type), System.Type> conversionCache,
            ConcurrentIndexed<(System.Type, System.Type), System.Type> methodConversionCache, 
            ConcurrentIndexed<(System.Type, System.Type), IVerifiableType> funcCache)
        {
            this.cache = runtimeTypeCache ?? throw new ArgumentNullException(nameof(runtimeTypeCache));
            this.conversionCache = conversionCache ?? throw new ArgumentNullException(nameof(conversionCache));
            this.methodConversionCache = methodConversionCache ?? throw new ArgumentNullException(nameof(methodConversionCache));
            this.funcCache = funcCache ?? throw new ArgumentNullException(nameof(funcCache));
        }

        public IVerifiableType LookUp(System.Type type) {

            // is it really ok for this to reference the instantiated version? 
            if (type == typeof(double))
            {
                return new NumberType(); ;
            }
            else if (type == typeof(bool))
            {
                return new BooleanType(); ;
            }
            else if (type == typeof(string))
            {
                return new StringType();
            }
            else if (type == typeof(Empty))
            {
                return new EmptyType();
            }
            else if (type == typeof(object))
            {
                return new AnyType();
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>)) {
                var args = type.GetGenericArguments();
                return funcCache[(args[0], args[1])];
            }

            return cache[type];
        }

        public void Add(System.Type type, IVerifiableType verifiableType)
        {
            cache.AddOrThrow(type, verifiableType);
        }
    }

    class TypeVisitor : IOpenBoxesContext<Nothing>
    {


        private readonly TypePassTypeTracker typeTracker;

        public TypeVisitor(TypePassTypeTracker typeTracker)
        {
            this.typeTracker = typeTracker ?? throw new ArgumentNullException(nameof(typeTracker));
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
            return typeTracker.IdempotentAddType(verifiableType);
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

            typeTracker.IdempotentAddObject(codeElement);

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

        internal void HandleDependencies(IProject<Assembly, object> project)
        {
            //foreach (var member in project.DependencyScope.Members)
            //{
            //    HandleType(member.Value.Value.Type);
            //}
            typeTracker.HandleDependencies(project);

        }

        public Nothing GenericMethodDefinition(IGenericMethodDefinition co)
        {
            throw new NotImplementedException();
        }
    }
}
