using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

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

            public class Type : TypeProblemNode<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType
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
                public List<Value> Values { get; } = new List<Value>();
                //public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                //public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }

                public bool IsPlaceHolder { get; }
            }

            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class MethodType : TypeProblemNode<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IScope
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
                //public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IScope>();
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
            public class Scope : TypeProblemNode<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>, IScope
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
            public class Object : TypeProblemNode<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IStaticScope> Parent { get; set; } = Possibly.IsNot<IStaticScope>();
                public Dictionary<IKey, Member> PublicMembers { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                //public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                //public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
            }
            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class Method : TypeProblemNode<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>, IScope, IHaveInputAndOutput
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
            public static void HasValue(IStaticScope parent, Value value)
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
            public void HasMethodType(IScope parent, IKey key, MethodType type)
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
            public static Member HasMembersPossiblyOnParent(IScope parent, IKey key, Member member)
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

            public Value CreateValue(IStaticScope scope, IKey typeKey, IConvertTo<Value, PlaceholderValue> converter)
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
                else {
                    throw new Exception("this is probably really an IError - you tried to add a member somewhere one cannot go");
                }
                res.Context = Possibly.Is(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePublicMember<T>(
                T scope,
                IKey key,
                IOrType<IKey, IError> typeKey,
                IConvertTo<Member, WeakMemberDefinition> converter)
                where T : IStaticScope, IHavePublicMembers
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPublicMember(scope, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePublicMember<T>(
                T scope,
                IKey key,
                IConvertTo<Member, WeakMemberDefinition> converter)
                where T : IStaticScope, IHavePublicMembers
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPublicMember(scope, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
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

            public Member CreatePrivateMember<T>(
                T scope, 
                IKey key, 
                IOrType<IKey, IError> typeKey, 
                IConvertTo<Member, WeakMemberDefinition> converter)
                where T:IStaticScope,IHavePrivateMembers
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPrivateMember(scope, key, res);
                res.Context = Possibly.Is<IStaticScope>(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreatePrivateMember<T>(
                T scope, 
                IKey key, 
                IConvertTo<Member, WeakMemberDefinition> converter)
                where T : IStaticScope, IHavePrivateMembers
            {
                var res = new Member(this, key.ToString()!, converter);
                HasPrivateMember(scope, key, res);
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

            public Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                if (scope.PossibleMembers.TryGetValue(key, out var res1))
                {
                    return res1;
                }
                var res = new Member(this, "possibly on parent -" + key.ToString(), converter);
                res = HasMembersPossiblyOnParent(scope, key, res);
                res.Context = Possibly.Is(scope);
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
            public Object CreateObjectOrModule(IStaticScope parent, IKey key, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter)
            {
                var res = new Object(this, key.ToString()!, converter);
                IsChildOf(parent, res);
                HasObject(parent, key, res);
                return res;
            }

            public Method CreateMethod(IStaticScope parent, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                // TODO
                // dang it!
                // these should not call these methods! they don't go in the member list!
                var returns = CreateTransientMember(res);
                res.Returns = Possibly.Is(returns);
                var input = CreatePrivateMember(res, new NameKey(inputName), inputConverter);
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
                        // TODO
                        // dang it!
                        // these should not call these methods! they don't go in the member list!
                        res.Input = Possibly.Is(CreatePrivateMember(res, new NameKey(inputName), Prototypist.Toolbox.OrType.Make<IKey, IError>(typeKey.Value), inputConverter));
                    }
                    else
                    {                
                        // TODO
                        // dang it!
                        // these should not call these methods! they don't go in the member list!
                        res.Input = Possibly.Is(CreatePrivateMember(res, new NameKey(inputName), inputConverter));
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


            public OrType CreateOrType(IScope s, IKey key, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode1, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode2, IConvertTo<OrType, WeakTypeOrOperation> converter)
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

            private static void HasOrType(IScope scope, IKey kay, OrType orType1)
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


            public TransientMember GetReturns(IScope s)
            {
                if (s is Method method)
                {
                    return GetReturns(method);
                }
                else if (s.Parent is IIsDefinately<IScope> definatelyScope)
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
                    var methodInputKey = new NameKey("implicit input - " + Guid.NewGuid());
                    var inputMember = new Member(this, methodInputKey.ToString()!, new WeakMemberDefinitionConverter(false, methodInputKey))
                    {
                        LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")))
                    };
                    inferredMethodType.Input = Possibly.Is(inputMember);

                    var returnMember = new TransientMember(this, "implicit return -" + Guid.NewGuid());
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
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
                    var methodInputKey = new NameKey("implicit input - " + Guid.NewGuid());
                    var inputMember = new Member(this, methodInputKey.ToString()!, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
                    inferredMethodType.Input = Possibly.Is(inputMember);

                    var returnMember = new TransientMember(this, "implicit return -" + Guid.NewGuid());
                    inputMember.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(new InferredType(this, "implicit input")));
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

                foreach (var (node, possibleMembers) in typeProblemNodes.OfType<IScope>().Select(x => (x, x.PossibleMembers)))
                {
                    foreach (var pair in possibleMembers)
                    {
                        TryGetMember(node, pair.Key).IfElse(member => TryMerge(pair.Value, member!), () => HasPrivateMember(node, pair.Key, pair.Value));
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

                // flow up stream
                // very sloppy and slow
                // if I ever am worried about speed I am sure this will be a canidate
                bool go;
                do
                {
                    go = false;

                    foreach (var (from, to) in assignments)
                    {
                        // nothing should look up to null at this point
                        var toType = to.LooksUp.GetOrThrow();
                        var fromType = from.LooksUp.GetOrThrow();

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

                // OrType needs to be it't own block and we need to flow in to both sides
                {
                    if (deferredToType.Is4(out var orType)) {
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

            void MergeIntoOrType(OrType orType, KeyValuePair<IKey, Member> memberPair) {

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

                    if (bigOr.Is<OrType>(out var leftOr)) {

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
                        if (haveTypes is IScope scope && scope.Objects.TryGetValue(key, out var res))
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
                        orType.Left.If<TypeReference, int>(x => HandleHopefulMember(key, hopeful,GetType(x)));
                        orType.Right.If<TypeReference, int>(x => HandleHopefulMember(key, hopeful, GetType(x)));

                    },
                    inferredType => { },
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
                    if (pair.Key.SafeIs(out IScope fromScope) && fromScope.Parent.SafeIs(out IIsDefinately<IScope> defScope))
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

                        if (lookUpFrom.Context.SafeIs(out IIsDefinately<IScope> definateContext))
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

                // hasGenerics -- the root of the root will have had its generics replaced
                // for the rest of the tree the generics will need to be copied
                T Copy<T>(T innerFrom, T innerTo)
                    where T : ITypeProblemNode
                {
                    map.Add(innerFrom, innerTo);

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
                            foreach (var item in innerFromScope.Refs)
                            {
                                var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                HasReference(innerScopeTo, newValue);
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
                            foreach (var member in innerFromScope.TransientMembers)
                            {
                                var newValue = Copy(member, new TransientMember(this, $"copied from {((TypeProblemNode)member).debugName}"));
                                HasTransientMember(innerScopeTo, newValue);
                            }
                        }

                        {
                            foreach (var @object in innerFromScope.Objects)
                            {
                                var newValue = Copy(@object.Value, new Object(this, $"copied from {((TypeProblemNode)@object.Value).debugName}", @object.Value.Converter));
                                HasObject(innerScopeTo, @object.Key, newValue);
                            }

                        }

                        {
                            foreach (var type in innerFromScope.Types)
                            {
                                var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Key, type.Value.Converter, type.Value.IsPlaceHolder));
                                HasType(innerScopeTo, type.Key, newValue);
                            }
                        }
                        {
                            foreach (var method in innerFromScope.Methods)
                            {
                                var newValue = Copy(method.Value, new Method(this, $"copied from {((TypeProblemNode)method.Value).debugName}", method.Value.Converter));
                                HasMethod(innerScopeTo, method.Key, newValue);
                            }
                        }

                        {
                            foreach (var type in innerFromScope.OrTypes)
                            {
                                var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                HasOrType(innerScopeTo, type.Key, newValue);
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

            // returns true if the target was modified 
            bool Flow(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowFrom, IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo)
            {
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

                    {
                        if (flowTo.Is2(out var deferredToHaveType))
                        {
                            foreach (var memberPair in fromType!.PublicMembers)
                            {
                                if (deferredToHaveType.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
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

                    {
                        if (flowTo.Is3(out var deferredToObject))
                        {
                            foreach (var memberPair in fromType!.PublicMembers)
                            {
                                if (deferredToObject.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
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

                    {
                        if (flowTo.Is5(out var deferredToInferred))
                        {

                            foreach (var memberPair in fromType!.PublicMembers)
                            {
                                if (deferredToInferred.PublicMembers.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
                                }
                                else
                                {
                                    var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                    HasPublicMember(deferredToInferred, memberPair.Key, newValue);
                                    newValue.LooksUp = memberPair.Value.LooksUp;
                                    res = true;
                                }
                            }
                        }
                    }
                }

                if (flowFrom.Is4(out var deferringOrType)) {

                    throw new NotImplementedException();

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
                }

                if (flowFrom.Is5(out var deferringInferred))
                {

                    {
                        if (flowTo.Is1(out var deferredToMethod))
                        {
                            if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns && deferredToMethod.Returns is IIsDefinately<TransientMember> deferredToMethodReturns)
                            {
                                res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToMethodReturns.Value));
                            }


                            if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput && deferredToMethod.Input is IIsDefinately<Member> deferredToMethodInput)
                            {
                                res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToMethodInput.Value));
                            }
                        }
                    }

                    {
                        if (flowTo.Is5(out var deferredToInferred))
                        {
                            if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns)
                            {
                                if (deferredToInferred.Returns is IIsDefinately<TransientMember> deferredToInferredReturns)
                                {
                                    res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToInferredReturns.Value));
                                }
                                else
                                {
                                    deferredToInferred.Returns = deferringInferred.Returns;
                                    res = true;
                                }
                            }

                            if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput)
                            {
                                if (deferredToInferred.Input is IIsDefinately<Member> deferredToInferredInput)
                                {
                                    res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToInferredInput.Value));
                                }
                                else
                                {
                                    deferredToInferred.Input = deferringInferred.Input;
                                    res = true;
                                }
                            }
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

            IIsPossibly<Member> TryGetMember(IStaticScope context, IKey key)
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



            public TypeProblem2(IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> moduleConverter)
            {

                Primitive = new Scope(this, "base", rootConverter);
                Dependency = CreateScope(Primitive, rootConverter);
                ModuleRoot = CreateObjectOrModule(CreateScope(Dependency, rootConverter), new ImplicitKey(Guid.NewGuid()), moduleConverter);

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
                // TODO here too
                // inputs and output are only sort of member
                // well maybe they are private members 
                // totally are private members 
                // why am I so dumb
                res.Input = Possibly.Is(CreatePrivateMember(res, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T1")), new WeakMemberDefinitionConverter(false, methodInputKey)));
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
