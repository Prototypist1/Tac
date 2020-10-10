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
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{

    public class IndexerList {

        public IIsPossibly<int> GetOrAdd(IVerifiableType fromType, IVerifiableType toType) {
            if (fromType == toType)
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
        List<IOrType<Guid, IMemberDefinition>> locals = new List<IOrType<Guid, IMemberDefinition>>();

        ILGenerator backing;
        string debugString = "";

        public DebuggableILGenerator(ILGenerator backing, string name)
        {
            debugString += name +": "+ Environment.NewLine;
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        internal void Emit(OpCode code, string str)
        {
            str += code.ToString() + ", " + str + Environment.NewLine;
            backing.Emit(code, str);
        }

        internal void Emit(OpCode code, MethodInfo method)
        {
            debugString += code.ToString() + ", " + method.Name + /*"(" + string.Join<string>(',', method.GetParameters().Select(x => x.ParameterType.Name)) + ")" +*/ Environment.NewLine;
            backing.Emit(code, method);
        }

        internal void Emit(OpCode code, double dub)
        {
            debugString += code.ToString() + ", " + dub + Environment.NewLine;
            backing.Emit(code, dub);
        }

        internal void Emit(OpCode code, Label label)
        {
            debugString += code.ToString() + ", " + label + Environment.NewLine;
            backing.Emit(code, label);
        }


        internal void Emit(OpCode code, System.Type type)
        {
            debugString += code.ToString() + ", " + type.Name + Environment.NewLine;
            backing.Emit(code, type);
        }
        internal void Emit(OpCode code, FieldInfo field)
        {
            debugString += code.ToString() + ", " + field.Name + Environment.NewLine;
            backing.Emit(code, field);
        }

        internal void Emit(OpCode code)
        {
            debugString += code.ToString() + Environment.NewLine;
            backing.Emit(code);
        }
        internal void Emit(OpCode code, ConstructorInfo rootSelfField)
        {
            debugString += code.ToString() + ", " + rootSelfField.DeclaringType.FullName + /*"(" + string.Join<string>(',', rootSelfField.GetParameters().Select(x=>x.ParameterType.Name)) + ")"+*/ Environment.NewLine;
            backing.Emit(code, rootSelfField);
        }

        internal LocalBuilder DeclareLocal(System.Type type, IMemberDefinition member)
        {
            var or = OrType.Make<Guid,IMemberDefinition>(member);
            if (locals.Contains(or)) {
                throw new Exception("already added");
            }

            locals.Add(or);
            debugString += "local, " + type.Name + ", " + member.Key + Environment.NewLine;
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

            debugString += "local, " + type.Name + ", " +  id + Environment.NewLine;
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

        internal void EmitCall(OpCode code, MethodInfo methodInfo, System.Type[] type)
        {
            debugString += code.ToString() + ", " + methodInfo.Name + /*"(" + string.Join<string>(',', methodInfo.GetParameters().Select(x => x.ParameterType.Name)) + ")" +*/ Environment.NewLine;
            backing.EmitCall(code,methodInfo,type);
        }

        internal void MarkLabel(Label topOfElseLabel)
        {
            debugString += topOfElseLabel.ToString() + Environment.NewLine;
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

        
        public int EvaluationStackDepth { get; private set; } = 0;
        private IIsPossibly<DebuggableILGenerator> generator;

        public GeneratorHolder(IIsPossibly<DebuggableILGenerator> generator)
        {
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public DebuggableILGenerator GetGeneratorAndUpdateStack(int stackChange)
        {
            EvaluationStackDepth += stackChange;
            return generator.GetOrThrow();
        }

        public string DebugString() {
            return generator.GetOrThrow().GetDeubbingSting();
        }
    }

    public abstract class TacCompilation {
        public Indexer[] indexerArray;
        public IVerifiableType[] verifyableTypesArray;
        public Func<object, object> main;
        public abstract void Init();
    } 



    class AssemblerVisitor : IOpenBoxesContext<Nothing>
    {

        public readonly List<DebuggableILGenerator> gens ;

        public readonly IndexerList indexerList;
        public readonly VerifyableTypesList verifyableTypesList;



        private readonly MemberKindLookup memberKindLookup;
        private readonly ExtensionLookup extensionLookup;
        private readonly RealizedMethodLookup realizedMethodLookup;
        private readonly Dictionary<IVerifiableType, System.Type> typeCache;
        public readonly TypeBuilder rootType;
        public readonly FieldBuilder rootSelfField;


        public readonly GeneratorHolder generatorHolder;

        // TODO 
        // this should be a lot better
        // 1 - mutliple AssemblerVisitor are going to share a evaluationStackDepth
        // so it need to be a reference of some sort
        // 2 - I think it should be bound to emit in some way, so I can't forget to modify the stack depth



        private IReadOnlyList<ICodeElement> stack;
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
             RealizedMethodLookup realizedMethodLookup)
        {
            var typebuilder = moduleBuilder.DefineType(GenerateName(), TypeAttributes.Public & TypeAttributes.Class, typeof(TacCompilation));
            var selfField = typebuilder.DefineField(GenerateName() + "_self", typebuilder, FieldAttributes.Static | FieldAttributes.Public);

            var initMethod = typebuilder.DefineMethod(nameof(TacCompilation.Init), MethodAttributes.Public | MethodAttributes.Virtual);
            typebuilder.DefineMethodOverride(initMethod, typeof(TacCompilation).GetMethod(nameof(TacCompilation.Init)));

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
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ret);
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
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Add_Ovf);
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
                    var toCSharpeType = typeCache[fromType];
                    if (new [] { typeof(bool), typeof(double)}.Contains(toCSharpeType) ) {
                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Box, toCSharpeType);
                    }
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
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, indexersField.Value);
                LoadInt(definateIndexer.Value);
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);

                GetVerifyableType(toType);

                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Newobj, castConstructor.Value);
            }
        }

        private void GetVerifyableType(IVerifiableType toType)
        {
            if (verifyableTypesList.GetOrAdd(toType).SafeIs(out IIsDefinately<int> definateType))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldsfld, rootSelfField);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, typesField.Value);
                LoadInt(definateType.Value);
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);
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
            return typeof(TacCastObject).GetConstructor(new[] { typeof(ITacObject), typeof(Indexer), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> tacObjectConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacObject).GetConstructor(new System.Type[] { }) ?? throw new NullReferenceException("should not be null!");
        });


        private readonly Lazy<ConstructorInfo> tacMethod_Complex_ComplexConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacMethod_Complex_Complex).GetConstructor(new[] { typeof(Func<ITacObject, ITacObject>), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> tacMethod_Complex_SimpleConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacMethod_Complex_Simple).GetConstructor(new[] { typeof(Func<ITacObject, object>), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> tacMethod_Simple_ComplexConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacMethod_Simple_Complex).GetConstructor(new[] { typeof(Func<object, ITacObject>), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        private readonly Lazy<ConstructorInfo> tacMethod_Simple_SimpleConstructor = new Lazy<ConstructorInfo>(() =>
        {
            return typeof(TacMethod_Simple_Simple).GetConstructor(new[] { typeof(Func<object, object>) ,typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        });

        public Nothing AssignOperation(IAssignOperation co)
        {

            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            // be careful this does not leave anything on the stack
            // =: in tac returns returns the value just saved
            // we need it not to do that if nothing is going to consume that
            var leaveOnStack = this.stack.Last().SafeIs(out IOperation _);


            // {870866D9-D3EC-47B1-B7D3-6966EE651F5F}
            // storing and loading have a lot in commmon

            // the kind of thing the taget is define how we proceed
            if (co.Right.SafeIs(out IMemberReference memberReference))
            {
                // see if it is on the closure 
                // walk up the stack and hope you run in to it
                foreach (var frame in stack.Reverse())
                {
                    if (extensionLookup.TryGetClosure(frame, out var closure))
                    {
                        // these are fields!!

                        if (closure.closureMember.Contains(memberReference.MemberDefinition))
                        {
                            // this
                            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg_0);

                            var realizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(frame));

                            var field = realizedMethod.fields[memberReference.MemberDefinition];

                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            if (leaveOnStack)
                            {
                                // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

                                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Dup);

                                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                                StoreLocal(loc.LocalIndex);

                                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, field);

                                LoadLocal(loc.LocalIndex);
                            }
                            else
                            {
                                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, field);
                            }

                            return new Nothing();
                        }
                    }
                }

                if (memberKindLookup.IsArgument(memberReference.MemberDefinition, out var orTypeArg))
                {
                    // I only allow 1 argument 
                    // 0th arg is this
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Dup);
                    }
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Starg, 1);
                    return new Nothing();
                }

                if (memberKindLookup.IsLocal(memberReference.MemberDefinition, out var orTypeLocal))
                {
                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    if (leaveOnStack)
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Dup);
                    }
                    return orTypeLocal.SwitchReturns(
                        entryPoint =>
                        {
                            StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberReference.MemberDefinition));
                            return new Nothing();
                        },
                        imp =>
                        {
                            StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberReference.MemberDefinition));
                            return new Nothing();
                        },
                        method =>
                        {
                            StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberReference.MemberDefinition));
                            return new Nothing();
                        });
                }

                if (memberKindLookup.IsField(memberReference.MemberDefinition, out var orTypeField))
                {
                    return orTypeField.SwitchReturns(
                        imp =>
                        {

                            // this is the closure

                            // I need a reference to this
                            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg, 0);

                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            // I need the field info...
                            var realizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(imp));
                            var field = realizedMethod.fields[memberReference.MemberDefinition];

                            if (leaveOnStack)
                            {

                                // TODO I could end up with many of this switching locals of the same type in one method
                                // i should probably store and reuse them
                                // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

                                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Dup);

                                var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                                StoreLocal(loc.LocalIndex);

                                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, field);

                                LoadLocal(loc.LocalIndex);
                            }
                            else {
                                generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, field);
                            }

                            return new Nothing();
                        },
                        obj =>
                        {

                            // object would usally need to be part of a path
                            // how else would you know what where to look for the field
                            // but there is one excpetion
                            // object initiation
                            if (!stack.Last().SafeIs(out IObjectDefiniton _)) {
                                throw new Exception("this should only happen in object init");
                            }

                            // we count on having a reference to the object already on the stack

                            // 1st parm, the new value
                            co.Left.Convert(this.Push(co));
                            PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                            // second parm, the index
                            var index = Array.IndexOf(obj.Scope.Members.Values.Select(x => x.Value).ToArray(), memberReference.MemberDefinition);
                            LoadInt(index);

                            if (typeCache[memberReference.MemberDefinition.Type] == typeof(ITacObject))
                            {
                                switch (memberReference.MemberDefinition.Access)
                                {
                                    case Access.ReadOnly:
                                        throw new Exception("this should have benn handled inside assignment");
                                    case Access.ReadWrite:
                                        generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexMemberReturn.Value : setComplexMember.Value, new[] { typeof(int) });
                                        return new Nothing();
                                    case Access.WriteOnly:
                                        generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexWriteonlyMemberReturn.Value : setComplexWriteonlyMember.Value, new[] { typeof(int) });
                                        return new Nothing();
                                    default:
                                        throw new Exception("that is unexpected");
                                }
                            }
                            else
                            {
                                generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, (leaveOnStack ? setSimpleMemberReturn.Value : setSimpleMember.Value), new[] { typeof(int) });
                                return new Nothing();
                            }


                            throw new Exception("this is part of a path and we are explictily not part of a path, we are a member ref directly inside an assignment");

                        });
                }

                if (memberKindLookup.IsStaticField(memberReference.MemberDefinition, out var fieldInfo))
                {

                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldsfld, rootSelfField);

                    co.Left.Convert(this.Push(co));
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                    if (leaveOnStack)
                    {

                        // TODO I could end up with many of this switching locals of the same type in one method
                        // i should probably store and reuse them
                        // {6820D180-0335-40E4-A9AA-22130FB3BC6D} I do this in other places

                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Dup);

                        var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(typeCache[memberReference.MemberDefinition.Type], Guid.NewGuid());
                        StoreLocal(loc.LocalIndex);

                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, fieldInfo);

                        LoadLocal(loc.LocalIndex);
                    }
                    else
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, fieldInfo);
                    }

                    return new Nothing();
                }

                throw new Exception("should have been one of those things...");
            }
            else if (co.Right.SafeIs(out IPathOperation path))
            {
                // who we are calling it on
                path.Left.Convert(this.Push(co));
                if (path.Right.SafeIs(out IMemberReference pathMemberReference))
                {

                    // this "b" inside a path like: a.b
                    // we count on "a" to have already been load
                    if (memberKindLookup.IsField(memberReference.MemberDefinition, out var orTypeField))
                    {
                        return orTypeField.SwitchReturns(
                            imp =>
                            {
                                throw new Exception("we are part of a path so we know it is an ojbect");
                            },
                            obj =>
                            {
                                // 1st parm, the new value
                                co.Left.Convert(this.Push(co));
                                PossiblyConvert(co.Left.Returns(), co.Right.Returns());

                                // second parm, the index
                                var index = Array.IndexOf(obj.Scope.Members.Values.Select(x => x.Value).ToArray(), pathMemberReference.MemberDefinition);
                                LoadInt(index);

                                if (typeCache[pathMemberReference.MemberDefinition.Type] == typeof(ITacObject))
                                {
                                    switch (pathMemberReference.MemberDefinition.Access)
                                    {
                                        case Access.ReadOnly:
                                            throw new Exception("this should have benn handled inside assignment");
                                        case Access.ReadWrite:
                                            generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexMemberReturn.Value : setComplexMember.Value, new[] { typeof(int) });
                                            return new Nothing();
                                        case Access.WriteOnly:
                                            generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, leaveOnStack ? setComplexWriteonlyMemberReturn.Value : setComplexWriteonlyMember.Value, new[] { typeof(int) });
                                            return new Nothing();
                                        default:
                                            throw new Exception("that is unexpected");
                                    }
                                }
                                else
                                {
                                    generatorHolder.GetGeneratorAndUpdateStack(leaveOnStack ? -2 : -3).EmitCall(OpCodes.Callvirt, (leaveOnStack ? setSimpleMemberReturn.Value : setSimpleMember.Value).MakeGenericMethod(typeCache[pathMemberReference.MemberDefinition.Type]), new[] { typeof(int) });
                                    return new Nothing();
                                }
                            });
                    }
                    else
                    {
                        throw new Exception("should be a field");
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
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
            }
            else
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
            }
            return new Nothing();
        }
        public Nothing ConstantNumber(IConstantNumber codeElement)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_R8, codeElement.Value);
            return new Nothing();
        }
        public Nothing ConstantString(IConstantString co)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldstr, co.Value);
            return new Nothing();
        }
        public Nothing EmptyInstance(IEmptyInstance co)
        {
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldnull);
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
                var bottomOfElse = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
                myIf.Operands[0].Convert(nextNext);
                // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                if (this.stack.Last().SafeIs(out IOperation _))
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
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Br, bottomOfElse);
                generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(topOfElseLabel);
                co.Operands[1].Convert(next);
                generatorHolder.GetGeneratorAndUpdateStack(0).MarkLabel(bottomOfElse);

                return new Nothing();
            }
            else {
                co.Left.Convert(next);
                // duplicate code { 9EAD95C4 - 6FAD - 4911 - 94EE - 106528B7A3B2}
                if (this.stack.Last().SafeIs(out IOperation _))
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


        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {



            var realizedMethod = realizedMethodLookup.GetValueOrThrow(OrType.Make<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>(entryPointDefinition));
            var myMethod = realizedMethod.type.DefineMethod(
                GenerateName(), 
                MethodAttributes.Public, 
                CallingConventions.HasThis, 
                typeof(object), new[] { typeof(object) });


            var gen = new DebuggableILGenerator(myMethod.GetILGenerator(), "main");

            // I need to declare the locals
            foreach (var local in entryPointDefinition.Scope.Members)
            {
                gen.DeclareLocal(typeCache[ local.Value.Value.Type], local.Value.Value);
            }

            var inner = this.Push(entryPointDefinition, gen);
            foreach (var line in entryPointDefinition.Body)
            {
                line.Convert(inner);
            }

            // everything is in a method on rootType
            // ldarg_0 is this
            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Ldarg_0);

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Newobj, realizedMethod.defaultConstructor);

            // now I need to make a TacMethod or whatever

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, myMethod);
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Newobj, typeof(Func<object, object>).GetConstructors().First());           // TODO lazy this reflection

            var field = typeof(TacCompilation).GetField(nameof(TacCompilation.main)) ?? throw new NullReferenceException("that field better exist");

            generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(OpCodes.Stfld, field);

            return new Nothing();
        }

        public Nothing IfTrueOperation(IIfOperation co)
        {
            var next = this.Push(co);

            var label = generatorHolder.GetGeneratorAndUpdateStack(0).DefineLabel();
            co.Operands[0].Convert(next);
            // duplicate code {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
            if (this.stack.Last().SafeIs(out IOperation _))
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
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Clt);
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

            // see if it is on the closure 
            // walk up the stack and hope you run in to it

            foreach (var frame in stack.Reverse())
            {
                if (extensionLookup.TryGetClosure(frame, out var closure))
                {
                    // these are fields!!

                    if (closure.closureMember.Contains(memberReference.MemberDefinition))
                    {
                        // this
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg_0);

                        var realizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(frame));

                        var field = realizedMethod.fields[memberReference.MemberDefinition];

                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, field);

                        return new Nothing();
                    }
                }
            }

            return  EmitMemberReference(memberReference.MemberDefinition);

        }

        /// this does not look in the closure
        private Nothing EmitMemberReference(IMemberDefinition memberDefinition) {


            if (memberKindLookup.IsArgument(memberDefinition, out var orTypeArg))
            {
                return orTypeArg.SwitchReturns(
                    imp =>
                    {
                        // I only allow 1 argument 
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                        return new Nothing();
                    },
                    method =>
                    {
                        // I only allow 1 argument 
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                        return new Nothing();
                    });
            }

            if (memberKindLookup.IsLocal(memberDefinition, out var orTypeLocal))
            {
                return orTypeLocal.SwitchReturns(
                    entryPoint =>
                    {
                        LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                        return new Nothing();
                    },
                    imp =>
                    {
                        LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                        return new Nothing();
                    },
                    method =>
                    {
                        LoadLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDefinition));
                        return new Nothing();
                    });
            }

            if (memberKindLookup.IsField(memberDefinition, out var orTypeField))
            {
                return orTypeField.SwitchReturns(
                    imp =>
                    {
                        // this is the closure

                        // I need a reference to this
                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldarg_1);

                        // I need the field info...
                        var realizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(imp));
                        var field = realizedMethod.fields[memberDefinition];

                        generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
                        // no change in stack

                        return new Nothing();
                    },
                    obj =>
                    {

                        // this "b" inside a path like: a.b
                        // we count on "a" to have already been load

                        var index = Array.IndexOf(obj.Scope.Members.Values.Select(x => x.Value).ToArray(), memberDefinition);
                        LoadInt(index);

                        if (typeCache[memberDefinition.Type] == typeof(ITacObject))
                        {
                            switch (memberDefinition.Access)
                            {
                                case Access.ReadOnly:
                                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexReadonlyMember.Value, new[] { typeof(int) });
                                    return new Nothing();
                                case Access.ReadWrite:
                                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getComplexMember.Value, new[] { typeof(int) });
                                    return new Nothing();
                                case Access.WriteOnly:
                                    throw new Exception("this should have benn handled inside assignment");
                                default:
                                    throw new Exception("that is unexpected");
                            }
                        }
                        else
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, getSimpleMember.Value.MakeGenericMethod(typeCache[memberDefinition.Type]), new[] { typeof(int) });
                            return new Nothing();
                        }
                    });
            }

            if (memberKindLookup.IsStaticField(memberDefinition, out var fieldInfo))
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldsfld, rootSelfField);

                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, fieldInfo);

                return new Nothing();
            }


            throw new Exception("how did we end up here?");
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
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0); 
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_2); 
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_4); 
                    return;
                case 5:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_5); 
                    return;
                case 6:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4_7);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldc_I4, value); 
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
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stloc_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stloc_2);
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stloc_3);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stloc_S, index);
                    return;
            }
        }


        private void LoadLocal(int index)
        {
            switch (index)
            {
                case 0:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                    return;
                case 1:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldloc_1);
                    return;
                case 2:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldloc_2);
                    return;
                case 3:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldloc_3);
                    return;
                default:
                    generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Ldloc_S, index);
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
            var myMethod = realizedMethod.type.DefineMethod(name, MethodAttributes.Public, CallingConventions.HasThis, ToITacObjectOrOject(typeCache[ method.OutputType]), new[] { ToITacObjectOrOject(typeCache[method.InputType]) });

            var gen = new DebuggableILGenerator( myMethod.GetILGenerator(), name);

            // I need to declare the locals
            foreach (var local in method.Scope.Members)
            {
                if (local.Value.Value != method.ParameterDefinition)
                {
                    gen.DeclareLocal(typeCache[local.Value.Value.Type], local.Value.Value);
                }
            }

            var inner = this.Push(method, gen);
            foreach (var line in method.Body)
            {
                line.Convert(inner);
            }

            // create new instance
            // get the default constuctor 
            var constructor = realizedMethod.defaultConstructor;

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Newobj, constructor);


            // pass stuff in to the closure
            if (extensionLookup.TryGetClosure(method, out var ourClosure))
            {
                // find our parent
                var frame = stack.Reverse().Select(frame => { return extensionLookup.TryGetClosure(frame, out var _)? frame:null; }).Where(x => x != null).First() ?? throw new ArgumentNullException("should find one") ;

                foreach (var member in ourClosure.closureMember)
                {
                    if (realizedMethod.fields.TryGetValue(member, out var fieldInfo)) {

                        generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);

                        var outerRealizedMethod = realizedMethodLookup.GetValueOrThrow(ConvertToMethodlike(frame));
                        // if it is on the closure read the field
                        if (outerRealizedMethod.fields.TryGetValue(member, out var field))
                        {
                            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
                        }
                        else {
                            EmitMemberReference(member);
                        }

                        // now we need to push the new value on to out closure 
                        generatorHolder.GetGeneratorAndUpdateStack(-2).Emit(System.Reflection.Emit.OpCodes.Stfld, fieldInfo);
                    }
                }
            }

            // now I need to make a TacMethod or whatever

            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Ldftn, myMethod);


            // TODO lazy all this reflection
            generatorHolder.GetGeneratorAndUpdateStack(0).Emit(System.Reflection.Emit.OpCodes.Newobj, typeof(Func<,>).MakeGenericType(new System.Type[] { typeCache[method.InputType], typeCache[method.OutputType] }).GetConstructors().First());
            GetVerifyableType(method.Returns());

            if (typeCache[method.InputType] == typeof(ITacObject))
            {
                if (typeCache[method.OutputType] == typeof(ITacObject))
                {

                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Newobj, tacMethod_Complex_ComplexConstructor.Value);
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Newobj, tacMethod_Complex_SimpleConstructor.Value);
                }
            }
            else {
                if (typeCache[method.OutputType] == typeof(ITacObject))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Newobj, tacMethod_Simple_ComplexConstructor.Value);
                }
                else
                {
                    generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Newobj, tacMethod_Simple_SimpleConstructor.Value);
                }
            }

            return new Nothing();
        }



        // {4E963BB1-1C86-4F75-BD4C-3F9BE16386A9}
        private static readonly Random random = new Random();
        private static string GenerateName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Nothing ModuleDefinition(IModuleDefinition module)
        {
            throw new NotImplementedException();
            return Walk(module.StaticInitialization, module);
        }

        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Mul_Ovf);
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
            var loc = generatorHolder.GetGeneratorAndUpdateStack(0).DeclareLocal(inType, Guid.NewGuid());
            StoreLocal(loc.LocalIndex);

            co.Right.Convert(this.Push(co));
            LoadLocal(loc.LocalIndex);

            if (inType == typeof(ITacObject))
            {
                if (outType == typeof(ITacObject))
                {
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexComplex.Value, new[] { typeof(int) });
                    // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                    if (!this.stack.Last().SafeIs(out IOperation _))
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
                else
                {
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callComplexSimple.Value.MakeGenericMethod(outType), new[] { typeof(int) });
                    // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                    if (!this.stack.Last().SafeIs(out IOperation _))
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
            }
            else {
                if (outType == typeof(ITacObject))
                {
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleComplex.Value.MakeGenericMethod(inType), new[] { typeof(int) });
                    // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                    if (!this.stack.Last().SafeIs(out IOperation _))
                    {
                        generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Pop);
                    }
                }
                else
                {
                    PossiblyConvert(co.Left.Returns(), co.Right.Returns());
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Callvirt, callSimpleSimple.Value.MakeGenericMethod(inType,outType), new[] { typeof(int) });
                    // similar idea {9EAD95C4-6FAD-4911-94EE-106528B7A3B2}
                    if (!this.stack.Last().SafeIs(out IOperation _))
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

            generatorHolder.GetGeneratorAndUpdateStack(1).Emit(System.Reflection.Emit.OpCodes.Newobj, tacObjectConstructor.Value);



            // itit field
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                GetVerifyableType(@object.Returns());
                var field = typeof(TacObject).GetField(nameof(TacObject.type)) ?? throw new NullReferenceException("that field better exist");
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stfld, field);
            }

            // init member
            {
                generatorHolder.GetGeneratorAndUpdateStack(1).Emit(OpCodes.Dup);
                LoadInt(@object.Scope.Members.Count);
                generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Newarr, typeof(object[]));
                var field = typeof(TacObject).GetField(nameof(TacObject.members)) ?? throw new NullReferenceException("that field better exist");
                generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Stfld, field);
            }

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
            if (!this.stack.Last().SafeIs(out IOperation _))
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
            }
            end:




            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Ret);

            return new Nothing();
        }

        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Walk(co.Operands, co);
            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(System.Reflection.Emit.OpCodes.Sub_Ovf);
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
            generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Is)), new System.Type[] { });

            generatorHolder.GetGeneratorAndUpdateStack(-1).Emit(OpCodes.Brfalse, topOfElseLabel);

            if (memberKindLookup.IsLocal(memberDef, out var orTypeLocal))
            {
                if (typeCache[memberDef.Type] == typeof(bool))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(bool));
                }
                else
                if (typeCache[memberDef.Type] == typeof(double))
                {
                    generatorHolder.GetGeneratorAndUpdateStack(0).Emit(OpCodes.Unbox_Any, typeof(double));
                }
                else
                {
                    GetVerifyableType(memberDef.Type);
                    generatorHolder.GetGeneratorAndUpdateStack(-1).EmitCall(OpCodes.Call, typeof(AssemblyWalkerHelp).GetMethod(nameof(AssemblyWalkerHelp.TryAssignOperationHelper_Cast)), new System.Type[] { });
                }

                orTypeLocal.SwitchReturns(
                    entryPoint =>
                    {
                        StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));
                        return new Nothing();
                    },
                    imp =>
                    {
                        StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));
                        return new Nothing();
                    },
                    method =>
                    {
                        StoreLocal(generatorHolder.GetGeneratorAndUpdateStack(0).GetLocalIndex(memberDef));
                        return new Nothing();
                    });
            }
            else {
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

        private Nothing Walk(IEnumerable<ICodeElement> elements, ICodeElement element)
        {
            var inner = this.Push(element);
            foreach (var line in elements)
            {
                line.Convert(inner);
            }

            return new Nothing();
        }
    }


    public static class AssemblyWalkerHelp {


        public static object TryAssignOperationHelper_Cast(object o, IVerifiableType targetType)
        {
            if (o.SafeIs(out ITacObject tacObject))
            {
                if (tacObject.TacType() == targetType)
                {
                    return o;
                }
                return new TacCastObject(tacObject, Indexer.Create(tacObject.TacType(), targetType), targetType);
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
            if (o.SafeIs(out ITacObject verifiableType))
            {
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
