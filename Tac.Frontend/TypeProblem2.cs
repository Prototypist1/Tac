using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Xunit.Sdk;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {


        internal partial class TypeProblem2 
        {
            public abstract class TypeProblemNode : ITypeProblemNode
            {
                public readonly string debugName;

                public TypeProblemNode(Builder problem, string debugName)
                {
                    Problem = problem ?? throw new ArgumentNullException(nameof(problem));
                    this.debugName = debugName;
                    problem.Register(this);
                }

                public Builder Problem { get; }

            }

            public abstract class TypeProblemNode<Tin, Tout> : TypeProblemNode//, IConvertable<T>
            {

                public TypeProblemNode(Builder problem, string debugName, IConvertTo<Tin, Tout> converter) : base(problem, debugName)
                {
                    Converter = converter;
                }

                internal IConvertTo<Tin, Tout> Converter { get; }
            }
            public class TypeReference : TypeProblemNode<TypeReference, IFrontendType>, ILookUpType
            {
                public TypeReference(Builder problem, string debugName, IConvertTo<TypeReference, IFrontendType> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                {
                    return new Dictionary<IKey, Member>();
                }
            }

            public class Value : TypeProblemNode<Value, PlaceholderValue>, IValue
            {
                public Value(Builder problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();

                public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                {
                    return HopefulMembers;
                }
            }
            public class Member : TypeProblemNode<IOrType<Tpn.IFlowNode, IError>, WeakMemberDefinition>, IMember
            {
                public Member(Builder problem, string debugName, IConvertTo<IOrType<Tpn.IFlowNode, IError>, WeakMemberDefinition> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();

                public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                {
                    return HopefulMembers;
                }
            }

            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();

                public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                {
                    return HopefulMembers;
                }
            }

            // I don't really think possible members belong here
            // they come from the member matcher
            // the member matcher matches x and y type{x;y;}
            // but really THAT should be smarter
            // if you are in a type (or an object)
            // on the left side of the equals
            // you are making members
            // right now this will fail
            // if the parent has a member of this name
            public class Type : TypeProblemNode<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType, IHavePossibleMembers
            {
                public Type(
                    Builder problem,
                    string debugName,
                    IIsPossibly<IOrType<NameKey, ImplicitKey>> key,
                    IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter,
                    bool isPlaceHolder,
                    IIsPossibly<Guid> primitiveId
                    ) : base(problem, debugName, converter)
                {
                    Key = key;
                    IsPlaceHolder = isPlaceHolder;
                    PrimitiveId = primitiveId;
                }

                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Guid> PrimitiveId { get; }
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }

                public bool IsPlaceHolder { get; }
            }

            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members

            // is a method type really a scope??
            public class MethodType : TypeProblemNode<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IScope, IHavePossibleMembers
            {
                public MethodType(Builder problem, string debugName, IConvertTo<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PrivateMembers { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();

                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

            }

            public class InferredType : TypeProblemNode, IHaveInputAndOutput, IHavePublicMembers //, IScope
            {
                public InferredType(Builder problem, string debugName) : base(problem, debugName)
                {
                }
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();
            }

            public class OrType : TypeProblemNode<OrType, WeakTypeOrOperation> 
            {
                public OrType(Builder problem, string debugName, IConvertTo<OrType, WeakTypeOrOperation> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<TypeReference> Left { get; set; } = Possibly.IsNot<TypeReference>();
                public IIsPossibly<TypeReference> Right { get; set; } = Possibly.IsNot<TypeReference>();
            }
            public class Scope : TypeProblemNode<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>, IScope, IHavePossibleMembers
            {
                public Scope(Builder problem, string debugName, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PrivateMembers { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

            }
            public class Object : TypeProblemNode<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType, IHavePossibleMembers
            {
                public Object(Builder problem, string debugName, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter) : base(problem, debugName, converter)
                {
                    InitizationScope = new Scope(problem, debugName, innerConverter)
                    {
                        Parent = Possibly.Is(this)
                    };
                }
                public readonly Scope InitizationScope;
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();


            }
            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class Method : TypeProblemNode<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>, IScope, IHaveInputAndOutput, IHavePossibleMembers
            {
                public Method(Builder problem, string debugName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PrivateMembers { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();
            }

            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }

            public readonly Builder builder;

            // these are pretty much the same
            private List<(ILookUpType, ILookUpType)> assignments = new List<(ILookUpType, ILookUpType)>();

            // pretty sure it is not safe to solve more than once 
            // ^ I don't trust him
            public TypeSolution Solve()
            {
                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>) && x.TypeKey.Is3(out var _)))
                {
                    var type = new InferredType(this.builder, $"for {((TypeProblemNode)node).debugName}");
                    node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type));
                }

                // what happens here if x.TypeKey.Is2??
                if (toLookUp.Any(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>) && x.TypeKey.Is2(out var _)))
                {
                    throw new NotImplementedException();
                }

                // generics register themsleves 
                var realizedGeneric = new Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                foreach (var methodType in typeProblemNodes.OfType<MethodType>().Where(x => x.GenericOverlays.Any()))
                {
                    var key = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(methodType), methodType.GenericOverlays.Values.ToArray());
                }
                foreach (var type in typeProblemNodes.OfType<Type>().Where(x => x.GenericOverlays.Any()))
                {
                    var key = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), type.GenericOverlays.Values.ToArray());
                }

                toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>)).ToArray();

                // overlay generics
                while (toLookUp.Any())
                {
                    foreach (var node in toLookUp)
                    {
                        LookUpOrOverlayOrThrow(node, realizedGeneric);
                    }
                    toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>)).ToArray();
                }

                // members that might be on parents 
                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();

                foreach (var node in typeProblemNodes)
                {
                    if (node is IHavePossibleMembers possibleMembers && node is IStaticScope staticScope)
                    {
                        foreach (var pair in possibleMembers.PossibleMembers)
                        {
                            TryGetMember(staticScope, pair.Key).IfElse(member => TryMerge(pair.Value, member!), () =>
                            {

                                if (node is IHavePublicMembers havePublicMembers)
                                {

                                    Builder.HasPublicMember(havePublicMembers, pair.Key, pair.Value);
                                }
                                else if (node is IHavePrivateMembers havePrivateMembers)
                                {
                                    Builder.HasPrivateMember(havePrivateMembers, pair.Key, pair.Value);
                                }
                                else
                                {
                                    throw new Exception("uhhhh");
                                }
                            });
                        }
                    }
                }


                // hopeful members and methods are a little rough around the edges
                // they are very similar yet implemented differently 

                // hopeful members 
                foreach (var (node, hopeful) in typeProblemNodes.OfType<IValue>().Select(x => (x, x.HopefulMembers)))
                {
                    foreach (var pair in hopeful)
                    {
                        HandleHopefulMember(pair.Key, pair.Value, GetType(node));
                    }
                }

                // hopeful methods 
                foreach (var (node, hopefulMethod) in typeProblemNodes.OfType<IValue>().Select(x => (x, x.HopefulMethod)))
                {
                    if (hopefulMethod is IIsDefinately<InferredType> definately)
                    {
                        var type = GetType(node);
                        if (type.Is1(out var methodType))
                        {
                            if (!methodType.Equals(hopefulMethod))
                            {
                                node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType));

                                var defererReturns = definately.Value.Returns.GetOrThrow();
                                var deferredToReturns = methodType.Returns.GetOrThrow();
                                TryMerge(defererReturns, deferredToReturns);

                                var defererInput = definately.Value.Input.GetOrThrow();
                                var deferredToInput = methodType.Input.GetOrThrow();
                                TryMerge(defererInput, deferredToInput);
                            }
                        }
                        else if (type.Is5(out var dummy))
                        {
                            // these need to be merged 
                            // dummy could have hopeful members
                            // altho if it does that is an exception
                            // the merge logic is alittle different
                            // for now I will throw if dummy has members
                            if (dummy.PublicMembers.Any())
                            {
                                node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(
                                    Error.Other("you can't have hopeful members and be a hopeful method")));
                            }

                            // I don't think these can happen if they do I want to know
                            if (dummy.Input is IIsDefinately<Member>)
                            {
                                throw new NotImplementedException();
                            }
                            if (dummy.Returns is IIsDefinately<Member>)
                            {
                                throw new NotImplementedException();
                            }

                            node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(definately.Value));
                        }
                        else
                        {
                            throw new Exception("no good!");
                        }
                    }
                }

                // ------------ flow time!

                var ors = typeProblemNodes.Select(node => TryGetType(node)).OfType<IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>>().Select(x => x.Value).Distinct().ToArray();

                var orsToFlowNodesBuild = new Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();
                var orsToFlowNodesLookup = new Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>>();

                foreach (var methodType in ors.Select(x => (x.Is1(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(methodType);
                    var value = ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.MethodType>(methodType));

                    orsToFlowNodesBuild.Add(key, value);
                    orsToFlowNodesLookup.Add(key, value);
                }
                foreach (var type in ors.Select(x => (x.Is2(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    if (type.PrimitiveId.Is(out var guid))
                    {
                        var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type);
                        var flowNode = ToOr(new PrimitiveFlowNode(type, guid));
                        orsToFlowNodesBuild.Add(key, flowNode);
                        orsToFlowNodesLookup.Add(key, flowNode);
                    }
                    else
                    {
                        var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type);
                        var value = ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.Type>(type));

                        orsToFlowNodesBuild.Add(key, value);
                        orsToFlowNodesLookup.Add(key, value);
                    }
                }
                foreach (var @object in ors.Select(x => (x.Is3(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(@object);
                    var value = ToOr(new ConcreteFlowNode<Tpn.TypeProblem2.Object>(@object));

                    orsToFlowNodesBuild.Add(key, value);
                    orsToFlowNodesLookup.Add(key, value);
                }
                foreach (var inferred in ors.Select(x => (x.Is5(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(inferred);

                    var concrete = new ConcreteFlowNode<Tpn.TypeProblem2.InferredType>(inferred);
                    orsToFlowNodesBuild.Add(key, ToOr(concrete));
                    var inferredFlowNode = new InferredFlowNode(Possibly.Is(inferred));
                    inferredFlowNode.Sources.Add(new SourcePath(Prototypist.Toolbox.OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(concrete),new List<IOrType< Tpn.Member, Tpn.Input, Tpn.Output>>()));
                    orsToFlowNodesLookup.Add(key, ToOr(inferredFlowNode));
                }
                foreach (var error in ors.Select(x => (x.Is6(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(error);
                    var value = ToOr(new ConcreteFlowNode<IError>(error));

                    orsToFlowNodesBuild.Add(key, value);
                    orsToFlowNodesLookup.Add(key, value);
                }

                var todo = ors.Select(x => (x.Is4(out var v), v)).Where(x => x.Item1).Select(x => x.v).ToArray();
                var excapeValve = 0;

                // or types are a bit of a project because they might depend on each other
                while (todo.Any())
                {
                    excapeValve++;
                    var nextTodo = new List<TypeProblem2.OrType>();
                    foreach (var or in todo)
                    {
                        if (TryToOuterFlowNode(orsToFlowNodesLookup, or, out var res))
                        {
                            orsToFlowNodesBuild[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(or)] = res;
                            orsToFlowNodesLookup[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(or)] = res;
                        }
                        else
                        {
                            nextTodo.Add(or);
                        }
                    }
                    todo = nextTodo.ToArray();
                    if (excapeValve > 100000)
                    {
                        throw new Exception("we are probably stuck");
                    }
                }


                // we create members on our new representation
                foreach (var hasPublicMembers in ors.Select(x => (x.Is(out IHavePublicMembers members), members)).Where(x => x.Item1).Select(x => x.members))
                {
                    foreach (var member in hasPublicMembers.PublicMembers)
                    {
                        orsToFlowNodesBuild[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasPublicMembers)].Is1OrThrow().Members.Add(
                            member.Key,
                            orsToFlowNodesLookup[member.Value.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                }

                // we create input and output on our new implmentation
                foreach (var hasInputAndOutput in ors.Select(x => (x.Is(out IHaveInputAndOutput io), io)).Where(x => x.Item1).Select(x => x.io))
                {
                    if (hasInputAndOutput.Input.Is(out var input))
                    {
                        orsToFlowNodesBuild[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasInputAndOutput)].Is1OrThrow().Input =
                                    Possibly.Is(orsToFlowNodesLookup[input.LooksUp.GetOrThrow().SwitchReturns(
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                    if (hasInputAndOutput.Returns.Is(out var output))
                    {
                        orsToFlowNodesBuild[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasInputAndOutput)].Is1OrThrow().Output = Possibly.Is(
                                orsToFlowNodesLookup[output.LooksUp.GetOrThrow().SwitchReturns(
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                }

                excapeValve = 0;

                bool go;
                do
                {
                    go = false;

                    foreach (var (from, to) in assignments)
                    {
                        var toType = orsToFlowNodesLookup[to.LooksUp.GetOrThrow().SwitchReturns(
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))];
                        var fromType = orsToFlowNodesLookup[from.LooksUp.GetOrThrow().SwitchReturns(
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))];

                        //if (fromType.GetValueAs(out IFlowNode _).CanFlow(toType.GetValueAs(out IVirtualFlowNode _), new List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)>()))
                        //{
                            go |= fromType.GetValueAs(out IFlowNode _).Flow(toType.GetValueAs(out IVirtualFlowNode _), new List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)>());
                        //}
                    }

                    excapeValve++;
                    if (excapeValve > 1000000)
                    {
                        throw new Exception("probably stuck in a loop");
                    }

                } while (go);

                return new TypeSolution(
                    typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>).ToDictionary(x => x, x => x.LooksUp.GetOrThrow()),
                    typeProblemNodes.OfType<OrType>().ToDictionary(x => x, x => (x.Left.GetOrThrow(), x.Right.GetOrThrow())),
                    typeProblemNodes.OfType<IStaticScope>().Where(x => x.EntryPoints.Any()).ToDictionary(x => x, x => x.EntryPoints.Single()),
                    orsToFlowNodesLookup.Where(x=> x.Key.Is1(out var y) && y is MethodType).Select(x=>((MethodType)x.Key.Is1OrThrow(), (IFlowNode<MethodType>)x.Value.GetValueAs(out IFlowNode _))).ToDictionary(x=>x.Item1, x=>x.Item2),
                    orsToFlowNodesLookup.Where(x => x.Key.Is1(out var y) && y is Type).Select(x => ((Type)x.Key.Is1OrThrow(), (IFlowNode<Type>)x.Value.GetValueAs(out IFlowNode _))).ToDictionary(x => x.Item1, x => x.Item2),
                    orsToFlowNodesLookup.Where(x => x.Key.Is1(out var y) && y is Object).Select(x => ((Object)x.Key.Is1OrThrow(), (IFlowNode<Object>)x.Value.GetValueAs(out IFlowNode _))).ToDictionary(x => x.Item1, x => x.Item2),
                    orsToFlowNodesLookup.Where(x => x.Key.Is1(out var y) && y is OrType).Select(x => ((OrType)x.Key.Is1OrThrow(), (IFlowNode<OrType>)x.Value.GetValueAs(out IFlowNode _))).ToDictionary(x => x.Item1, x => x.Item2),
                    orsToFlowNodesLookup.Where(x => x.Key.Is1(out var y) && y is InferredType).Select(x => ((InferredType)x.Key.Is1OrThrow(), (IFlowNode<InferredType>)x.Value.GetValueAs(out IFlowNode _))).ToDictionary(x => x.Item1, x => x.Item2));
            }

            #region Helpers


            private static bool TryToOuterFlowNode(Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> orsToFlowNodes, Tpn.TypeProblem2.OrType or, out IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode> res)
            {
                if (orsToFlowNodes.TryGetValue(GetType(or.Left.GetOrThrow()).SwitchReturns(
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)), out var left) &&
                           orsToFlowNodes.TryGetValue(GetType(or.Right.GetOrThrow()).SwitchReturns(
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)), out var right))
                {

                    res = ToOr(new OrFlowNode(new[] { left, right }, Possibly.Is(or)));
                    return true;
                }
                res = default;
                return false;
            }


            // probably a method on defered type
            void TryMerge(IValue deferer, IValue deferredTo)
            {

                var defererType = GetType(deferer);
                if (defererType.Is5(out var deferringInferred).Not())
                {
                    throw new Exception("we can't merge that!");
                }
                var deferredToType = GetType(deferredTo);

                // we replace anything looking up the deferer
                // it will now look up the deferredTo
                var toReplace = new List<ILookUpType>();

                foreach (var lookUper in typeProblemNodes.OfType<ILookUpType>())
                {
                    if (lookUper.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> lookUperLooksUp && lookUperLooksUp.Value.Equals(defererType))
                    {
                        toReplace.Add(lookUper);
                    }
                }

                foreach (var key in toReplace)
                {
                    key.LooksUp = Possibly.Is(deferredToType);
                }

                // this removes the deferer from all assignments
                // replacing it with what is is defered to
                // why do I need this?
                {
                    if (deferredTo.SafeIs<ITypeProblemNode, ICanAssignFromMe>(out var deferredToLeft))
                    {
                        if (deferredTo.SafeIs<ITypeProblemNode, ICanBeAssignedTo>(out var deferredToRight))
                        {
                            var nextAssignments = new List<(ILookUpType, ILookUpType)>();
                            foreach (var assignment in assignments)
                            {
                                var left = assignment.Item1 == deferer ? deferredToLeft : assignment.Item1;
                                var right = assignment.Item2 == deferer ? deferredToRight : assignment.Item2;
                                nextAssignments.Add((left, right));
                            }
                            assignments = nextAssignments;
                        }
                        else
                        {
                            var nextAssignments = new List<(ILookUpType, ILookUpType)>();
                            foreach (var assignment in assignments)
                            {
                                var left = assignment.Item1 == deferer ? deferredToLeft : assignment.Item1;
                                nextAssignments.Add((left, assignment.Item2));
                            }
                            assignments = nextAssignments;
                        }
                    }
                    else if (deferredTo.SafeIs<ITypeProblemNode, ICanBeAssignedTo>(out var deferredToRight))
                    {
                        var nextAssignments = new List<(ILookUpType, ILookUpType)>();

                        foreach (var assignment in assignments)
                        {
                            var right = assignment.Item2 == deferer ? deferredToRight : assignment.Item2;
                            nextAssignments.Add((assignment.Item1, right));
                        }
                        assignments = nextAssignments;
                    }
                }

                // we merge input and output if we are defering to a method type
                {
                    if (deferredToType.Is1(out var deferredToMethod))
                    {
                        if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringReturns && deferredToMethod.Returns is IIsDefinately<TransientMember> deferredToReturns)
                        {
                            TryMerge(deferringReturns.Value, deferredToReturns.Value);
                        }


                        if (deferringInferred.Input is IIsDefinately<Member> deferringInput && deferredToMethod.Input is IIsDefinately<Member> deferredToInput)
                        {
                            TryMerge(deferringInput.Value, deferredToInput.Value);
                        }
                    }
                }

                // if we are defering to an Type, Object
                // flow members
                {
                    if (IsNotInferedHasMembers(deferredToType, out var hasMembers))
                    {
                        foreach (var memberPair in deferringInferred.PublicMembers)
                        {
                            if (hasMembers!.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                            {
                                TryMerge(memberPair.Value, deferedToMember);
                            }
                            else
                            {
                                throw new Exception("the implicit type has members the real type does not");
                                //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                //HasMember(deferredToHaveType, memberPair.Key, newValue);
                                //lookUps[newValue] = lookUps[memberPair.Value];
                            }
                        }
                    }
                }

                // OrType needs to be its own block and we need to flow in to both sides
                {
                    if (deferredToType.Is4(out var orType))
                    {
                        foreach (var memberPair in deferringInferred.PublicMembers)
                        {
                            MergeIntoOrType(orType, memberPair);
                        }
                    }
                }

                // if we are defering to an infered type
                {
                    if (deferredToType.Is5(out var deferredToInferred))
                    {

                        foreach (var memberPair in deferringInferred.PublicMembers)
                        {
                            if (deferredToInferred.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                            {
                                TryMerge(memberPair.Value, deferedToMember);
                            }
                            else
                            {
                                var newValue = new Member(this.builder, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                Builder.HasPublicMember(deferredToInferred, memberPair.Key, newValue);
                                newValue.LooksUp = memberPair.Value.LooksUp;
                            }
                        }

                        if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns)
                        {
                            if (deferredToInferred.Returns is IIsDefinately<TransientMember> deferredToInferredReturns)
                            {
                                TryMerge(deferringInferredReturns.Value, deferredToInferredReturns.Value);
                            }
                            else
                            {
                                deferredToInferred.Returns = deferringInferred.Returns;
                            }
                        }

                        if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput)
                        {
                            if (deferredToInferred.Input is IIsDefinately<Member> deferredToInferredInput)
                            {
                                TryMerge(deferringInferredInput.Value, deferredToInferredInput.Value);
                            }
                            else
                            {
                                deferredToInferred.Input = deferringInferred.Input;
                            }
                        }
                    }
                }
            }

            void MergeIntoOrType(OrType orType, KeyValuePair<IKey, Member> memberPair)
            {

                if (orType.Left is IIsDefinately<TypeReference> leftRef)
                {
                    var bigOr = GetType(leftRef.Value);
                    if (bigOr.Is<IHavePublicMembers>(out var leftWithPublicMembers))
                    {
                        if (leftWithPublicMembers.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                        {
                            TryMerge(memberPair.Value, deferedToMember);
                        }
                        else
                        {
                            throw new Exception("the implicit type has members the real type does not");
                            //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                            //HasMember(deferredToHaveType, memberPair.Key, newValue);
                            //lookUps[newValue] = lookUps[memberPair.Value];
                        }
                    }

                    if (bigOr.Is<OrType>(out var leftOr))
                    {
                        MergeIntoOrType(leftOr, memberPair);
                    }
                }

                if (orType.Right is IIsDefinately<TypeReference> rightRef)
                {
                    var bigOr = GetType(rightRef.Value);
                    if (bigOr.Is<IHavePublicMembers>(out var rightWithPublicMembers))
                    {
                        if (rightWithPublicMembers.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                        {
                            TryMerge(memberPair.Value, deferedToMember);
                        }
                        else
                        {
                            throw new Exception("the implicit type has members the real type does not");
                            //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                            //HasMember(deferredToHaveType, memberPair.Key, newValue);
                            //lookUps[newValue] = lookUps[memberPair.Value];
                        }
                    }

                    if (bigOr.Is<OrType>(out var RightOr))
                    {
                        MergeIntoOrType(RightOr, memberPair);
                    }
                }
            }

            IOrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrOverlayOrThrow(ILookUpType node, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> realizedGeneric)
            {

                if (node.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> nodeLooksUp)
                {
                    return nodeLooksUp.Value;
                }

                // if we don't have a lookup we damn well better have a context and a key
                var res = TryLookUpOrOverlay(node.Context.GetOrThrow(), node.TypeKey.Is1OrThrow(), realizedGeneric);
                node.LooksUp = Possibly.Is(res);
                return res;
            }

            IOrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrOverlayOrThrow2(IStaticScope from, IKey key, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> realizedGeneric)
            {
                return TryLookUpOrOverlay(from, key, realizedGeneric);
            }

            IOrType<MethodType, Type, Object, OrType, InferredType, IError> TryLookUpOrOverlay(IStaticScope from, IKey key, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> realizedGeneric)
            {

                if (key is GenericNameKey genericNameKey)
                {

                    var types = genericNameKey.Types.Select(typeKey => typeKey.SwitchReturns(
                        x => LookUpOrOverlayOrThrow2(from, x, realizedGeneric),
                        y => throw new NotImplementedException("Type could not be resolved")) // I want to use NIEs to keep my code near compilablity. once I have a lot in type problem node, I can think about how to handle them.
                    ).ToArray();


                    var outerLookedUp = LookUpOrOverlayOrThrow2(from, genericNameKey.Name, realizedGeneric);

                    return outerLookedUp.SwitchReturns(method =>
                    {
                        var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(method), types);

                        if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                        {
                            return res2;
                        }

                        // this is duplicate code - 94875369485
                        var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                        foreach (var (oldType, newType) in types.Zip(method.GenericOverlays, (x, y) => (y.Value, x)))
                        {
                            map[oldType] = newType;
                        }

                        var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(method), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this.builder, $"generated-generic-{method.debugName}", method.Converter)), map);

                        return @explicit.SwitchReturns(v1 =>
                        {
                            realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
                        }, v2 =>
                        {
                            realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
                        });

                    },
                    type =>
                    {
                        var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), types);

                        if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                        {
                            return res2;
                        }

                        // this is duplicate code - 94875369485
                        var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                        foreach (var (oldType, newType) in types.Zip(type.GenericOverlays, (x, y) => (y.Value, x)))
                        {
                            map[oldType] = newType;
                        }

                        var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this.builder, $"generated-generic-{type.debugName}", type.Key, type.Converter, type.IsPlaceHolder, Possibly.IsNot<Guid>())), map);

                        return @explicit.SwitchReturns(v1 =>
                        {
                            realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
                        },
                        v2 =>
                        {
                            realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
                        });

                    },
                    @object => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object),
                    orType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType),
                    inferredType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferredType),
                    error => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error));
                }
                else if (TryLookUp(from, key, out var res2))
                {
                    //:'(
                    // I am sad about the !
                    return res2!;
                }
                else
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Error.TypeNotFound($"could not find type for {key.ToString()} in {from.ToString()}"));
                }
            }

            // [MaybeNullWhen(false)] grumble
            static bool TryLookUp(IStaticScope haveTypes, IKey key, out OrType<MethodType, Type, Object, OrType, InferredType, IError>? result)
            {
                while (true)
                {
                    {
                        if (haveTypes.Types.TryGetValue(key, out var res))
                        {
                            result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                            return true;
                        }
                    }
                    {
                        if (haveTypes is IStaticScope scope && scope.Objects.TryGetValue(key, out var res))
                        {
                            result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                            return true;
                        }
                    }
                    {
                        if (haveTypes.OrTypes.TryGetValue(key, out var res))
                        {
                            result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                            return true;
                        }
                    }
                    {
                        if (haveTypes.MethodTypes.TryGetValue(key, out var res))
                        {
                            result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                            return true;
                        }
                    }
                    if (haveTypes.SafeIs(out Type _) || haveTypes.SafeIs(out MethodType _))
                    {
                        Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> genericOverlays;
                        if (haveTypes.SafeIs(out Type type))
                        {
                            genericOverlays = type.GenericOverlays;
                        }
                        else if (haveTypes.SafeIs(out MethodType methodType1))
                        {
                            genericOverlays = methodType1.GenericOverlays;
                        }
                        else
                        {
                            throw new Exception("💩💩💩💩💩");
                        }

                        if (genericOverlays.TryGetValue(key, out var res))
                        {
                            if (res.Is1(out var methodType))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType);
                                return true;
                            }
                            else if (res.Is2(out var innerType))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(innerType);
                                return true;
                            }
                            else if (res.Is6(out var error))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error);
                                return true;
                            }
                            else
                            {
                                throw new Exception("uh oh! we hit a type we did not want");
                            }
                        }
                    }

                    if (haveTypes.Parent.SafeIs(out IIsDefinately<IStaticScope> defScope))
                    {
                        haveTypes = defScope.Value;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }
            }

            void HandleHopefulMember(IKey key, Member hopeful, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type)
            {
                type.Switch(
                    x => { },
                    x =>
                    {
                        if (x.PublicMembers.TryGetValue(key, out var member))
                        {
                            TryMerge(hopeful, member);
                        }
                        // uhh this member is an error
                        // do I need to do something?
                    },
                    x =>
                    {
                        if (x.PublicMembers.TryGetValue(key, out var member))
                        {
                            TryMerge(hopeful, member);
                        }
                        // uhh this member is an error
                        // do I need to do something?
                    },
                    orType =>
                    {
                        // we pretty much need to recurse
                        // we hope this member is on both sides

                        // can or types even be implicit?
                        // no but they can have members that are implicit 
                        // plus maybe they could be in the future this some sort of implicit key word
                        // number || implict 
                        //orType.Left.If(x => HandleHopefulMember(key, hopeful, GetType(x)));
                        //orType.Right.If(x => HandleHopefulMember(key, hopeful, GetType(x)));

                        //orType.Left.IfElse(x => 
                        //    orType.Right.IfElse(
                        //        y => HandleHopefulMember(key, hopeful,  GetType(x), GetType(y)),
                        //        ()=> HandleHopefulMember(key, hopeful, GetType(x))),
                        //    () => orType.Right.If(x => HandleHopefulMember(key, hopeful, GetType(x))));

                        // you are here!
                        // after the first one merges it might not be a infered type
                        // so it can't merge

                        // how does this work?? or them and then merge?
                        // yes, you or them and then merge

                        //--- no!
                        // I think it actually does not defer
                        // maybe it just has an assignment relationship to each?

                        // maybe it just doesn't defer?
                        // ok for now it just doesn't defer 
                        // kind of makes sense, who would it defer to?


                    },
                    inferredType =>
                    {
                        if (inferredType.PublicMembers.TryGetValue(key, out var member))
                        {
                            TryMerge(hopeful, member);
                        }
                        else
                        {
                            Builder.HasPublicMember(inferredType, key, hopeful);
                        }
                    },
                    error =>
                    {

                    });
            }

            IOrType<MethodType, Type> CopyTree(IOrType<MethodType, Type> from, IOrType<MethodType, Type> to, IReadOnlyDictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> overlayed)
            {

                var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();

                from.Switch(from1 =>
                {
                    if (to.Is1(out var to1))
                    {
                        Copy(from1, to1);
                    }
                    else
                    {
                        throw new Exception("or exception");
                    }
                },
                from2 =>
                {
                    if (to.Is2(out var to2))
                    {
                        Copy(from2, to2);
                    }
                    else
                    {
                        throw new Exception("or exception");
                    }
                });

                foreach (var pair in map)
                {
                    if (pair.Key.SafeIs(out IStaticScope fromScope) && fromScope.Parent.SafeIs(out IIsDefinately<IStaticScope> defScope))
                    {
                        to.Switch(method =>
                        {
                            method.Parent = Possibly.Is(CopiedToOrSelf(defScope.Value));
                        },
                        type =>
                        {
                            type.Parent = Possibly.Is(CopiedToOrSelf(defScope.Value));
                        });
                    }
                }

                var oldAssignments = assignments.ToArray();
                foreach (var pair in map)
                {
                    if (pair.Key.SafeIs(out ICanBeAssignedTo assignedToFrom) && pair.Value.SafeIs(out ICanBeAssignedTo assignedToTo))
                    {
                        foreach (var item in oldAssignments)
                        {
                            if (item.Item2 == assignedToFrom)
                            {
                                assignments.Add((CopiedToOrSelf(item.Item1), assignedToTo));
                            }
                        }
                    }

                    if (pair.Value.SafeIs(out ICanAssignFromMe assignFromFrom) && pair.Value.SafeIs(out ICanAssignFromMe assignFromTo))
                    {
                        foreach (var item in oldAssignments)
                        {
                            if (item.Item1 == assignFromFrom)
                            {
                                assignments.Add((assignFromTo, CopiedToOrSelf(item.Item2)));
                            }
                        }
                    }
                }

                foreach (var pair in map)
                {
                    if (pair.Key.SafeIs(out ILookUpType lookUpFrom) && pair.Value.SafeIs(out ILookUpType lookUpTo))
                    {

                        lookUpTo.TypeKey = lookUpFrom.TypeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));

                        if (lookUpFrom.Context.SafeIs(out IIsDefinately<IStaticScope> definateContext))
                        {
                            lookUpTo.Context = Possibly.Is(CopiedToOrSelf(definateContext.Value));
                        }
                    }

                    if (pair.Key.SafeIs(out OrType orFrom) && pair.Value.SafeIs(out OrType orTo))
                    {
                        // from should have been populated
                        Builder.Ors(orTo, CopiedToOrSelf(orFrom.Left.GetOrThrow()), CopiedToOrSelf(orFrom.Right.GetOrThrow()));
                    }


                    if (pair.Key.SafeIs(out Method methodFrom) && pair.Value.SafeIs(out Method methodTo))
                    {
                        methodTo.Input = Possibly.Is(CopiedToOrSelf(methodFrom.Input.GetOrThrow()));
                        methodTo.Returns = Possibly.Is(CopiedToOrSelf(methodFrom.Returns.GetOrThrow()));
                    }

                    if (pair.Key.SafeIs(out MethodType methodFromType) && pair.Value.SafeIs(out MethodType methodToType))
                    {
                        methodToType.Input = Possibly.Is(CopiedToOrSelf(methodFromType.Input.GetOrThrow()));
                        methodToType.Returns = Possibly.Is(CopiedToOrSelf(methodFromType.Returns.GetOrThrow()));
                    }

                    if (pair.Key.SafeIs(out InferredType inferedFrom) && pair.Value.SafeIs(out InferredType inferedTo))
                    {
                        inferedTo.Input = Possibly.Is(CopiedToOrSelf(inferedFrom.Input.GetOrThrow()));
                        inferedTo.Returns = Possibly.Is(CopiedToOrSelf(inferedFrom.Returns.GetOrThrow()));
                    }
                }

                return to;

                T CopiedToOrSelf<T>(T item)
                    where T : ITypeProblemNode
                {
                    if (map.TryGetValue(item, out var res))
                    {
                        return (T)res;
                    }
                    return item;
                }

                // TOOD copy should me a instance method

                // hasGenerics -- the root of the root will have had its generics replaced
                // for the rest of the tree the generics will need to be copied
                T Copy<T>(T innerFrom, T innerTo)
                    where T : ITypeProblemNode
                {
                    map.Add(innerFrom, innerTo);


                    if (innerFrom.SafeIs<ITypeProblemNode, IStaticScope>(out var innerFromStaticScope) && innerTo.SafeIs<ITypeProblemNode, IStaticScope>(out var innerStaticScopeTo))
                    {

                        {
                            foreach (var item in innerFromStaticScope.Refs)
                            {
                                var newValue = Copy(item, new TypeReference(this.builder, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                Builder.HasReference(innerStaticScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var @object in innerFromStaticScope.Objects)
                            {
                                var newValue = Copy(@object.Value, new Object(this.builder, $"copied from {((TypeProblemNode)@object.Value).debugName}", @object.Value.Converter, @object.Value.InitizationScope.Converter));
                                Builder.HasObject(innerStaticScopeTo, @object.Key, newValue);
                            }

                        }

                        {
                            foreach (var type in innerFromStaticScope.Types)
                            {
                                var newValue = Copy(type.Value, new Type(this.builder, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Key, type.Value.Converter, type.Value.IsPlaceHolder, type.Value.PrimitiveId));
                                Builder.HasType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                        {
                            foreach (var method in innerFromStaticScope.Methods)
                            {
                                var newValue = Copy(method.Value, new Method(this.builder, $"copied from {((TypeProblemNode)method.Value).debugName}", method.Value.Converter));
                                Builder.HasMethod(innerStaticScopeTo, method.Key, newValue);
                            }
                        }

                        {
                            foreach (var type in innerFromStaticScope.OrTypes)
                            {
                                var newValue = Copy(type.Value, new OrType(this.builder, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                Builder.HasOrType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                    }


                    if (innerFrom.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerFromPublicMembers) && innerTo.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerToPublicMembers))
                    {


                        {
                            foreach (var member in innerFromPublicMembers.PublicMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this.builder, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                Builder.HasPublicMember(innerToPublicMembers, member.Key, newValue);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IScope>(out var innerFromScope) && innerTo.SafeIs<ITypeProblemNode, IScope>(out var innerScopeTo))
                    {

                        {
                            foreach (var item in innerFromScope.Values)
                            {
                                var newValue = Copy(item, new Value(this.builder, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                Builder.HasValue(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.TransientMembers)
                            {
                                var newValue = Copy(member, new TransientMember(this.builder, $"copied from {((TypeProblemNode)member).debugName}"));
                                Builder.HasTransientMember(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.PrivateMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this.builder, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                Builder.HasPrivateMember(innerScopeTo, member.Key, newValue);
                            }
                        }

                        {
                            foreach (var possible in innerFromScope.PossibleMembers)
                            {
                                var newValue = Copy(possible.Value, new Member(this.builder, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                Builder.HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, Type>(out var innerFromType) && innerTo.SafeIs<ITypeProblemNode, Type>(out var innerTypeTo))
                    {

                        foreach (var type in innerFromType.GenericOverlays)
                        {
                            if (overlayed.TryGetValue(type.Value, out var toType))
                            {
                                builder.HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, toType);
                            }
                            else
                            {
                                builder.HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, type.Value);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, MethodType>(out var innerFromMethodType) && innerTo.SafeIs<ITypeProblemNode, MethodType>(out var innerMethodTypeTo))
                    {

                        foreach (var type in innerFromMethodType.GenericOverlays)
                        {
                            if (overlayed.TryGetValue(type.Value, out var toType))
                            {
                                builder.HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, toType);
                            }
                            else
                            {
                                builder.HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, type.Value);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IValue>(out var innerFromHopeful) && innerTo.SafeIs<ITypeProblemNode, IValue>(out var innerToHopeful))
                    {

                        foreach (var possible in innerFromHopeful.HopefulMembers)
                        {
                            var newValue = Copy(possible.Value, new Member(this.builder, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                            Builder.HasHopefulMember(innerToHopeful, possible.Key, newValue);
                        }


                        if (innerFromHopeful.HopefulMethod is IIsDefinately<InferredType> infered)
                        {
                            var newValue = Copy(infered.Value, new InferredType(this.builder, $"copied from {((TypeProblemNode)infered.Value).debugName}"));
                            innerToHopeful.HopefulMethod = Possibly.Is(newValue);
                        }
                    }

                    // return is handled higher up
                    //if (innerFrom.SafeIs<ITypeProblemNode, IHaveInputAndOutput>(out var innerFromIO) && innerTo.SafeIs<ITypeProblemNode, IHaveInputAndOutput>(out var innerToIO))
                    //{
                    //    // yucky cast!
                    //    // map is really T -> T
                    //    // we know this
                    //    // but maybe should be wrapped up so we don't have to know this here
                    //    innerToIO.Input = Possibly.Is((Member)map[innerFromIO.Input.GetOrThrow()]);
                    //    innerToIO.Returns = Possibly.Is((TransientMember)map[innerFromIO.Returns.GetOrThrow()]);
                    //}

                    return innerTo;
                }
            }


            // TODO this does not need to be a method
            static bool IsNotInferedHasMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, out IHavePublicMembers? haveMembers)
            {
                var res = false;
                (haveMembers, res) = type.SwitchReturns<(IHavePublicMembers?, bool)>(
                    v1 => (default, false),
                    v2 => (v2, true),
                    v3 => (v3, true),
                    v4 => (default, false),
                    v5 => (default, false),
                    v1 => (default, false));

                return res;
            }

            // OK I think OrTypes should just make a infered type to represent them 

            // it seems very expensive to me to be making all these infered types

            // once apon a time I did something like this and I needed to cache the concrete representations of OrTypes
            // I think this might cause trouble:
            // Type X {
            //      Y | X member;
            //}
            //
            // Type Y {
            //    Y member;
            // }
            // this is really going to try to walk it
            // and that is bad!

            // actually that just might cause trouble more generally
            // x;
            // Y y;
            // x =: y;
            // Type Y {
            //    Y member;
            // }

            // I am not sure that is fixable
            // RIP type problem

            // so infered types.
            // they are uniquely indentified by what flows in to them
            // but that is so complex to maintain!

            // ok so here is the plan
            // I am going to have a dictionary maps: list of nodes flowing in  ->  thing they are flowing in to
            // I think the RHS is always an InferredType you can't really flow to anything else
            // we don't bother making a new infered type if one already exists with our inflows
            // if are inflow is new (it must be signular if it is new) we make one
            // if we are adding a new inflow we copy the existing inferred type that has the existing inflows and then add to it

            // for this to work we don't want any inferred types directly referenced 
            // for each existing inferred type we make a new inferred type and set the old type as an inflow to the new type

            // but that kind of mean different representations in different parts of the pipeline
            // I would love to do that but it would be a real project

            // I think I need some new fields for this phase
            // the big type or is different here
            // it needs to include in-flow-set, a new class I need to make

            static IIsPossibly<Member> TryGetMember(IStaticScope context, IKey key)
            {
                while (true)
                {
                    if (context is IHavePrivateMembers privateMembers && privateMembers.PrivateMembers.TryGetValue(key, out var member))
                    {
                        return Possibly.Is(member);
                    }
                    if (context is IHavePublicMembers publicMembers && publicMembers.PublicMembers.TryGetValue(key, out var member2))
                    {
                        return Possibly.Is(member2);
                    }
                    if (context.Parent is IIsDefinately<IStaticScope> defParent)
                    {
                        context = defParent.Value;
                    }
                    else
                    {
                        return Possibly.IsNot<Member>();
                    }
                }
            }

            #endregion

            static IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> TryGetType(ITypeProblemNode value) {
                if (value.SafeIs(out ILookUpType lookup))
                {
                    // look up needs to be populated at this point
                    return Possibly.Is( lookup.LooksUp.GetOrThrow());
                }
                if (value.SafeIs(out MethodType methodType))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType));
                }
                if (value.SafeIs(out Type type))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type));

                }
                if (value.SafeIs(out Object @object))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object));
                }
                if (value.SafeIs(out OrType orType))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType));
                }
                return Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
            }

            static IOrType<MethodType, Type, Object, OrType, InferredType, IError> GetType(ITypeProblemNode value)
            {
                return TryGetType(value).IfElseReturn(
                    x => x, 
                    () =>
                {
                    throw new Exception($"flaming pile of piss. Type unexpected {value.GetType().Name}");
                    // well, I guess I now know that we have a duality
                    // you either are a type, or you have a type
                    // 

                    // or you are a scope
                });

               
            }


            public TypeProblem2(IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> moduleConverter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter)
            {
                builder = new Builder(this);
                Primitive = new Scope(this.builder, "base", rootConverter);
                Dependency = builder.CreateScope(Primitive, rootConverter);
                ModuleRoot = builder.CreateObjectOrModule(builder.CreateScope(Dependency, rootConverter), new ImplicitKey(Guid.NewGuid()), moduleConverter, innerConverter);

                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("block")), new PrimitiveTypeConverter(new BlockType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("number")), new PrimitiveTypeConverter(new NumberType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("string")), new PrimitiveTypeConverter(new StringType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("bool")), new PrimitiveTypeConverter(new BooleanType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("empty")), new PrimitiveTypeConverter(new EmptyType()), Possibly.Is(Guid.NewGuid()));

                // shocked this works...
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1"))), new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2"))) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] { new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1")), new WeakTypeDefinitionConverter()), new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2")), new WeakTypeDefinitionConverter()) };

                var res = new MethodType(
                    this.builder,
                    $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                builder.HasMethodType(Primitive, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this.builder, $"generic-parameter-{placeholder.key}", Possibly.Is(placeholder.key), placeholder.converter, true, Possibly.IsNot<Guid>());
                    builder.HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(res), placeholder.key.SwitchReturns<IKey>(x => x, x => x), Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(placeholderType));
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                // here it is ok for these to be members because we are using a method type
                res.Input = Possibly.Is(builder.CreatePrivateMember(res, res, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T1")), new WeakMemberDefinitionConverter(Model.Elements.Access.ReadWrite, methodInputKey)));
                res.Returns = Possibly.Is(builder.CreateTransientMember(res, new NameKey("T2")));
                builder.IsChildOf(Primitive, res);
            }

            private class GenericTypeKey
            {
                private readonly IOrType<MethodType, Type> primary;
                private readonly IOrType<MethodType, Type, Object, OrType, InferredType, IError>[] parameters;

                public GenericTypeKey(IOrType<MethodType, Type> primary, IOrType<MethodType, Type, Object, OrType, InferredType, IError>[] parameters)
                {
                    this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
                    this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
                }

                public override bool Equals(object? obj)
                {
                    return Equals(obj as GenericTypeKey);
                }

                public bool Equals(GenericTypeKey? other)
                {
                    return other != null &&
                        primary.Equals(other.primary) &&
                        parameters.Length == other.parameters.Length &&
                        parameters.Zip(other.parameters, (x, y) => x.Equals(y)).All(x => x);
                }

                public override int GetHashCode()
                {
                    return unchecked(primary.GetHashCode() + parameters.Aggregate(0, (y, x) => unchecked(y + x.GetHashCode())));
                }
            }
        }
    }
}
