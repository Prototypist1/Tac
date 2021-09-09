using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.IEnumerable;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return new Dictionary<IKey, Member>();
                //}
            }

            public class Value : TypeProblemNode<Value, PlaceholderValue>, IValue, ILookUpType
            {
                public Value(Builder problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return HopefulMembers;
                //}
            }
            public class Member : TypeProblemNode, IMember
            {
                public Member(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return HopefulMembers;
                //}
            }

            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(Builder problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IStaticScope> Context { get; set; } = Possibly.IsNot<IStaticScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<InferredType> Hopeful { get; set; } = Possibly.IsNot<InferredType>();

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return HopefulMembers;
                //}
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
                public Dictionary<IKey, Type> Generics { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }
                public IIsPossibly<IInterfaceType> External { get; }
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

                public Dictionary<IKey, Type> Generics { get; } = new Dictionary<IKey, Type>();
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

            //public class PrimitiveScope : Scope
            //{
            //    public PrimitiveScope(Builder problem, string debugName, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter) : base(problem, debugName, converter)
            //    {
            //    }


            //}
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
            public class Method : TypeProblemNode<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>>, IScope, IHaveInputAndOutput, IHavePossibleMembers
            {
                public Method(Builder problem, string debugName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter) : base(problem, debugName, converter)
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
                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public Dictionary<IKey, Type> Generics { get; } = new Dictionary<IKey, Type>();

            }

            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new();


            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }

            public readonly Builder builder;

            // these are pretty much the same
            private readonly List<(ILookUpType, ILookUpType)> assignments = new();
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
                foreach (var node in toLookUp.Where(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>) && x.TypeKey.Is3(out var _)))
                {
                    var type = new InferredType(this.builder, $"for {((TypeProblemNode)node).DebugName}");
                    node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type));
                }

                // what happens here if x.TypeKey.Is2??
                if (toLookUp.Any(x => !(x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>) && x.TypeKey.Is2(out var _)))
                {
                    throw new NotImplementedException();
                }

                #region Handle generics


                var keyAndTypes = new Dictionary<GenericTypeKey, OrType<MethodType, Type>>();
                // two way map.. probably need a TwoWayDictonary type, that wraps a pair of dictionaries 
                var typeParametersAndTypes = new Dictionary<GenericTypeKey.TypeParameter, Type>();
                var overlaysToTypeParameters = new Dictionary<Type, GenericTypeKey.TypeParameter>();

                // why do I order by height?
                // we register types top down
                // so types lower down can look up type higher up
                // in perticular overlaysToTypeParameters
                // need to be populated top down
                // for type [t1] outer { generic-method [t2] [t1,t2] outer }
                // we want to outer to go first so t1 can resolve to the typeParameter
                // the mechinism here is overlaysToTypeParameters
                foreach (var item in typeProblemNodes.OfType<Type>().Where(x => x.Generics.Any()).OrderBy(x => Height(x)))
                {
                    var visitor = KeyVisitor.Base(y => LookUpOrError(item, y), overlaysToTypeParameters);
                    var key = new DoubleGenericNameKey(item.Key.GetOrThrow().Is1OrThrow(), item.Generics.Keys.Select(x => x.SafeCastTo(out NameKey _)).ToArray(), item.Generics.Keys.Select(x => Prototypist.Toolbox.OrType.Make<IKey, IError>(x)).ToArray());
                    var genericNameKey = visitor.DuobleGenericNameKey_BetterType_PassInPrimary(key, Prototypist.Toolbox.OrType.Make<MethodType, Type>( item));

                    foreach (var (overlay, typeParameter) in item.Generics.Join(genericNameKey.sourceTypes,x=>x.Key, x=>x.Key, (x,y)=>(x.Value,y.Value)))
                    {
                        overlaysToTypeParameters.Add(overlay, typeParameter);
                        typeParametersAndTypes.Add(typeParameter, overlay);
                    }

                    keyAndTypes.TryAdd(genericNameKey, Prototypist.Toolbox.OrType.Make<MethodType, Type>(item));
                }
                foreach (var item in typeProblemNodes.OfType<MethodType>().Where(x => x.Generics.Any()).OrderBy(x => Height(x)))
                {
                    var visitor = KeyVisitor.Base(y => LookUpOrError(item, y), overlaysToTypeParameters);
                    var key = new DoubleGenericNameKey(new NameKey("method"), item.Generics.Keys.Select(x=>x.SafeCastTo(out NameKey _)).ToArray() , item.Generics.Keys.Select(x => Prototypist.Toolbox.OrType.Make<IKey, IError>(x)).ToArray());
                    var genericNameKey = visitor.DuobleGenericNameKey_BetterType_PassInPrimary(key, Prototypist.Toolbox.OrType.Make<MethodType, Type >(item));

                    foreach (var (overlay, typeParameter) in item.Generics.Join(genericNameKey.sourceTypes, x => x.Key, x => x.Key, (x, y) => (x.Value, y.Value)))
                    {
                        overlaysToTypeParameters.Add(overlay, typeParameter);
                        typeParametersAndTypes.Add(typeParameter, overlay);
                    }

                    keyAndTypes.TryAdd(genericNameKey, Prototypist.Toolbox.OrType.Make<MethodType, Type>(item));
                }

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

                var completeLookUps = typeProblemNodes.OfType<ILookUpType>()
                       .Where(x => x.LooksUp.Is(out var _)) // why do somethings already know what they look up? yeah, inferred types (type test {x;})
                       .ToList();

                //if (typeProblemNodes.OfType<ILookUpType>()
                //       .Where(x => (x.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>)).Any()) {
                //    throw new Exception("does this happen at this point?");

                //    completeLookUps.AddRange();
                //}

                var incompleteLookups = typeProblemNodes.OfType<ILookUpType>()
                       .Except(completeLookUps)
                       .ToArray();

                var steam = 0;
                
                // this while is where because copy tree creates new looks
                while (incompleteLookups.Length != 0)
                {
                    steam++;
                    if (steam > 1000) {
                        throw new Exception("in a loop!");
                    }

                    // convert all keys to a consistant format
                    // 1. make all the new types for the GenericTypeKeys 
                    var lookupsAndTarget = incompleteLookups
                       .Select(x => (x, x.TypeKey.Is1OrThrow()/*if we don't have a look up we should have a typeKey*/.Visit(KeyVisitor.Base(y => LookUpOrError(x.Context.GetOrThrow(), y), overlaysToTypeParameters))))
                       .ToList();

                    // create types for realized and double generics
                    // 2. make their GenericOverlays from GenericTypeKey.sourceTypes + put them in a dict 
                    var distintTargets = lookupsAndTarget
                        .SelectMany(x => Walk(x.Item2)) // we have to "walk" for pair[pair[chicken]] we want to resolve pair[pair[chicken]] and pair[chicken]
                        .Except(keyAndTypes.Keys)
                        .Distinct().ToArray();

                    foreach (var (genericTypeKey, newType) in distintTargets
                        .Select(genericTypeKey =>
                        {
                            var newType = genericTypeKey.primary.SwitchReturns(
                                methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this.builder, genericTypeKey.ToString(), methodType.Converter)),
                                type => Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this.builder, genericTypeKey.ToString(), type.Key, type.Converter,  Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>())));

                            // do I really need to make place holders?
                            // yeah, for a reference like generic-method [t1] [t1, t1] 
                            foreach (var placeholder in genericTypeKey.sourceTypes)
                            {
                                var key = new ImplicitKey(Guid.NewGuid());
                                var placeholderType = new Type(this.builder, placeholder.ToString(), Possibly.Is(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(key)), new WeakTypeDefinitionConverter(),  Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>());
                                typeParametersAndTypes.Add(placeholder.Value, placeholderType);
                                builder.HasGenericType(
                                    newType.SwitchReturns(
                                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x),
                                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(x)),
                                    key,
                                    placeholderType);
                                overlaysToTypeParameters.Add(placeholderType, placeholder.Value);
                            }

                            return (genericTypeKey, newType);
                        }))
                    {
                        keyAndTypes.Add(genericTypeKey, newType);
                    }

                    // do circcle examples work?
                    // can they overlay over each other?
                    // A [T]
                    // B [T]
                    // generic-A [T1] [generic-B [T2] [Pair[T1,T2]]
                    // generic-B [T1] [generic-A [T2] [Pair[T1,T2]]
                    // this is fine, the type that is being overlayed does need to be populated 

                    // 4. copy tree 
                    foreach (var (key, value) in distintTargets.Select(x => (x, keyAndTypes[x])))
                    {
                        var map = key.primary.SwitchReturns(x => x.Generics.Values, x => x.Generics.Values)
                            .Zip(
                                key.parameters.Select(parameter => parameter.SwitchReturns(
                                    methodType => (IOrType<MethodType, Type, Object, OrType, InferredType, IError>)Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType),
                                    type => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type),
                                    @object => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object),
                                    orType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType),
                                    inferredType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferredType),
                                    ierror => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(ierror),
                                    typeParameter => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(typeParametersAndTypes[typeParameter]),
                                    genericTypeKey => keyAndTypes[genericTypeKey].SwitchReturns(
                                        methodType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType),
                                        type => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type)))),
                                (from, to) => (from, to))
                            .ToDictionary(pair => pair.from, pair => pair.to);

                        CopyTree(key.primary, value, map);
                    }

                    completeLookUps.AddRange(lookupsAndTarget.Select(x => x.x));

                    incompleteLookups = typeProblemNodes.OfType<ILookUpType>()
                       .Except(completeLookUps)
                       .ToArray();
                }
                #endregion

                {
                    var lookupsAndTarget = typeProblemNodes.OfType<ILookUpType>()
                       .Where(x => x.LooksUp.IsNot())
                       .Select(x => (x, x.TypeKey.Is1OrThrow()/*if we don't have a look up we should have a typeKey*/.Visit(KeyVisitor.Base(y => LookUpOrError(x.Context.GetOrThrow(), y),  overlaysToTypeParameters))))
                       .ToList();

                    // resolve lookups
                    // 3. look up
                    //      normal stuff looks up in th normal way with look up or throw
                    //      GenericTypeKeys look up in the dict from step 2 
                    //      GenericTypeKeys.TypeParameter looks up the parent and then the type
                    foreach (var (lookup, target) in lookupsAndTarget)
                    {
                        target.Switch(
                            methodType => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType)); },
                            type => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type)); },
                            @object => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object)); },
                            orType => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType)); },
                            inferredType => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferredType)); },
                            ierror => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(ierror)); },
                            typeParameter => { lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(typeParametersAndTypes[typeParameter])); },
                            genericTypeKey =>
                            {
                                if (!keyAndTypes.ContainsKey(genericTypeKey)) {
                                    lookup.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Error.Other("type not found")));
                                }
                                else {
                                    lookup.LooksUp = keyAndTypes[genericTypeKey].SwitchReturns(
                                        methodType => Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType)),
                                        type => Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type)));
                                }
                            });
                    }
                }
                // assignemtns become flow
                // this is a more flexable type
                // assignments are values
                // flow are types
                // you can flow types without having values
                // we do this with hopeful stuff 
                // we could not use types before because maybe values don't have types until solve 

                var flows = new List<(IOrType<ITypeProblemNode, IError> From, IOrType<ITypeProblemNode, IError> To)> { };

                //var deferingTypes = new Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ITypeProblemNode, IError>> { };

                foreach (var (from, to) in assignments)
                {
                    var toType = to.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));
                    var fromType = from.LooksUp.GetOrThrow().SwitchReturns(
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));


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
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x));

                                var assignedFrom = member.LooksUp.GetOrThrow().SwitchReturns(
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
                                        x => Prototypist.Toolbox.OrType.Make<ITypeProblemNode, IError>(x),
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
                    .OfType<IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>>()
                    .Select(x => x.Value)
                    .Distinct()
                    .ToArray();

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
                    inferredFlowNode.ReturnedSources.Add(new SourcePath(Prototypist.Toolbox.OrType.Make<PrimitiveFlowNode, ConcreteFlowNode, OrFlowNode, InferredFlowNode>(concrete), new List<IOrType<Tpn.Member, Tpn.Input, Tpn.Output>>()));
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

                    foreach (var (from, to) in flows)
                    {
                        go |= orsToFlowNodesLookup[to].GetValueAs(out IFlowNode _).MustAccept(
                            orsToFlowNodesLookup[from].GetValueAs(out IVirtualFlowNode _),
                            new List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)>());

                    }

                    foreach (var (from, to) in flows)
                    {
                        go |= orsToFlowNodesLookup[from].GetValueAs(out IFlowNode _).MustReturn(
                            orsToFlowNodesLookup[to].GetValueAs(out IVirtualFlowNode _),
                            new List<(IVirtualFlowNode, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>)>());
                    }

                    excapeValve++;
                    if (excapeValve > 1000000)
                    {
                        throw new Exception("probably stuck in a loop");
                    }

                } while (go);

                return new TypeSolution(ors, orsToFlowNodesLookup);
            }



            #region Helpers

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

            private static HashSet<GenericTypeKey> Walk(IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> toWalk) {
                var res = new HashSet<GenericTypeKey>();
                Walk(toWalk, res);
                return res;
            }

            private static void Walk(IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> toWalk, HashSet<GenericTypeKey> set)
            {

                if (toWalk.Is8(out var genericTypeKey))
                {
                    if (set.Contains(genericTypeKey))
                    {
                        return;
                    }
                    set.Add(genericTypeKey);

                    foreach (var item in genericTypeKey.parameters)
                    {
                        Walk(item, set);
                    }
                }
            }

            //IOrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrOverlayOrThrow(ILookUpType node, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> realizedGeneric)
            //{

            //    if (node.LooksUp is IIsDefinately<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> nodeLooksUp)
            //    {
            //        return nodeLooksUp.Value;
            //    }

            //    // if we don't have a lookup we damn well better have a context and a key
            //    var res = TryLookUpOrOverlay(node.Context.GetOrThrow(), node.TypeKey.Is1OrThrow(), realizedGeneric);
            //    node.LooksUp = Possibly.Is(res);
            //    return res;
            //}

            //IOrType<MethodType, Type, Object, OrType, InferredType, IError> TryLookUpOrOverlay(IStaticScope from, IKey key, Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> realizedGeneric)
            //{
            //    if (key.SafeIs(out DoubleGenericNameKey doubleGenericNameKey)) {


            //        // do I need to cache locally?
            //        // that is, on the from.

            //        // two generic-method [T] [T, Builder]
            //        // could be different if defined in scopes with different Builder types
            //        // but they could also be the same

            //        // I am not sure what I currently do with generics handles this very nicely... 
            //        // realizedGeneric is global 
            //        // yeah, actually it does
            //        // because it resolves all the types so the two builders would be different


            //        // I think I can just overlay MethodType with GenericTypeKey and have some of the types be Types that are
            //        // GenericOverlays from a new MethodType built here


            //        // 1. create new methodType with the right type overlays
            //        // 2. create the GenericTypeKey to overlay the generic methodType with the second set from the DoubleGenericNameKey

            //        // but the GenericTypeKey is going use the generic type parameters from the new methodType so it will never match anything
            //        // so I need a new key type

            //        // it needs to use real looked up types when possible
            //        // but some sort of dummy types for the generics
            //        // the type is a set of trees made from visiting DoubleGenericNameKey.DependentTypes

            //        // how does it handle genenic-method [T] [T, generic-mthod [T1] [T1, T]]?
            //        // it would try to look up the inner generic-mthod and end up here
            //        // and that would try to look up T  ...and would it find it? it better.

            //        // I think maybe these method types should be added in "populate scope"
            //        // no because you can't look all the type up...

            //        // the inner one has to be looking up from inside the outer method definition
            //        // after all the real type defintions have gone, these have to go

            //        // and they have to understand where they are in the scope stack
            //        // in this case:
            //        // genenic-method [T] [T, generic-mthod [T1] [T1, T]]
            //        // the outer need to go first
            //        // and it need to look stuff up but not try to look up the inner
            //        // once the outer has made a methodType (ut just need to have GenericOverlays it can't have input/output yet) the inner can look up "T" off it

            //        // 1. build types for the tree
            //        //      start outer
            //        //          start inner
            //        //          complete inner
            //        //      complete outer
            //        // 2. build key for inner
            //        //      build key for outer

            //        // what happen when we have generic inside a double generic
            //        // double generic inside generic inside double generic?






            //    }

            //    if (key is GenericNameKey genericNameKey)
            //    {

            //        var types = genericNameKey.Types.Select(typeKey => typeKey.SwitchReturns(
            //            x => TryLookUpOrOverlay(from, x, realizedGeneric),
            //            y => throw new NotImplementedException("Type could not be resolved")) // I want to use NIEs to keep my code near compilablity. once I have a lot in type problem node, I can think about how to handle them.
            //        ).ToArray();


            //        var outerLookedUp = LookUpOrError(from, genericNameKey.Name);

            //        return outerLookedUp.SwitchReturns(
            //            methodType =>
            //            {
            //                var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(methodType), types);

            //                if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
            //                {
            //                    return res2;
            //                }

            //                // this is duplicate code - 94875369485
            //                var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
            //                foreach (var (oldType, newType) in types.Zip(methodType.GenericOverlays, (x, y) => (y.Value, x)))
            //                {
            //                    map[oldType] = newType;
            //                }

            //                var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(methodType), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this.builder, $"generated-generic-{methodType.DebugName}", methodType.Converter)), map);

            //                return @explicit.SwitchReturns(v1 =>
            //                {
            //                    realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
            //                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
            //                }, v2 =>
            //                {
            //                    realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
            //                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
            //                });

            //            },
            //            type =>
            //            {
            //                var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(type), types);

            //                if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
            //                {
            //                    return res2;
            //                }

            //                // this is duplicate code - 94875369485
            //                var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
            //                foreach (var (oldType, newType) in types.Zip(type.GenericOverlays, (x, y) => (y.Value, x)))
            //                {
            //                    map[oldType] = newType;
            //                }

            //                var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this.builder, $"generated-generic-{type.DebugName}", type.Key, type.Converter, type.IsPlaceHolder, Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>())), map);

            //                return @explicit.SwitchReturns(v1 =>
            //                {
            //                    realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
            //                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
            //                },
            //                v2 =>
            //                {
            //                    realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
            //                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
            //                });

            //            },
            //            @object => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object),
            //            orType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType),
            //            inferredType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferredType),
            //            error => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error));
            //    }
            //    else if (TryLookUp(from, key, out var res2))
            //    {
            //        //:'(
            //        // I am sad about the !
            //        return res2!;
            //    }
            //    else
            //    {
            //        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Error.TypeNotFound($"could not find type for {key} in {from}"));
            //    }
            //}

            static OrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrError(IStaticScope haveTypes, IKey key)
            {
                while (true)
                {
                    {
                        if (haveTypes.Types.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                        }
                    }
                    {
                        if (haveTypes is IStaticScope scope && scope.Objects.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                        }
                    }
                    {
                        if (haveTypes.OrTypes.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                        }
                    }
                    {
                        if (haveTypes.MethodTypes.TryGetValue(key, out var res))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                        }
                    }
                    if (haveTypes.SafeIs(out Type _) || haveTypes.SafeIs(out MethodType _) || haveTypes.SafeIs(out Method _))
                    {
                        {
                            Dictionary<IKey, Type> generics;
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

                            if (generics.TryGetValue(key, out var res))
                            {
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                            }
                        }
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
                            else if (haveTypes.SafeIs(out Method method))
                            {
                                genericOverlays = method.GenericOverlays;
                            }
                            else
                            {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (genericOverlays.TryGetValue(key, out var res))
                            {
                                if (res.Is1(out var methodType))
                                {
                                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType);
                                }
                                else if (res.Is2(out var innerType))
                                {
                                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(innerType);
                                }
                                else if (res.Is6(out var error))
                                {
                                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error); ;
                                }
                                else
                                {
                                    throw new Exception("uh oh! we hit a type we did not want");
                                }
                            }
                        }
                    }

                    if (haveTypes.Parent.SafeIs(out IIsDefinately<IStaticScope> defScope))
                    {
                        haveTypes = defScope.Value;
                    }
                    else
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Error.TypeNotFound($"could not find type for {key} in {haveTypes}"));
                    }
                }
            }

            // TODO
            // why do I even have this
            // nothing ever really uses the return, I just shove an erro in the out...
            // [MaybeNullWhen(false)] grumble
            //static bool TryLookUp(IStaticScope haveTypes, IKey key, out OrType<MethodType, Type, Object, OrType, InferredType, IError>? result)
            //{

            //}

            //void HandleHopefulMember(IKey key, Member hopeful, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, Dictionary<IValue,IValue> deference)
            //{
            //    type.Switch(
            //        x => {
            //            throw new Exception("a method can't have a hopeful member");
            //        },
            //        x =>
            //        {
            //            if (x.PublicMembers.TryGetValue(key, out var member))
            //            {
            //                TryMerge(hopeful, member, deference);
            //            }
            //            // uhh this member is an error
            //            // do I need to do something?
            //        },
            //        x =>
            //        {
            //            if (x.PublicMembers.TryGetValue(key, out var member))
            //            {
            //                TryMerge(hopeful, member, deference);
            //            }
            //            // uhh this member is an error
            //            // do I need to do something?
            //        },
            //        orType =>
            //        {
            //            // we pretty much need to recurse
            //            // we hope this member is on both sides

            //            // can or types even be implicit?
            //            // no but they can have members that are implicit 
            //            // plus maybe they could be in the future this some sort of implicit key word
            //            // number || implict 
            //            //orType.Left.If(x => HandleHopefulMember(key, hopeful, GetType(x)));
            //            //orType.Right.If(x => HandleHopefulMember(key, hopeful, GetType(x)));

            //            //orType.Left.IfElse(x => 
            //            //    orType.Right.IfElse(
            //            //        y => HandleHopefulMember(key, hopeful,  GetType(x), GetType(y)),
            //            //        ()=> HandleHopefulMember(key, hopeful, GetType(x))),
            //            //    () => orType.Right.If(x => HandleHopefulMember(key, hopeful, GetType(x))));

            //            // after the first one merges it might not be a infered type
            //            // so it can't merge

            //            // how does this work?? or them and then merge?
            //            // yes, you or them and then merge

            //            //--- no!
            //            // I think it actually does not defer
            //            // maybe it just has an assignment relationship to each?

            //            // maybe it just doesn't defer?
            //            // ok for now it just doesn't defer 
            //            // kind of makes sense, who would it defer to?




            //        },
            //        inferredType =>
            //        {
            //            if (inferredType.PublicMembers.TryGetValue(key, out var member))
            //            {
            //                TryMerge(hopeful, member, deference);
            //            }
            //            else
            //            {
            //                Builder.HasPublicMember(inferredType, key, hopeful);
            //            }
            //        },
            //        error =>
            //        {

            //        });
            //}

            // should take IOrType<(MethodType from ,MethodType to),  (Type from , Type to)> pair 
            IOrType<MethodType, Type> CopyTree(IOrType<MethodType, Type> from, IOrType<MethodType, Type> to, IReadOnlyDictionary<Type, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> overlayed)
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
                                var newValue = Copy(item, new Value(this.builder, $"copied from {((TypeProblemNode)item).DebugName}", item.Converter));
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
                                else {
                                    builder.HasOverlayedGeneric(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(innerTypeTo), type.Key, toType);
                                }
                            }
                        }
                        {
                            foreach (var or in innerFromType.GenericOverlays)
                            {
                                if (or.Value.Is2(out var type) && overlayed.TryGetValue(type, out var toType))
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
                                if (or.Value.Is2(out var type) && overlayed.TryGetValue(type, out var toType))
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
                                if (or.Value.Is2(out var type) && overlayed.TryGetValue(type, out var toType))
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


            // TODO this does not need to be a method
            //static bool IsNotInferedHasMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, out IHavePublicMembers? haveMembers)
            //{
            //    var res = false;
            //    (haveMembers, res) = type.SwitchReturns<(IHavePublicMembers?, bool)>(
            //        v1 => (default, false),
            //        v2 => (v2, true),
            //        v3 => (v3, true),
            //        v4 => (default, false),
            //        v5 => (default, false),
            //        v1 => (default, false));

            //    return res;
            //}

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

            static IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> TryGetType(ITypeProblemNode value)
            {
                if (value.SafeIs(out ILookUpType lookup))
                {
                    // look up needs to be populated at this point
                    return Possibly.Is(lookup.LooksUp.GetOrThrow());
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
                if (value.SafeIs(out InferredType inferred))
                {
                    return Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferred));
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
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1"))), new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2"))) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] {
                    new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1")), new WeakTypeDefinitionConverter()),
                    new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2")), new WeakTypeDefinitionConverter()) };

                rootMethod = new MethodType(
                    this.builder,
                    $"generic-{key}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                builder.HasMethodType(Primitive, key, rootMethod);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this.builder, $"generic-parameter-{placeholder.key}", Possibly.Is(placeholder.key), placeholder.converter, Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>());
                    builder.HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(rootMethod), placeholder.key.SwitchReturns<IKey>(x => x, x => x), placeholderType);
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                // here it is ok for these to be members because we are using a method type
                rootMethod.Input = Possibly.Is(builder.CreatePrivateMember(rootMethod, rootMethod, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T1"))));
                rootMethod.Returns = Possibly.Is(builder.CreateTransientMember(rootMethod, new NameKey("T2"), $"return of {rootMethod.DebugName} "));
                builder.IsChildOf(Primitive, rootMethod);

                Dependency = builder.CreateScope(Primitive, rootConverter);
                initDependencyScope(this);
                ModuleRoot = rootScopePopulateScope.InitizeForTypeProblem(this);


            }

            // "key" in the sense this as used as a key for a dictionary
            // describes an overlayed generic type
            private class GenericTypeKey
            {
                // eqaulity for these get a big complex
                // we have a bit of a circle equality situtation
                // say we have GenericTypeKey's G1 and G2
                // with TypeParameter G1T and G2T respectively
                // G1<G1T> is G2<G2T> 
                // to there we check G1 and G2 are the same if their type parameters are equal
                // and G1T and G2T are the same if thier GenericTYpeKey's are equal
                // 
                // we solve this by first comparing G1 and G2 if they are equal 
                // we complere G1T and G2T and tell them to do the comparision assume G1 equals G2
                //
                // things can of cource get pretty complex with generic in generic in generic
                // maybe unit test this?
                public class GenericTypeKeyEqualityComparer : EqualityComparer<GenericTypeKey>
                {
                    // this equality pattern matches the TheyAreUs of type GenericMethodType
                    // {A1078BB9-DF91-48C0-998A-95673AD1272B}
                    public bool Equals(GenericTypeKey? x, GenericTypeKey? y, List<(GenericTypeKey, GenericTypeKey)> assumeTrue)
                    {
                        if (x == y)
                        {
                            return true;
                        }

                        if ((x == null) || (y == null))
                        {
                            return false;
                        }

                        // this is just "contains"
                        // we use any and reference equals to avoid calls to "Equals" which was causing a stack overflow
                        if (assumeTrue.Any(pair => (ReferenceEquals( pair.Item1, x) && ReferenceEquals(pair.Item2, y)) || (ReferenceEquals(pair.Item1, y) && ReferenceEquals(pair.Item2, x))))
                        {
                            return true;
                        }
                        assumeTrue.Add((x, y));


                        return
                           x.primary.Equals(y.primary) &&
                           x.parameters.Length == x.parameters.Length &&
                           x.parameters.Zip(y.parameters, (innerX, innerY) =>
                           {

                               if (innerX.Is7(out var x7) && innerY.Is7(out var y7))
                               {
                                   return x7.Equals(y7, (nextX, nextY) => this.Equals(nextX, nextY, assumeTrue));
                               }

                               return innerX.Equals(innerY);
                           }).All(x => x);
                    }

                    public override bool Equals(GenericTypeKey? x, GenericTypeKey? y) => Equals(x, y, new List<(GenericTypeKey, GenericTypeKey)>());

                    public override int GetHashCode([DisallowNull] GenericTypeKey obj)
                    {
                        return HashCode.Combine(obj.primary, obj.parameters.Length);
                    }
                }

                public IOrType<MethodType, Type> primary;
                // I kind of feel like parameters
                // should not be all of this stuff
                // just Type, IError, TypeParameter and GenericTypeKey
                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, TypeParameter, GenericTypeKey>[] parameters;
                public Dictionary<NameKey, GenericTypeKey.TypeParameter> sourceTypes;

                public GenericTypeKey(NameKey[] types)
                {
                    int i = 0;
                    this.sourceTypes = types?.ToDictionary(x => x, x => new GenericTypeKey.TypeParameter(i++, this)) ?? throw new ArgumentNullException(nameof(types));
                }

                public override bool Equals(object? obj)
                {
                    return Equals(obj as GenericTypeKey);
                }

                public bool Equals(GenericTypeKey? other)
                {
                    return new GenericTypeKeyEqualityComparer().Equals(this, other);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(primary, parameters.Length);
                }

                public override string ToString()
                {
                    return primary.ToString() + " [" + String.Join(", ", sourceTypes.Select(x => x.Value.ToString())) + "] " + " [" + String.Join(", ", parameters.Select(x => x.ToString())) + "] ";
                }
                public class TypeParameter
                {
                    public readonly int index;
                    public GenericTypeKey owner;

                    public TypeParameter(int index, GenericTypeKey owner)
                    {
                        this.index = index;
                        this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
                    }

                    public bool Equals(TypeParameter typeParameter, Func<GenericTypeKey, GenericTypeKey, bool> equal)
                    {
                        return index == typeParameter.index && equal(this.owner, typeParameter.owner);
                    }

                    public override bool Equals(object? obj)
                    {
                        return obj is TypeParameter other &&
                            index == other.index &&
                            new GenericTypeKeyEqualityComparer().Equals(this.owner, other.owner);
                    }

                    public override int GetHashCode()
                    {
                        return HashCode.Combine(index, owner);
                    }

                    public override string ToString()
                    {
                        return "T" + index;
                    }
                }
            }


            //private class DoubleGenericTypeKey
            //{
            //    private readonly IOrType<MethodType, Type, Method> primary;
            //    private readonly IOrType<MethodType, Type, Object, OrType, InferredType, IError, TypeParameter, DoubleGenericTypeKey, GenericTypeKey>[] parameters;

            //    public DoubleGenericTypeKey(IOrType<MethodType, Type, Method> primary, IOrType<MethodType, Type, Object, OrType, InferredType, IError, TypeParameter, DoubleGenericTypeKey, GenericTypeKey>[] parameters)
            //    {
            //        this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
            //        this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            //    }



            //}

            // we only handle control back for simple look ups, for generics we build the GenericTypeKey
            // if I do that GenericTypeKey is going to need to be able to hold  DoubleGenericTypeKey.TypeParameter and DoubleGenericTypeKey
            // maybe I can combine GenericTypeKey and DoubleGenericTypeKey

            // why does this exist?
            // ultimate we want to determine equality of complex generics
            // is generic-method [t1,ta] [t1, generic-method [t2,tb] [Pair[t1,t2], Pair[ta,tb]]] the same as generic-method [t11,ta1] [t11, generic-method [t21,tb1] [Pair[t11,t21], Pair[ta1,tb1]]]? (yes, names don't matter, provided Pair resolves to the same type in the contexts where both are used)
            // so the goal is to better representation of these things with the simple types resolved and the names replaced by indexes
            // this creates those representations
            // we couldn't ask the keys to do that themselves
            // because sometimes your rep depends on the rep of the key you were defined in
            // for example: generic-method [T1,T2] [T1, Pair[T2]], Pair need to look up T2 from the generic-method that contains it
            private class KeyVisitor : IKeyVisitor<IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>>
            {
                private readonly Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>> lookup;
                private readonly Dictionary<NameKey, GenericTypeKey.TypeParameter> generics;
                //private readonly MethodType rootMethod;

                private KeyVisitor(
                    Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>> lookup,
                    Dictionary<NameKey, GenericTypeKey.TypeParameter> sourceGenerics)
                {
                    this.generics = sourceGenerics ?? throw new ArgumentNullException(nameof(sourceGenerics));
                    this.lookup = key =>
                    {
                        if (key.SafeIs(out NameKey nameKey) && this.generics.TryGetValue(nameKey, out var foundIt))
                        {
                            return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(foundIt);
                        }
                        return lookup(key);
                    };
                    //this.rootMethod = rootMethod ?? throw new ArgumentNullException(nameof(rootMethod));
                }

                public static KeyVisitor Base(Func<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> lookup, Dictionary<Type, GenericTypeKey.TypeParameter> overlaysToTypeParameters)
                {
                    return new KeyVisitor(
                        key => lookup(key).SwitchReturns(
                            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(x),
                            type => {
                                if (overlaysToTypeParameters.TryGetValue(type, out var typeParameter)) {
                                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(typeParameter);
                                }
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(type);
                            },
                            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(x),
                            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(x),
                            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(x),
                            x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter>(x)),
                        new Dictionary<NameKey, GenericTypeKey.TypeParameter>());
                }

                private KeyVisitor Next(GenericTypeKey genericType)
                {
                    var context = new KeyVisitor(
                        lookup,
                        genericType.sourceTypes);
                    return context;
                }

                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> DoubleGenericNameKey(DoubleGenericNameKey doubleGenericNameKey)
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(DoubleGenericNameKey_BetterType(doubleGenericNameKey));
                }


                public GenericTypeKey DoubleGenericNameKey_BetterType(DoubleGenericNameKey doubleGenericNameKey)
                {
                    //if (doubleGenericNameKey.Name.Name != "generic-method")
                    //{
                    //    throw new Exception("we really only suppory methods");
                    //}

                    var primary = lookup(doubleGenericNameKey.Name).SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"));

                    return DuobleGenericNameKey_BetterType_PassInPrimary(doubleGenericNameKey, primary);
                }

                public GenericTypeKey DuobleGenericNameKey_BetterType_PassInPrimary(DoubleGenericNameKey doubleGenericNameKey, IOrType<MethodType, Type> primary)
                {
                    var res = new GenericTypeKey(doubleGenericNameKey.Types);

                    var context = Next(res);

                    res.parameters = doubleGenericNameKey.DependentTypes.Select(dependentType => dependentType.SwitchReturns(
                                x => x.Visit(context),
                                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x))).ToArray();
                    res.primary = primary;

                    return res;
                }

                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> GenericNameKey(GenericNameKey genericNameKey)
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(GenericNameKey_BetterTyped(genericNameKey));
                }

                public GenericTypeKey GenericNameKey_BetterTyped(GenericNameKey genericNameKey)
                {
                    var res = new GenericTypeKey(Array.Empty<NameKey>());
                    var context = Next(res);

                    res.parameters = genericNameKey.Types.Select(dependentType => dependentType.SwitchReturns(
                        x => x.Visit(context),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x))).ToArray();

                    res.primary = lookup(genericNameKey.Name).SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type>(x),
                        x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"),
                            x => throw new Exception("only methodType, Type, and Method can have generic parameters"));

                    return res;
                }

                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> ImplicitKey(ImplicitKey implicitKey)
                {
                    return lookup(implicitKey).SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x));
                }

                public IOrType<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey> NameKey(NameKey nameKey)
                {
                    return lookup(nameKey).SwitchReturns(
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x),
                        x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError, GenericTypeKey.TypeParameter, GenericTypeKey>(x));
                }
            }
        }
    }
}
