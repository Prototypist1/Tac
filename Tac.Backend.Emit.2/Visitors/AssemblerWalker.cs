using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Tac.Backend.Emit._2.Lookup;
using Tac.Backend.Emit.Support;
using Tac.Backend.Emit._2.Visitors;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Prototypist.TaskChain;

namespace Tac.Backend.Emit._2.Walkers
{

    public class Enclosed<T>
    {
        public T value;
    }

    public class IndexerList
    {

        public IIsPossibly<int> GetOrAdd(IVerifiableType fromType, IVerifiableType toType)
        {
            if (fromType.Equals(toType))
            {
                return Possibly.IsNot<int>();
            }

            var myIndexer = Indexer.Create(fromType, toType);
            if (myIndexer == null)
            {
                return Possibly.IsNot<int>();
            }

            var index = indexers.IndexOf(myIndexer);
            if (index != -1)
            {
                return Possibly.Is(index);
            }

            indexers.Add(myIndexer);

            return Possibly.Is(indexers.Count - 1);
        }

        public List<Indexer> indexers = new List<Indexer>();
    }

    //public class VerifyableTypesList
    //{

    //    public IIsPossibly<int> GetOrAdd(IVerifiableType type)
    //    {

    //        var index = types.IndexOf(type);
    //        if (index != -1)
    //        {
    //            return Possibly.Is(index);
    //        }

    //        types.Add(type);

    //        return Possibly.Is(types.Count - 1);
    //    }

    //    public List<IVerifiableType> types = new List<IVerifiableType>();
    //}


    public class DebuggableILGenerator
    {
        public int EvaluationStackDepth { get; set; } = 0;
        List<IOrType<Guid, IMemberDefinition>> locals = new List<IOrType<Guid, IMemberDefinition>>();

        ILGenerator backing;
        string debugString = "";

        public DebuggableILGenerator(ILGenerator backing, string name)
        {
            debugString += name + ": " + Tabs() + Environment.NewLine;
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        internal void Emit(OpCode code, string str)
        {
            debugString += code.ToString() + ", " + str + Environment.NewLine;
            backing.Emit(code, str);
        }

        internal void Emit(OpCode code, MethodInfo method)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + method.Name + "<" + string.Join<string>(", ", method.GetGenericArguments().Select(x => x.FullName)) + ">" + Environment.NewLine;
            backing.Emit(code, method);
        }

        internal void Emit(OpCode code, double dub)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + dub + Environment.NewLine;
            backing.Emit(code, dub);
        }

        internal void Emit(OpCode code, byte dub)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + dub + Environment.NewLine;
            backing.Emit(code, dub);
        }

        internal void Emit(OpCode code, short dub)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + dub + Environment.NewLine;
            backing.Emit(code, dub);
        }

        internal void Emit(OpCode code, Label label)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + label + Environment.NewLine;
            backing.Emit(code, label);
        }


        internal void Emit(OpCode code, System.Type type)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + type.FullName + Environment.NewLine;
            backing.Emit(code, type);
        }
        internal void Emit(OpCode code, FieldInfo field)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + field.Name + Environment.NewLine;
            backing.Emit(code, field);
        }

        internal void Emit(OpCode code)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + Environment.NewLine;
            backing.Emit(code);
        }
        internal void Emit(OpCode code, ConstructorInfo rootSelfField)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + rootSelfField.DeclaringType.FullName + /*"<" + string.Join<string>(", ", rootSelfField.GetGenericArguments().Select(x => x.FullName)) + ">" +*/ Environment.NewLine;
            backing.Emit(code, rootSelfField);
        }

        internal LocalBuilder DeclareLocal(System.Type type, IMemberDefinition member)
        {
            var or = OrType.Make<Guid, IMemberDefinition>(member);
            if (locals.Contains(or))
            {
                throw new Exception("already added");
            }

            locals.Add(or);
            debugString += EvaluationStackDepth + ": " + Tabs() + "local, " + type.FullName + ", " + member.Key + Environment.NewLine;
            return backing.DeclareLocal(type);
        }

        internal LocalBuilder DeclareLocal(System.Type type, Guid id)
        {

            var or = OrType.Make<Guid, IMemberDefinition>(id);
            if (locals.Contains(or))
            {
                throw new Exception("already added");
            }

            locals.Add(or);

            debugString += EvaluationStackDepth + ": " + Tabs() + "local, " + type.FullName + ", " + id + Environment.NewLine;
            return backing.DeclareLocal(type);
        }

        internal int GetLocalIndex(Guid id)
        {

            var or = OrType.Make<Guid, IMemberDefinition>(id);
            var index = locals.IndexOf(or);
            if (index == -1)
            {
                throw new Exception("local was never defined");
            }
            return index;
        }

        internal int GetLocalIndex(IMemberDefinition member)
        {
            var or = OrType.Make<Guid, IMemberDefinition>(member);
            var index = locals.IndexOf(or);
            if (index == -1)
            {
                throw new Exception("local was never defined");
            }
            return index;
        }

        private string Tabs()
        {
            var res = "";
            for (int i = 0; i < EvaluationStackDepth; i++)
            {
                res += '\t';
            }
            return res;
        }

        //internal void EmitCall(OpCode code, MethodInfo methodInfo, System.Type[]? optionalTypes)
        //{
        //    debugString += EvaluationStackDepth + ": " +Tabs() + code.ToString() + ", " + methodInfo.Name + /*"(" + string.Join<string>(',', methodInfo.GetParameters().Select(x => x.ParameterType.Name)) + ")" +*/ Environment.NewLine;
        //    backing.EmitCall(code,methodInfo,optionalTypes);
        //}

        internal void EmitCall(OpCode code, MethodInfo methodInfo)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + methodInfo.Name + "<" + string.Join<string>(", ", methodInfo.GetGenericArguments().Select(x => x.FullName)) + ">" + Environment.NewLine;
            backing.Emit(code, methodInfo);
        }

        internal void MarkLabel(Label topOfElseLabel)
        {
            debugString += EvaluationStackDepth + ": " + Tabs() + topOfElseLabel.ToString() + Environment.NewLine;
            backing.MarkLabel(topOfElseLabel);
        }

        internal Label DefineLabel()
        {
            return backing.DefineLabel();
        }

        public string GetDeubbingSting()
        {
            return debugString.ToString();
        }
    }

    class GeneratorHolder
    {


        public int EvaluationStackDepth => generator.GetOrThrow().EvaluationStackDepth;
        private IIsPossibly<DebuggableILGenerator> generator;

        public GeneratorHolder(IIsPossibly<DebuggableILGenerator> generator)
        {
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public DebuggableILGenerator GetGeneratorAndUpdateStack(int stackChange)
        {
            var res = generator.GetOrThrow();
            res.EvaluationStackDepth += stackChange;
            return res;
        }

        public string DebugString()
        {
            return generator.GetOrThrow().GetDeubbingSting();
        }
    }


    public abstract class TacCompilation
    {
        //public Indexer[] indexerArray;
        //public IVerifiableType[] verifyableTypesArray;
        public ConcurrentIndexed<System.Type, IVerifiableType> typeCache;
        public ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache;
        public abstract void Init();
    }
    public abstract class TacCompilation<Tin, Tout> : TacCompilation
    {

        public Func<Tin, Tout> main;
    }



    class AssemblerVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly List<DebuggableILGenerator> gens;
        private readonly ModuleBuilder moduleBuilder;
        public readonly IndexerList indexerList;
        //public readonly VerifyableTypesList verifyableTypesList;



        private readonly MemberKindLookup memberKindLookup;
        private readonly ExtensionLookup extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        private readonly ConcurrentIndexed<IVerifiableType, TypeBuilder> typeCache;
        private readonly ConcurrentIndexed<IObjectDefiniton, TypeBuilder> objectCache;
        private readonly ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes;
        public readonly TypeBuilder rootType;
        public readonly FieldBuilder rootSelfField;


        public readonly GeneratorHolder generatorHolder;

        private IReadOnlyList<ICodeElement> stack;
        private readonly Stack<LocalVariableInfo> thisStack = new Stack<LocalVariableInfo>();




        // I think I want an object to hold the state
        // I need it for stuff like entryPointField
        // since I am creating a new visitor for each run
        //private FieldBuilder entryPointField;

        private AssemblerVisitor(
            IReadOnlyList<ICodeElement> stack,
            GeneratorHolder generatorHolder,
            MemberKindLookup memberKindLookup,
            ExtensionLookup extensionLookup,
            ConcurrentIndexed<IVerifiableType, TypeBuilder> typeCache,
            ConcurrentIndexed<IObjectDefiniton, TypeBuilder> objectCache,
            ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes,
            TypeBuilder rootType,
             FieldBuilder rootSelfField,
             IndexerList indexerList,
             //VerifyableTypesList verifyableTypesList,
             RealizedMethodLookup realizedMethodLookup,
             List<DebuggableILGenerator> gens
            )
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this.generatorHolder = generatorHolder ?? throw new ArgumentNullException(nameof(generatorHolder));
            this.memberKindLookup = memberKindLookup ?? throw new ArgumentNullException(nameof(memberKindLookup));
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
            this.typeCache = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
            this.objectCache = objectCache ?? throw new ArgumentNullException(nameof(objectCache));
            this.conversionTypes = conversionTypes ?? throw new ArgumentNullException(nameof(conversionTypes));
            this.rootType = rootType ?? throw new ArgumentNullException(nameof(rootType));
            this.rootSelfField = rootSelfField ?? throw new ArgumentNullException(nameof(rootSelfField));
            this.indexerList = indexerList ?? throw new ArgumentNullException(nameof(indexerList));
            //this.verifyableTypesList = verifyableTypesList ?? throw new ArgumentNullException(nameof(verifyableTypesList));
            this.realizedMethodLookup = realizedMethodLookup ?? throw new ArgumentNullException(nameof(realizedMethodLookup));
            this.gens = gens ?? throw new ArgumentNullException(nameof(gens));
        }

        public static (AssemblerVisitor, Action) Create(
            MemberKindLookup memberKindLookup,
            ExtensionLookup extensionLookup,
            ConcurrentIndexed<IVerifiableType, TypeBuilder> typeCache,
            ConcurrentIndexed<IObjectDefiniton, TypeBuilder> objectCache,
            ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes,
            ModuleBuilder moduleBuilder,
            RealizedMethodLookup realizedMethodLookup,
            System.Type tin,
            System.Type tout)
        {
            var typebuilder = moduleBuilder.DefineType(GenerateName(), TypeAttributes.Public & TypeAttributes.Class, typeof(TacCompilation<,>).MakeGenericType(tin, tout));
            var selfField = typebuilder.DefineField(GenerateName() + "_self", typebuilder, FieldAttributes.Static | FieldAttributes.Public);
            var typeCacheField = typebuilder.DefineField(GenerateName() + "_typeCache", typeof(ConcurrentIndexed<System.Type, IVerifiableType>), FieldAttributes.Static | FieldAttributes.Public);

            var initMethod = typebuilder.DefineMethod(nameof(TacCompilation<int, int>.Init), MethodAttributes.Public | MethodAttributes.Virtual);
            typebuilder.DefineMethodOverride(initMethod, typeof(TacCompilation<,>).MakeGenericType(tin, tout).GetMethod(nameof(TacCompilation<int, int>.Init)));

            var gen = new DebuggableILGenerator(initMethod.GetILGenerator(), "Init");


            var gens = new List<DebuggableILGenerator> { gen };
            var generatorHolder = new GeneratorHolder(Possibly.Is(gen));

            // set the self field
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stsfld, selfField);
            // set the 
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stsfld, typeCacheField);


            return
                (new AssemblerVisitor(new List<ICodeElement>(), generatorHolder, memberKindLookup, extensionLookup, typeCache, objectCache, conversionTypes, typebuilder, selfField, new IndexerList(), /*new VerifyableTypesList(),*/ realizedMethodLookup, gens), () =>
                {
                    while (generatorHolder.EvaluationStackDepth > 0)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ret);
                }
            );
        }

        public AssemblerVisitor Push(ICodeElement another)
        {
            var list = stack.ToList();
            list.Add(another);
            return new AssemblerVisitor(list, generatorHolder, memberKindLookup, extensionLookup, typeCache, objectCache, conversionTypes, rootType, rootSelfField, indexerList, /*verifyableTypesList,*/ realizedMethodLookup, gens);
        }


        public AssemblerVisitor Push(ICodeElement another, DebuggableILGenerator gen)
        {
            var list = stack.ToList();
            list.Add(another);
            gens.Add(gen);
            return new AssemblerVisitor(list, new GeneratorHolder(Possibly.Is(gen)), memberKindLookup, extensionLookup, typeCache, objectCache, conversionTypes, rootType, rootSelfField, indexerList, /*verifyableTypesList,*/ realizedMethodLookup, gens);
        }

        public Nothing AddOperation(IAddOperation co)
        {
            //generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
            //generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_R8, 2.0);
            //generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Add_Ovf);

            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Add_Ovf);
            return new Nothing();
        }

        private void PossiblyConvert(IVerifiableType fromType, IVerifiableType toType)
        {
            // handle "any"
            if (toType.SafeIs(out IAnyType _))
            {
                if (fromType.SafeIs(out IAnyType _))
                {
                    return;
                }
                else
                {
                    if (typeCache.TryGetValue(fromType, out var toCSharpeType))
                    {
                        if (new[] { typeof(bool), typeof(double) }.Contains(toCSharpeType))
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, toCSharpeType);
                        }
                    }
                    return;
                }
            }

            // we don't cast for an or unless we need to
            if (toType.SafeIs(out ITypeOr _))
            {

                if (typeCache.TryGetValue(fromType, out var toCSharpeType))
                {
                    if (new[] { typeof(bool), typeof(double) }.Contains(toCSharpeType))
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, toCSharpeType);
                        return;
                    }
                }
                if (typeCache[toType] == typeof(object))
                {
                    return;
                }
            }

            {
                if (typeCache.TryGetValue(fromType, out var fromCSharpeType) && typeCache.TryGetValue(toType, out var toCSharpeType))
                {
                    var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(fromCSharpeType, Guid.NewGuid());
                    StoreLocal(loc.LocalIndex);
                    // TODO
                    // this is a bit weird, it is called both at runtime and at complie time
                    // but they share a type lookup table
                    // this means we can't really ship assemblies
                    var type = AssemblyWalkerHelp.EmitTypeThatWrapsAndImplementsCompileTime(fromCSharpeType, toCSharpeType, moduleBuilder, conversionTypes);

                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, type.GetConstructor(new System.Type[] { }));
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    LoadLocal(loc.LocalIndex);
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Stfld, type.GetField(AssemblyWalkerHelp.backingName));
                }
            }

            // we create the indexer now
            // and we put it in a big array
            // this is kind of a hack
            // it means the code that I am emitting cannot be run standalone
            // it will only work inline
            //    if (indexerList.GetOrAdd(fromType, toType).SafeIs(out IIsDefinately<int> definateIndexer))
            //{
            //    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            //    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, indexersField.Value);
            //    LoadInt(definateIndexer.Value);
            //    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ldelem_Ref);

            //    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, castConstructor.Value);
            //}
        }

        private void GetVerifyableType(System.Type toType)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.typeCache)));

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldtoken, toType);
            generatorHolder.GetGeneratorAndUpdateStack(0).EmitCall(OpCodes.Call, typeof(System.Type).GetMethod(nameof(System.Type.GetTypeFromHandle)));

            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, typeof(ConcurrentIndexed<System.Type, IVerifiableType>).GetMethod("get_Item"));

            //if (verifyableTypesList.GetOrAdd(toType).SafeIs(out IIsDefinately<int> definateType))
            //{
            //    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            //    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation));
            //    LoadInt(definateType.Value);
            //    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ldelem_Ref);
            //}
            //else
            //{
            //    throw new Exception("you shit");
            //}
        }

        private readonly Lazy<FieldInfo> typeCacheField = new Lazy<FieldInfo>(() =>
        {
            return typeof(TacCompilation).GetField(nameof(TacCompilation.typeCache)) ?? throw new NullReferenceException("should not be null!");
        });

        //private readonly Lazy<FieldInfo>  indexersField = new Lazy<FieldInfo>(() =>
        //{
        //    return typeof(TacCompilation).GetField(nameof(TacCompilation.indexerArray)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<FieldInfo> typesField = new Lazy<FieldInfo>(() =>
        //{
        //    return typeof(TacCompilation).GetField(nameof(TacCompilation.verifyableTypesArray)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<ConstructorInfo> castConstructor = new Lazy<ConstructorInfo>(() =>
        //{
        //    return typeof(TacCastObject).GetConstructor(new[] { typeof(ITacObject), typeof(Indexer)}) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<ConstructorInfo> tacObjectConstructor = new Lazy<ConstructorInfo>(() =>
        //{
        //    return typeof(TacObject).GetConstructor(new System.Type[] { }) ?? throw new NullReferenceException("should not be null!");
        //});


        //private readonly Lazy<ConstructorInfo> tacMethod_Complex_ComplexConstructor = new Lazy<ConstructorInfo>(() =>
        //{
        //    return typeof(TacMethod_Complex_Complex).GetConstructor(new[] { typeof(Func<ITacObject, ITacObject>), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        //});

        //private ConstructorInfo tacMethod_Complex_SimpleConstructor (System.Type output)
        //{
        //    return typeof(TacMethod_Complex_Simple<>).MakeGenericType(output).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(typeof(ITacObject),output), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        //}

        //private ConstructorInfo tacMethod_Simple_ComplexConstructor(System.Type input)
        //{
        //    return typeof(TacMethod_Simple_Complex<>).MakeGenericType(input).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(input,typeof(ITacObject)), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        //}

        //private ConstructorInfo tacMethod_Simple_SimpleConstructor (System.Type input, System.Type output)
        //{
        //    return typeof(TacMethod_Simple_Simple<,>).MakeGenericType(input,output).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(input, output), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        //}


        internal IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> CurrentContext()
        {
            foreach (var item in stack.Reverse())
            {
                if (item.SafeIs(out IEntryPointDefinition entryPointDefinition))
                {
                    return OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(entryPointDefinition);
                }

                if (item.SafeIs(out IImplementationDefinition implementationDefinition))
                {
                    return OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(implementationDefinition);
                }

                if (item.SafeIs(out IInternalMethodDefinition internalMethodDefinition))
                {
                    return OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(internalMethodDefinition);
                }

                if (item.SafeIs(out IRootScope rootScope))
                {
                    return OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(rootScope);
                }
            }
            throw new Exception("context not found");
        }

        internal bool IsLocal(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> context, IMemberDefinition member, out IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> orType)
        {
            // is a local but not in anyone's closure          
            orType = default;
            return !extensionLookup.InAnyCLosure(member) && memberKindLookup.IsLocal(member, out orType);

        }

        // note an enclosed argument counts as an enclosed local 
        // a local that needs to go in a closure
        internal bool IsEnclosedLocal(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> context, IMemberDefinition member, out IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> orType)
        {
            // or:
            // is our local but in someone's closure
            // is our argument but in someone's closure
            if (extensionLookup.InAnyCLosure(member))
            {

                context.Is(out ICodeElement lookingFor);

                if (memberKindLookup.IsLocal(member, out orType) && orType.Is(out ICodeElement found) && lookingFor.Equals(found))
                {
                    return true;
                }
                if (memberKindLookup.IsArgument(member, out var argOrType) && argOrType.Is(out ICodeElement found2) && lookingFor.Equals(found2))
                {

                    orType = argOrType.SwitchReturns(
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(x),
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(x),
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(x));
                    return true;
                }
            }
            orType = default;
            return false;
        }

        internal bool IsArgument(IMemberDefinition member, out IOrType<IImplementationDefinition, IInternalMethodDefinition, IEntryPointDefinition> orType)
        {
            // is a argument but not in anyone's closure 
            orType = default;
            return !extensionLookup.InAnyCLosure(member) && memberKindLookup.IsArgument(member, out orType);
        }

        internal bool IsField(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> context, IMemberDefinition member, out IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition> orType)
        {

            if (memberKindLookup.IsField(member, out var orTypes))
            {
                context.Is(out ICodeElement lookingFor);

                if (orTypes.Any(x => x.Is(out ICodeElement found) && found.Equals(lookingFor)))
                {
                    orType = context.SwitchReturns(
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(x),
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(x),
                        x => OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(x),
                        x => throw new Exception("can't be roorScope"));
                    return true;
                }
            }
            orType = default;
            return false;
        }

        internal bool IsTacField(IMemberDefinition member, out IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> orType)
        {
            return memberKindLookup.IsTacField(member, out orType);
        }

        internal bool IsStaticField(IMemberDefinition member, out FieldInfo module)
        {
            return memberKindLookup.IsStaticField(member, out module);
        }


        public Nothing AssignOperation(IAssignOperation co)
        {

            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            // be careful this does not leave anything on the stack
            // =: in tac returns returns the value just saved
            // we need it not to do that if nothing is going to consume that
            var leaveOnStack = this.stack.Any() && this.stack.Last().SafeIs(out IOperation _);


            // {870866D9-D3EC-47B1-B7D3-6966EE651F5F}
            // storing and loading have a lot in commmon

            // the kind of thing the taget is define how we proceed
            if (co.Right.SafeIs(out IMemberReference memberReference))
            {
                var context = CurrentContext();


                if (IsArgument(memberReference.MemberDefinition, out var orTypeArg))
                {
                    // I only allow 1 argument 
                    // 0th arg is this
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    }
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Starg, (short)1);
                    return new Nothing();
                }
                else


                if (IsEnclosedLocal(context, memberReference.MemberDefinition, out var _))
                {

                    LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberReference.MemberDefinition));

                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    }

                    var field = typeof(Enclosed<>).MakeGenericType(typeCache[memberReference.MemberDefinition.Type]).GetField(nameof(Enclosed<int>.value));

                    generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
                    return new Nothing();
                }
                else


                if (IsLocal(context, memberReference.MemberDefinition, out var _))
                {
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    }
                    return StoreLocal(memberReference.MemberDefinition);
                }
                else


                if (IsTacField(memberReference.MemberDefinition, out var orTypeTacField))
                {

                    // we count on having a reference to the object already on the stack

                    // 1st parm, the new value
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                    var cSharpType = orTypeTacField.SwitchReturns(
                        x => typeCache[x.Returns()],
                        x => typeCache[x],
                        x => typeCache[x]);

                    return CallSet(leaveOnStack, memberReference, cSharpType);

                    throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                }
                else

                // field includes stuff on the closure
                if (IsField(context, memberReference.MemberDefinition, out var _))
                {
                    var realizedMethod = context.SwitchReturns(
                        entryPoint => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPoint)),
                        imp => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp)),
                        method => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method)),
                        root => throw new Exception("a root scope can't have a field"));

                    // this
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);

                    return realizedMethod.fieldOrFieldPair[memberReference.MemberDefinition].SwitchReturns(
                        field =>
                        {
                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                            StoreField(field, leaveOnStack, memberReference);

                            return new Nothing();
                        },
                        pair =>
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);

                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            StoreField(pair.path, leaveOnStack, memberReference);

                            return new Nothing();
                        },
                        pair =>
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);


                            // 1st parm, the new value
                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            return CallSet(leaveOnStack, memberReference, typeCache[co.Right.Returns()]);

                            throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                        });
                }
                else

                if (IsStaticField(memberReference.MemberDefinition, out var fieldInfo))
                {

                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);

                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                    if (leaveOnStack)
                    {

                        // TODO I could end up with many of this switching locals of the same type in one method
                        // i should probably store and reuse them
                        // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);

                        var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                        StoreLocal(loc.LocalIndex);

                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, fieldInfo);

                        LoadLocal(loc.LocalIndex);
                    }
                    else
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, fieldInfo);
                    }

                    return new Nothing();
                }

                throw new Exception("should have been one of those things...");
            }
            else if (co.Right.SafeIs(out IPathOperation path))
            {

                // the value
                // we evaulate this first and then store it in a local
                // (a > b).x =: (c > d).y
                // the user expects a > b to go before c > d
                // even tho it is the second parameter of setComplexMember/setSimpleMember
                co.Left.Convert(this.Push(co));

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[co.Left.Returns()], Guid.NewGuid());
                StoreLocal(loc.LocalIndex);

                // who we are calling it on
                path.Left.Convert(this.Push(co));

                // load the value out of the local
                LoadLocal(loc.LocalIndex);
                PossiblyConvert(co.Left.Returns(), co.Right.Returns());


                if (path.Right.SafeIs(out IMemberReference pathMemberReference))
                {

                    // this "b" inside a path like: a.b
                    // we count on "a" to have already been load

                    var returned = path.Left.Returns();

                    if (returned.SafeIs(out IInterfaceType _) || returned.SafeIs(out ITypeOr _))
                    {
                        return CallSet(leaveOnStack, pathMemberReference, typeCache[returned]);
                    }
                    else
                    {
                        throw new Exception("we are in a path, so it need to be something with members");
                    }
                }
                else
                {
                    throw new Exception("should be a reference");
                }
            }
            else
            {
                throw new Exception("if it is not a reference.... what is it?");
            }
        }

        private void StoreField(FieldInfo field, bool leaveOnStack, IMemberReference memberReference)
        {
            if (leaveOnStack)
            {
                // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                StoreLocal(loc.LocalIndex);

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);

                LoadLocal(loc.LocalIndex);
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
            }
        }

        private void InnerCallSet(IMemberReference memberReference, System.Type CSharpType)
        {
            generatorHolder.GetGeneratorAndUpdateStack(-2).EmitCall(OpCodes.Call, CSharpType.GetMethod($"set_{TypeVisitor.ConvertName(memberReference.MemberDefinition.Key.CastTo<NameKey>().Name)}"));
        }

        private Nothing CallSet(bool leaveOnStack, IMemberReference memberReference, System.Type CSharpType)
        {
            if (memberReference.MemberDefinition.Access == Access.ReadOnly)
            {
                throw new Exception("this should have benn handled inside assignment");
            }


            if (leaveOnStack)
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                StoreLocal(loc.LocalIndex);

                InnerCallSet(memberReference, CSharpType);
                LoadLocal(loc.LocalIndex);
            }
            else
            {
                InnerCallSet(memberReference, CSharpType);
            }
            return new Nothing();


            //if (typeCache[memberReference.MemberDefinition.Type] == typeof(ITacObject))
            //{
            //    switch (memberReference.MemberDefinition.Access)
            //    {
            //        case Access.ReadOnly:
            //            throw new Exception("this should have benn handled inside assignment");
            //        case Access.ReadWrite:
            //            generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexMemberReturn.Value : setComplexMember.Value);
            //            return new Nothing();
            //        case Access.WriteOnly:
            //            generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexWriteonlyMemberReturn.Value : setComplexWriteonlyMember.Value);
            //            return new Nothing();
            //        default:
            //            throw new Exception("that is unexpected");
            //    }
            //}
            //else
            //{
            //    generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, (leaveOnStack ? setSimpleMemberReturn.Value : setSimpleMember.Value).MakeGenericMethod(typeCache[memberReference.MemberDefinition.Type]));
            //    return new Nothing();
            //}
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            foreach (var local in codeElement.Scope.Members)
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[local.Value.Value.Type], local.Value.Value);
            }

            // this is nothing to MSIL
            return Walk(codeElement.Body, codeElement);
        }

        public Nothing ConstantBool(IConstantBool constantBool)
        {
            if (constantBool.Value)
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_0);
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_1);
            }
            return new Nothing();
        }
        public Nothing ConstantNumber(IConstantNumber codeElement)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_R8, codeElement.Value);
            return new Nothing();
        }
        public Nothing ConstantString(IConstantString co)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldstr, co.Value);
            return new Nothing();
        }
        public Nothing EmptyInstance(IEmptyInstance co)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldnull);
            return new Nothing();
        }
        public Nothing TypeDefinition(IInterfaceType codeElement) => new Nothing();


        public Nothing ElseOperation(IElseOperation co)
        {

            var next = this.Push(co);

            if (co.Operands[0].SafeIs(out IIfOperation myIf))
            {

                var nextNext = next.Push(myIf);
                var topOfElseLabel = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
                myIf.Operands[0].Convert(nextNext);
                // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                if (this.stack.Any() && this.stack.Last().SafeIs(out IOperation _))
                {
                    // we dup so that we return
                    // else in tac returns false if it ran, true otherwise
                    // so you can do
                    // ... else {} > someMethod
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    // this is a very important assumption
                    // the {} of the if CANNOT leave anything on the stack
                    // I don't think that should happen very often since each statement tend to clear it's stack
                    // often but not always, right here we are leaving something on the statck
                    // that is why we need to check something is consuming it 
                }
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, topOfElseLabel);
                myIf.Operands[1].Convert(nextNext);

                // if we are the last line in the method
                // say
                // ture then { 1 return } else { 0 return }
                // than the branch produces invalid code 
                if (IsLastLine(co))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(topOfElseLabel);
                    co.Operands[1].Convert(next);
                }
                else
                {
                    var bottomOfElse = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Br, bottomOfElse);
                    generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(topOfElseLabel);
                    co.Operands[1].Convert(next);
                    generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(bottomOfElse);
                }

                return new Nothing();
            }
            else
            {
                co.Left.Convert(next);
                // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                if (this.stack.Any() && this.stack.Last().SafeIs(out IOperation _))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                }

                var bottomOfElse = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, bottomOfElse);

                co.Right.Convert(next);
                generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(bottomOfElse);

                return new Nothing();
            }
        }

        private bool IsLastLine(ICodeElement co)
        {
            foreach (var item in stack.Reverse())
            {
                if (item is IOperation)
                {
                    return false;
                }
                if (item is IInternalMethodDefinition method)
                {
                    return method.Body.Last() == co;
                }
                if (item is IEntryPointDefinition entry)
                {
                    return entry.Body.Last() == co;
                }
                if (item is IBlockDefinition block)
                {
                    if (block.Body.Last() == co)
                    {
                        co = block;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            throw new Exception("I don't think you should get here");
        }

        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {



            var realizedMethod = realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPointDefinition));
            var myMethod = realizedMethod.type.DefineMethod(
                GenerateName(),
                MethodAttributes.Public,
                CallingConventions.HasThis,
                typeCache[entryPointDefinition.OutputType], new System.Type[] { typeCache[entryPointDefinition.InputType] });


            var gen = new DebuggableILGenerator(myMethod.GetILGenerator(), "main");

            // I need to declare the locals


            var inner = this.Push(entryPointDefinition, gen);
            inner.DeclareLocals(entryPointDefinition.Scope);
            foreach (var line in entryPointDefinition.Body)
            {
                line.Convert(inner);
            }

            // everything is in a method on rootType
            // ldarg_0 is this
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, realizedMethod.defaultConstructor);


            // pass stuff in to the closure
            // this is similar to the code in method but not quite the same
            PopulateTheClosure(entryPointDefinition, realizedMethod);

            // now I need to make a TacMethod or whatever

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, myMethod);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeCache[entryPointDefinition.InputType], typeCache[entryPointDefinition.OutputType]).GetConstructors().First());           // TODO lazy this reflection

            var mainField = typeof(TacCompilation<,>).MakeGenericType(typeCache[entryPointDefinition.InputType], typeCache[entryPointDefinition.OutputType]).GetField(nameof(TacCompilation<int, int>.main)) ?? throw new NullReferenceException("that field better exist");

            generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, mainField);

            return new Nothing();
        }

        private void DeclareLocals(IFinalizedScope scope)
        {
            var context = CurrentContext();

            foreach (var local in scope.Members)
            {
                if (IsLocal(context, local.Value.Value, out var _))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[local.Value.Value.Type], local.Value.Value);
                }
                else if (IsEnclosedLocal(context, local.Value.Value, out var _))
                {
                    var enclosedType = typeof(Enclosed<>).MakeGenericType(typeCache[local.Value.Value.Type]);

                    var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(enclosedType, local.Value.Value);

                    // for enclosed locals we also need to init them 

                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, enclosedType.GetConstructors().First());

                    StoreLocal(loc.LocalIndex);
                }
                else if (IsArgument(local.Value.Value, out var _))
                {
                    // do nothing
                }
                else
                {
                    throw new Exception("should really be a local or an enclosed local");
                }
            }
        }

        public Nothing IfTrueOperation(IIfOperation co)
        {
            var next = this.Push(co);

            var label = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
            co.Operands[0].Convert(next);
            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            if (this.stack.Any() && this.stack.Last().SafeIs(out IOperation _))
            {
                // we dup so that we return
                // if in tac returns true if it ran, false otherwise
                // so you can do
                // ... if {} > someMethod
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                // this is a very important assumption
                // the {} of the if CANNOT leave anything on the stack
                // I don't think that should happen very often since each statement tend to clear it's stack
                // often but not always, right here we are leaving something on the statck
                // that is why we need to check something is consuming it 
            }
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, label);
            co.Operands[1].Convert(next);
            generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(label);
            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition implementation)
        {
            return Walk(implementation.MethodBody, implementation);
        }

        public Nothing LastCallOperation(ILastCallOperation co)
        {
            return Walk(co.Operands, co);
        }

        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Clt);
            return new Nothing();
        }

        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            return new Nothing();
        }

        private IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> ConvertToMethodlike(ICodeElement frame)
        {
            if (frame.SafeIs(out IInternalMethodDefinition method))
            {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method);
            }
            if (frame.SafeIs(out IImplementationDefinition imp))
            {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp);
            }
            if (frame.SafeIs(out IEntryPointDefinition entry))
            {
                return OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entry);
            }
            throw new Exception("should have been one of those");
        }


        // TODO
        // AAaaa! member reference could be static!
        // could it tho?
        // module mean static -- horrible name
        // but how are they implemented
        // if they are not defined inside another module they are static member of some root thing
        // if they are not on root they are just objects

        // I need to make a root object to host entrypoint
        // and modules, but only modules
        // I need to review modules, they are dumb


        public Nothing MemberReferance(IMemberReference memberReference)
        {
            // we need to determine what type of member reference it is 
            // is it a local?
            // ldloc
            // is it an argument 
            // ldarg


            // {870866D9-D3EC-47B1-B7D3-6966EE651F5F}
            // storing and loading have a lot in commmon
            return EmitMemberReference(memberReference.MemberDefinition, false);

        }

        /// this does not look in the closure
        private Nothing EmitMemberReference(IMemberDefinition memberDefinition, bool forClosure)
        {

            var context = CurrentContext();


            if (IsArgument(memberDefinition, out var _))
            {
                if (forClosure)
                {
                    throw new Exception("a closure should not be picking up a arg");
                }

                // I only allow 1 argument 
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
                return new Nothing();
            }

            if (IsEnclosedLocal(context, memberDefinition, out var _))
            {
                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                if (!forClosure)
                {
                    var field = typeof(Enclosed<>).MakeGenericType(typeCache[memberDefinition.Type]).GetField(nameof(Enclosed<int>.value));
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, field);
                }
                return new Nothing();
            }

            if (IsLocal(context, memberDefinition, out var _))
            {
                if (forClosure)
                {
                    throw new Exception("a closure should not be picking up a local");
                }

                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                return new Nothing();
            }

            if (IsTacField(memberDefinition, out var orTypeTacField))
            {

                if (forClosure)
                {
                    // I think?
                    throw new Exception("a closure should not be picking up a tac field");
                }

                var cSharpType = orTypeTacField.SwitchReturns(
                    x => typeCache[x.Returns()],
                    x => typeCache[x],
                    x => typeCache[x]);

                // this "b" inside a path like: a.b
                // we count on "a" to have already been load

                return CallGet(memberDefinition, cSharpType);
            }

            if (IsField(context, memberDefinition, out var _))
            {
                var realizedMethod = context.SwitchReturns(
                    entryPoint => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPoint)),
                    imp => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp)),
                    method => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method)),
                    root => throw new Exception("a root scope can't have a field"));

                // this
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);

                return realizedMethod.fieldOrFieldPair[memberDefinition].SwitchReturns(
                    field =>
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, field);
                        return new Nothing();
                    },
                    pair =>
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);
                        if (!forClosure)
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.path);
                        }
                        return new Nothing();
                    },
                    pair =>
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);
                        if (!forClosure)
                        {
                            CallGet(memberDefinition, pair.funcField.FieldType);
                        }
                        return new Nothing();
                    });

            }

            if (IsStaticField(memberDefinition, out var fieldInfo))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);

                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, fieldInfo);

                return new Nothing();
            }


            throw new Exception("how did we end up here?");
        }

        private Nothing CallGet(IMemberDefinition memberDefinition, System.Type CSharpType)
        {
            if (memberDefinition.Access == Access.WriteOnly)
            {
                throw new Exception("this should nenver have been accessed");
            }
            generatorHolder.GetGeneratorAndUpdateStack(0).EmitCall(OpCodes.Call, CSharpType.GetMethod($"get_{TypeVisitor.ConvertName(memberDefinition.Key.CastTo<NameKey>().Name)}"));

            return new Nothing();


            //if (typeCache[memberDefinition.Type] == typeof(ITacObject))
            //{
            //    switch (memberDefinition.Access)
            //    {
            //        case Access.ReadOnly:
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexReadonlyMember.Value);
            //            return new Nothing();
            //        case Access.ReadWrite:
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexMember.Value);
            //            return new Nothing();
            //        case Access.WriteOnly:
            //            throw new Exception("this should have benn handled inside assignment");
            //        default:
            //            throw new Exception("that is unexpected");
            //    }
            //}
            //else
            //{
            //    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getSimpleMember.Value.MakeGenericMethod(typeCache[memberDefinition.Type]));
            //    return new Nothing();
            //}
        }

        //private readonly Lazy<MethodInfo> getComplexReadonlyMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.GetComplexReadonlyMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> getComplexMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.GetComplexMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> getSimpleMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.GetSimpleMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setComplexWriteonlyMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexWriteonlyMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setComplexMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setSimpleMember = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetSimpleMember)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setComplexWriteonlyMemberReturn = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexWriteonlyMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setComplexMemberReturn = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> setSimpleMemberReturn = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.SetSimpleMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        //});


        //private readonly Lazy<MethodInfo> callSimpleSimple = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Simple_Simple)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> callSimpleComplex = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Simple_Complex)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> callComplexSimple = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Complex_Simple)) ?? throw new NullReferenceException("should not be null!");
        //});

        //private readonly Lazy<MethodInfo> callComplexComplex = new Lazy<MethodInfo>(() =>
        //{
        //    return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Complex_Complex)) ?? throw new NullReferenceException("should not be null!");
        //});



        private void LoadInt(int value)
        {
            switch (value)
            {
                case 0:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_7);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4, value);
                    return;
            }
        }


        private void StoreLocal(int index)
        {
            switch (index)
            {
                case -1:
                    throw new Exception("that is not a good index");
                case 0:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stloc_3);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stloc_S, (byte)index);
                    return;
            }
        }


        private void LoadLocal(int index)
        {
            switch (index)
            {
                case 0:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldloc_3);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldloc_S, (byte)index);
                    return;
            }
        }

        //public System.Type ToITacObjectOrOject(System.Type type ) {
        //    if (type == typeof(ITacObject)) {
        //        return type;
        //    }
        //    return typeof(object);
        //}


        public Nothing MethodDefinition(IInternalMethodDefinition method)
        {

            var realizedMethod = realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method));
            var name = GenerateName();
            var myMethod = realizedMethod.type.DefineMethod(name, MethodAttributes.Public, CallingConventions.HasThis, typeCache[method.OutputType], new[] { typeCache[method.InputType] });

            var gen = new DebuggableILGenerator(myMethod.GetILGenerator(), name);

            var inner = this.Push(method, gen);
            // I need to declare the locals
            inner.DeclareLocals(method.Scope);
            // I might need to enclose the argument
            inner.EncloseArg(method);
            foreach (var line in method.Body)
            {
                line.Convert(inner);
            }

            // create new instance
            // get the default constuctor 
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, realizedMethod.defaultConstructor);


            // pass stuff in to the closure
            PopulateTheClosure(method, realizedMethod);

            // now I need to make a TacMethod or whatever

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, myMethod);

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(new System.Type[] { typeCache[method.InputType], typeCache[method.OutputType] }).GetConstructors().First());

            //GetVerifyableType(method.Returns());

            //generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj,);

            //if (typeCache[method.InputType] == typeof(ITacObject))
            //{
            //    if (typeCache[method.OutputType] == typeof(ITacObject))
            //    {

            //        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Complex_ComplexConstructor.Value);
            //    }
            //    else
            //    {
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Complex_SimpleConstructor(typeCache[method.OutputType]));
            //    }
            //}
            //else
            //{
            //    if (typeCache[method.OutputType] == typeof(ITacObject))
            //    {
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Simple_ComplexConstructor(typeCache[method.InputType]));
            //    }
            //    else
            //    {
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Simple_SimpleConstructor(typeCache[method.InputType], typeCache[method.OutputType]));
            //    }
            //}

            return new Nothing();
        }

        private void EncloseArg(IInternalMethodDefinition method)
        {
            if (IsEnclosedLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(method), method.ParameterDefinition, out var _))
            {
                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(method.ParameterDefinition));

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);

                var field = typeof(Enclosed<>).MakeGenericType(typeCache[method.ParameterDefinition.Type]).GetField(nameof(Enclosed<int>.value));

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
            }
        }

        private void PopulateTheClosure(ICodeElement method, RealizedMethod realizedMethod)
        {
            if (extensionLookup.TryGetClosure(method, out var ourClosure))
            {
                foreach (var member in ourClosure.closureMember)
                {

                    if (realizedMethod.fieldOrFieldPair.TryGetValue(member.Key, out var fieldInfoOr))
                    {

                        var fieldInfo = fieldInfoOr.SwitchReturns(fieldInfo => fieldInfo, pair => pair.funcField, pair => pair.funcField);

                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);

                        if (fieldInfoOr.Is3(out var _))
                        {
                            // an object was captured by the closure

                            // the object could have been captured by a series of closures
                            // object { x:=5 ; y = method { method { x return; } return }; };
                            var context = CurrentContext();
                            if (IsField(context, member.Key, out var _))
                            {
                                EmitMemberReference(member.Key, true);
                            }
                            else if (thisStack.TryPeek(out var localVariableInfo))
                            {
                                LoadLocal(localVariableInfo.LocalIndex);
                            }
                            else
                            {
                                throw new Exception("I don't know what's going on");
                            }
                        }
                        else
                        {
                            EmitMemberReference(member.Key, true);
                        }

                        // now we need to push the new value on to out closure 
                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, fieldInfo);

                    }
                }
            }
        }



        // {4E963BB1-1C86-4F75-BD4C-3F9BE16386A9}
        private static readonly Random random = new Random();
        private static string GenerateName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Mul_Ovf);
            return new Nothing();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {

            if (!co.Right.Returns().SafeIs(out IMethodType method))
            {
                throw new Exception("it has got to be a method");
            }

            var inType = typeCache[method.InputType];
            var outType = typeCache[method.OutputType];


            // the method and then the input on to the stack
            // but I need to evaluate them in the other order
            // I need a local

            // TODO I could probably reuse this local
            // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

            co.Left.Convert(this.Push(co));
            var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[co.Left.Returns()], Guid.NewGuid());
            StoreLocal(loc.LocalIndex);

            co.Right.Convert(this.Push(co));
            LoadLocal(loc.LocalIndex);

            // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            var leaveOnStack = this.stack.Any() && this.stack.Last().SafeIs(out IOperation _);

            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, typeof(Func<,>).MakeGenericType(inType, outType).GetMethod("Invoke"));
            if (!leaveOnStack)
            {
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            }

            //if (inType == typeof(ITacObject))
            //{
            //    if (outType == typeof(ITacObject))
            //    {
            //        PossiblyConvert(co.Left.Returns(), method.InputType);
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexComplex.Value);
            //        if (!leaveOnStack)
            //        {
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            //        }
            //    }
            //    else
            //    {
            //        PossiblyConvert(co.Left.Returns(), method.InputType);
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexSimple.Value.MakeGenericMethod(outType));
            //        if (!leaveOnStack)
            //        {
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            //        }
            //    }
            //}
            //else {
            //    if (outType == typeof(ITacObject))
            //    {
            //        PossiblyConvert(co.Left.Returns(), method.InputType);
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleComplex.Value.MakeGenericMethod(inType));
            //        if (!leaveOnStack)
            //        {
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            //        }
            //    }
            //    else
            //    {
            //        PossiblyConvert(co.Left.Returns(), method.InputType);
            //        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleSimple.Value.MakeGenericMethod(inType,outType));

            //        if (!leaveOnStack)
            //        {
            //            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            //        }
            //    }
            //}
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton @object)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, objectCache.GetOrThrow(@object).GetConstructor(new System.Type[] { }));

            // init members
            var next = this.Push(@object);
            foreach (var assignment in @object.Assignments)
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                assignment.Convert(next);
            }

            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            // TODO
            // this is a little ugly 
            // it would be better to dup one less time instead of poping
            var leaveOnStack = this.stack.Any() && this.stack.Last().SafeIs(out IOperation _);
            if (!leaveOnStack)
            {
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            }

            return new Nothing();
        }

        public Nothing PathOperation(IPathOperation path)
        {
            // all the goods here are inside
            return Walk(path.Operands, path);
        }

        public Nothing ReturnOperation(IReturnOperation co)
        {

            // we need to clear the evaluation stack except the value we are returning
            // this means tracking its depth

            while (generatorHolder.EvaluationStackDepth > 0)
            {
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            }

            co.Result.Convert(this.Push(co));

            // this could leave more than one thing on the stack!
            // so I really need to do the pop here!
            if (generatorHolder.EvaluationStackDepth > 1)
            {
                throw new Exception("you lazy!");
            }


            // there could be a conversion here!
            // we need to walk up the stack till we hit a method (or possibly an entrypoint)
            // and get it's output type

            foreach (var frame in stack.Reverse())
            {
                if (frame.SafeIs(out IInternalMethodDefinition method))
                {
                    PossiblyConvert(co.Result.Returns(), method.OutputType);
                    goto end;
                }


                if (frame.SafeIs(out IEntryPointDefinition entryPoint))
                {
                    PossiblyConvert(co.Result.Returns(), entryPoint.OutputType);
                    goto end;
                }
            }
        end:




            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ret);

            return new Nothing();
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Sub_Ovf);
            return new Nothing();
        }

        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            foreach (var local in tryAssignOperation.Scope.Members)
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[local.Value.Value.Type], local.Value.Value);
            }

            var topOfElseLabel = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
            var bottomOfElse = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();


            tryAssignOperation.Left.Convert(this.Push(tryAssignOperation));
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);

            var memberDef = tryAssignOperation.Right.SafeCastTo(out IMemberReference _).MemberDefinition;

            GetVerifyableType(typeCache[memberDef.Type]);

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.typeCache)));

            // I am just going to write this staticly in C#
            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Is)));

            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, topOfElseLabel);

            if (typeCache[memberDef.Type] == typeof(bool))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(bool));
            }
            else if (typeCache[memberDef.Type] == typeof(double))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(double));
            }
            else
            {
                // we need to put this type of the stack typeCache[memberDef.Type];
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldtoken, typeCache[memberDef.Type]);

                // type typeCache
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.typeCache)));

                // wrapsAndImplementsCache
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.wrapsAndImplementsCache)));
                
                generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Cast)));
            }

            var context = CurrentContext();

            if (IsLocal(context, memberDef, out var _))
            {
                StoreLocal(memberDef);
            }
            else if (IsEnclosedLocal(context, memberDef, out var _))
            {
                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));

                var field = typeof(Enclosed<>).MakeGenericType(typeCache[memberDef.Type]).GetField(nameof(Enclosed<int>.value));

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);

            }
            else
            {
                throw new Exception("should always be a local");
            }

            tryAssignOperation.Block.Convert(this.Push(tryAssignOperation));
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Br, bottomOfElse);

            // if false pop the one we pused
            generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(topOfElseLabel);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(bottomOfElse);

            return new Nothing();
        }

        private Nothing StoreLocal(IMemberDefinition memberDef)
        {
            StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));
            return new Nothing();
        }

        private Nothing Walk(IEnumerable<ICodeElement> elements, ICodeElement element)
        {
            var inner = this.Push(element);
            foreach (var line in elements)
            {
                line.Convert(inner);
            }

            return new Nothing();
        }

        public Nothing RootScope(IRootScope rootScope)
        {
            var inner = this.Push(rootScope);
            inner.DeclareLocals(rootScope.Scope);
            foreach (var line in rootScope.Assignments)
            {
                line.Convert(inner);
            }

            rootScope.EntryPoint.Convert(inner);

            return new Nothing();
        }
    }


    public static class AssemblyWalkerHelp
    {


        public static object TryAssignOperationHelper_Cast(object o, System.Type targetType, ConcurrentIndexed<System.Type, IVerifiableType> typeCache, ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache)
        {
            if (o == null)
            {
                return o;
            }
            if (o.SafeIs(out double _))
            {
                return o;
            }
            if (o.SafeIs(out string _))
            {
                return o;
            }
            if (o.SafeIs(out bool _))
            {
                return o;
            }

            // if it is a method...
            // we might need to wrap it to get the types right and wrapping might require emitting of conversion types 
            var oType = o.GetType();
            if (oType.IsGenericType && targetType.IsGenericType)
            {
                var genericOType = oType.GetGenericTypeDefinition();
                var genericTargetType = targetType.GetGenericTypeDefinition();
                if (genericOType == typeof(Func<,>) && genericTargetType == typeof(Func<,>))
                {
                    var oArgs = oType.GetGenericArguments();
                    var targetTypeArgs = targetType.GetGenericArguments();
                    if (targetTypeArgs[0].IsAssignableTo(oArgs[0])  && oArgs[1].IsAssignableTo(targetTypeArgs[1])) {
                        return o;
                    }
                    if (targetTypeArgs[0].IsAssignableTo(oArgs[0])) {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncO));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[1]);
                        return generic.Invoke(null, new object[] {o, typeCache, wrapsAndImplementsCache});
                    }
                    else 
                    if (oArgs[1].IsAssignableTo(targetTypeArgs[1])) {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncI));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[0]);
                        return generic.Invoke(null, new object[] { o, typeCache, wrapsAndImplementsCache });
                    }
                    else
                    {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncIO));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[0], oArgs[1]);
                        return generic.Invoke(null, new object[] { o, typeCache, wrapsAndImplementsCache });
                    }
                }
                else
                {
                    // tac generic type are erased
                    // a C# generic type would be wrapped when it entered Tac land
                    // I don't think this should ever be hit
                    throw new System.Exception("umm does it mean??");
                }
            }

            return EmitAndInstantiateConvertingType(o, targetType, typeCache, wrapsAndImplementsCache);
        }

        // I need to create a function of the right type
        // I can't do that from runtime times (the result of GetType() in perticular)
        // 
        public static Func<TIn, TOut> ConvertingFuncIO<TIn, TOut, TInInner, TOutInner>(Func<TInInner, TOutInner> inner, ConcurrentIndexed<System.Type, IVerifiableType> typeCache, ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache) {
            return (TIn x) => { return (TOut)EmitAndInstantiateConvertingType(inner((TInInner)EmitAndInstantiateConvertingType(x, typeof(TInInner), typeCache, wrapsAndImplementsCache)), typeof(TOut), typeCache, wrapsAndImplementsCache); };
        }
        public static Func<TIn, TOut> ConvertingFuncO<TIn, TOut, TOutInner>(Func<TIn, TOutInner> inner, ConcurrentIndexed<System.Type, IVerifiableType> typeCache, ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache)
        {
            return (TIn x) => { return (TOut)EmitAndInstantiateConvertingType(inner(x), typeof(TOut), typeCache, wrapsAndImplementsCache); };
        }
        public static Func<TIn, TOut> ConvertingFuncI<TIn, TOut, TInInner>(Func<TInInner, TOut> inner, ConcurrentIndexed<System.Type, IVerifiableType> typeCache, ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache)
        {
            return (TIn x) => { return inner((TInInner)EmitAndInstantiateConvertingType(x, typeof(TInInner), typeCache, wrapsAndImplementsCache)); };
        }

        public static bool TryAssignOperationHelper_Is(object o, IVerifiableType targetType, ConcurrentIndexed<System.Type, IVerifiableType> typeCache)
        {
            if (o == null && targetType.SafeIs(out IEmptyType _))
            {
                return true;
            }
            if (o.SafeIs(out double _))
            {
                return targetType.SafeIs(out INumberType _);
            }
            if (o.SafeIs(out string _))
            {
                return targetType.SafeIs(out IStringType _);
            }
            if (o.SafeIs(out bool _))
            {
                return targetType.SafeIs(out IBooleanType _);
            }

            var oType = o.GetType();
            if (oType.IsGenericType)
            {
                var genericOType = oType.GetGenericTypeDefinition();
                if (genericOType == typeof(Func<,>))
                {
                    var args = oType.GetGenericArguments();
                    return targetType.TryGetIO().Is(out var IO) &&
                        IO.input.TheyAreUs(typeCache[args[0]], new List<(IVerifiableType, IVerifiableType)>()) &&
                        typeCache[args[1]].TheyAreUs(IO.output, new List<(IVerifiableType, IVerifiableType)>());
                }
                else {
                    throw new System.Exception("umm does it mean??");
                }
            }

            // unwrap
            while (true)
            {
                var field = o.GetType().GetField(backingName);
                if (field == null)
                {
                    break;
                }
                o = field.GetValue(o);
            }

            return targetType.TheyAreUs(typeCache[o.GetType()], new List<(IVerifiableType, IVerifiableType)>());
        }

        public const string backingName = "__backing";
        //private static ConcurrentIndexed<(System.Type, System.Type), (TypeBuilder,IVerifiableType)> wrapsAndImplementsCache = new ConcurrentIndexed<(System.Type, System.Type), TypeBuilder>();

        // the thing that is being wrapped could be a wrapped it self
        // so we need peel off until we hit the base

        // for the members those tend to already be interfaces...
        // we can't get the backing and flatten
        // I guess if backing was an interface...
        // and we knew all these interface had it
        // we could pull the backing object
        // but we could known't the backings type at compile time
        // but... I think not knowing it at compile time is fine
        // we can call this at runtime (or a method like it that also does the wrapping) on the runtime type 
        // that is a real trade off tho.... 
        // on the one hand you have:
        // layored look ups: every wrapper wrappers around an inner wrapper
        // on ther other hand you have GetType() + dictionary look ups on all the members + possibly a lot of reflection  
        //
        // I mean we would try to flatten the top level "wrapped"
        // but trying to flatten every member seems like a bit much
        public static System.Type EmitTypeThatWrapsAndImplementsCompileTime(
            System.Type wrapped,
            System.Type implements,
            ModuleBuilder moduleBuilder,
            ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> wrapsAndImplementsCache)
        {

            // sometime you're good
            if (wrapped.IsAssignableTo(implements))
            {
                return wrapped;
            }

            var fillOut = false;

            var res = wrapsAndImplementsCache.GetOrAdd((wrapped, implements), () =>
           {
               var type = moduleBuilder.DefineType(wrapped.Name + "_" + implements.Name, TypeAttributes.Public, implements);

               fillOut = true;

               return type;

           });

            // we pull this out because of the danger of GetOrAdd(x inside a call to GetOrAdd(x
            // I suspect that would block indefinately
            if (fillOut)
            {

                res.AddInterfaceImplementation(implements);

                var backing = res.DefineField(AssemblyWalkerHelp.backingName, wrapped, FieldAttributes.Public);

                foreach (var propertyInfo in implements.GetProperties())
                {
                    CreateProperty(wrapped, (x, y) => EmitTypeThatWrapsAndImplementsCompileTime(x, y, moduleBuilder, wrapsAndImplementsCache), res, backing, propertyInfo);
                }
            }

            return res;
        }

        public static System.Type EmitTypeThatWrapsAndImplementsRunTime(
            System.Type wrapped,
            System.Type implements,
            ModuleBuilder moduleBuilder,
            ConcurrentIndexed<System.Type, IVerifiableType> typeCache,
            ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache)
        {

            // sometime you're good
            if (wrapped.IsAssignableTo(implements))
            {
                return wrapped;
            }

            TypeBuilder? fillOut = null;

            var res = wrapsAndImplementsCache.GetOrAdd((wrapped, implements), () =>
            {
                var type = moduleBuilder.DefineType(wrapped.Name + "_" + implements.Name, TypeAttributes.Public, implements);

                fillOut = type;

                typeCache[type] = typeCache[implements];

                return type;

            });

            // we pull this out because of the danger of GetOrAdd(x inside a call to GetOrAdd(x
            // I suspect that would block indefinately
            if (fillOut != null)
            {

                fillOut.AddInterfaceImplementation(implements);


                var backing = fillOut.DefineField(AssemblyWalkerHelp.backingName, wrapped, FieldAttributes.Public);

                foreach (var propertyInfo in implements.GetProperties())
                {
                    CreateProperty(wrapped, (x,y)=> EmitTypeThatWrapsAndImplementsRunTime(x,y,moduleBuilder,typeCache,wrapsAndImplementsCache), fillOut, backing, propertyInfo);
                }
                fillOut.CreateType();
            }

            return res;
        }

        private static void CreateProperty(System.Type wrapped, Func<System.Type, System.Type, System.Type> EmitTypeThatWrapsAndImplements, TypeBuilder? fillOut, FieldBuilder backing, PropertyInfo propertyInfo)
        {
            var property = fillOut.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new System.Type[0]);
            var backingFeild = wrapped.GetField("_" + propertyInfo.Name.ToLower());

            if (propertyInfo.CanRead)
            {
                var getter = fillOut.DefineMethod("get_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, propertyInfo.PropertyType, new System.Type[0]);
                var getGenerator = getter.GetILGenerator();

                var weAreConverting = !backingFeild.FieldType.IsAssignableTo(propertyInfo.PropertyType);
                IIsPossibly<System.Type> conversitionType = Possibly.IsNot<System.Type>();
                if (weAreConverting)
                {
                    conversitionType = Possibly.Is(EmitTypeThatWrapsAndImplements(backingFeild.FieldType, propertyInfo.PropertyType));
                    getGenerator.Emit(OpCodes.Newobj, conversitionType.GetOrThrow().GetConstructor(new System.Type[] { }));
                    getGenerator.Emit(OpCodes.Dup);
                }
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, backing);
                getGenerator.Emit(OpCodes.Ldfld, backingFeild);
                if (weAreConverting)
                {
                    getGenerator.Emit(OpCodes.Stfld, conversitionType.GetOrThrow().GetField(AssemblyWalkerHelp.backingName));
                }
                getGenerator.Emit(OpCodes.Ret);
                property.SetGetMethod(getter);
                fillOut.DefineMethodOverride(getter, propertyInfo.GetGetMethod());
            }
            if (propertyInfo.CanWrite)
            {
                var setter = fillOut.DefineMethod("set_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new System.Type[] { propertyInfo.PropertyType });
                var setGenerator = setter.GetILGenerator();
                setGenerator.Emit(OpCodes.Ldarg_0);

                var weAreConverting = !backingFeild.FieldType.IsAssignableFrom(propertyInfo.PropertyType);
                IIsPossibly<System.Type> conversitionType = Possibly.IsNot<System.Type>();
                if (weAreConverting)
                {
                    conversitionType = Possibly.Is(EmitTypeThatWrapsAndImplements(backingFeild.FieldType, propertyInfo.PropertyType));
                    setGenerator.Emit(OpCodes.Newobj, conversitionType.GetOrThrow().GetConstructor(new System.Type[] { }));
                    setGenerator.Emit(OpCodes.Dup);
                }
                setGenerator.Emit(OpCodes.Ldarg_1);
                if (weAreConverting)
                {
                    setGenerator.Emit(OpCodes.Stfld, conversitionType.GetOrThrow().GetField("backing"));
                }
                setGenerator.Emit(OpCodes.Stfld, backing);
                setGenerator.Emit(OpCodes.Ret);
                property.SetSetMethod(setter);
                fillOut.DefineMethodOverride(setter, propertyInfo.GetSetMethod());
            }
        }

        // this might need to creat it's module builder...
        public static object EmitAndInstantiateConvertingType(object from, System.Type implements, ConcurrentIndexed<System.Type, IVerifiableType> typeCache, ConcurrentIndexed<(System.Type, System.Type), System.Type> wrapsAndImplementsCache)
        {

            // TODO I really don't have to do this reflectively
            // backingName should be an interface
            while (true)
            {
                var field = from.GetType().GetField(backingName);
                if (field == null)
                {
                    break;
                }
                from = field.GetValue(from);
            }
            var assemblyName = new AssemblyName();
            assemblyName.Name = Compiler.GenerateName();
            var assembly = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assembly.DefineDynamicModule(Compiler.GenerateName());

            var resType = EmitTypeThatWrapsAndImplementsRunTime(from.GetType(), implements, moduleBuilder, typeCache, wrapsAndImplementsCache);

            var res = Activator.CreateInstance(resType);

            // TODO
            // this would be better if I had an interface
            var backingField = resType.GetField(backingName);
            backingField.SetValue(res, from);

            return res;
        }
    }
}
