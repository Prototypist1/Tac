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
using Tac.Backend.Emit.Visitors;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Prototypist.TaskChain;

namespace Tac.Backend.Emit.Walkers
{
    /// <summary>
    /// encosed is used for locals or fields that are picked up in a closure
    /// </summary>
    public class Enclosed<T>
    {
        public T value;
    }

    //public class IndexerList
    //{

    //    public IIsPossibly<int> GetOrAdd(IVerifiableType fromType, IVerifiableType toType)
    //    {
    //        if (fromType.Equals(toType))
    //        {
    //            return Possibly.IsNot<int>();
    //        }

    //        var myIndexer = Indexer.Create(fromType, toType);
    //        if (myIndexer == null)
    //        {
    //            return Possibly.IsNot<int>();
    //        }

    //        var index = indexers.IndexOf(myIndexer);
    //        if (index != -1)
    //        {
    //            return Possibly.Is(index);
    //        }

    //        indexers.Add(myIndexer);

    //        return Possibly.Is(indexers.Count - 1);
    //    }

    //    public List<Indexer> indexers = new List<Indexer>();
    //}

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
            debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + method.Name + (method.GetGenericArguments().Any() ? "<" + string.Join<string>(", ", method.GetGenericArguments().Select(x => x.FullName)) + ">" : "") + " " + method.DeclaringType.Name + Environment.NewLine;
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

        //internal void EmitCall(OpCode code, MethodInfo methodInfo)
        //{
        //    debugString += EvaluationStackDepth + ": " + Tabs() + code.ToString() + ", " + methodInfo.Name + "<" + string.Join<string>(", ", methodInfo.GetGenericArguments().Select(x => x.FullName)) + ">" + " " + methodInfo.DeclaringType.Name +  Environment.NewLine;
        //    backing.Emit(code, methodInfo);
        //}

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

    public class GeneratorHolder
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
        public RunTimeTypeTracker runTimeTypeTracker;
        public abstract void Init();
    }

    public abstract class TacCompilation<Tin, Tout> : TacCompilation
    {

        public Func<Tin, Tout> main;
    }

    public abstract class TacCompilation<Tin, Tout, TDependencies> : TacCompilation<Tin, Tout>
    {

        public TDependencies dependencies;
    }

    class AssemblerVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly List<DebuggableILGenerator> gens;
        private readonly ModuleBuilder moduleBuilder;
        //public readonly IndexerList indexerList;
        //public readonly VerifyableTypesList verifyableTypesList;



        private readonly MemberKindLookup memberKindLookup;
        private readonly WhoDefinedMemberByMethodlike extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        private readonly AssemblerTypeTracker typeTracker;
        //private readonly ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes;
        public readonly TypeBuilder rootType;
        public readonly FieldBuilder rootSelfField;
        public readonly System.Type dependenciesType;
        public readonly FieldInfo dependencyField;

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
            WhoDefinedMemberByMethodlike extensionLookup,
            AssemblerTypeTracker typeTracker,
            //ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes,
            TypeBuilder rootType,
             FieldBuilder rootSelfField,
             //IndexerList indexerList,
             //VerifyableTypesList verifyableTypesList,
             RealizedMethodLookup realizedMethodLookup,
             List<DebuggableILGenerator> gens,
             ModuleBuilder moduleBuilder,
             FieldInfo dependencyField,
             System.Type dependenciesType
            )
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this.generatorHolder = generatorHolder ?? throw new ArgumentNullException(nameof(generatorHolder));
            this.memberKindLookup = memberKindLookup ?? throw new ArgumentNullException(nameof(memberKindLookup));
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
            this.typeTracker = typeTracker ?? throw new ArgumentNullException(nameof(typeTracker));
            //this.conversionTypes = conversionTypes ?? throw new ArgumentNullException(nameof(conversionTypes));
            this.rootType = rootType ?? throw new ArgumentNullException(nameof(rootType));
            this.rootSelfField = rootSelfField ?? throw new ArgumentNullException(nameof(rootSelfField));
            //this.indexerList = indexerList ?? throw new ArgumentNullException(nameof(indexerList));
            //this.verifyableTypesList = verifyableTypesList ?? throw new ArgumentNullException(nameof(verifyableTypesList));
            this.realizedMethodLookup = realizedMethodLookup ?? throw new ArgumentNullException(nameof(realizedMethodLookup));
            this.gens = gens ?? throw new ArgumentNullException(nameof(gens));
            this.moduleBuilder = moduleBuilder ?? throw new ArgumentNullException(nameof(moduleBuilder));
            this.dependencyField = dependencyField ?? throw new ArgumentNullException(nameof(dependencyField));
            this.dependenciesType = dependenciesType ?? throw new ArgumentNullException(nameof(dependenciesType));
        }

        public static (AssemblerVisitor, Action) Create(
            MemberKindLookup memberKindLookup,
            WhoDefinedMemberByMethodlike extensionLookup,
            AssemblerTypeTracker typeTracker,
            ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> conversionTypes,
            ModuleBuilder moduleBuilder,
            RealizedMethodLookup realizedMethodLookup,
            System.Type tin,
            System.Type tout,
            List<DebuggableILGenerator> gens,
            System.Type dependenciesType)
        {

            var typebuilder = moduleBuilder.DefineType(GenerateName(), TypeAttributes.Public & TypeAttributes.Class, typeof(TacCompilation<,,>).MakeGenericType(tin, tout, dependenciesType));
            var selfField = typebuilder.DefineField(GenerateName() + "_self", typebuilder, FieldAttributes.Static | FieldAttributes.Public);
            //var typeCacheField = typebuilder.DefineField(GenerateName() + "_typeCache", typeof(ConcurrentIndexed<System.Type, IVerifiableType>), FieldAttributes.Static | FieldAttributes.Public);

            var initMethod = typebuilder.DefineMethod(nameof(TacCompilation<int, int>.Init), MethodAttributes.Public | MethodAttributes.Virtual);
            typebuilder.DefineMethodOverride(initMethod, typeof(TacCompilation<,>).MakeGenericType(tin, tout).GetMethod(nameof(TacCompilation<int, int>.Init)));

            var gen = new DebuggableILGenerator(initMethod.GetILGenerator(), "Init");


            gens.Add(gen);
            var generatorHolder = new GeneratorHolder(Possibly.Is(gen));

            // set the self field
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stsfld, selfField);
            // set the 
            //generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
            //generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Stsfld, typeCacheField);

            //.MakeGenericType(tin, tout, dependenciesType) ... werid that I don't need this... or maybe I do?
            // is the problem that dependenciesType is an early form that has not yet been created?
            var dependencyField = typeof(TacCompilation<,,>).MakeGenericType(tin, tout, dependenciesType).GetField(nameof(TacCompilation<int, int, object>.dependencies))!;
            return
                (new AssemblerVisitor(new List<ICodeElement>(), generatorHolder, memberKindLookup, extensionLookup, typeTracker, /*conversionTypes,*/ typebuilder, selfField, /*new IndexerList(),*/ /*new VerifyableTypesList(),*/ realizedMethodLookup, gens, moduleBuilder, dependencyField, dependenciesType), () =>
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
            return new AssemblerVisitor(list, generatorHolder, memberKindLookup, extensionLookup, typeTracker, /*conversionTypes,*/ rootType, rootSelfField, /*indexerList,*/ /*verifyableTypesList,*/ realizedMethodLookup, gens, moduleBuilder, dependencyField, dependenciesType);
        }


        public AssemblerVisitor Push(ICodeElement another, DebuggableILGenerator gen)
        {
            var list = stack.ToList();
            list.Add(another);
            gens.Add(gen);
            return new AssemblerVisitor(list, new GeneratorHolder(Possibly.Is(gen)), memberKindLookup, extensionLookup, typeTracker, /*conversionTypes,*/ rootType, rootSelfField, /*indexerList,*/ /*verifyableTypesList,*/ realizedMethodLookup, gens, moduleBuilder, dependencyField, dependenciesType);
        }

        public Nothing AddOperation(IAddOperation co)
        {

            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Add_Ovf);
            return new Nothing();
        }

        private void GetVerifyableType(System.Type toType)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.runTimeTypeTracker)));

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldtoken, toType);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Call, typeof(System.Type).GetMethod(nameof(System.Type.GetTypeFromHandle)));

            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Callvirt, typeof(RunTimeTypeTracker).GetMethod(nameof(RunTimeTypeTracker.LookUp)));

        }

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
            return !extensionLookup.InAnyClosure(member) && memberKindLookup.IsLocal(member, out orType);

        }

        // note an enclosed argument counts as an enclosed local 
        // a local that needs to go in a closure
        internal bool IsEnclosedLocal(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> context, IMemberDefinition member, out IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> orType)
        {
            // or:
            // is our local but in someone's closure
            // is our argument but in someone's closure
            if (extensionLookup.InAnyClosure(member))
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
            return !extensionLookup.InAnyClosure(member) && memberKindLookup.IsArgument(member, out orType);
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

        //internal bool IsTacField(IMemberDefinition member, out IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> orType)
        //{
        //    return memberKindLookup.IsTacField(member, out orType);
        //}

        //internal bool IsStaticField(IMemberDefinition member, out FieldInfo module)
        //{
        //    return memberKindLookup.IsStaticField(member, out module);
        //}


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

                    AssemblyWalkerHelp.LoadLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberReference.MemberDefinition));

                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                    }

                    var field = typeof(Enclosed<>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(memberReference.MemberDefinition.Type)).GetField(nameof(Enclosed<int>.value));

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


                if (memberKindLookup.IsTacField(memberReference.MemberDefinition, out var orTypeTacField))
                {

                    // we count on having a reference to the object already on the stack

                    // 1st parm, the new value
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                    var cSharpType = orTypeTacField.SwitchReturns(
                        x => typeTracker.ResolvePossiblyPrimitive(x.Returns()),
                        x => typeTracker.ResolvePossiblyPrimitive(x),
                        x => typeTracker.ResolvePossiblyPrimitive(x));

                    return CallSet(leaveOnStack, memberReference, cSharpType);

                    throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                }
                else if (IsField(context, memberReference.MemberDefinition, out var _)) // field includes stuff on the closure
                {
                    var realizedMethod = context.SwitchReturns(
                        entryPoint => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPoint)),
                        imp => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(imp)),
                        method => realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(method)),
                        root => throw new Exception("a root scope can't have a field"));

                    // this
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);

                    return realizedMethod.fieldOrFieldPair[memberReference.MemberDefinition].SwitchReturns(
                        // it's on "this"
                        // it could be an implementations context
                        field =>
                        {
                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                            StoreField(field, leaveOnStack, memberReference);

                            return new Nothing();
                        },
                        // it's on a field that is on "this"
                        // probably it is on the clouse
                        // clouse fields are of type Enclosed<T>
                        // so that they can be accessed by the place they were defined and the method that picked them up
                        pair =>
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);

                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                            StoreField(pair.path, leaveOnStack, memberReference);

                            return new Nothing();
                        },
                        // it's a property of "this"
                        // take
                        // y := object { x := 5; f = method [int;int] { return x; }
                        // our object "y" is captured by the closure of the method
                        // and x is a propertry
                        pair =>
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, pair.funcField);


                            // 1st parm, the new value
                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            return CallSet(leaveOnStack, memberReference, typeTracker.ResolvePossiblyPrimitive(co.Right.Returns()));

                            throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                        });
                }
                else

                if (memberKindLookup.IsStaticField(memberReference.MemberDefinition, out var fieldInfo))
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

                        var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(memberReference.MemberDefinition.Type), Guid.NewGuid());
                        AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);

                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, fieldInfo);

                        AssemblyWalkerHelp.LoadLocal(generatorHolder, loc.LocalIndex);
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

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(co.Left.Returns()), Guid.NewGuid());
                AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);

                // who we are calling it on
                path.Left.Convert(this.Push(co));

                // load the value out of the local
                AssemblyWalkerHelp.LoadLocal(generatorHolder, loc.LocalIndex);
                PossiblyConvert(co.Left.Returns(), co.Right.Returns());


                if (path.Right.SafeIs(out IMemberReference pathMemberReference))
                {

                    // this "b" inside a path like: a.b
                    // we count on "a" to have already been load

                    var returned = path.Left.Returns();

                    if (returned.SafeIs(out IInterfaceType _) || returned.SafeIs(out ITypeOr _))
                    {
                        return CallSet(leaveOnStack, pathMemberReference, typeTracker.ResolvePossiblyPrimitive(returned));
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

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(memberReference.MemberDefinition.Type), Guid.NewGuid());
                AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);

                AssemblyWalkerHelp.LoadLocal(generatorHolder, loc.LocalIndex);
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
            }
        }

        private void InnerCallSet(IMemberReference memberReference, System.Type CSharpType)
        {
            generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Callvirt, CSharpType.GetMethod($"set_{TypeTracker.ConvertName(memberReference.MemberDefinition.Key.CastTo<NameKey>().Name)}"));
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
                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(memberReference.MemberDefinition.Type), Guid.NewGuid());
                AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);

                InnerCallSet(memberReference, CSharpType);
                AssemblyWalkerHelp.LoadLocal(generatorHolder, loc.LocalIndex);
            }
            else
            {
                InnerCallSet(memberReference, CSharpType);
            }
            return new Nothing();

        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            foreach (var local in codeElement.Scope.Members)
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(local.Value.Value.Type), local.Value.Value);
            }

            // this is nothing to MSIL
            return Walk(codeElement.Body, codeElement);
        }

        public Nothing ConstantBool(IConstantBool constantBool)
        {
            if (constantBool.Value)
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldc_I4_0);
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
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brtrue, bottomOfElse);

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
                typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.OutputType),
                new System.Type[] { typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.InputType) });


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
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.InputType), typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.OutputType)).GetConstructors().First());           // TODO lazy this reflection

            var mainField = typeof(TacCompilation<,>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.InputType), typeTracker.ResolvePossiblyPrimitive(entryPointDefinition.OutputType)).GetField(nameof(TacCompilation<int, int>.main)) ?? throw new NullReferenceException("that field better exist");

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
                    generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(local.Value.Value.Type), local.Value.Value);
                }
                else if (IsEnclosedLocal(context, local.Value.Value, out var _))
                {
                    var enclosedType = typeof(Enclosed<>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(local.Value.Value.Type));

                    var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(enclosedType, local.Value.Value);

                    // for enclosed locals we also need to init them 

                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, enclosedType.GetConstructors().First());

                    AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);
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
                AssemblyWalkerHelp.LoadLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                if (!forClosure)
                {
                    var field = typeof(Enclosed<>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(memberDefinition.Type)).GetField(nameof(Enclosed<int>.value));
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

                AssemblyWalkerHelp.LoadLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                return new Nothing();
            }

            if (memberKindLookup.IsTacField(memberDefinition, out var orTypeTacField))
            {

                if (forClosure)
                {
                    // I think?
                    throw new Exception("a closure should not be picking up a tac field");
                }

                var cSharpType = orTypeTacField.SwitchReturns(
                    x => typeTracker.ResolvePossiblyPrimitive(x.Returns()),
                    x => typeTracker.ResolvePossiblyPrimitive(x),
                    x => typeTracker.ResolvePossiblyPrimitive(x));

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
            if (memberKindLookup.IsStaticField(memberDefinition, out var fieldInfo))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);

                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, fieldInfo);

                return new Nothing();
            }

            if (memberKindLookup.IsDependency(memberDefinition))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, dependencyField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, dependenciesType.GetField(TypeTracker.ConvertName(memberDefinition.Key.SafeCastTo(out NameKey _).Name)));
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
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Callvirt, CSharpType.GetMethod($"get_{TypeTracker.ConvertName(memberDefinition.Key.CastTo<NameKey>().Name)}"));

            return new Nothing();
        }

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
            var myMethod = realizedMethod.type.DefineMethod(name, MethodAttributes.Public, CallingConventions.HasThis, typeTracker.ResolvePossiblyPrimitive(method.OutputType), new[] { typeTracker.ResolvePossiblyPrimitive(method.InputType) });

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

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(new System.Type[] { typeTracker.ResolvePossiblyPrimitive(method.InputType), typeTracker.ResolvePossiblyPrimitive(method.OutputType) }).GetConstructors().First());

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
                AssemblyWalkerHelp.LoadLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(method.ParameterDefinition));

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);

                var field = typeof(Enclosed<>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(method.ParameterDefinition.Type)).GetField(nameof(Enclosed<int>.value));

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
                                AssemblyWalkerHelp.LoadLocal(generatorHolder, localVariableInfo.LocalIndex);
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
        public static string GenerateName()
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

            var inType = typeTracker.ResolvePossiblyPrimitive(method.InputType);
            var outType = typeTracker.ResolvePossiblyPrimitive(method.OutputType);


            // the method and then the input on to the stack
            // but I need to evaluate them in the other order
            // I need a local

            // TODO I could probably reuse this local
            // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

            co.Left.Convert(this.Push(co));
            var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(co.Left.Returns()), Guid.NewGuid());
            AssemblyWalkerHelp.StoreLocal(generatorHolder, loc.LocalIndex);

            co.Right.Convert(this.Push(co));
            AssemblyWalkerHelp.LoadLocal(generatorHolder, loc.LocalIndex);

            // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            var leaveOnStack = this.stack.Any() && this.stack.Last().SafeIs(out IOperation _);

            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Callvirt, typeof(Func<,>).MakeGenericType(inType, outType).GetMethod("Invoke"));
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
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, typeTracker.ResolveObject(@object).GetConstructor(new System.Type[] { }));

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
                generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeTracker.ResolvePossiblyPrimitive(local.Value.Value.Type), local.Value.Value);
            }

            var topOfElseLabel = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
            var bottomOfElse = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();


            tryAssignOperation.Left.Convert(this.Push(tryAssignOperation));

            // box primitives if you need to
            var returns = tryAssignOperation.Left.Returns();
            if (returns.SafeIs(out INumberType _))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, typeof(double));
            }
            if (returns.SafeIs(out IBooleanType _))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, typeof(bool));
            }

            var memberDef = tryAssignOperation.Right.SafeCastTo(out IMemberReference _).MemberDefinition;

            GetVerifyableType(typeTracker.ResolvePossiblyPrimitive(memberDef.Type));

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.runTimeTypeTracker)));

            // I am just going to write this staticly in C#
            generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Is)));

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);


            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(ValueTuple<object,bool>).GetField(nameof(ValueTuple<object, bool>.Item2)));

            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, topOfElseLabel);

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(ValueTuple<object, bool>).GetField(nameof(ValueTuple<object, bool>.Item1)));

            if (typeTracker.ResolvePossiblyPrimitive(memberDef.Type) == typeof(bool))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(bool));

            }
            else if (typeTracker.ResolvePossiblyPrimitive(memberDef.Type) == typeof(double))
            {
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(double));
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldtoken, typeTracker.ResolvePossiblyPrimitive(memberDef.Type));
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Call, typeof(System.Type).GetMethod(nameof(System.Type.GetTypeFromHandle)));

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.runTimeTypeTracker)));

                // wrapsAndImplementsCache
                //generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldsfld, rootSelfField);
                //generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, typeof(TacCompilation).GetField(nameof(TacCompilation.wrapsAndImplementsCache)));

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Cast)));
            }

            var context = CurrentContext();

            if (IsLocal(context, memberDef, out var _))
            {
                StoreLocal(memberDef);
            }
            else if (IsEnclosedLocal(context, memberDef, out var _))
            {
                AssemblyWalkerHelp.LoadLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));

                var field = typeof(Enclosed<>).MakeGenericType(typeTracker.ResolvePossiblyPrimitive(memberDef.Type)).GetField(nameof(Enclosed<int>.value));

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
            AssemblyWalkerHelp.StoreLocal(generatorHolder, generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));
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

        void PossiblyConvert(IVerifiableType fromType, IVerifiableType toType)
        {
            AssemblyWalkerHelp.EmitConvertIfNeededCompileTime(typeTracker.ResolvePossiblyPrimitive(fromType), typeTracker.ResolvePossiblyPrimitive(toType), typeTracker, generatorHolder, moduleBuilder, x => gens.Add(x));
        }
    }

    public static class AssemblyWalkerHelp
    {



        internal static void StoreLocal(GeneratorHolder generatorHolder, int index)
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


        internal static void LoadLocal(GeneratorHolder generatorHolder, int index)
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

        // assumes from is loaded on to the stack
        public static void EmitConvertIfNeededCompileTime(
            System.Type fromCSharpeType,
            System.Type toCSharpeType,
            AssemblerTypeTracker typeTracker,
            GeneratorHolder generatorHolder,
            ModuleBuilder moduleBuilder,
            Action<DebuggableILGenerator> addGen)
        {

            // handle "any"
            if (toCSharpeType == typeof(object))
            {
                if (new[] { typeof(bool), typeof(double) }.Contains(fromCSharpeType))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, fromCSharpeType);
                }
                return;
            }

            // remember, anything is assignable to object but some stuff needs boxing
            if (fromCSharpeType.IsAssignableTo(toCSharpeType))
            {
                return;
            }

            if (fromCSharpeType.IsGenericType && fromCSharpeType.GetGenericTypeDefinition() == typeof(Func<,>)
                && toCSharpeType.IsGenericType && toCSharpeType.GetGenericTypeDefinition() == typeof(Func<,>))
            {

                TypeBuilder? typeBuilder = null;

                var conversionType = typeTracker.methodConversionCache.GetOrAdd((fromCSharpeType, toCSharpeType), () =>
                {
                    var name = Compiler.GenerateName();
                    typeBuilder = moduleBuilder.DefineType(name);


                    return typeBuilder;
                });


                // we can't call GetOrAdd inside GetOrAdd it might block forever
                // which we might if the type we are building out reference it self
                // so we build it up after 
                if (typeBuilder != null)
                {

                    var fromInput = fromCSharpeType.GetGenericArguments()[0];
                    var fromOutput = fromCSharpeType.GetGenericArguments()[1];
                    var toInput = toCSharpeType.GetGenericArguments()[0];
                    var toOutput = toCSharpeType.GetGenericArguments()[1];

                    var field = typeBuilder.DefineField(Compiler.GenerateName(), fromCSharpeType, FieldAttributes.Private | FieldAttributes.InitOnly);
                    var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig, CallingConventions.Standard, new[] { fromCSharpeType });
                    var ctorConstructorIL = new DebuggableILGenerator(constructor.GetILGenerator(), ".ctor of " + typeBuilder.Name);
                    addGen(ctorConstructorIL);
                    var ctorGenHolder = new GeneratorHolder(Possibly.Is(ctorConstructorIL));
                    ctorGenHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
                    ctorGenHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Call, typeof(object).GetConstructors().First());
                    ctorGenHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
                    ctorGenHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
                    ctorGenHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);
                    ctorGenHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ret);

                    var methodName = Compiler.GenerateName();
                    var wrapped = typeBuilder.DefineMethod(Compiler.GenerateName(), MethodAttributes.Public | MethodAttributes.HideBySig, toOutput, new[] { toInput });
                    var methodConstructorIL = new DebuggableILGenerator(wrapped.GetILGenerator(), methodName + " of " + typeBuilder.Name);
                    addGen(methodConstructorIL);
                    var methodGenHolder = new GeneratorHolder(Possibly.Is(methodConstructorIL));
                    methodGenHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
                    methodGenHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, field);
                    methodGenHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);
                    EmitConvertIfNeededCompileTime(toInput, fromInput, typeTracker, methodGenHolder, moduleBuilder, addGen);
                    var invoke = fromCSharpeType.GetMethod("Invoke");
                    methodGenHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Callvirt, invoke);
                    EmitConvertIfNeededCompileTime(fromOutput, toOutput, typeTracker, methodGenHolder, moduleBuilder, addGen);
                    methodGenHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ret);

                    typeBuilder.CreateType();

                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, constructor);
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, wrapped);
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, toCSharpeType.GetConstructors().First());           // TODO lazy this reflection 
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, conversionType.GetConstructors().Single());
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, conversionType.GetMethods().Single());
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, toCSharpeType.GetConstructors().First());
                }
            }
            else
            {

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(fromCSharpeType, Guid.NewGuid());
                StoreLocal(generatorHolder, loc.LocalIndex);

                var type = EmitTypeThatWrapsAndImplementsCompileTime(fromCSharpeType, toCSharpeType, typeTracker, generatorHolder, moduleBuilder, typeTracker.conversionCache, addGen);

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, type.GetConstructor(new System.Type[] { }));
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                LoadLocal(generatorHolder, loc.LocalIndex);
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, type.GetField(AssemblyWalkerHelp.backingName));
            }
        }

        public static System.Type EmitTypeThatWrapsAndImplementsCompileTime(
            System.Type wrapped,
            System.Type implements,
            AssemblerTypeTracker typeTracker,
            GeneratorHolder generatorHolder,
            ModuleBuilder moduleBuilder,
            ConcurrentIndexed<(System.Type, System.Type), TypeBuilder> wrapsAndImplementsCache,
            Action<DebuggableILGenerator> addGen)
        {

            var fillOut = false;

            var res = wrapsAndImplementsCache.GetOrAdd((wrapped, implements), () =>
            {
                var type = moduleBuilder.DefineType(wrapped.Name + "_as_" + implements.Name, TypeAttributes.Public, null);

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
                    CreateProperty(wrapped, (x, y, gen) => EmitConvertIfNeededCompileTime(x, y, typeTracker, gen, moduleBuilder, addGen), res, backing, propertyInfo, addGen);
                }
                res.CreateType();
            }

            return res;
        }


        public static object TryAssignOperationHelper_Cast(object o, System.Type targetType, RunTimeTypeTracker typeTracker)
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
                    if (targetTypeArgs[0].IsAssignableTo(oArgs[0]) && oArgs[1].IsAssignableTo(targetTypeArgs[1]))
                    {
                        return o;
                    }
                    if (targetTypeArgs[0].IsAssignableTo(oArgs[0]))
                    {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncO));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[1]);
                        return generic.Invoke(null, new object[] { o, typeTracker });
                    }
                    else
                    if (oArgs[1].IsAssignableTo(targetTypeArgs[1]))
                    {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncI));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[0]);
                        return generic.Invoke(null, new object[] { o, typeTracker });
                    }
                    else
                    {
                        MethodInfo method = typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.ConvertingFuncIO));
                        MethodInfo generic = method.MakeGenericMethod(targetTypeArgs[0], targetTypeArgs[1], oArgs[0], oArgs[1]);
                        return generic.Invoke(null, new object[] { o, typeTracker });
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

            return EmitAndInstantiateConvertingType(o, targetType, typeTracker);
        }

        // I need to create a function of the right type
        // I can't do that from runtime times (the result of GetType() in perticular)
        // 
        public static Func<TIn, TOut> ConvertingFuncIO<TIn, TOut, TInInner, TOutInner>(Func<TInInner, TOutInner> inner, RunTimeTypeTracker typeTracker)
        {
            return (TIn x) => { return (TOut)TryAssignOperationHelper_Cast(inner((TInInner)TryAssignOperationHelper_Cast(x, typeof(TInInner), typeTracker)), typeof(TOut), typeTracker); };
        }
        public static Func<TIn, TOut> ConvertingFuncO<TIn, TOut, TOutInner>(Func<TIn, TOutInner> inner, RunTimeTypeTracker typeTracker)
        {
            return (TIn x) => { return (TOut)TryAssignOperationHelper_Cast(inner(x), typeof(TOut), typeTracker); };
        }
        public static Func<TIn, TOut> ConvertingFuncI<TIn, TOut, TInInner>(Func<TInInner, TOut> inner, RunTimeTypeTracker typeTracker)
        {
            return (TIn x) => { return inner((TInInner)TryAssignOperationHelper_Cast(x, typeof(TInInner), typeTracker)); };
        }

        public static void EmitConvertWhenNeededRunTime(
          System.Type fromCSharpeType,
          System.Type toCSharpeType,
          RunTimeTypeTracker typeTracker,
          GeneratorHolder generatorHolder,
          ModuleBuilder moduleBuilder)
        {

            // handle "any"
            if (toCSharpeType == typeof(object))
            {
                if (new[] { typeof(bool), typeof(double) }.Contains(fromCSharpeType))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, fromCSharpeType);
                }
            }

            // remember, anything is assignable to object but some stuff needs boxing
            if (fromCSharpeType.IsAssignableTo(toCSharpeType))
            {
                return;
            }

            if (fromCSharpeType.IsGenericType && fromCSharpeType.GetGenericTypeDefinition() == typeof(Func<,>)
                && toCSharpeType.IsGenericType && toCSharpeType.GetGenericTypeDefinition() == typeof(Func<,>))
            {

                TypeBuilder? typeBuilder = null;

                var conversionType = typeTracker.methodConversionCache.GetOrAdd((fromCSharpeType, toCSharpeType), () =>
                {
                    var name = Compiler.GenerateName();
                    typeBuilder = moduleBuilder.DefineType(name);

                    typeTracker.Add(typeBuilder, typeTracker.LookUp(toCSharpeType));

                    return typeBuilder;
                });


                // we can't call GetOrAdd inside GetOrAdd it might block forever
                // which we might if the type we are building out reference it self
                // so we build it up after 
                if (typeBuilder != null)
                {

                    var fromInput = fromCSharpeType.GetGenericArguments()[0];
                    var fromOutput = fromCSharpeType.GetGenericArguments()[1];
                    var toInput = toCSharpeType.GetGenericArguments()[0];
                    var toOutput = toCSharpeType.GetGenericArguments()[1];

                    var field = typeBuilder.DefineField(Compiler.GenerateName(), fromCSharpeType, FieldAttributes.Private | FieldAttributes.InitOnly);
                    var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.HideBySig, CallingConventions.Standard, new[] { fromCSharpeType });
                    var ctorConstructorIL = new DebuggableILGenerator(constructor.GetILGenerator(), ".ctor of " + typeBuilder.Name);

                    ctorConstructorIL.Emit(OpCodes.Ldarg_0);
                    ctorConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
                    ctorConstructorIL.Emit(OpCodes.Ldarg_0);
                    ctorConstructorIL.Emit(OpCodes.Ldarg_1);
                    ctorConstructorIL.Emit(OpCodes.Stfld, field);
                    ctorConstructorIL.Emit(OpCodes.Ret);

                    var methodName = Compiler.GenerateName();
                    var wrapped = typeBuilder.DefineMethod(Compiler.GenerateName(), MethodAttributes.Public | MethodAttributes.HideBySig, toOutput, new[] { toInput });
                    var methodConstructorIL = new DebuggableILGenerator(wrapped.GetILGenerator(), methodName + " of " + typeBuilder.Name);

                    methodConstructorIL.Emit(OpCodes.Ldarg_0);
                    methodConstructorIL.Emit(OpCodes.Ldfld, field);
                    methodConstructorIL.Emit(OpCodes.Ldarg_1);
                    EmitConvertWhenNeededRunTime(toInput, fromInput, typeTracker, generatorHolder, moduleBuilder);
                    var invoke = fromCSharpeType.GetMethod("Invoke");
                    methodConstructorIL.Emit(OpCodes.Callvirt, invoke);
                    EmitConvertWhenNeededRunTime(fromOutput, toOutput, typeTracker, generatorHolder, moduleBuilder);
                    methodConstructorIL.Emit(OpCodes.Ret);

                    // this needs to hit reguardless 
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, constructor);
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, wrapped);
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, toCSharpeType.GetConstructors().First());           // TODO lazy this reflection 

                    typeBuilder.CreateType();
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, conversionType.GetConstructors().Single());
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, conversionType.GetMethods().Single());
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newobj, toCSharpeType.GetConstructors().First());
                }
            }

            {

                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(fromCSharpeType, Guid.NewGuid());
                StoreLocal(generatorHolder, loc.LocalIndex);

                var type = EmitTypeThatWrapsAndImplementsRunTime(fromCSharpeType, toCSharpeType, moduleBuilder, typeTracker);

                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Newobj, type.GetConstructor(new System.Type[] { }));
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                LoadLocal(generatorHolder, loc.LocalIndex);
                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, type.GetField(AssemblyWalkerHelp.backingName));
            }
        }


        // this might need to creat it's module builder...
        public static object EmitAndInstantiateConvertingType(
            object from,
            System.Type implements,
            RunTimeTypeTracker typeTracker)
        {

            // TODO I really don't have to do this reflectively
            // backingName should be an interface
            // sorta duplicate
            // {82834BE1-06E2-4EFC-8F37-B9ADCB4381BD}
            // .. not sure we ever actually need to unwrap
            // TryAssignOperationHelper_Is would be called first and that unwraps
            while (true)
            {
                // early exit let's not make a dynamic assembly if we don't have to..
                if ((from?.GetType() ?? typeof(Empty)).IsAssignableTo(implements))
                {
                    return from;
                }

                if (from == null)
                {
                    break;
                }

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

            var wrapped = from?.GetType() ?? typeof(Empty);
            var resType = EmitTypeThatWrapsAndImplementsRunTime(wrapped, implements, moduleBuilder, typeTracker); ;

            var res = Activator.CreateInstance(resType);

            // TODO
            // this would be better if I had an interface
            var backingField = resType.GetField(backingName);
            backingField.SetValue(res, from);

            return res;
        }


        /// <returns>null when false, </returns>
        public static (object, bool) TryAssignOperationHelper_Is(object o, IVerifiableType targetType, RunTimeTypeTracker typeCache)
        {
            if (o == null)
            {
                return (o, targetType.SafeIs(out IEmptyType _));
            }
            if (o.SafeIs(out double _))
            {
                return (o, targetType.SafeIs(out INumberType _));
            }
            if (o.SafeIs(out string _))
            {
                return (o, targetType.SafeIs(out IStringType _));
            }
            if (o.SafeIs(out bool _))
            {
                return (o, targetType.SafeIs(out IBooleanType _));
            }

            var oType = o.GetType();
            if (oType.IsGenericType)
            {
                var genericOType = oType.GetGenericTypeDefinition();
                if (genericOType == typeof(Func<,>))
                {
                    var args = oType.GetGenericArguments();
                    if (targetType.TryGetIO().Is(out var IO) &&
                        IO.input.TheyAreUs(typeCache.LookUp(args[0]), new List<(IVerifiableType, IVerifiableType)>()) &&
                        typeCache.LookUp(args[1]).TheyAreUs(IO.output, new List<(IVerifiableType, IVerifiableType)>()))
                    {
                        return (o, true);
                    }
                    else
                    {
                        return (o, false);
                    }
                }
                else
                {
                    throw new System.Exception("umm does it mean??");
                }
            }

            // unwrap
            var unwrapped = Unwrap(o);

            return (unwrapped, targetType.TheyAreUs(typeCache.LookUp(unwrapped.GetType()), new List<(IVerifiableType, IVerifiableType)>()));
        }

        // sorta duplicate
        // {82834BE1-06E2-4EFC-8F37-B9ADCB4381BD}
        private static object Unwrap(object o)
        {
            while (true)
            {
                if (o == null)
                {
                    return new Empty();
                }

                var field = o.GetType().GetField(backingName);
                if (field == null)
                {
                    break;
                }
                o = field.GetValue(o);
            }
            return o;
        }

        public const string backingName = "__backing";


        // note this doesn't early exit
        // if wrapped is implements
        // it goes ahead and wraps it anyway
        // it is
        public static System.Type EmitTypeThatWrapsAndImplementsRunTime(
            System.Type wrapped,
            System.Type implements,
            ModuleBuilder moduleBuilder,
            RunTimeTypeTracker typeTracker
            )
        {

            TypeBuilder? fillOut = null;

            var res = typeTracker.conversionCache.GetOrAdd((wrapped, implements), () =>
            {
                var type = moduleBuilder.DefineType(wrapped.Name + "_as_" + implements.Name, TypeAttributes.Public, null);

                fillOut = type;

                typeTracker.Add(type, typeTracker.LookUp(implements));

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
                    CreateProperty(wrapped, (x, y, gen) => EmitConvertWhenNeededRunTime(x, y, typeTracker, gen, moduleBuilder), fillOut, backing, propertyInfo, x => { });
                }
                fillOut.CreateType();
            }

            return res;
        }

        private static void CreateProperty(
            System.Type wrapped,
            Action<System.Type, System.Type, GeneratorHolder> convert,
            TypeBuilder? fillOut,
            FieldBuilder backing,
            PropertyInfo propertyInfo,
            Action<DebuggableILGenerator> addGen)
        {
            var property = fillOut.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new System.Type[0]);
            //var backingFeild = wrapped.GetMethod("get_" + propertyInfo.Name.ToLower());

            var wrappedProperty = wrapped.GetProperty(propertyInfo.Name);

            //var tacFromMember = wrappedVerfiable.TryGetMember(new NameKey(propertyInfo.Name), new List<(IVerifiableType, IVerifiableType)>()).GetOrThrow();
            //var tacToMember = targetVerfiable.TryGetMember(new NameKey(propertyInfo.Name), new List<(IVerifiableType, IVerifiableType)>()).GetOrThrow();

            if (propertyInfo.CanRead)
            {
                var getter = fillOut.DefineMethod("get_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot, propertyInfo.PropertyType, new System.Type[0]);
                var getGenerator = new DebuggableILGenerator(getter.GetILGenerator(), "get_" + propertyInfo.Name + " of " + propertyInfo.DeclaringType.Name + " on " + fillOut.Name);
                addGen(getGenerator);
                var holder = new GeneratorHolder(Possibly.Is(getGenerator));


                holder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
                holder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, backing);
                holder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Callvirt, wrappedProperty.GetGetMethod());

                convert(wrappedProperty.PropertyType, property.PropertyType, holder);

                holder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Ret);
                property.SetGetMethod(getter);
                fillOut.DefineMethodOverride(getter, propertyInfo.GetGetMethod());
            }
            if (propertyInfo.CanWrite)
            {
                var setter = fillOut.DefineMethod("set_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot, null, new System.Type[] { propertyInfo.PropertyType });
                var setGenerator = new DebuggableILGenerator(setter.GetILGenerator(), "set_" + propertyInfo.Name + " of " + propertyInfo.DeclaringType.Name + " on " + fillOut.Name);
                addGen(setGenerator);
                var holder = new GeneratorHolder(Possibly.Is(setGenerator));

                holder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);
                holder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldfld, backing);

                holder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_1);

                convert(property.PropertyType, wrappedProperty.PropertyType, holder);

                holder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Callvirt, wrappedProperty.GetSetMethod());
                holder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ret);
                property.SetSetMethod(setter);
                fillOut.DefineMethodOverride(setter, propertyInfo.GetSetMethod());
            }
        }

    }
}
