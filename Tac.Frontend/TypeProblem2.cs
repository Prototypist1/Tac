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
        internal class TypeProblem2 : ISetUpTypeProblem
        {
            public abstract class TypeProblemNode : ITypeProblemNode
            {
                public readonly string debugName;

                public TypeProblemNode(TypeProblem2 problem, string debugName)
                {
                    Problem = problem ?? throw new ArgumentNullException(nameof(problem));
                    this.debugName = debugName;
                    problem.Register(this);
                }

                public ISetUpTypeProblem Problem { get; }

            }

            public abstract class TypeProblemNode<Tin, Tout> : TypeProblemNode//, IConvertable<T>
            {

                public TypeProblemNode(TypeProblem2 problem, string debugName, IConvertTo<Tin, Tout> converter) : base(problem, debugName)
                {
                    Converter = converter;
                }

                internal IConvertTo<Tin, Tout> Converter { get; }
            }
            public class TypeReference : TypeProblemNode<TypeReference, IOrType<IFrontendType, IError>>, ILookUpType
            {
                public TypeReference(TypeProblem2 problem, string debugName, IConvertTo<TypeReference, IOrType<IFrontendType, IError>> converter) : base(problem, debugName, converter)
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
                public Value(TypeProblem2 problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
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
            public class Member : TypeProblemNode<Member, WeakMemberDefinition>, IMember
            {
                public Member(TypeProblem2 problem, string debugName, IConvertTo<Member, WeakMemberDefinition> converter) : base(problem, debugName, converter)
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
                public TransientMember(TypeProblem2 problem, string debugName) : base(problem, debugName)
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

            // this gets a lot of weird stuff from static scope
            // I think static scope gets it from object
            // object gets it because it has initization
            // so values and stuff are ok
            // I should think about how that works tho
            // does not seem right 
            public class Type : TypeProblemNode<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType, IHavePossibleMembers
            {
                public Type(
                    TypeProblem2 problem,
                    string debugName,
                    IIsPossibly<IOrType<NameKey, ImplicitKey>> key,
                    IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter,
                    bool isPlaceHolder
                    ) : base(problem, debugName, converter)
                {
                    Key = key;
                    IsPlaceHolder = isPlaceHolder;
                }

                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();


                public List<Scope> EntryPoints { get; } = new List<Scope>();
                //public List<Value> Values { get; } = new List<Value>();
                //public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
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
                public MethodType(TypeProblem2 problem, string debugName, IConvertTo<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
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
                public InferredType(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }
                //public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();
                //public List<Scope> EntryPoints { get; } = new List<Scope>();
                //public List<Value> Values { get; } = new List<Value>();
                //public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                //public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                //public List<TypeReference> Refs { get; } = new List<TypeReference>();
                //public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                //public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                //public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                //public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                //public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();


                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return Members;
                //}
            }

            public class OrType : TypeProblemNode<OrType, WeakTypeOrOperation> //, IHavePublicMembers
            {
                public OrType(TypeProblem2 problem, string debugName, IConvertTo<OrType, WeakTypeOrOperation> converter) : base(problem, debugName, converter)
                {
                }
                //public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<TypeReference> Left { get; set; } = Possibly.IsNot<TypeReference>();
                public IIsPossibly<TypeReference> Right { get; set; } = Possibly.IsNot<TypeReference>();

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    throw new NotImplementedException();
                //    //var res = new Dictionary<IKey, Member>();
                //    //var left = Left.GetOrThrow();
                //    //var right = Right.GetOrThrow();


                //    //var leftMembers = left.GetPublicMembers();
                //    //foreach (var rightPair in right.GetPublicMembers())
                //    //{
                //    //    if (leftMembers.TryGetValue(rightPair.Key, out var leftValue)) {
                //    //        var member = new Member(Problem.SafeCastTo<ISetUpTypeProblem,TypeProblem2>(), $"generated or member out of {leftValue!.debugName} and {rightPair.Value.debugName}", leftValue.Converter)
                //    //        {
                //    //            LooksUp =
                //    //            new OrType() { 
                //    //                Left = Possibly.Is(TypeProblem2.GetType(leftValue)),
                //    //                Right = Possibly.Is(TypeProblem2.GetType(rightPair.Value))

                //    //            }



                //    //        };
                //    //        res[leftMember.Key] = member;
                //    //    }
                //    //} 

                //    //rightMembers = ;
                //    //foreach (var leftMember in GetMembers2(GetType(left)))
                //    //{
                //    //    if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                //    //    {
                //    //        // TODO
                //    //        // else where you use an orType for the type of members defined on both side of an OrType
                //    //        // if they are the same type
                //    //        if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
                //    //        {

                //    //        }
                //    //    }
                //    //}

                //    //orTypeMembers[orType] = res;

                //    //return res;

                //}
            }
            public class Scope : TypeProblemNode<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>, IScope, IHavePossibleMembers
            {
                public Scope(TypeProblem2 problem, string debugName, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter) : base(problem, debugName, converter)
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

                //public IReadOnlyDictionary<IKey, Member> GetPublicMembers()
                //{
                //    return new Dictionary<IKey, Member>();
                //}
            }
            public class Object : TypeProblemNode<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType, IHavePossibleMembers
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter) : base(problem, debugName, converter)
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
                //public List<Value> Values { get; } = new List<Value>();
                //public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
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
                public Method(TypeProblem2 problem, string debugName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter) : base(problem, debugName, converter)
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

            private class Inflow
            {
                public readonly List<FlowNode> inFlows = new List<FlowNode>();

                public Inflow(FlowNode toAdd)
                {
                    inFlows.Add(toAdd);
                }

                public override bool Equals(object? obj)
                {
                    return obj != null && obj is Inflow inflow && inFlows.SetEqual(inflow.inFlows);
                }

                public override int GetHashCode()
                {
                    return inFlows.Sum(x => x.GetHashCode());
                }

                internal Inflow AddAsNew(FlowNode flowFrom)
                {
                    var res = new Inflow(flowFrom);
                    foreach (var inFlow in inFlows)
                    {
                        res.inFlows.Add(inFlow);
                    }
                    return res;
                }
            }

            private class FlowNode
            {
                public FlowNode(bool accepts)
                {
                    this.Accepts = accepts;
                }

                public bool Accepts { get; }
                public Dictionary<IKey, IOrType<FlowNode, Inflow>> Members { get; } = new Dictionary<IKey, IOrType<FlowNode, Inflow>>();
                public IOrType<FlowNode, Inflow>? Input { get; set; }
                public IOrType<FlowNode, Inflow>? Output { get; set; }

                internal FlowNode Copy()
                {
                    var res = new FlowNode(Accepts);

                    foreach (var pair in Members)
                    {
                        res.Members[pair.Key] = pair.Value;
                    }

                    res.Input = Input;
                    res.Output = Output;

                    return res;
                }
            }

            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }
            public Type NumberType { get; }
            public Type StringType { get; }
            public Type BooleanType { get; }
            public Type EmptyType { get; }


            // these are pretty much the same
            private List<(ILookUpType, ILookUpType)> assignments = new List<(ILookUpType, ILookUpType)>();


            #region Building APIs

            public void IsChildOf(IStaticScope parent, IStaticScope kid)
            {
                kid.Parent = Possibly.Is(parent);
            }
            public static void HasValue(IScope parent, Value value)
            {
                parent.Values.Add(value);
            }
            public static void HasReference(IStaticScope parent, TypeReference reference)
            {
                parent.Refs.Add(reference);
            }


            public void HasEntryPoint(IStaticScope parent, Scope entry)
            {
                parent.EntryPoints.Add(entry);
            }

            public static void HasType(IStaticScope parent, IKey key, Type type)
            {
                parent.Types.Add(key, type);
            }
            public void HasMethodType(IStaticScope parent, IKey key, MethodType type)
            {
                parent.MethodTypes.Add(key, type);
            }
            // why do objects have keys?
            // that is wierd
            public static void HasObject(IStaticScope parent, IKey key, Object @object)
            {
                parent.Objects.Add(key, @object);
            }

            public void HasPlaceholderType(IOrType<MethodType, Type> parent, IKey key, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type)
            {
                parent.Switch(x => x.GenericOverlays.Add(key, type), x => x.GenericOverlays.Add(key, type));
            }
            public static void HasPrivateMember(IHavePrivateMembers parent, IKey key, Member member)
            {
                parent.PrivateMembers.Add(key, member);
            }
            public static void HasPublicMember(IHavePublicMembers parent, IKey key, Member member)
            {
                parent.PublicMembers.Add(key, member);
            }
            public static void HasMethod(IStaticScope parent, IKey key, Method method)
            {
                parent.Methods.Add(key, method);
            }


            public static void HasTransientMember(IScope parent, TransientMember member)
            {
                parent.TransientMembers.Add(member);
            }
            public static Member HasMembersPossiblyOnParent(IHavePossibleMembers parent, IKey key, Member member)
            {

                parent.PossibleMembers.TryAdd(key, member);
                return parent.PossibleMembers[key];
            }
            public static Member HasHopefulMember(IValue parent, IKey key, Member member)
            {
                parent.HopefulMembers.TryAdd(key, member);
                return parent.HopefulMembers[key];
            }

            private T Register<T>(T typeProblemNode)
                where T : ITypeProblemNode
            {
                typeProblemNodes.Add(typeProblemNode);
                return typeProblemNode;
            }

            public void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo)
            {
                AssertIs(assignedFrom, assignedTo);
            }

            public void AssertIs(ILookUpType assignedFrom, ILookUpType assignedTo)
            {
                assignments.Add((assignedFrom, assignedTo));
            }

            public Value CreateValue(IScope scope, IKey typeKey, IConvertTo<Value, PlaceholderValue> converter)
            {
                var res = new Value(this, typeKey.ToString()!, converter);
                HasValue(scope, res);
                res.Context = Possibly.Is(scope);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            public Member CreateMember(
                IStaticScope scope,
                IKey key,
                IOrType<IKey, IError> typeKey,
                WeakMemberDefinitionConverter converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                if (scope is IHavePublicMembers publicMembers)
                {
                    HasPublicMember(publicMembers, key, res);
                }
                else
                if (scope is IHavePrivateMembers privateMembers)
                {
                    HasPrivateMember(privateMembers, key, res);
                }
                else
                {
                    throw new Exception("this is probably really an IError - you tried to add a member somewhere one cannot go");
                }
                res.Context = Possibly.Is(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePublicMember(
                IStaticScope scope,
                IHavePublicMembers havePublicMembers,
                IKey key,
                IOrType<IKey, IError> typeKey,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPublicMember(havePublicMembers, key, res);
                res.Context = Possibly.Is(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePublicMember(
                IStaticScope scope,
                IHavePublicMembers havePublicMembers,
                IKey key,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPublicMember(havePublicMembers, key, res);
                res.Context = Possibly.Is(scope);
                return res;
            }

            public Member CreatePublicMember(
                IHavePublicMembers scope,
                IKey key,
                IOrType<MethodType, Type, Object, OrType, InferredType, IError> type,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPublicMember(scope, key, res);
                res.LooksUp = Possibly.Is(type);
                return res;
            }

            public Member CreatePrivateMember(
                IStaticScope scope,
                IHavePrivateMembers havePrivateMembers,
                IKey key,
                IOrType<IKey, IError> typeKey,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPrivateMember(havePrivateMembers, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePrivateMember(
                IStaticScope scope,
                IHavePrivateMembers havePrivateMembers,
                IKey key,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPrivateMember(havePrivateMembers, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
                return res;
            }

            public Member CreatePrivateMember(
                IHavePrivateMembers scope,
                IKey key,
                IOrType<MethodType, Type, Object, OrType, InferredType, IError> type,
                IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPrivateMember(scope, key, res);
                res.LooksUp = Possibly.Is(type);
                return res;
            }

            public Member CreateMemberPossiblyOnParent(IStaticScope scope, IHavePossibleMembers havePossibleMembers, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                // this is weird, but since C# does not have and types...
                // scope and havePossibleMembers are expected to be the same object
                if (!ReferenceEquals(scope, havePossibleMembers))
                {
                    throw new Exception($"{scope} and {havePossibleMembers} should be the same object");
                }

                if (havePossibleMembers.PossibleMembers.TryGetValue(key, out var res1))
                {
                    return res1;
                }

                var res = new Member(this, "possibly on parent -" + key.ToString(), converter);
                res = HasMembersPossiblyOnParent(havePossibleMembers, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
                return res;
            }

            public TypeReference CreateTypeReference(IStaticScope context, IKey typeKey, IConvertTo<TypeReference, IOrType<IFrontendType, IError>> converter)
            {
                var res = new TypeReference(this, typeKey.ToString()!, converter);
                HasReference(context, res);
                res.Context = Possibly.Is(context);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            public Scope CreateScope(IStaticScope parent, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter)
            {
                var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).debugName}", converter);
                IsChildOf(parent, res);
                return res;
            }

            public Type CreateType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(this, key.ToString()!, Possibly.Is(key), converter, false);
                IsChildOf(parent, res);
                HasType(parent, key.SwitchReturns<IKey>(x => x, x => x), res);
                return res;
            }

            public Type CreateType(IStaticScope parent, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var key = new ImplicitKey(Guid.NewGuid());
                var res = new Type(this, key.ToString()!, Possibly.IsNot<IOrType<NameKey, ImplicitKey>>(), converter, false);
                IsChildOf(parent, res);
                // migiht need this, let's try without first
                //HasType(parent, key, res);
                return res;
            }


            public Type CreateGenericType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(
                    this,
                    $"generic-{key}-{placeholders.Aggregate("", (x, y) => x + "-" + y)}",
                    Possibly.Is(key),
                    converter,
                    false);
                IsChildOf(parent, res);
                HasType(parent, key.SwitchReturns<IKey>(x => x, x => x), res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(
                        this,
                        $"generic-parameter-{placeholder.key}",
                        Possibly.Is(placeholder.key),
                        placeholder.converter,
                        true);
                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(res), placeholder.key.SwitchReturns<IKey>(x => x, x => x), Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(placeholderType));
                }
                return res;
            }

            // why do objects have keys?
            // that is wierd
            public Object CreateObjectOrModule(IStaticScope parent, IKey key, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter)
            {
                var res = new Object(this, key.ToString()!, converter, innerConverter);
                IsChildOf(parent, res);
                HasObject(parent, key, res);
                return res;
            }

            public Method CreateMethod(IStaticScope parent, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                // here it is ok for these to be members because we are using a method
                var returns = CreateTransientMember(res);
                res.Returns = Possibly.Is(returns);
                var input = CreatePrivateMember(res, res, new NameKey(inputName), inputConverter);
                res.Input = Possibly.Is(input);
                return res;
            }


            public Method CreateMethod(IStaticScope parent, IOrType<TypeProblem2.TypeReference, IError> inputType, IOrType<TypeProblem2.TypeReference, IError> outputType, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                if (!inputType.Is1(out var inputTypeValue))
                {
                    throw new NotImplementedException();
                }
                if (!outputType.Is1(out var outputTypeValue))
                {
                    throw new NotImplementedException();
                }

                var res = new Method(this, $"method{{inputName:{inputName},inputType:{inputTypeValue.debugName},outputType:{outputTypeValue.debugName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                {
                    var returns = outputTypeValue.TypeKey is IIsDefinately<IKey> typeKey ? CreateTransientMember(res, typeKey.Value) : CreateTransientMember(res);
                    res.Returns = Possibly.Is(returns);
                }
                {
                    if (inputTypeValue.TypeKey is IIsDefinately<IKey> typeKey)
                    {

                        // here it is ok for these to be members because we are using a method
                        res.Input = Possibly.Is(CreatePrivateMember(res, res, new NameKey(inputName), Prototypist.Toolbox.OrType.Make<IKey, IError>(typeKey.Value), inputConverter));
                    }
                    else
                    {

                        // here it is ok for these to be members because we are using a method
                        res.Input = Possibly.Is(CreatePrivateMember(res, res, new NameKey(inputName), inputConverter));
                    }
                }
                return res;
            }


            public Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, "hopeful - " + key.ToString()!, converter);
                res = HasHopefulMember(scope, key, res);
                return res;
            }


            public OrType CreateOrType(IStaticScope s, IKey key, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode1, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode2, IConvertTo<OrType, WeakTypeOrOperation> converter)
            {
                if (!setUpSideNode1.Is1(out var node1))
                {
                    throw new NotImplementedException();
                }
                if (!setUpSideNode2.Is1(out var node2))
                {
                    throw new NotImplementedException();
                }

                var res = new OrType(this, $"{node1.debugName} || {node2.debugName}", converter);
                Ors(res, node1, node2);
                HasOrType(s, key, res);

                return res;

            }
            public MethodType GetMethod(IOrType<MethodType, Type, Object, OrType, InferredType, IError> input, IOrType<MethodType, Type, Object, OrType, InferredType, IError> output)
            {
                throw new NotImplementedException();
            }

            private static void Ors(OrType orType, TypeReference a, TypeReference b)
            {
                orType.Left = Possibly.Is(a);
                orType.Right = Possibly.Is(b);
            }

            private static void HasOrType(IStaticScope scope, IKey kay, OrType orType1)
            {
                scope.OrTypes[kay] = orType1;
            }


            public void IsNumber(IScope parent, ILookUpType target)
            {
                // super weird that this has to be a transient member
                var thing = CreateTransientMember(parent, new NameKey("number"));
                AssertIs(target, thing);
            }

            public void IsBlock(IScope parent, ILookUpType target)
            {
                // super weird that this has to be a transient member
                var thing = CreateTransientMember(parent, new NameKey("block"));
                AssertIs(target, thing);
            }

            public void IsBool(IScope parent, ILookUpType target)
            {
                // super weird that this has to be a transient member
                var thing = CreateTransientMember(parent, new NameKey("bool"));
                AssertIs(target, thing);
            }

            public void IsEmpty(IScope parent, ILookUpType target)
            {
                // super weird that this has to be a transient member
                var thing = CreateTransientMember(parent, new NameKey("empty"));
                AssertIs(target, thing);
            }

            public void IsString(IScope parent, ILookUpType target)
            {
                // super weird that this has to be a transient member
                var thing = CreateTransientMember(parent, new NameKey("string"));
                AssertIs(target, thing);
            }

            private TransientMember CreateTransientMember(IScope parent)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = Possibly.Is(parent);
                return res;
            }


            private TransientMember CreateTransientMember(IScope parent, IKey typeKey)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = Possibly.Is(parent);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            // ok
            // so... this can not be converted
            // it is not a real method
            // it is just something of type method
            // it is really just a type
            //
            public Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var thing = CreateTransientMember(parent);
                var method = CreateMethod(parent, "input", converter, inputConverter);
                IsAssignedTo(target, thing);
                return method;
            }

            #endregion


            public TransientMember GetReturns(IStaticScope s)
            {
                if (s is Method method)
                {
                    return GetReturns(method);
                }
                else if (s.Parent is IIsDefinately<IStaticScope> definatelyScope)
                {
                    return GetReturns(definatelyScope.Value);
                }
                else
                {
                    throw new Exception("s.Parent should not be null");
                }
            }

            internal TransientMember GetReturns(Method method)
            {
                return method.Returns.GetOrThrow();
            }

            //public Member CreateMember(IScope scope, IKey key, IOrType<IKey, IError> typeKey, IConvertTo<Member, WeakMemberDefinition> converter)
            //{
            //    var res = new Member(this, key.ToString()!, converter);
            //    HasPrivateMember(scope, key, res);
            //    res.Context = Possibly.Is(scope);
            //    res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
            //    return res;
            //}

            //private TransientMember CreateTransientMember(IScope parent)
            //{
            //    var res = new TransientMember(this, "");
            //    HasTransientMember(parent, res);
            //    res.Context = Possibly.Is(parent);
            //    return res;
            //}

            public TransientMember GetReturns(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Returns.GetOrThrow();
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    // shared code {A9E37392-760B-427D-852E-8829EEFCAE99}
                    // we don't use has member input/output doesn't go in the member list
                    // it is not a public member
                    // and infered to do not have private members
                    var methodInputKey = new NameKey("implicit input - " + Guid.NewGuid());
                    var inputMember = new Member(this, methodInputKey.ToString()!, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
                    inferredMethodType.Input = Possibly.Is(inputMember);

                    var returnMember = new TransientMember(this, "implicit return -" + Guid.NewGuid());
                    returnMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
                    inferredMethodType.Returns = Possibly.Is(returnMember);

                    return returnMember;
                }
            }

            public Member GetInput(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Input.GetOrThrow();
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    // shared code {A9E37392-760B-427D-852E-8829EEFCAE99}
                    // we don't use has member input/output doesn't go in the member list
                    // it is not a public member
                    // and infered to do not have private members
                    var methodInputKey = new NameKey("implicit input - " + Guid.NewGuid());
                    var inputMember = new Member(this, methodInputKey.ToString()!, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
                    inferredMethodType.Input = Possibly.Is(inputMember);

                    var returnMember = new TransientMember(this, "implicit return -" + Guid.NewGuid());
                    returnMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
                    inferredMethodType.Returns = Possibly.Is(returnMember);

                    return inputMember;
                }
            }

            public Member GetInput(Method method)
            {
                return method.Input.GetOrThrow();
            }


            public IIsPossibly<IKey> GetKey(TypeReference type)
            {
                return type.TypeKey.Possibly1();
            }

            // pretty sure it is not safe to solve more than once 
            public ITypeSolution Solve()
            {
                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>) && x.TypeKey.Is3(out var _)))
                {
                    var type = new InferredType(this, $"for {((TypeProblemNode)node).debugName}");
                    node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type));
                }

                // what happens here if x.TypeKey.Is2??
                if (toLookUp.Any(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>) && x.TypeKey.Is2(out var _)))
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

                toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>)).ToArray();

                // overlay generics
                while (toLookUp.Any())
                {
                    foreach (var node in toLookUp)
                    {
                        LookUpOrOverlayOrThrow(node, realizedGeneric);
                    }
                    toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>)).ToArray();
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

                                    HasPublicMember(havePublicMembers, pair.Key, pair.Value);
                                }
                                else if (node is IHavePrivateMembers havePrivateMembers)
                                {
                                    HasPrivateMember(havePrivateMembers, pair.Key, pair.Value);
                                }
                                else
                                {
                                    throw new Exception("uhhhh");
                                }
                            });
                        }
                    }
                }

                //foreach (var (node, possibleMembers) in typeProblemNodes.OfType<IHavePossibleMembers>().Select(x => (x, x.PossibleMembers)))
                //{
                //    foreach (var pair in possibleMembers)
                //    {
                //        TryGetMember(node, pair.Key).IfElse(member => TryMerge(pair.Value, member!), () => HasPrivateMember(node, pair.Key, pair.Value));
                //    }
                //}

                // hopeful members and methods are a little rough around the edges
                // they are very similar yet implemented differently 

                // hopeful members 
                foreach (var (node, hopeful) in typeProblemNodes.OfType<IValue>().Select(x => (x, x.HopefulMembers)))
                {
                    foreach (var pair in hopeful)
                    {

                        HandleHopefulMember(pair.Key, pair.Value, GetType(node));

                        //GetType(node).Switch(
                        //    x => { },
                        //    x =>
                        //    {
                        //        if (x.PublicMembers.TryGetValue(pair.Key, out var member))
                        //        {
                        //            TryMerge(pair.Value, member);
                        //        }
                        //        // uhh this member is an error
                        //        // do I need to do something?
                        //    },
                        //    x =>
                        //    {
                        //        if (x.PublicMembers.TryGetValue(pair.Key, out var member))
                        //        {
                        //            TryMerge(pair.Value, member);
                        //        }
                        //        // uhh this member is an error
                        //        // do I need to do something?
                        //    },
                        //    orType =>
                        //    {
                        //        // we pretty much need to recurse
                        //        // we hope this member is on both sides

                        //        // can or types even be implicit?
                        //        // no but they can have members that are implicit 
                        //        // plus maybe they could be in the future this some sort of implicit key word
                        //        // number || implict 
                        //        orType.Left.IfIs(x =>
                        //        {
                        //            GetType(x)
                        //        });

                        //    },
                        //    inferredType => { },
                        //    error =>
                        //    {

                        //    });

                        //if (GetMembers2().TryGetValue(pair.Key, out var member))
                        //{
                        //    TryMerge(pair.Value, member);
                        //}
                        //else if (GetType(node).Is5(out var inferred))
                        //{
                        //    HasPublicMember(inferred, pair.Key, pair.Value);
                        //}
                        //else
                        //{
                        //    throw new Exception("member could not be handled ");
                        //}
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

                // -------------------------------------
                // we change representation
                var ors = typeProblemNodes.Select(node => GetType(node)).ToArray();

                var inflows = new List<(Inflow, FlowNode)>();

                var orsToFlowNodes = ors.ToDictionary(x =>
                {
                    if (x.Is(out ITypeProblemNode node))
                    {
                        return node;
                    }
                    throw new Exception("that ain't right");
                }, x =>
                {
                    if (x.Is5(out var _))
                    {
                        // inferred types are their own inflow
                        var res = new FlowNode(true);
                        inflows.Add((new Inflow(res), res));
                        return res;
                    }
                    else
                    {
                        return new FlowNode(false);
                    }
                });

                // we create members on our new representation
                foreach (var hasPublicMembers in ors.Select(x => (x.Is(out IHavePublicMembers members), members)).Where(x => x.Item1).Select(x => x.members))
                {
                    foreach (var member in hasPublicMembers.PublicMembers)
                    {
                        orsToFlowNodes[hasPublicMembers].Members[member.Key] = GetFlowNodeOrInflow(member.Value, orsToFlowNodes, inflows); //Prototypist.Toolbox.OrType.Make<FlowNode, Inflow>(orsToFlowNodes[member.Value]);
                    }
                }

                // we create input and output on our new implmentation
                foreach (var hasInputAndOutput in ors.Select(x => (x.Is(out IHaveInputAndOutput io), io)).Where(x => x.Item1).Select(x => x.io))
                {
                    orsToFlowNodes[hasInputAndOutput].Input = GetFlowNodeOrInflow(hasInputAndOutput.Input.GetOrThrow(), orsToFlowNodes, inflows);
                    orsToFlowNodes[hasInputAndOutput].Output = GetFlowNodeOrInflow(hasInputAndOutput.Returns.GetOrThrow(), orsToFlowNodes, inflows);
                }

                // TODO ortypes!
                //foreach (var hasInputAndOutput in ors.Select(x => (x.Is(out IHaveInputAndOutput io), io)).Where(x => x.Item1).Select(x => x.io))
                //{
                //    Merge(
                //                GetType(deferringOrType.Left.GetOrThrow()),
                //                GetType(deferringOrType.Left.GetOrThrow()));
                //}

                // A | B ab;
                // ab =: C c;
                // C flows in to A and B

                // C c;
                // c =: A | B ab;
                // nothing flows in to c? 

                // TODO
                // I am not really flowing primitive types
                // intness to needs to flow

                // 5 =: c
                // c =: number | string b
                // this should work
                // but right now it doesn't
                // c needs to become "number | string" as well 
                // well, not egactly "number | string" more like it has a list of things it could possibly be and they include "number | string", "number" and "string"
                // if we have this:
                // 5 =: c
                // c =: number | string b
                // c =: number d
                // it better become a number 
                // so we have a could-be list
                // and flows intersect the list
                

                bool go;
                do
                {
                    go = false;

                    foreach (var (from, to) in assignments)
                    {
                        var toType = orsToFlowNodes[to];
                        var fromType = orsToFlowNodes[from];

                        go |= Flow(toType, fromType, inflows);

                    }

                    //foreach (var (from, to) in assertions)
                    //{
                    //    // nothing should look up to null at this point
                    //    var fromType = from.LooksUp.GetOrThrow();

                    //    go |= Flow(to, fromType);

                    //}

                } while (go);



                // -------------------------------------- old stuff
                var toInflows = new List<InferredType>();
                var flowLookUps = new Dictionary<ITypeProblemNode, IOrType<MethodType, Type, Object, OrType, Inflow, IError>>();

                foreach (var node in typeProblemNodes)
                {
                    // nothing should look up to null at this point
                    GetType(node).Switch(
                        x => flowLookUps[node] = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(x),
                        x => flowLookUps[node] = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(x),
                        x => flowLookUps[node] = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(x),
                        x => flowLookUps[node] = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(x),
                        x => toInflows.Add(x),
                        x => flowLookUps[node] = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(x));
                }

                //var flowLookUps = typeProblemNodes.ToDictionary(x => x, x => GetType(x).SwitchReturns(
                //    y=> Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(y),
                //    y => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(y),
                //    y => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(y),
                //    y => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(y),
                //    y => {
                //        toInflows.Add(y);
                //        var inflow = new Inflow(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(y));
                //        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(inflow);
                //    },
                //    y => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, Inflow, IError>(y)
                //    ));

                var inflowMap = new Dictionary<Inflow, InferredType>();

                foreach (var toInflow in toInflows)
                {
                    var inferred = new InferredType(this, $"flow target for: {toInflow.debugName} ");
                    var toInflowOr = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(toInflow);
                    Flow(
                        Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferred),
                        toInflowOr,
                        inflowMap);

                    inflowMap[new Inflow(toInflowOr)] = inferred;
                }

                // flow up stream
                // very sloppy and slow
                // if I ever am worried about speed I am sure this will be a canidate
                bool go;
                do
                {
                    go = false;

                    foreach (var (from, to) in assignments)
                    {
                        var toType = flowLookUps[to];
                        var fromType = flowLookUps[from];

                        go |= Flow(toType, fromType);

                    }

                    //foreach (var (from, to) in assertions)
                    //{
                    //    // nothing should look up to null at this point
                    //    var fromType = from.LooksUp.GetOrThrow();

                    //    go |= Flow(to, fromType);

                    //}

                } while (go);


                // we dont flow downstream

                return new TypeSolution(
                    typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>).ToDictionary(x => x, x => x.LooksUp.GetOrThrow()),
                    typeProblemNodes.OfType<IHavePublicMembers>().ToDictionary(x => x, x => (IReadOnlyList<Member>)x.PublicMembers.Select(y => y.Value).ToArray()),
                    typeProblemNodes.OfType<IHavePrivateMembers>().ToDictionary(x => x, x => (IReadOnlyList<Member>)x.PrivateMembers.Select(y => y.Value).ToArray()),
                    typeProblemNodes.OfType<OrType>().ToDictionary(x => x, x => (x.Left.GetOrThrow(), x.Right.GetOrThrow())),
                    typeProblemNodes.Select(x =>
                    {
                        if (x.SafeIs(out Method m))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(m));
                        }

                        if (x.SafeIs(out MethodType mt))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(mt));
                        }

                        if (x.SafeIs(out InferredType it) && it.Input is IIsDefinately<Member>)
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<IOrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<IOrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<IOrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Input.GetOrThrow(), v1 => v1.Input.GetOrThrow(), v1 => v1.Input.GetOrThrow())),
                    typeProblemNodes.Select(x =>
                    {
                        if (x.SafeIs(out Method m))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(m));
                        }
                        if (x.SafeIs(out MethodType mt))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(mt));
                        }
                        if (x.SafeIs(out InferredType it) && it.Returns is IIsDefinately<TransientMember>)
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<IOrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<IOrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<IOrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Returns.GetOrThrow(), v1 => v1.Returns.GetOrThrow(), v1 => v1.Returns.GetOrThrow())),
                    typeProblemNodes.OfType<IStaticScope>().Where(x => x.EntryPoints.Any()).ToDictionary(x => x, x => x.EntryPoints.Single()));

            }

            private IOrType<FlowNode, Inflow> GetFlowNodeOrInflow(ITypeProblemNode value, Dictionary<ITypeProblemNode, FlowNode> orsToFlowNodes, List<(Inflow, FlowNode)> inflows)
            {
                var type = GetType(value);

                if (type.Is5(out var inferred))
                {

                    return Prototypist.Toolbox.OrType.Make<FlowNode, Inflow>(inflows.Single(x => ReferenceEquals(x.Item2, orsToFlowNodes[inferred])).Item1);

                }
                return Prototypist.Toolbox.OrType.Make<FlowNode, Inflow>(orsToFlowNodes[value]);
            }


            #region Helpers

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
                    if (lookUper.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> lookUperLooksUp && lookUperLooksUp.Value.Equals(defererType))
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
                                var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                HasPublicMember(deferredToInferred, memberPair.Key, newValue);
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

                if (node.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> nodeLooksUp)
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

                        var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(method), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this, $"generated-generic-{method.debugName}", method.Converter)), map);

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

                        var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this, $"generated-generic-{type.debugName}", type.Key, type.Converter, type.IsPlaceHolder)), map);

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
                        orType.Left.If<TypeReference, int>(x => HandleHopefulMember(key, hopeful, GetType(x)));
                        orType.Right.If<TypeReference, int>(x => HandleHopefulMember(key, hopeful, GetType(x)));

                    },
                    inferredType =>
                    {
                        if (inferredType.PublicMembers.TryGetValue(key, out var member))
                        {
                            TryMerge(hopeful, member);
                        }
                        else
                        {
                            HasPublicMember(inferredType, key, hopeful);
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
                        Ors(orTo, CopiedToOrSelf(orFrom.Left.GetOrThrow()), CopiedToOrSelf(orFrom.Right.GetOrThrow()));
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
                                var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                HasReference(innerStaticScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var @object in innerFromStaticScope.Objects)
                            {
                                var newValue = Copy(@object.Value, new Object(this, $"copied from {((TypeProblemNode)@object.Value).debugName}", @object.Value.Converter, @object.Value.InitizationScope.Converter));
                                HasObject(innerStaticScopeTo, @object.Key, newValue);
                            }

                        }

                        {
                            foreach (var type in innerFromStaticScope.Types)
                            {
                                var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Key, type.Value.Converter, type.Value.IsPlaceHolder));
                                HasType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                        {
                            foreach (var method in innerFromStaticScope.Methods)
                            {
                                var newValue = Copy(method.Value, new Method(this, $"copied from {((TypeProblemNode)method.Value).debugName}", method.Value.Converter));
                                HasMethod(innerStaticScopeTo, method.Key, newValue);
                            }
                        }

                        {
                            foreach (var type in innerFromStaticScope.OrTypes)
                            {
                                var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                HasOrType(innerStaticScopeTo, type.Key, newValue);
                            }
                        }

                    }


                    if (innerFrom.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerFromPublicMembers) && innerTo.SafeIs<ITypeProblemNode, IHavePublicMembers>(out var innerToPublicMembers))
                    {


                        {
                            foreach (var member in innerFromPublicMembers.PublicMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                HasPublicMember(innerToPublicMembers, member.Key, newValue);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IScope>(out var innerFromScope) && innerTo.SafeIs<ITypeProblemNode, IScope>(out var innerScopeTo))
                    {

                        {
                            foreach (var item in innerFromScope.Values)
                            {
                                var newValue = Copy(item, new Value(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                HasValue(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.TransientMembers)
                            {
                                var newValue = Copy(member, new TransientMember(this, $"copied from {((TypeProblemNode)member).debugName}"));
                                HasTransientMember(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var member in innerFromScope.PrivateMembers)
                            {
                                var newValue = Copy(member.Value, new Member(this, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                HasPrivateMember(innerScopeTo, member.Key, newValue);
                            }
                        }

                        {
                            foreach (var possible in innerFromScope.PossibleMembers)
                            {
                                var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, Type>(out var innerFromType) && innerTo.SafeIs<ITypeProblemNode, Type>(out var innerTypeTo))
                    {

                        foreach (var type in innerFromType.GenericOverlays)
                        {
                            if (overlayed.TryGetValue(type.Value, out var toType))
                            {
                                HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, toType);
                            }
                            else
                            {
                                HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, type.Value);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, MethodType>(out var innerFromMethodType) && innerTo.SafeIs<ITypeProblemNode, MethodType>(out var innerMethodTypeTo))
                    {

                        foreach (var type in innerFromMethodType.GenericOverlays)
                        {
                            if (overlayed.TryGetValue(type.Value, out var toType))
                            {
                                HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, toType);
                            }
                            else
                            {
                                HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, type.Value);
                            }
                        }
                    }

                    if (innerFrom.SafeIs<ITypeProblemNode, IValue>(out var innerFromHopeful) && innerTo.SafeIs<ITypeProblemNode, IValue>(out var innerToHopeful))
                    {

                        foreach (var possible in innerFromHopeful.HopefulMembers)
                        {
                            var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                            HasHopefulMember(innerToHopeful, possible.Key, newValue);
                        }


                        if (innerFromHopeful.HopefulMethod is IIsDefinately<InferredType> infered)
                        {
                            var newValue = Copy(infered.Value, new InferredType(this, $"copied from {((TypeProblemNode)infered.Value).debugName}"));
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
            static bool IsHasPublicMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, out IHavePublicMembers? haveMembers)
            {
                var res = false;
                (haveMembers, res) = type.SwitchReturns<(IHavePublicMembers?, bool)>(
                    v1 => (default, false),
                    v2 => (v2, true),
                    v3 => (v3, true),
                    v4 => (default, false),
                    v5 => (v5, true),
                    v1 => (default, false));

                return res;
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

            FlowNode ToFlowNode(IOrType<FlowNode, Inflow> orType, List<(Inflow, FlowNode)> map) {
                return orType.SwitchReturns(
                    x => x,
                    x => map.Single(y => ReferenceEquals(y.Item1, x)).Item2);
            }

            bool Flow(FlowNode flowFrom, FlowNode flowTo, List<(Inflow, FlowNode)> map)
            {

                var res = false;

                if (flowTo.Accepts) {
                    var inflow = map.Single(x => ReferenceEquals(x, flowTo)).Item1;

                    if (!inflow.inFlows.Contains(flowFrom)) {
                        var newFlowTo = flowTo.Copy();
                        var newInfow = inflow.AddAsNew(flowFrom);
                        map.Add((newInfow, newFlowTo));
                        return Flow(flowFrom, newFlowTo,map);
                    }
                }

                foreach (var member in flowFrom.Members)
                {
                    if (flowTo.Members.TryGetValue(member.Key, out var existingMember))
                    {
                        res |= Flow(ToFlowNode(member.Value, map), ToFlowNode(existingMember, map), map);
                    }
                    else if (flowTo.Accepts)
                    {
                        flowTo.Members[member.Key] = member.Value;
                        res = true;
                    }
                    else { 
                        // it does not accept new members
                    }
                }

                if (flowFrom.Input != null) {
                    if (flowTo.Input != null)
                    {
                        res |= Flow(ToFlowNode(flowFrom.Input, map), ToFlowNode(flowTo.Input, map), map);
                    }
                    else if (flowTo.Accepts)
                    {
                        flowTo.Input = flowFrom.Input;
                        res = true;
                    }
                    else
                    {
                        // it does not accept new inputs
                    }
                }

                if (flowFrom.Output != null)
                {
                    if (flowTo.Output != null)
                    {
                        res |= Flow(ToFlowNode(flowFrom.Output, map), ToFlowNode(flowTo.Output, map), map);
                    }
                    else if (flowTo.Accepts)
                    {
                        flowTo.Output = flowFrom.Output;
                        res = true;
                    }
                    else
                    {
                        // it does not accept new inputs
                    }
                }

                return res;

            }

            bool Flow(IOrType<MethodType, Type, Object, OrType, Inflow, IError> flowFrom, IOrType<MethodType, Type, Object, OrType, Inflow, IError> flowTo, Dictionary<Inflow, InferredType> map)
            {
                if (flowTo.Is5(out var inflowTo))
                {

                    var value = flowFrom.SwitchReturns(
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x),
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x),
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x),
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x),
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(map[x]),
                    x => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(x));

                    if (!inflowTo.inFlows.Contains(value))
                    {
                        return false;
                    }

                    var target = map[inflowTo];
                }
            }

            // returns true if the target was modified 
            bool Flow(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowFrom, IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo, Dictionary<Inflow, InferredType> map)
            {
                // update the map

                var res = false;

                if (flowFrom.Is1(out var fromMethod) && flowTo.Is1(out var toMethod))
                {
                    var inFlowFrom = GetType(fromMethod.Input.GetOrThrow());
                    var inFlowTo = GetType(toMethod.Input.GetOrThrow());

                    res |= Flow(inFlowFrom, inFlowTo);


                    var returnFlowFrom = GetType(fromMethod.Returns.GetOrThrow());
                    var retrunFlowTo = GetType(toMethod.Returns.GetOrThrow());

                    res |= Flow(returnFlowFrom, retrunFlowTo);

                }

                if (IsHasPublicMembers(flowFrom, out var fromType))
                {
                    res |= FlowMembers(flowTo, fromType!.PublicMembers);
                }

                if (flowFrom.Is4(out var deferringOrType))
                {
                    res |= Flow(
                        flowTo,
                        Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(
                            Merge(
                                GetType(deferringOrType.Left.GetOrThrow()),
                                GetType(deferringOrType.Left.GetOrThrow()))));

                    // flow input and flow output 


                    // TODO
                    // so x =: int or string y
                    // we now now that x is:
                    // - int
                    // - string
                    // - int or string

                    // I guess what I am saying is x is also int or string

                    // if x is already a int or a string or a (int or string) we are cool
                    // if x is nothing (an empty inferred type) it becomes a (int or string) too
                    // if x is something else... some kind of error 

                    // so this really not a big deal
                    // we don't know what type an or type is so it is hard to flow anything
                    // we do what to flow members shared by both sides of the or type
                }

                if (flowFrom.Is5(out var deferringInferred))
                {
                    res |= FlowIO(flowTo, deferringInferred.Returns, deferringInferred.Input);
                }

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

            private InferredType Merge(IOrType<MethodType, Type, Object, OrType, InferredType, IError> leftType, IOrType<MethodType, Type, Object, OrType, InferredType, IError> rightType)
            {
                var res = new InferredType(this, "yuck");

                var leftMembers = GetMembers(leftType);
                var rightMembers = GetMembers(rightType);

                foreach (var leftMember in leftMembers)
                {
                    if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                    {
                        // this is sinful!
                        var newValue = new Member(this, $"zzz", new WeakMemberDefinitionConverter(false, leftMember.Key));
                        HasPublicMember(res, leftMember.Key, newValue);
                        newValue.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Merge(leftMember.Value, rightMember)));
                    }
                }

                // handle inputs and outputs 
                if (leftType.Is<IHaveInputAndOutput>(out var leftIO) &&
                    rightType.Is<IHaveInputAndOutput>(out var rightIO) &&
                    leftIO.Input is IIsDefinately<Member> leftI &&
                    rightIO.Input is IIsDefinately<Member> rightI &&
                    leftIO.Returns is IIsDefinately<TransientMember> leftO &&
                    rightIO.Returns is IIsDefinately<TransientMember> rightO)
                {

                    var mergedI = Merge(GetType(leftI.Value), GetType(rightI.Value));
                    var mergedO = Merge(GetType(leftO.Value), GetType(rightO.Value));

                    // shared code {A9E37392-760B-427D-852E-8829EEFCAE99}
                    var methodInputKey = new NameKey("merged implicit input - " + Guid.NewGuid());
                    // this is sinful!
                    var inputMember = new Member(this, methodInputKey.ToString()!, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(mergedI));
                    res.Input = Possibly.Is(inputMember);

                    var returnMember = new TransientMember(this, "merged implicit return -" + Guid.NewGuid());
                    returnMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(mergedO));
                    res.Returns = Possibly.Is(returnMember);

                }

                return res;
            }

            private Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GetMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type)
            {
                return type.SwitchReturns(
                    x => new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>(),
                    x => x.PublicMembers.ToDictionary(y => y.Key, y => GetType(y.Value)),
                    x => x.PublicMembers.ToDictionary(y => y.Key, y => GetType(y.Value)),
                    x => Merge(
                        GetType(x.Left.GetOrThrow()),
                        GetType(x.Right.GetOrThrow())).PublicMembers.ToDictionary(y => y.Key, y => GetType(y.Value)),
                    x => x.PublicMembers.ToDictionary(y => y.Key, y => GetType(y.Value)),
                    x => { throw new NotImplementedException("I'll deal with this later, when I have a more concrete idea of what it means. aka, when it bites me in the ass"); }
                );
            }

            //private bool FlowFromOr(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo, IOrType<MethodType, Type, Object, OrType, InferredType, IError> leftType, IOrType<MethodType, Type, Object, OrType, InferredType, IError> rightType)
            //{
            //    var res = false;

            //    if (leftType.Is<IHavePublicMembers>(out var leftHasPublicMembers))
            //    {
            //        if (rightType.Is<IHavePublicMembers>(out var rightHasPublicMembers))
            //        {
            //            foreach (var leftMember in leftHasPublicMembers.PublicMembers)
            //            {
            //                if (rightHasPublicMembers.PublicMembers.TryGetValue(leftMember.Key, out var rightMember))
            //                {

            //                    // first the member has to exist
            //                    // so we flow an empty infered type
            //                    res |= FlowMember(flowTo, leftMember.Key, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "or type from")));
            //                    // and then we have to flow all the shared members to it

            //                }

            //            }
            //        }
            //    }

            //    // TODO flow IO!

            //    return res;
            //}

            private bool FlowIO(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo, IIsPossibly<TransientMember> deferringReturns, IIsPossibly<Member> deferringInput)
            {
                var res = false;
                {
                    if (flowTo.Is1(out var deferredToMethod))
                    {
                        if (deferringReturns is IIsDefinately<TransientMember> deferringInferredReturns && deferredToMethod.Returns is IIsDefinately<TransientMember> deferredToMethodReturns)
                        {
                            res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToMethodReturns.Value));
                        }


                        if (deferringInput is IIsDefinately<Member> deferringInferredInput && deferredToMethod.Input is IIsDefinately<Member> deferredToMethodInput)
                        {
                            res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToMethodInput.Value));
                        }
                    }
                }

                {
                    if (flowTo.Is5(out var deferredToInferred))
                    {
                        if (deferringReturns is IIsDefinately<TransientMember> deferringInferredReturns)
                        {
                            if (deferredToInferred.Returns is IIsDefinately<TransientMember> deferredToInferredReturns)
                            {
                                res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToInferredReturns.Value));
                            }
                            else
                            {
                                deferredToInferred.Returns = deferringReturns;
                                res = true;
                            }
                        }

                        if (deferringInput is IIsDefinately<Member> deferringInferredInput)
                        {
                            if (deferredToInferred.Input is IIsDefinately<Member> deferredToInferredInput)
                            {
                                res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToInferredInput.Value));
                            }
                            else
                            {
                                deferredToInferred.Input = deferringInput;
                                res = true;
                            }
                        }
                    }
                }

                return res;
            }

            private bool FlowMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo, Dictionary<IKey, TypeProblem2.Member> fromMembers)
            {
                var res = false;
                foreach (var memberPair in fromMembers)
                {
                    res |= FlowMember(flowTo, memberPair.Key, GetType(memberPair.Value));
                }
                return res;
            }

            private bool FlowMember(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo, IKey key, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type)
            {
                var res = false;
                {
                    if (flowTo.Is2(out var deferredToHaveType))
                    {

                        if (deferredToHaveType.PublicMembers.TryGetValue(key, out var deferedToMember))
                        {
                            res |= Flow(type, GetType(deferedToMember));
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

                {
                    if (flowTo.Is3(out var deferredToObject))
                    {

                        if (deferredToObject.PublicMembers.TryGetValue(key, out var deferedToMember))
                        {
                            res |= Flow(type, GetType(deferedToMember));
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

                {
                    if (flowTo.Is5(out var deferredToInferred))
                    {

                        if (deferredToInferred.PublicMembers.TryGetValue(key, out var deferedToMember))
                        {
                            res |= Flow(type, GetType(deferedToMember));
                        }
                        else
                        {
                            // TODO
                            // new WeakMemberDefinitionConverter(
                            // is trash!
                            // I need to think about how the converters are passed in
                            // I might be able to pass the to here
                            // I can revert this so it passes in
                            var newValue = new Member(this, $"flowed member", new WeakMemberDefinitionConverter(false, key));
                            HasPublicMember(deferredToInferred, key, newValue);
                            newValue.LooksUp = Possibly.Is(type);
                            res = true;
                        }

                    }
                }

                return res;
            }


            // these are werid
            // get members should just be a method no my type problem nodes

            // copy is the same.

            // also
            // GetMembers2 vs GetMembers is really sloppy
            //IReadOnlyDictionary<IKey, Member> GetMembers2(IOrType<MethodType, Type, Object, OrType, InferredType, IError> or, Dictionary<OrType, Dictionary<IKey, Member>> orTypeMembers)
            //{

            //    return or.SwitchReturns(
            //        _ => new Dictionary<IKey, Member>(),
            //        v2 => GetPublicMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v2), orTypeMembers),
            //        v3 => GetPublicMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v3), orTypeMembers),
            //        v4 => GetPublicMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v4), orTypeMembers),
            //        _ => new Dictionary<IKey, Member>(),
            //        _ => new Dictionary<IKey, Member>()
            //        );
            //}

            //IReadOnlyDictionary<IKey, Member> GetPublicMembers(IOrType<IHaveMembers, OrType> type, Dictionary<OrType, Dictionary<IKey, Member>> orTypeMembers)
            //{
            //    // not everyone exposes members
            //    // a scope does not
            //    // a type does 
            //    // a method does not
            //    // an inferred type does - but not input and output 
            //    // input and output should not go in members?
            //    // I hate the type problem 


            //    if (type.Is1(out var explictType))
            //    {
            //        return explictType.Members;
            //    }

            //    if (type.Is2(out var orType))
            //    {
            //        if (orTypeMembers.TryGetValue(orType, out var res))
            //        {
            //            return res;
            //        }

            //        res = new Dictionary<IKey, Member>();
            //        var left = orType.Left.GetOrThrow();
            //        var right = orType.Right.GetOrThrow();

            //        var rightMembers = GetMembers2(GetType(right), orTypeMembers);
            //        foreach (var leftMember in GetMembers2(GetType(left), orTypeMembers))
            //        {
            //            if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
            //            {
            //                // TODO
            //                // else where you use an orType for the type of members defined on both side of an OrType
            //                // if they are the same type
            //                if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
            //                {
            //                    var member = new Member(this, $"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}", leftMember.Value.Converter)
            //                    {
            //                        LooksUp = Possibly.Is(GetType(rightMember))
            //                    };
            //                    res[leftMember.Key] = member;
            //                }
            //            }
            //        }

            //        orTypeMembers[orType] = res;

            //        return res;
            //    }

            //    throw new Exception($"{type.GetType()} unexpected");

            //}

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

            static IOrType<MethodType, Type, Object, OrType, InferredType, IError> GetType(ITypeProblemNode value)
            {
                if (value.SafeIs(out ILookUpType lookup))
                {
                    // look up needs to be populated at this point
                    return lookup.LooksUp.GetOrThrow();
                }
                if (value.SafeIs(out MethodType methodType))
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType);
                }

                if (value.SafeIs(out Type type))
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type);

                }
                if (value.SafeIs(out Object @object))
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object);
                }
                if (value.SafeIs(out OrType orType))
                {
                    return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType);
                }

                throw new Exception("flaming pile of piss");
                // well, I guess I now know that we have a duality
                // you either are a type, or you have a type
                // 
            }



            public TypeProblem2(IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> moduleConverter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter)
            {

                Primitive = new Scope(this, "base", rootConverter);
                Dependency = CreateScope(Primitive, rootConverter);
                ModuleRoot = CreateObjectOrModule(CreateScope(Dependency, rootConverter), new ImplicitKey(Guid.NewGuid()), moduleConverter, innerConverter);

                CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("block")), new PrimitiveTypeConverter(new BlockType()));
                NumberType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("number")), new PrimitiveTypeConverter(new NumberType()));
                StringType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("string")), new PrimitiveTypeConverter(new StringType()));
                BooleanType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("bool")), new PrimitiveTypeConverter(new BooleanType()));
                EmptyType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("empty")), new PrimitiveTypeConverter(new EmptyType()));

                // shocked this works...
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1"))), new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2"))) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] { new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1")), new WeakTypeDefinitionConverter()), new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2")), new WeakTypeDefinitionConverter()) };

                var res = new MethodType(
                    this,
                    $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                HasMethodType(Primitive, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.key}", Possibly.Is(placeholder.key), placeholder.converter, true);
                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(res), placeholder.key.SwitchReturns<IKey>(x => x, x => x), Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(placeholderType));
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                // here it is ok for these to be members because we are using a method type
                res.Input = Possibly.Is(CreatePrivateMember(res, res, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T1")), new WeakMemberDefinitionConverter(false, methodInputKey)));
                res.Returns = Possibly.Is(CreateTransientMember(res, new NameKey("T2")));
                IsChildOf(Primitive, res);
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
