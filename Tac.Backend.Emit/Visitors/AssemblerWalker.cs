using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Support;
using Tac.Backend.Emit.Visitors;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{

    public class Enclosed<T> {
        public T value;
    }


    public class IndexerList {

        public IIsPossibly<int> GetOrAdd(IVerifiableType fromType, IVerifiableType toType) {
            if (fromType.Equals( toType))
            {
                return Possibly.IsNot<int>();
            }

            var myIndexer = Indexer.Create(fromType, toType);
            if (myIndexer == null)
            {
                return Possibly.IsNot<int>();
            }

            var index = indexers.IndexOf(myIndexer);
            if (index != -1) {
                return Possibly.Is(index);
            }

            indexers.Add(myIndexer);

            return Possibly.Is(indexers.Count - 1);
        }

        public List<Indexer> indexers = new List<Indexer>();
    }

    public class VerifyableTypesList
    {

        public IIsPossibly<int> GetOrAdd(IVerifiableType type)
        {

            var index = types.IndexOf(type);
            if (index != -1)
            {
                return Possibly.Is(index);
            }

            types.Add(type);

            return Possibly.Is(types.Count - 1);
        }

        public List<IVerifiableType> types = new List<IVerifiableType>();
    }


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
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + method.Name  + "<" + string.Join<string>(", ", method.GetGenericArguments().Select(x => x.FullName)) + ">" + Environment.NewLine;
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
            var or = OrType.Make<Guid,IMemberDefinition>(member);
            if (locals.Contains(or)) {
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

            debugString += EvaluationStackDepth + ": " + Tabs() + "local, " + type.FullName + ", " +  id + Environment.NewLine;
            return backing.DeclareLocal(type);
        }

        internal int GetLocalIndex(Guid id) {

            var or = OrType.Make<Guid, IMemberDefinition>(id);
            var index = locals.IndexOf(or);
            if (index == -1) {
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

        private string Tabs() {
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

        public string GetDeubbingSting() {
            return debugString.ToString();
        }
    }

    class GeneratorHolder {


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

        public string DebugString() {
            return generator.GetOrThrow().GetDeubbingSting();
        }
    }


    public abstract class TacCompilation
    {
        public Indexer[] indexerArray;
        public IVerifiableType[] verifyableTypesArray;
        public abstract void Init();
    }
    public abstract class TacCompilation<Tin,Tout>: TacCompilation
    {

        public Func<Tin, Tout> main;
    } 



    class AssemblerVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly List<DebuggableILGenerator> gens ;
        private readonly ModuleBuilder moduleBuilder;
        public readonly IndexerList indexerList;
        public readonly VerifyableTypesList verifyableTypesList;



        private readonly MemberKindLookup memberKindLookup;
        private readonly ExtensionLookup extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        private readonly Dictionary<IVerifiableType, System.Type> typeCache;
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
            Dictionary<IVerifiableType, System.Type> typeCache,
            TypeBuilder rootType,
             FieldBuilder rootSelfField,
             IndexerList indexerList,
             VerifyableTypesList verifyableTypesList,
             RealizedMethodLookup realizedMethodLookup,
             List<DebuggableILGenerator> gens
            )
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this.generatorHolder = generatorHolder ?? throw new ArgumentNullException(nameof(generatorHolder));
            this.memberKindLookup = memberKindLookup ?? throw new ArgumentNullException(nameof(memberKindLookup));
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
            this.typeCache = typeCache ?? throw new ArgumentNullException(nameof(typeCache));
            this.rootType = rootType ?? throw new ArgumentNullException(nameof(rootType));
            this.rootSelfField = rootSelfField ?? throw new ArgumentNullException(nameof(rootSelfField));
            this.indexerList = indexerList ?? throw new ArgumentNullException(nameof(indexerList));
            this.verifyableTypesList = verifyableTypesList ?? throw new ArgumentNullException(nameof(verifyableTypesList));
            this.realizedMethodLookup = realizedMethodLookup ?? throw new ArgumentNullException(nameof(realizedMethodLookup));
            this.gens = gens ?? throw new ArgumentNullException(nameof(gens));
        }

        public static (AssemblerVisitor,Action) Create(
            MemberKindLookup memberKindLookup,
            ExtensionLookup extensionLookup,
            Dictionary<IVerifiableType, System.Type> typeCache,
            ModuleBuilder moduleBuilder,
            RealizedMethodLookup realizedMethodLookup,
            System.Type tin,
            System.Type tout)
        {
            var typebuilder = moduleBuilder.DefineType(GenerateName(), TypeAttributes.Public & TypeAttributes.Class, typeof(TacCompilation<,>).MakeGenericType(tin, tout));
            var selfField = typebuilder.DefineField(GenerateName() + "_self", typebuilder, FieldAttributes.Static | FieldAttributes.Public);

            var initMethod = typebuilder.DefineMethod(nameof(TacCompilation<int,int>.Init), MethodAttributes.Public | MethodAttributes.Virtual);
            typebuilder.DefineMethodOverride(initMethod, typeof(TacCompilation<,>).MakeGenericType(tin,tout).GetMethod(nameof(TacCompilation<int,int>.Init)));

            var gen = new DebuggableILGenerator(initMethod.GetILGenerator(), "Init");


            var gens = new List<DebuggableILGenerator> { gen };
            var generatorHolder = new GeneratorHolder(Possibly.Is(gen));

            // set the self field
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stsfld, selfField);

            return
                (new AssemblerVisitor(new List<ICodeElement>(), generatorHolder, memberKindLookup, extensionLookup, typeCache, typebuilder, selfField, new IndexerList(), new VerifyableTypesList(), realizedMethodLookup, gens), () =>
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
            return new AssemblerVisitor(list, generatorHolder,memberKindLookup,extensionLookup,typeCache,rootType,rootSelfField,indexerList,verifyableTypesList, realizedMethodLookup,gens);
        }


        public AssemblerVisitor Push(ICodeElement another, DebuggableILGenerator gen)
        {
            var list = stack.ToList();
            list.Add(another);
            gens.Add(gen);
            return new AssemblerVisitor(list, new GeneratorHolder(Possibly.Is(gen)), memberKindLookup,extensionLookup,typeCache,rootType,rootSelfField, indexerList, verifyableTypesList, realizedMethodLookup,gens);
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
            if (toType.SafeIs(out IAnyType _)) {
                if (fromType.SafeIs(out IAnyType _))
                {
                    return;
                }
                else {
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
            if (toType.SafeIs(out ITypeOr _)) {

                if (typeCache.TryGetValue(fromType, out var toCSharpeType))
                {
                    if (new[] { typeof(bool), typeof(double) }.Contains(toCSharpeType))
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, toCSharpeType);
                        return;
                    }
                }
                if (typeCache[toType] == typeof(object)) {
                    return;
                }
            }

            // we create the indexer now
            // and we put it in a big array
            // this is kind of a hack
            // it means the code that I am emitting cannot be run standalone
            // it will only work inline
            if (indexerList.GetOrAdd(fromType, toType).SafeIs(out IIsDefinately<int> definateIndexer))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, indexersField.Value);
                LoadInt(definateIndexer.Value);
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ldelem_Ref);

                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, castConstructor.Value);
            }
        }

        private void GetVerifyableType(IVerifiableType toType)
        {
            if (verifyableTypesList.GetOrAdd(toType).SafeIs(out IIsDefinately<int> definateType))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typesField.Value);
                LoadInt(definateType.Value);
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ldelem_Ref);
            }
            else
            {
                throw new Exception("you shit");
            }
        }

        private readonly Lazy<FieldInfo>  indexersField = new Lazy<FieldInfo>(() =>
        {
            return typeof(TacCompilation).GetField(nameof(TacCompilation.indexerArray)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<FieldInfo> typesField = new Lazy<FieldInfo>(() =>
        {
            return typeof(TacCompilation).GetField(nameof(TacCompilation.verifyableTypesArray)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> castConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacCastObject).GetConstructor(new[] { typeof(ITacObject), typeof(Indexer)}) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> tacObjectConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacObject).GetConstructor(new System.Type[] { }) ?? throw new NullReferenceException("should not be null!");
        });


        private readonly Lazy<ConstructorInfo> tacMethod_Complex_ComplexConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacMethod_Complex_Complex).GetConstructor(new[] { typeof(Func<ITacObject, ITacObject>), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        private ConstructorInfo tacMethod_Complex_SimpleConstructor (System.Type output)
        {
            return typeof(TacMethod_Complex_Simple<>).MakeGenericType(output).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(typeof(ITacObject),output), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        }

        private ConstructorInfo tacMethod_Simple_ComplexConstructor(System.Type input)
        {
            return typeof(TacMethod_Simple_Complex<>).MakeGenericType(input).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(input,typeof(ITacObject)), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        }

        private ConstructorInfo tacMethod_Simple_SimpleConstructor (System.Type input, System.Type output)
        {
            return typeof(TacMethod_Simple_Simple<,>).MakeGenericType(input,output).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(input, output), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        }


        internal IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> CurrentContext() {
            foreach (var item in stack.Reverse())
            {
                if (item.SafeIs(out IEntryPointDefinition entryPointDefinition)) {
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
            if (extensionLookup.InAnyCLosure(member)) {

                context.Is(out ICodeElement lookingFor);

                if (memberKindLookup.IsLocal(member, out orType) && orType.Is(out ICodeElement found)  && lookingFor.Equals(found))
                {
                    return true;
                }
                if (memberKindLookup.IsArgument(member, out var argOrType) && argOrType.Is(out ICodeElement found2) && lookingFor.Equals(found2)) {

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

            if (memberKindLookup.IsField(member, out var orTypes)) {
                context.Is(out ICodeElement lookingFor );

                if (orTypes.Any(x => x.Is(out ICodeElement found) && found.Equals(lookingFor))) {
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
                } else


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
                } else


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


                if (IsTacField(memberReference.MemberDefinition, out var orTypeTacField))
                {
                    
                    // we count on having a reference to the object already on the stack

                    // 1st parm, the new value
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                    var index = orTypeTacField.SwitchReturns(
                        obj => Array.IndexOf(obj.Scope.Members.OrderBy(x => ((NameKey)x.Key).Name).Select(x => x.Value.Value).ToArray(), memberReference.MemberDefinition),
                        type => throw new Exception("this should be part of a path"),
                        orType => throw new Exception("this should be part of a path"));

                    if (!stack.Last().SafeIs<ICodeElement, IObjectDefiniton>(out var _))
                    {
                        throw new Exception("this should only happen in object init");
                    }

                    if (index < 0) {
                        throw new Exception("index should not be negetive!");
                    }

                    // second parm, the index
                    LoadInt(index);
                    return CallSet(leaveOnStack, memberReference);

                    throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                } else

                // field includes stuff on the closure
                if (IsField(context,memberReference.MemberDefinition, out var _))
                {
                    var realizedMethod = context.SwitchReturns(
                        entryPoint => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPoint)),
                        imp =>  realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp)),
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

                            // second parm, the index
                            var index = Array.IndexOf(pair.scope.Members.OrderBy(x => ((NameKey)x.Key).Name).Select(x => x.Value.Value).ToArray(), memberReference.MemberDefinition);

                            if (index < 0)
                            {
                                throw new Exception("index should not be negetive!");
                            }

                            LoadInt(index);
                            return CallSet(leaveOnStack, memberReference);

                            throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                        });
                } else

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

                    if (returned.SafeIs(out IInterfaceType obj))
                    {
                        var index = Array.IndexOf(obj.Members.OrderBy(x => ((NameKey)x.Key).Name).ToArray(), pathMemberReference.MemberDefinition);

                        if (index < 0)
                        {
                            throw new Exception("index should not be negetive!");
                        }

                        // second parm, the index
                        LoadInt(index);
                        return CallSet(leaveOnStack, pathMemberReference);
                    }
                    else if (returned.SafeIs(out ITypeOr typeOr))
                    {
                        var index = Array.IndexOf(typeOr.Members.OrderBy(x => ((NameKey)x.Key).Name).ToArray(), pathMemberReference.MemberDefinition);

                        if (index < 0)
                        {
                            throw new Exception("index should not be negetive!");
                        }

                        // second parm, the index
                        LoadInt(index);
                        return CallSet(leaveOnStack, pathMemberReference);
                    }
                    else {
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

        private Nothing CallSet(bool leaveOnStack, IMemberReference memberReference)
        {
            if (typeCache[memberReference.MemberDefinition.Type] == typeof(ITacObject))
            {
                switch (memberReference.MemberDefinition.Access)
                {
                    case Access.ReadOnly:
                        throw new Exception("this should have benn handled inside assignment");
                    case Access.ReadWrite:
                        generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexMemberReturn.Value : setComplexMember.Value);
                        return new Nothing();
                    case Access.WriteOnly:
                        generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexWriteonlyMemberReturn.Value : setComplexWriteonlyMember.Value);
                        return new Nothing();
                    default:
                        throw new Exception("that is unexpected");
                }
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, (leaveOnStack ? setSimpleMemberReturn.Value : setSimpleMember.Value).MakeGenericMethod(typeCache[memberReference.MemberDefinition.Type]));
                return new Nothing();
            }
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            foreach (var local in codeElement.Scope.Members)
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[local.Value.Value.Type],local.Value.Value);
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
            else {
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
                if (item is IOperation) {
                    return false;
                }
                if (item is IInternalMethodDefinition  method) {
                    return method.Body.Last() == co;
                }
                if (item is IEntryPointDefinition entry)
                {
                    return entry.Body.Last() == co;
                }
                if (item is IBlockDefinition block) {
                    if (block.Body.Last() == co)
                    {
                        co = block;
                    }
                    else {
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

            var mainField = typeof(TacCompilation<,>).MakeGenericType(typeCache[entryPointDefinition.InputType],typeCache[entryPointDefinition.OutputType]).GetField(nameof(TacCompilation<int,int>.main)) ?? throw new NullReferenceException("that field better exist");

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
                else if (IsArgument(local.Value.Value,out var _)) { 
                    // do nothing
                }else
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
            return  EmitMemberReference(memberReference.MemberDefinition,false);

        }

        /// this does not look in the closure
        private Nothing EmitMemberReference(IMemberDefinition memberDefinition, bool forClosure) {

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

            if (IsLocal(context,memberDefinition, out var _))
            {
                if (forClosure)
                {
                    throw new Exception("a closure should not be picking up a local");
                }

                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                return new Nothing();
            }

            if (IsTacField(memberDefinition, out var orTypeTacField)) {

                if (forClosure)
                {
                    // I think?
                    throw new Exception("a closure should not be picking up a tac field");
                }

                var index = orTypeTacField.SwitchReturns(
                    obj => Array.IndexOf(obj.Scope.Members.OrderBy(x => ((NameKey)x.Key).Name).Select(x => x.Value.Value).ToArray(), memberDefinition),
                    type => Array.IndexOf(type.Members.OrderBy(x => ((NameKey)x.Key).Name).ToArray(), memberDefinition),
                    orType => Array.IndexOf(orType.Members.OrderBy(x => ((NameKey)x.Key).Name).ToArray(), memberDefinition));

                if (index < 0)
                {
                    throw new Exception("index should not be negetive!");
                }

                // this "b" inside a path like: a.b
                // we count on "a" to have already been load

                LoadInt(index);
                return CallGet(memberDefinition);
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
                            var index = Array.IndexOf(pair.scope.Members.OrderBy(x => ((NameKey)x.Key).Name).Select(x => x.Value.Value).ToArray(), memberDefinition);

                            if (index < 0)
                            {
                                throw new Exception("index should not be negetive!");
                            }

                            LoadInt(index);
                            CallGet(memberDefinition);
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

        private Nothing CallGet(IMemberDefinition memberDefinition)
        {
            if (typeCache[memberDefinition.Type] == typeof(ITacObject))
            {
                switch (memberDefinition.Access)
                {
                    case Access.ReadOnly:
                        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexReadonlyMember.Value);
                        return new Nothing();
                    case Access.ReadWrite:
                        generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexMember.Value);
                        return new Nothing();
                    case Access.WriteOnly:
                        throw new Exception("this should have benn handled inside assignment");
                    default:
                        throw new Exception("that is unexpected");
                }
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getSimpleMember.Value.MakeGenericMethod(typeCache[memberDefinition.Type]));
                return new Nothing();
            }
        }

        private readonly Lazy<MethodInfo> getComplexReadonlyMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.GetComplexReadonlyMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> getComplexMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.GetComplexMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> getSimpleMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.GetSimpleMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setComplexWriteonlyMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexWriteonlyMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setComplexMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setSimpleMember = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetSimpleMember)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setComplexWriteonlyMemberReturn = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexWriteonlyMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setComplexMemberReturn = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetComplexMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> setSimpleMemberReturn = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.SetSimpleMemberReturn)) ?? throw new NullReferenceException("should not be null!");
        });


        private readonly Lazy<MethodInfo> callSimpleSimple = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Simple_Simple)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> callSimpleComplex = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Simple_Complex)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> callComplexSimple = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Complex_Simple)) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<MethodInfo> callComplexComplex = new Lazy<MethodInfo>(() =>
        {
            return typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Complex_Complex)) ?? throw new NullReferenceException("should not be null!");
        });



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

        public System.Type ToITacObjectOrOject(System.Type type ) {
            if (type == typeof(ITacObject)) {
                return type;
            }
            return typeof(object);
        }


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


            // TODO lazy all this reflection
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(new System.Type[] { typeCache[method.InputType], typeCache[method.OutputType] }).GetConstructors().First());
            GetVerifyableType(method.Returns());

            if (typeCache[method.InputType] == typeof(ITacObject))
            {
                if (typeCache[method.OutputType] == typeof(ITacObject))
                {

                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Complex_ComplexConstructor.Value);
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Complex_SimpleConstructor(typeCache[method.OutputType]));
                }
            }
            else
            {
                if (typeCache[method.OutputType] == typeof(ITacObject))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Simple_ComplexConstructor(typeCache[method.InputType]));
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Newobj, tacMethod_Simple_SimpleConstructor(typeCache[method.InputType], typeCache[method.OutputType]));
                }
            }

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
                            else {
                                throw new Exception("I don't know what's going on");
                            }
                        }
                        else { 
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

            if (inType == typeof(ITacObject))
            {
                if (outType == typeof(ITacObject))
                {
                    PossiblyConvert(co.Left.Returns(), method.InputType);
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexComplex.Value);
                    if (!leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
                else
                {
                    PossiblyConvert(co.Left.Returns(), method.InputType);
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexSimple.Value.MakeGenericMethod(outType));
                    if (!leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
            }
            else {
                if (outType == typeof(ITacObject))
                {
                    PossiblyConvert(co.Left.Returns(), method.InputType);
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleComplex.Value.MakeGenericMethod(inType));
                    if (!leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
                else
                {
                    PossiblyConvert(co.Left.Returns(), method.InputType);
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleSimple.Value.MakeGenericMethod(inType,outType));
                   
                    if (!leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
            }
            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton @object)
        {
            // ok, an object is an array 

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, tacObjectConstructor.Value);



            // itit field
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                GetVerifyableType(@object.Returns());
                var field = typeof(TacObject).GetField(nameof(TacObject.type)) ?? throw new NullReferenceException("that field better exist");
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
            }

            // init member
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                LoadInt(@object.Scope.Members.Count);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newarr, typeof(object));
                var field = typeof(TacObject).GetField(nameof(TacObject.members)) ?? throw new NullReferenceException("that field better exist");
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
            }

            // store this
            var thisLocal = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeof(ITacObject) , Guid.NewGuid());
            thisStack.Push(thisLocal);

            // init members
            var next = this.Push(@object);
            foreach (var assignment in @object.Assignments)
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                assignment.Convert(next);
            }

            thisStack.Pop();

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

            while (generatorHolder.EvaluationStackDepth > 0) {
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
            }

            co.Result.Convert(this.Push(co));

            // this could leave more than one thing on the stack!
            // so I really need to do the pop here!
            if (generatorHolder.EvaluationStackDepth > 1) {
                throw new Exception("you lazy!");
            }


            // there could be a conversion here!
            // we need to walk up the stack till we hit a method (or possibly an entrypoint)
            // and get it's output type

            foreach (var frame in stack.Reverse())
            {
                if (frame.SafeIs(out IInternalMethodDefinition method)) {
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

            GetVerifyableType(memberDef.Type);

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
                GetVerifyableType(memberDef.Type);
                generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Cast)));
            }

            var context = CurrentContext();

            if (IsLocal(context,memberDef, out var _))
            {
                StoreLocal(memberDef);
            }
            else if (IsEnclosedLocal(context, memberDef, out var _))
            {
                LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));

                var field = typeof(Enclosed<>).MakeGenericType(typeCache[memberDef.Type]).GetField(nameof(Enclosed<int>.value));

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);

            } else {
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


    public static class AssemblyWalkerHelp {


        public static object TryAssignOperationHelper_Cast(object o, IVerifiableType targetType)
        {
            if (o == null)
            {
                return o;
            }
            if (o.SafeIs(out ITacObject tacObject))
            {
                if (tacObject.TacType() == targetType)
                {
                    return o;
                }
                // we need to unroll all the casts and work with the root object
                while (tacObject is TacCastObject cast)
                {
                    tacObject = cast.@object;
                }
                return new TacCastObject(tacObject, Indexer.Create(tacObject.TacType(), targetType));
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
            throw new NotImplementedException();
        }

        public static bool TryAssignOperationHelper_Is(object o, IVerifiableType targetType)
        {
            if (o == null && targetType.SafeIs(out IEmptyType _)) {
                return true;
            }
            if (o.SafeIs(out ITacObject verifiableType))
            {


                //// for a cast type verifiableType.TacType() will return the type it was cast to
                //// it needs to return the base type!
                //throw new Exception("this is a bug!");
                
                return targetType.TheyAreUs(verifiableType.TacType(), new List<(IVerifiableType, IVerifiableType)>());
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
            throw new NotImplementedException();
        }
    }
}
