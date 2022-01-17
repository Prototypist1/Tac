using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.Model.Elements;
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
                public string DebugName { get; }

                public TypeProblemNode(Builder problem, string debugName)
                {
                    Problem = problem ?? throw new ArgumentNullException(nameof(problem));
                    this.DebugName = debugName;
                    problem.Register(this);
                }

                public Builder Problem { get; }

                public override string? ToString()
                {
                    return this.GetType().Name + $"({DebugName})";
                }
            }

            public abstract class TypeProblemNode<Tin, Tout> : TypeProblemNode//, IConvertable<T>
            {

                public TypeProblemNode(Builder problem, string debugName, IConvertTo<Tin, Tout> converter) : base(problem, debugName)
                {
                    Converter = converter;
                }

                internal IConvertTo<Tin, Tout> Converter { get; }
            }
            public class TypeReference : TypeProblemNode<TypeReference, IFrontendType<IVerifiableType>>, ILookUpType
            {
                public TypeReference(Builder problem, string debugName, IConvertTo<TypeReference, IFrontendType<IVerifiableType>> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
            }

            // can be assigned from, but not to, doesn't have a name
            public class Value : TypeProblemNode, IValue, ILookUpType
            {
                public Value(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();

                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();
            }

            // can be assigned to and from, has a name
            public class Member : TypeProblemNode, IMember
            {
                public Member(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();
            }

            // can be assigned to and from, doesn't have a name
            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();

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
            public class Type : TypeProblemNode<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>>, IExplicitType, IHavePossibleMembers
            {
                public Type(
                    Builder problem,
                    string debugName,
                    IIsPossibly<IOrType<NameKey, ImplicitKey>> key,
                    IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter,
                    IIsPossibly<Guid> primitiveId,
                    IIsPossibly<IInterfaceType> external
                    ) : base(problem, debugName, converter)
                {
                    Key = key ?? throw new ArgumentNullException(nameof(key));
                    PrimitiveId = primitiveId ?? throw new ArgumentNullException(nameof(primitiveId));
                    External = external ?? throw new ArgumentNullException(nameof(external));
                }

                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Guid> PrimitiveId { get; }
                //public List<Scope> EntryPoints { get; } = new List<Scope>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
                public Dictionary<NameKey, GenericTypeParameter> Generics { get; } = new Dictionary<NameKey, GenericTypeParameter>();
                public Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> GenericOverlays { get; } = new Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
                public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }
                public IIsPossibly<IInterfaceType> External { get; }
            }

            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members

            // is a method type really a scope??
            // I don't think so, I don't think this should have IScope or IHavePossibleMembers
            // it's input and output sort of count as members and I think that is why it is like this 
            public class MethodType : TypeProblemNode<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IScope, IHavePossibleMembers
            {
                public MethodType(Builder problem, string debugName, IConvertTo<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PrivateMembers { get; } = new Dictionary<IKey, Member>();
                //public List<Scope> EntryPoints { get; } = new List<Scope>();
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

                public Dictionary<NameKey, GenericTypeParameter> Generics { get; } = new Dictionary<NameKey, GenericTypeParameter>();
                public Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> GenericOverlays { get; } = new Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
            }

            public class InferredType : TypeProblemNode, IHaveInputAndOutput, IHavePublicMembers //, IScope
            {
                public readonly IIsPossibly<GenericTypeParameter> constraintFor = Possibly.IsNot<GenericTypeParameter>();

                public InferredType(Builder problem, string debugName) : base(problem, debugName)
                {
                }
                public InferredType(Builder problem, string debugName, GenericTypeParameter constraintFor) : base(problem, debugName)
                {
                    this.constraintFor = Possibly.Is(constraintFor);
                }
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();

                // TODO
                // can infered types have generics?
                // seems like they could
                public Dictionary<NameKey, GenericTypeParameter> Generics { get; } = new Dictionary<NameKey, GenericTypeParameter>();
            }

            public class OrType : TypeProblemNode<OrType, IFrontendType<IVerifiableType>>
            {
                public OrType(Builder problem, string debugName, IConvertTo<OrType, IFrontendType<IVerifiableType>> converter) : base(problem, debugName, converter)
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
                //public List<Scope> EntryPoints { get; } = new List<Scope>();
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

            public class Object : TypeProblemNode<Object, IOrType<WeakObjectDefinition, WeakRootScope>>, IExplicitType, IHavePossibleMembers
            {
                public Object(Builder problem, string debugName, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakRootScope>> converter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter) : base(problem, debugName, converter)
                {
                    InitizationScope = new Scope(problem, debugName, innerConverter)
                    {
                        Parent = Possibly.Is(this)
                    };
                }
                public readonly Scope InitizationScope;
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();
                //public List<Scope> EntryPoints { get; } = new List<Scope>();
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
            public class Method : TypeProblemNode<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>>, IScope, IHaveInputAndOutput
            {
                public Method(Builder problem, string debugName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PrivateMembers { get; } = new Dictionary<IKey, Member>();
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
                // {4BFD0274-B70F-4BD8-B290-63B69FF74FE7}
                public Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> GenericOverlays { get; } = new Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();
                public Dictionary<NameKey, GenericTypeParameter> Generics { get; } = new Dictionary<NameKey, GenericTypeParameter>();

            }

            public class GenericTypeParameter : TypeProblemNode
            {
                /// <param name="index">zero indexed</param>
                public GenericTypeParameter(Builder problem, string debugName, int index, IOrType<MethodType, Type, Method, InferredType> owner) : base(problem, debugName)
                {
                    constraint = new InferredType(problem, "constraint-for-" + debugName,this);
                    this.index = index;
                    this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
                }

                public readonly InferredType constraint;
                /// <summary>
                /// zero indexed 
                /// </summary>
                public readonly int index;
                public readonly IOrType<MethodType, Type, Method, InferredType> owner;
            }

            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new();


            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }

            public readonly Builder builder;

            // these are pretty much the same
            private readonly List<(IOrType<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>, IOrType<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>)> assignments = new();
            private readonly MethodType rootMethod;

            // I am interested in rewriting large parts of thi
            // instead of merging I can have the defering node flow in to the dominate node
            // the "outside" should get info out of here by looking up with keys 
            // instead of having members flow through 


            // pretty sure it is not safe to solve more than once 
            public TypeSolution Solve()
            {
                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => x.LooksUp.IsNot() && x.TypeKey.Is3(out var _)))
                {
                    var type = new InferredType(this.builder, $"for {((TypeProblemNode)node).DebugName}");
                    node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(type));
                }

                // what happens here if x.TypeKey.Is2??
                if (toLookUp.Any(x => x.LooksUp.IsNot() && x.TypeKey.Is2(out var _)))
                {
                    throw new NotImplementedException();
                }

                #region Handle generics



                // if type [t1] node {}
                // node [t1] [t1]
                // or is it
                // node [] [t1]
                // ?
                // we want next to look up to the root in this example:
                // type [t1] node { node [t1] next }
                // and next has a GenericTypeKey of node [] [t1] where t1 looks up t1 on the parent
                // so it is node [] [t1]
                // of course node [t1] [t1] collapese to node [] [t1]
                // I wonder if I should capture that here
                // no, I think I should capture that in GenericTypeKey
                // maybe in equalaity node [t1] [t1] = node [] [t1]
                // TODO ^ update this is wrong I use node [t1] [t1]

                // in 
                // type [t1] node { node [t1] next }
                // the type is defined as a GenericTypeKey with the type it created for T1 in GenericOverlays not a GenericTypeKey.TypeParameter
                // this... isn't great
                // 

                var steam = 0;

                var realizedGeneric = new Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>();


                // this while is where because copy tree creates new looks
                while (typeProblemNodes.OfType<ILookUpType>().Any(x => x.LooksUp.IsNot()))
                {
                    steam++;
                    if (steam > 1000)
                    {
                        throw new Exception("in a loop!");
                    }


                    foreach (var item in typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp.IsNot()).ToArray())
                    {
                        item.LooksUp = Possibly.Is(OuterLookUpOrError(item, realizedGeneric, WalkParent(item.Context.GetOrThrow())));

                    }

                    // do circcle examples work?
                    // can they overlay over each other?
                    // A [T]
                    // B [T]
                    // generic-A [T1] [generic-B [T2] [Pair[T1,T2]]
                    // generic-B [T1] [generic-A [T2] [Pair[T1,T2]]
                    // this is fine, the type that is being overlayed does need to be populated 

                }
                #endregion

                var flows = new List<(IOrType<ITypeProblemNode, IError> From, IOrType<ITypeProblemNode, IError> To)> { };

                foreach (var (from, to) in assignments)
                {
                    var toType = to.SwitchReturns(
                        y => y.LooksUp.GetOrThrow().SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)),
                        y => y.SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)));
                    var fromType = from.SwitchReturns(
                        y => y.LooksUp.GetOrThrow().SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)),
                        y => y.SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)));


                    flows.Add((From: fromType, To: toType));
                }

                // members that might be on parents 
                // parents need to be done before children 
                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();
                //var deference = new Dictionary<IValue, IValue>();

                foreach (var (possibleMembers, staticScope, node) in typeProblemNodes
                    .SelectMany(node =>
                    {
                        if (node is IHavePossibleMembers possibleMembers && node is IStaticScope staticScope)
                        {
                            return new (IHavePossibleMembers, IStaticScope, ITypeProblemNode)[] { (possibleMembers, staticScope, node) };
                        }
                        return Array.Empty<(IHavePossibleMembers, IStaticScope, ITypeProblemNode)>();
                    })
                    .OrderBy(x => Height(x.Item2)).ToArray()
                    )
                {
                    foreach (var pair in possibleMembers.PossibleMembers)
                    {
                        TryGetMember(staticScope, pair.Key).IfElse(
                            member =>
                            {
                                var assignedTo = pair.Value.LooksUp.GetOrThrow().SwitchReturns(
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));

                                var assignedFrom = member.LooksUp.GetOrThrow().SwitchReturns(
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));

                                flows.Add((From: assignedFrom, To: assignedTo));

                                // if anything looks up to the possible node, it should indstead look up to the node we defer to
                                // do we have to do this in a "deep" manor?
                                // there could be a whole tree of hopeful nodes off the possible node
                                // I think I am ok
                                // say we have z.b.c
                                // where b and c are hopeful
                                // b would look up to the right thing
                                // and then c would look up off b
                                // so as long is c is right b should be right 
                                var startingValue = pair.Value.LooksUp;
                                foreach (var item in typeProblemNodes.OfType<ILookUpType>())
                                {
                                    if (item.LooksUp == startingValue)
                                    {
                                        item.LooksUp = member.LooksUp;
                                    }
                                }
                                //deferingTypes.Add(flowFrom, flowTo);
                            },
                            () =>
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


                // hopeful members and methods are a little rough around the edges
                // they are very similar yet implemented differently 


                // hopeful members 
                foreach (var node in typeProblemNodes.OfType<IValue>())
                {
                    if (node.Hopeful.Is(out var hopeful))
                    {
                        var flowTo = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hopeful);
                        var flowFrom = node.LooksUp.GetOrThrow().SwitchReturns(
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));

                        flows.Add((From: flowFrom, To: flowTo));

                        // if anything looks up to the hopeful node, it should instead look up to the node we defer to
                        foreach (var item in typeProblemNodes.OfType<ILookUpType>())
                        {
                            if (item.LooksUp == hopeful)
                            {
                                item.LooksUp = node.LooksUp;
                            }
                        }
                    }
                }

                var ors = typeProblemNodes
                    .Select(node => TryGetType(node))
                    .SelectMany(x => { if (x.Is(out var res)) { return new[] { res }; } else { return new IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter,Method, IError>[] { }; } })
                    .Distinct()
                    .ToArray();

                var flowNodes2 = new Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>>();
                var concreteHelp = new Dictionary<InferredType, ConcreteFlowNode2>();

                foreach (var methodType in ors.Select(x => (x.Is1(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(methodType);

                    var flowNode2 = new ConcreteFlowNode2();
                    flowNodes2.Add(key, ToOr2(flowNode2));

                }
                foreach (var type in ors.Select(x => (x.Is2(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    if (type.PrimitiveId.Is(out var guid))
                    {
                        var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type);

                        var flowNode2 = new PrimitiveFlowNode2(guid, type);
                        flowNodes2.Add(key, ToOr2(flowNode2));
                    }
                    else
                    {
                        var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type);

                        var flowNode2 = new ConcreteFlowNode2();
                        flowNode2.HasMembers();
                        flowNodes2.Add(key, ToOr2(flowNode2));

                        if (type.External.Is(out var external))
                        {
                            var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(type)];
                            targetOr.Is1OrThrow().IsExternal(external);
                        }
                    }
                }
                foreach (var @object in ors.Select(x => (x.Is3(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(@object);

                    var flowNode2 = new ConcreteFlowNode2();
                    flowNode2.HasMembers();
                    flowNodes2.Add(key, ToOr2(flowNode2));
                }
                foreach (var inferred in ors.Select(x => (x.Is5(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(inferred);

                    var inferredFlowNode2 = new InferredFlowNode2();
                    flowNodes2.Add(key, ToOr2(inferredFlowNode2));

                    // we create a concrete flow node to pass the concrete aspects to the infered node
                    var concreteFlowNode2 = new ConcreteFlowNode2();
                    concreteHelp.Add(inferred, concreteFlowNode2);
                }
                foreach (var generic in ors.Select(x => (x.Is6(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    // no op
                }
                foreach (var method in ors.Select(x => (x.Is7(out Method v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(method);

                    var inferredFlowNode2 = new ConcreteFlowNode2();
                    flowNodes2.Add(key, ToOr2(inferredFlowNode2));
                }
                foreach (var error in ors.Select(x => (x.Is8(out IError v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var key = Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(error);

                    var inferredFlowNode2 = new InferredFlowNode2();
                    flowNodes2.Add(key, ToOr2(inferredFlowNode2));
                }

                {
                    var todo = ors.Select(x => (x.Is4(out var v), v)).Where(x => x.Item1).Select(x => x.v).ToArray();
                    var excapeValve = 0;

                    // or types are a bit of a project because they might depend on each other
                    while (todo.Any())
                    {
                        excapeValve++;
                        var nextTodo = new List<TypeProblem2.OrType>();
                        foreach (var or in todo)
                        {
                            if (TryToOuterFlowNode2(flowNodes2, or, out var res))
                            {
                                flowNodes2.Add(Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(or), ToOr2(res));
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
                }

                // we create members on our new representation
                foreach (var hasPublicMembers in ors.Select(x => (x.Is(out IHavePublicMembers members), members)).Where(x => x.Item1).Select(x => x.members))
                {
                    var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasPublicMembers)];

                    if (targetOr.Is1(out var concreteFlowNode2))
                    {

                    }
                    else if (targetOr.Is2(out var inferredFlowNode2))
                    {
                        concreteFlowNode2 = concreteHelp[hasPublicMembers.SafeCastTo(out InferredType _)];
                    }
                    else
                    {
                        continue;
                    }

                    foreach (var member in hasPublicMembers.PublicMembers)
                    {
                        concreteFlowNode2.AddMember(member.Key, flowNodes2[member.Value.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                }

                // private members - but just for methods 
                foreach (var method in ors.Select(x => (x.Is7(out Method v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var concreteFlowNode2 = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(method)].Is1OrThrow();


                    foreach (var member in method.PrivateMembers)
                    {
                        concreteFlowNode2.AddPrivateMember(member.Key, flowNodes2[member.Value.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                    x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                }

                // we create input and output on our new implmentation
                foreach (var hasInputAndOutput in ors.Select(x => (x.Is(out IHaveInputAndOutput io), io)).Where(x => x.Item1).Select(x => x.io))
                {
                    var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(hasInputAndOutput)];
                    if (targetOr.Is1(out var concreteFlowNode2))
                    {

                    }
                    else if (targetOr.Is2(out var inferredFlowNode2))
                    {
                        concreteFlowNode2 = concreteHelp[hasInputAndOutput.SafeCastTo(out InferredType _)];
                    }
                    else
                    {
                        continue;
                    }

                    if (hasInputAndOutput.Input.Is(out var input))
                    {
                        concreteFlowNode2.AddInput(flowNodes2[input.LooksUp.GetOrThrow().SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                    if (hasInputAndOutput.Returns.Is(out var output))
                    {
                        concreteFlowNode2.AddOutput(flowNodes2[output.LooksUp.GetOrThrow().SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.constraint),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x))]);
                    }
                }

                // we just do generics for method types
                // everyone one else still get erased
                // until I rethink that
                foreach (var methodType in ors.Select(x => (x.Is1(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(methodType)];
                    var target = targetOr.Is1OrThrow();

                    var constraints = methodType.Generics.Select(x => flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.Value.constraint)]).ToArray();
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        target.AddGeneric(constraints[i],i);
                        constraints[i].Is2OrThrow().IsConstraintFor(methodType.DebugName + "-" + i);
                    }
                }
                foreach (var method in ors.Select(x => (x.Is7(out Method v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(method)];
                    var target = targetOr.Is1OrThrow();

                    var constraints = method.Generics.Select(x => flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.Value.constraint)]).ToArray();
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        target.AddGeneric(constraints[i], i);
                        constraints[i].Is2OrThrow().IsConstraintFor(method.DebugName + "-" + i);
                    }
                }

                // infered types as well, they are probably methods
                foreach (var inferredType in ors.Select(x => (x.Is5(out var v), v)).Where(x => x.Item1).Select(x => x.v))
                {
                    var targetOr = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(inferredType)];
                    if (targetOr.Is1(out var concreteFlowNode2))
                    {

                    }
                    else if (targetOr.Is2(out var inferredFlowNode2))
                    {
                        concreteFlowNode2 = concreteHelp[inferredType.SafeCastTo(out InferredType _)];
                    }
                    else
                    {
                        continue;
                    }


                    var constraints = inferredType.Generics.Select(x => flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x.Value.constraint)]).ToArray();
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        concreteFlowNode2.AddGeneric(constraints[i], i);
                        constraints[i].Is2OrThrow().IsConstraintFor(inferredType.DebugName + "-" + i);
                    }
                }

                {
                    var excapeValve = 0;

                    bool go;
                    do
                    {
                        go = false;

                        // "to" and "from" here are what we are assigning to and what we are assinign form

                        foreach (var pair in concreteHelp)
                        {
                            var to = pair.Value;
                            var from = flowNodes2[Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(pair.Key)];

                            from.GetValueAs(out IFlowNode2 _).AcceptConstraints(to.GetConstraints(), new List<(IFlowNode2, IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>)>())
                                .Switch(_ => { }, _ => { go = true; }, _ => { throw new NotImplementedException("blacklist, restart the flow!"); });

                            to.AcceptConstraints(from.GetValueAs(out IFlowNode2 _).GetConstraints().ToDownStream().Select(x => x.Broaden()).ToHashSet(), new List<(IFlowNode2, IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>)>())
                                .Switch(_ => { }, _ => { go = true; }, _ => { throw new NotImplementedException("blacklist, restart the flow!"); });
                        }

                        foreach (var (from, to) in flows)
                        {
                            flowNodes2[from].GetValueAs(out IFlowNode2 _).AcceptConstraints(flowNodes2[to].GetValueAs(out IFlowNode2 _).GetConstraints(), new List<(IFlowNode2, IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>)>())
                                .Switch(_ => { }, _ => { go = true; }, _ => { throw new NotImplementedException("blacklist, restart the flow!"); });

                            flowNodes2[to].GetValueAs(out IFlowNode2 _).AcceptConstraints(flowNodes2[from].GetValueAs(out IFlowNode2 _).GetConstraints().ToDownStream().Select(x => x.Broaden()).ToHashSet(), new List<(IFlowNode2, IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>)>())
                                .Switch(_ => { }, _ => { go = true; }, _ => { throw new NotImplementedException("blacklist, restart the flow!"); });

                        }

                        excapeValve++;
                        if (excapeValve > 1000000)
                        {
                            throw new Exception("probably stuck in a loop");
                        }

                    } while (go);
                }

                return new TypeSolution(ors, flowNodes2);
            }



            #region Helpers

            // this is probably slow a dumb
            static IEnumerable<T> Add<T>(IEnumerable<T> current, T next)
            {
                foreach (var item in current)
                {
                    yield return item;
                }
                yield return next;
            }

            private static int Height(IStaticScope staticScope)
            {
                var res = 0;
                while (staticScope.Parent.Is(out var parent))
                {
                    staticScope = parent;
                    res++;
                }
                return res;
            }

            private static bool TryToOuterFlowNode2(Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> orsToFlowNodes, Tpn.TypeProblem2.OrType or, out OrFlowNode2 res)
            {
                if (orsToFlowNodes.TryGetValue(GetType(or.Left.GetOrThrow()).SwitchReturns(
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
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
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                           x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x)), out var right))
                {

                    res = new OrFlowNode2(left, right);
                    return true;
                }
                res = default;
                return false;
            }

            Prototypist.Toolbox.IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError> OuterLookUpOrError(ILookUpType item, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> realizedGenerics, IEnumerable<IStaticScope> contextStack)
            {
                if (item.LooksUp.Is(out var res))
                {
                    return res;
                }

                var key = item.TypeKey.Is1OrThrow()/*lazy Is1OrThrow*/;

                return OuterLookUpOrError(contextStack, key, realizedGenerics);

            }

            private IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError> OuterLookUpOrError(
                IEnumerable<IStaticScope> context,
                IKey key,
                Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> realizedGenerics)
            {
                IOrType<MethodType, Type>? newType = null;
                IOrType<MethodType, Type>? from = null;
                IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>[]? overlays = null;

                if (key.SafeIs(out GenericNameKey genericNameKey))
                {
                    from = LookUpOrError(context, genericNameKey.Name).SwitchReturns(
                        methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type>(methodType),
                        type => Prototypist.Toolbox.OrType.Make<MethodType, Type>(type),
                        _ => throw new Exception(""),
                        _ => throw new Exception(""),
                        _ => throw new Exception(""),
                        _ => throw new Exception(""),
                        _ => throw new Exception(""));

                    overlays = genericNameKey.Types.Select(x => OuterLookUpOrError(context, x.Is1OrThrow()/*lazy*/, realizedGenerics)).ToArray();

                    var genericTypeKey = new GenericTypeKey(from, overlays);

                    if (realizedGenerics.TryGetValue(genericTypeKey, out var res))
                    {
                        return res;
                    }

                    newType = from.SwitchReturns(
                        methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this.builder, key.ToString() ?? "", methodType.Converter)),
                        type => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this.builder, key.ToString() ?? "", type.Key, type.Converter, Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>())));

                    realizedGenerics.Add(genericTypeKey, newType.SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(x)));
                }
                else if (key.SafeIs(out DoubleGenericNameKey doubleGenericNameKey))
                {
                    from = LookUpOrError(context, doubleGenericNameKey.Name).SwitchReturns(
                       methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type>(methodType),
                       type => Prototypist.Toolbox.OrType.Make<MethodType, Type>(type),
                       _ => throw new Exception(""),
                       _ => throw new Exception(""),
                       _ => throw new Exception(""),
                       _ => throw new Exception(""),
                       _ => throw new Exception(""));


                    newType = from.SwitchReturns(
                       methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this.builder, key.ToString() ?? "", methodType.Converter)),
                       type => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this.builder, key.ToString() ?? "", type.Key, type.Converter, Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>())));

                    var i = 0;
                    foreach (var item in doubleGenericNameKey.Types)
                    {
                        builder.HasGenericType(
                            newType.SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x),
                                x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x)),
                            item,
                            new GenericTypeParameter(
                                builder,
                                $"generic-parameter-{item}",
                                i++,
                                newType.SwitchReturns(
                                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method, InferredType>(x),
                                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method, InferredType>(x))));
                    }

                    // ThisThenThose because we need to beable to look up the generics we defined above
                    // we set parent in copyTree
                    // so I didn't want to set it here as well
                    // probably would be a ok, but seemed dangerous
                    overlays = doubleGenericNameKey.DependentTypes.Select(x => OuterLookUpOrError(ThisThenThose(newType.GetValueAs(out IStaticScope _), context)/*lazy GetOrThrow*/, x.Is1OrThrow()/*lazy*/, realizedGenerics)).ToArray();
                }
                else
                {
                    return LookUpOrError(context, key);
                }

                if (newType == null || from == null || overlays == null)
                {
                    throw new Exception("this is sloppy");
                }

                // do I really need to make place holders?
                // yeah, for a reference like generic-method [t1] [t1, t1] 
                //var i = 0;
                //foreach (var placeholder in from.SwitchReturns(x => x.Generics, x => x.Generics))
                //{
                //    //var key = new ImplicitKey(Guid.NewGuid());
                //    var placeholderType = new GenericTypeParameter(this.builder, placeholder.ToString(), i++);
                //    //var placeholderType = new Type(this.builder, placeholder.ToString(), Possibly.Is(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(key)), new WeakTypeDefinitionConverter(),  Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>());

                //    builder.HasGenericType(
                //        newType.SwitchReturns(
                //            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x),
                //            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x)),
                //        placeholder.Key,
                //        placeholderType);
                //}

                var map = from.SwitchReturns(x => x.Generics.Values, x => x.Generics.Values)
                    .Zip(overlays, (from, to) => (from, to))
                    .ToDictionary(pair => pair.from, pair => pair.to);

                CopyTree(from, newType, map);

                return newType.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, TypeProblem2.GenericTypeParameter, IError>(x),
                                    x => Prototypist.Toolbox.OrType.Make<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, TypeProblem2.GenericTypeParameter, IError>(x));
            }

            // {7FF166EC-C833-4032-B674-32E82103EB0D}
            static Prototypist.Toolbox.IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError> LookUpOrError(IEnumerable<IStaticScope> haveTypeses, IKey key)
            {
                foreach (var haveTypes in haveTypeses)
                {

                    {
                        if (haveTypes.Types.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(res);
                        }
                    }
                    {
                        if (haveTypes is IStaticScope scope && scope.Objects.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(res);
                        }
                    }
                    {
                        if (haveTypes.OrTypes.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(res);
                        }
                    }
                    {
                        if (haveTypes.MethodTypes.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(res);
                        }
                    }
                    if (key.SafeIs(out NameKey nameKey) && (haveTypes.SafeIs(out Type _) || haveTypes.SafeIs(out MethodType _) || haveTypes.SafeIs(out Method _)))
                    {
                        {
                            Dictionary<NameKey, GenericTypeParameter> generics;
                            if (haveTypes.SafeIs(out Type type))
                            {
                                generics = type.Generics;
                            }
                            else if (haveTypes.SafeIs(out MethodType methodType1))
                            {
                                generics = methodType1.Generics;
                            }
                            else if (haveTypes.SafeIs(out Method method))
                            {
                                generics = method.Generics;
                            }
                            else
                            {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (generics.TryGetValue(nameKey, out var res))
                            {
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(res);
                            }
                        }
                        {
                            Dictionary<NameKey, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> genericOverlays;
                            if (haveTypes.SafeIs(out Type type))
                            {
                                genericOverlays = type.GenericOverlays;
                            }
                            else if (haveTypes.SafeIs(out MethodType methodType1))
                            {
                                genericOverlays = methodType1.GenericOverlays;
                            }
                            else if (haveTypes.SafeIs(out Method method))
                            {
                                genericOverlays = method.GenericOverlays;
                            }
                            else
                            {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (genericOverlays.TryGetValue(nameKey, out var res))
                            {
                                return res;
                                //if (res.Is1(out var methodType))
                                //{
                                //    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(methodType);
                                //}
                                //else if (res.Is2(out var innerType))
                                //{
                                //    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(innerType);
                                //}
                                //else if (res.Is7(out IError error))
                                //{
                                //    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(error); ;
                                //}
                                //else
                                //{
                                //    throw new Exception("uh oh! we hit a type we did not want");
                                //}
                            }
                        }
                    }
                }
                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(Error.TypeNotFound($"could not find type for {key}"));
            }

            private static IEnumerable<IStaticScope> WalkParent(IStaticScope source)
            {
                yield return source;
                while (source.Parent.SafeIs(out IIsDefinately<IStaticScope> defScope))
                {
                    source = defScope.Value;
                    yield return source;
                }
            }

            private static IEnumerable<IStaticScope> ThisThenThose(IStaticScope first, IEnumerable<IStaticScope> then)
            {
                yield return first;
                foreach (var item in then)
                {
                    yield return item;
                }
            }

            // should take IOrType<(MethodType from ,MethodType to),  (Type from , Type to)> pair 
            IOrType<MethodType, Type> CopyTree(IOrType<MethodType, Type> from, IOrType<MethodType, Type> to, IReadOnlyDictionary<GenericTypeParameter, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>> overlayed)
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

                foreach (var item in assignments.ToArray())
                {
                    var newFrom = item.Item1.SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(CopiedToOrSelf(x)),
                        x => x.SwitchReturns(
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter,Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(y))));
                    var newTo = item.Item2.SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(CopiedToOrSelf(x)),
                        x => x.SwitchReturns(
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(CopiedToOrSelf(y))),
                            y => Prototypist.Toolbox.OrType.Make<ILookUpType, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>>(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>(y))));
                    if (!item.Item1.Equals(newFrom) || !item.Item2.Equals(newTo))
                    {
                        assignments.Add((newFrom, newTo));
                    }
                }

                foreach (var pair in map)
                {
                    if (pair.Key.SafeIs(out ILookUpType lookUpFrom) && pair.Value.SafeIs(out ILookUpType lookUpTo))
                    {

                        lookUpTo.TypeKey = lookUpFrom.TypeKey.SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x),
                            x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x),
                            x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));

                        if (lookUpFrom.Context.SafeIs(out IIsDefinately<IStaticScope> definateContext))
                        {
                            lookUpTo.Context = Possibly.Is(CopiedToOrSelf(definateContext.Value));
                        }

                        // uhhh
                        // I don't think this works
                        // what if we are replacing T with int
                        // and this looks up Pair[T]
                        // maybe that is 
                        //if (lookUpFrom.LooksUp.Is(out var lookUp)) {
                        //    lookUpTo.LooksUp = Possibly.Is(lookUp.SwitchReturns(
                        //        x=> Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>( CopiedToOrSelf(x)),
                        //        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(CopiedToOrSelf(x)),
                        //        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(CopiedToOrSelf(x)),
                        //        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(CopiedToOrSelf(x)),
                        //        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(CopiedToOrSelf(x)),
                        //        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x)));
                        //}
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
                                var newValue = Copy(item, new TypeReference(this.builder, $"copied from {((TypeProblemNode)item).DebugName}", item.Converter));
                                Builder.HasReference(innerStaticScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var @object in innerFromStaticScope.Objects)
                            {
                                var newValue = Copy(@object.Value, new Object(this.builder, $"copied from {((TypeProblemNode)@object.Value).DebugName}", @object.Value.Converter, @object.Value.InitizationScope.Converter));
                                Builder.HasObject(innerStaticScopeTo, @object.Key, newValue);
                            }

                        }

                        {
                            foreach (var type in innerFromStaticScope.Types)
                            {
                                if (type.Value.External.Is(out var _))
                                {
                                    throw new Exception("I need to think about what this means");
                                    // it's a real problem
                                    // when I have a generic type in another assembly...
                                    // list is going to be an external generic type
                                    // how is that going to work?
                                    // {762D2B2D-607D-4D66-AC01-3A309DDB851D}
                                    // I think external generics have to be handled very differently

                                }

                                var newValue = Copy(type.Value, new Type(this.builder, $"copied from {((TypeProblemNode)type.Value).DebugName}", type.Value.Key, type.Value.Converter, type.Value.PrimitiveId, type.Value.External));
                                Builder.HasType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                        {
                            foreach (var method in innerFromStaticScope.Methods)
                            {
                                var newValue = Copy(method.Value, new Method(this.builder, $"copied from {((TypeProblemNode)method.Value).DebugName}", method.Value.Converter));
                                Builder.HasMethod(innerStaticScopeTo, method.Key, newValue);
                            }
                        }

                        {
                            foreach (var type in innerFromStaticScope.OrTypes)
                            {
                                var newValue = Copy(type.Value, new OrType(this.builder, $"copied from {((TypeProblemNode)type.Value).DebugName}", type.Value.Converter));
                                Builder.HasOrType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                    }


                    if (innerFrom.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerFromPublicMembers) && innerTo.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerToPublicMembers))
                    {
                        {
                            foreach (var member in innerFromPublicMembers.PublicMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this.builder, $"copied from {((TypeProblemNode)member.Value).DebugName}"));
                                Builder.HasPublicMember(innerToPublicMembers, member.Key, newValue);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IScope>(out var innerFromScope) && innerTo.SafeIs<ITypeProblemNode, IScope>(out var innerScopeTo))
                    {

                        {
                            foreach (var item in innerFromScope.Values)
                            {
                                var newValue = Copy(item, new Value(this.builder, $"copied from {((TypeProblemNode)item).DebugName}"));
                                Builder.HasValue(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.TransientMembers)
                            {
                                var newValue = Copy(member, new TransientMember(this.builder, $"copied from {((TypeProblemNode)member).DebugName}"));
                                Builder.HasTransientMember(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.PrivateMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this.builder, $"copied from {((TypeProblemNode)member.Value).DebugName}"));
                                Builder.HasPrivateMember(innerScopeTo, member.Key, newValue);
                            }
                        }

                        {
                            foreach (var possible in innerFromScope.PossibleMembers)
                            {
                                Builder.HasMembersPossiblyOnParent(innerScopeTo, possible.Key, () => Copy(possible.Value, new Member(this.builder, $"copied from {((TypeProblemNode)possible.Value).DebugName}")));
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, Type>(out var innerFromType) && innerTo.SafeIs<ITypeProblemNode, Type>(out var innerTypeTo))
                    {
                        {
                            foreach (var type in innerFromType.Generics)
                            {
                                if (!overlayed.TryGetValue(type.Value, out var toType))
                                {
                                    builder.HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerTypeTo), type.Key, type.Value);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerTypeTo), type.Key, toType);
                                }
                            }
                        }
                        {
                            foreach (var or in innerFromType.GenericOverlays)
                            {
                                if (or.Value.Is6(out var genericType) && overlayed.TryGetValue(genericType, out var toType))
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerTypeTo), or.Key, toType);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerTypeTo), or.Key, or.Value);
                                }
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, MethodType>(out var innerFromMethodType) && innerTo.SafeIs<ITypeProblemNode, MethodType>(out var innerMethodTypeTo))
                    {
                        {
                            foreach (var type in innerFromMethodType.Generics)
                            {
                                if (!overlayed.TryGetValue(type.Value, out var toType))
                                {
                                    builder.HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTypeTo), type.Key, type.Value);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTypeTo), type.Key, toType);
                                }
                            }
                        }
                        {
                            foreach (var or in innerFromMethodType.GenericOverlays)
                            {
                                if (or.Value.Is6(out var genericType) && overlayed.TryGetValue(genericType, out var toType))
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTypeTo), or.Key, toType);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTypeTo), or.Key, or.Value);
                                }
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, Method>(out var innerFromMethod) && innerTo.SafeIs<ITypeProblemNode, Method>(out var innerMethodTo))
                    {
                        {
                            foreach (var type in innerFromMethod.Generics)
                            {
                                if (!overlayed.TryGetValue(type.Value, out var toType))
                                {
                                    builder.HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTo), type.Key, type.Value);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTo), type.Key, toType);
                                }
                            }
                        }
                        {
                            foreach (var or in innerFromMethod.GenericOverlays)
                            {
                                if (or.Value.Is6(out var genericType) && overlayed.TryGetValue(genericType, out var toType))
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTo), or.Key, toType);
                                }
                                else
                                {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerMethodTo), or.Key, or.Value);
                                }
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IValue>(out var innerFromHopeful) && innerTo.SafeIs<ITypeProblemNode, IValue>(out var innerToHopeful))
                    {
                        if (innerFromHopeful.Hopeful.Is(out var infered))
                        {
                            var newValue = Copy(infered, new InferredType(this.builder, $"copied from {((TypeProblemNode)infered).DebugName}"));
                            innerToHopeful.Hopeful = Possibly.Is(newValue);
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

            static IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>> TryGetType(ITypeProblemNode value)
            {
                if (value.SafeIs(out ILookUpType lookup))
                {
                    // look up needs to be populated at this point
                    return Possibly.Is(lookup.LooksUp.GetOrThrow().SwitchReturns(
                        x=> Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(x)));
                }
                if (value.SafeIs(out MethodType methodType))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError >(methodType));
                }
                if (value.SafeIs(out Type type))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(type));
                }
                if (value.SafeIs(out Method method))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(method));
                }
                if (value.SafeIs(out Object @object))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(@object));
                }
                if (value.SafeIs(out OrType orType))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(orType));
                }
                if (value.SafeIs(out InferredType inferred))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>(inferred));
                }
                return Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError>>();
            }

            static IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, Method, IError> GetType(ITypeProblemNode value)
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


            public TypeProblem2(IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, RootScopePopulateScope rootScopePopulateScope, Action<TypeProblem2> initDependencyScope)
            {
                builder = new Builder(this);

                // primitives are probably really external types
                // but they are going to have special logic however I handle them 

                Primitive = new Scope(this.builder, "base", rootConverter);
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("block")), new PrimitiveTypeConverter(new BlockType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("number")), new PrimitiveTypeConverter(new NumberType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("string")), new PrimitiveTypeConverter(new StringType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("bool")), new PrimitiveTypeConverter(new BooleanType()), Possibly.Is(Guid.NewGuid()));
                builder.CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("empty")), new PrimitiveTypeConverter(new EmptyType()), Possibly.Is(Guid.NewGuid()));

                // we have to define the generic form of method
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] {
                    new TypeAndConverter(new NameKey("T_BASE_INPUT"), new WeakTypeDefinitionConverter()),
                    new TypeAndConverter(new NameKey("T_BASE_OUTPUT"), new WeakTypeDefinitionConverter()) };
                // {D9E7A7AF-75B6-4918-98E9-78DA72AEB6FF}
                // these names "T_BASE_INPUT" are trouble
                // I think if you create a generic method with the right generic
                // say method [T_BASE_INPUT,] [empty, T_BASE_INPUT]
                // it will LookUpOrError ({7FF166EC-C833-4032-B674-32E82103EB0D}) badly 
                // T_BASE_INPUT (the one on the base method) is overlayed with empty
                // but it will see that that is the name of a generic (on the generic method) and look up to that
                // TODO unit test

                rootMethod = new MethodType(
                    this.builder,
                    $"generic-{key}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                builder.HasMethodType(Primitive, key, rootMethod);
                var i = 0;
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new GenericTypeParameter(this.builder, $"generic-parameter-{placeholder.key}", i++, Prototypist.Toolbox.OrType.Make<MethodType, Type, Method, InferredType>(rootMethod));
                    //var placeholderType = new Type(this.builder, $"generic-parameter-{placeholder.key}", Possibly.Is(placeholder.key), placeholder.converter, Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>());
                    builder.HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(rootMethod), placeholder.key, placeholderType);
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                // here it is ok for these to be members because we are using a method type
                rootMethod.Input = Possibly.Is(builder.CreatePrivateMember(rootMethod, rootMethod, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T_BASE_INPUT"))));
                rootMethod.Returns = Possibly.Is(builder.CreateTransientMember(rootMethod, new NameKey("T_BASE_OUTPUT"), $"return of {rootMethod.DebugName} "));
                builder.IsChildOf(Primitive, rootMethod);

                Dependency = builder.CreateScope(Primitive, rootConverter);
                initDependencyScope(this);
                ModuleRoot = rootScopePopulateScope.InitizeForTypeProblem(this);
            }

            private class GenericTypeKey
            {
                private readonly IOrType<MethodType, Type> primary;
                private readonly IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>[] parameters;

                public GenericTypeKey(IOrType<MethodType, Type> primary, IOrType<MethodType, Type, Object, OrType, InferredType, GenericTypeParameter, IError>[] parameters)
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
                    unchecked
                    {
                        var res = primary.GetHashCode();

                        foreach (var parameter in parameters)
                        {
                            res += parameter.GetHashCode(); 
                        }
                        return res;
                    }
                }
            }
        }
    }
}
