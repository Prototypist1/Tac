using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal interface ISetUpTypeProblem
    {
        // a =: x

        void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
        Tpn.IValue CreateValue(Tpn.IScope scope, IKey typeKey);
        Tpn.IMember CreateMember(Tpn.IScope scope, IKey key, IKey typeKey);
        Tpn.IMember CreateMember(Tpn.IScope scope, IKey key);
        Tpn.ITypeReference CreateTypeReference(Tpn.IScope context, IKey typeKey);
        Tpn.IScope CreateScope(Tpn.IScope parent);
        Tpn.IType CreateType(Tpn.IScope parent, IKey key);
        Tpn.IType CreateGenericType(Tpn.IScope parent, IKey key, IReadOnlyList<IKey> placeholders);
        Tpn.IObject CreateObject(Tpn.IScope parent);
        Tpn.IMethod CreateMethod(Tpn.IScope parent, string inputName);
        Tpn.IMethod CreateMethod(Tpn.IScope parent, Tpn.ITypeReference inputType, Tpn.ITypeReference outputType, string inputName);
        Tpn.IMember GetReturns(Tpn.IScope s);
        Tpn.IMember CreateHopefulMember(IHaveHopefulMembers scope, IKey key);
    }

    internal static class TypeProblemNodeExtensions {

        public static IKey Key(this Tpn.ITypeReference type)
        {
            return type.Problem.GetKey(type);
        }

        public static Tpn.IMember Returns(this Tpn.IMethod method)
        {
            return method.Problem.GetReturns(method);
        }

        public static void AssignTo(this ICanAssignFromMe from, ICanBeAssignedTo to)
        {
            from.Problem.IsAssignedTo(from, to);
        }
    }

    internal interface ITypeProblemNode {
        TypeProblem2 Problem {get;}
    }

    internal interface IHaveMembersPossiblyOnParent : Tpn.IScope { }
    internal interface IHaveHopefulMembers : ILookUpType { }
    internal interface ILookUpType : ITypeProblemNode { }
    internal interface ICanAssignFromMe : ILookUpType, ITypeProblemNode { }
    internal interface ICanBeAssignedTo : ILookUpType, ITypeProblemNode { }

    public static class Tpn
    {

        internal interface ITypeReference : ITypeProblemNode, ILookUpType { }
        internal interface IValue : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe { }
        internal interface IMember : IValue, ICanBeAssignedTo { }
        internal interface IType : ITypeProblemNode, IScope { }
        internal interface IScope : ITypeProblemNode { }
        internal interface IObject : IHaveHopefulMembers, IHaveMembersPossiblyOnParent, IScope, ICanAssignFromMe { }
        internal interface IMethod : IHaveHopefulMembers, IHaveMembersPossiblyOnParent, IScope, ICanAssignFromMe { }

    }

    internal class TypeProblem2 : ISetUpTypeProblem
    {

        private abstract class TypeProblemNode: ITypeProblemNode
        {
            public TypeProblemNode(TypeProblem2 problem) {
                Problem = problem ?? throw new ArgumentNullException(nameof(problem));
                problem.Register(this);
            }

            public TypeProblem2 Problem { get; }
        }
        private class TypeReference : TypeProblemNode, Tpn.ITypeReference
        {
            public TypeReference(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Value : TypeProblemNode, Tpn.IValue
        {
            public Value(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Member : TypeProblemNode, Tpn.IMember
        {
            public Member(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Type : TypeProblemNode, Tpn.IType
        {
            public Type(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class InferedType : Type
        {
            public InferedType(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Scope : TypeProblemNode, Tpn.IScope
        {
            public Scope(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Object : TypeProblemNode, Tpn.IObject
        {
            public Object(TypeProblem2 problem) : base(problem)
            {
            }
        }
        private class Method : TypeProblemNode, Tpn.IMethod
        {
            public Method(TypeProblem2 problem) : base(problem)
            {
            }
        }

        // basic stuff
        private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

        public Tpn.IScope Root { get; }
        // relationships
        private readonly Dictionary<Tpn.IScope, Tpn.IScope> kidParent = new Dictionary<Tpn.IScope, Tpn.IScope>();

        private readonly Dictionary<Tpn.IMethod, Tpn.IMember> methodReturns = new Dictionary<Tpn.IMethod, Tpn.IMember>();

        private readonly Dictionary<Tpn.IScope, List<Tpn.IValue>> values = new Dictionary<Tpn.IScope, List<Tpn.IValue>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>> members = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>>();
        private readonly Dictionary<Tpn.IScope, List<Tpn.ITypeReference>> refs = new Dictionary<Tpn.IScope, List<Tpn.ITypeReference>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IType>> types = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IType>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IType>> genericOverlays = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IType>>();

        private readonly Dictionary<IHaveMembersPossiblyOnParent, Dictionary<IKey, Tpn.IMember>> possibleMembers = new Dictionary<IHaveMembersPossiblyOnParent, Dictionary<IKey, Tpn.IMember>>();
        private readonly Dictionary<IHaveHopefulMembers, Dictionary<IKey, Tpn.IMember>> hopefulMembers = new Dictionary<IHaveHopefulMembers, Dictionary<IKey, Tpn.IMember>>();
        private readonly List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
        // members
        private readonly Dictionary<ILookUpType, IKey> lookUpTypeKey = new Dictionary<ILookUpType, IKey>();
        private readonly Dictionary<ILookUpType, Tpn.IScope> lookUpTypeContext = new Dictionary<ILookUpType, Tpn.IScope>();

        private readonly Dictionary<GenericTypeKey, Tpn.IType> realizedGeneric = new Dictionary<GenericTypeKey, Tpn.IType>();

        #region Building APIs

        public void IsChildOf(Tpn.IScope parent, Tpn.IScope kid)
        {
            kidParent.Add(kid, parent);
        }
        public void HasValue(Tpn.IScope parent, Tpn.IValue value)
        {
            if (!values.ContainsKey(parent))
            {
                values.Add(parent, new List<Tpn.IValue>());
            }
            values[parent].Add(value);
        }
        public void HasReference(Tpn.IScope parent, Tpn.ITypeReference reference)
        {
            if (!refs.ContainsKey(parent))
            {
                refs.Add(parent, new List<Tpn.ITypeReference>());
            }
            refs[parent].Add(reference);
        }
        public void HasType(Tpn.IScope parent, IKey key, Tpn.IType type)
        {
            if (!types.ContainsKey(parent))
            {
                types.Add(parent, new Dictionary<IKey, Tpn.IType>());
            }
            types[parent].Add(key, type);
        }

        public void HasPlaceholderType(Tpn.IScope parent, IKey key, Tpn.IType type)
        {
            if (!genericOverlays.ContainsKey(parent))
            {
                genericOverlays.Add(parent, new Dictionary<IKey, Tpn.IType>());
            }
            genericOverlays[parent].Add(key, type);
        }
        public void HasMember(Tpn.IScope parent, IKey key, Tpn.IMember member)
        {
            if (!members.ContainsKey(parent))
            {
                members.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            members[parent].Add(key, member);
        }
        public void HasMembersPossiblyOnParent(IHaveMembersPossiblyOnParent parent, IKey key, Tpn.IMember member)
        {
            if (!possibleMembers.ContainsKey(parent))
            {
                possibleMembers.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            possibleMembers[parent].Add(key, member);
        }
        public void HasHopefulMember(IHaveHopefulMembers parent, IKey key, Tpn.IMember member)
        {

            if (!hopefulMembers.ContainsKey(parent))
            {
                hopefulMembers.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            hopefulMembers[parent].Add(key, member);
        }
       
        private T Register<T>(T typeProblemNode)
            where T : ITypeProblemNode
        {
            typeProblemNodes.Add(typeProblemNode);
            return typeProblemNode;
        }

        public void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo)
        {
            assignments.Add((assignedFrom, assignedTo));
        }

        public Tpn.IValue CreateValue(Tpn.IScope scope, IKey typeKey )
        {
            var res = new Value(this);
            HasValue(scope, res);
            lookUpTypeContext[res] = scope;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IMember CreateMember(Tpn.IScope scope, IKey key, IKey typeKey)
        {
            var res = new Member(this);
            HasMember(scope, key,res);
            lookUpTypeContext[res] = scope;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IMember CreateMember(Tpn.IScope scope, IKey key)
        {
            var res = new Member(this);
            HasMember(scope, key,res);
            lookUpTypeContext[res] = scope;
            return res;
        }



        public Tpn.ITypeReference CreateTypeReference(Tpn.IScope context, IKey typeKey)
        {
            var res = new TypeReference(this);
            HasReference(context, res);
            lookUpTypeContext[res] = context;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IScope CreateScope(Tpn.IScope parent)
        {
            var res = new Scope(this);
            IsChildOf(parent, res);
            return res;
        }

        public Tpn.IType CreateType(Tpn.IScope parent, IKey key)
        {
            var res = new Type(this);
            IsChildOf(parent, res);
            HasType(parent, key,res);
            return res;
        }

        public Tpn.IType CreateGenericType(Tpn.IScope parent, IKey key, IReadOnlyList<IKey> placeholders)
        {
            var res = new Type(this);
            IsChildOf(parent, res);
            HasType(parent, key, res);
            foreach (var placeholder in placeholders)
            {
                var placeholderType = new Type(this);
                HasPlaceholderType(res, placeholder, placeholderType);
            }
            return res;
        }

        public Tpn.IObject CreateObject(Tpn.IScope parent)
        {
            var res= new Object(this);
            IsChildOf(parent, res);
            return res;
        }

        public Tpn.IMethod CreateMethod(Tpn.IScope parent, string inputName)
        {
            var res = new Method(this);
            IsChildOf(parent, res);
            var returns = CreateMember(res, new ImplicitKey());
            methodReturns[res] = returns;
            CreateMember(res, new NameKey(inputName));
            return res;
        }


        public Tpn.IMethod CreateMethod(Tpn.IScope parent, Tpn.ITypeReference inputType, Tpn.ITypeReference outputType, string inputName)
        {

            var res = new Method(this);
            IsChildOf(parent, res);
            var returns = lookUpTypeKey.TryGetValue(inputType, out var outkey) ? CreateMember(res, new ImplicitKey(), outkey) : CreateMember(res, new ImplicitKey()) ;
            methodReturns[res] = returns;
            if (lookUpTypeKey.TryGetValue(inputType, out var inkey)) { CreateMember(res, new NameKey(inputName), inkey); } else { CreateMember(res, new NameKey(inputName)); }
            return res;
        }


        public Tpn.IMember CreateHopefulMember(IHaveHopefulMembers scope, IKey key)
        {
            var res = new Member(this);
            HasHopefulMember(scope, key, res);
            return res;
        }


        #endregion


        public Tpn.IMember GetReturns(Tpn.IScope s)
        {
            if (s is Tpn.IMethod method)
            {
                return GetReturns(method);
            }
            else {
                return GetReturns(kidParent[s]);
            }
        }


        internal Tpn.IMember GetReturns(Tpn.IMethod method)
        {
            return methodReturns[method];
        }

        internal IKey GetKey(Tpn.ITypeReference type)
        {
            return lookUpTypeKey[type];
        }

        // more to do 
        // returns
        // accepts
        // is of type
        // what about modules?

        public void Solve2()
        {
            // create types for everything 
            var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
            foreach (var node in toLookUp.Where(x => !lookUpTypeKey.ContainsKey(x)))
            {
                var key = new ImplicitKey();
                var type = new InferedType(this);
                lookUps[node] = type;
                lookUpTypeKey[node] = key;
                HasType(lookUpTypeContext[node], key, type);
            }
            toLookUp = typeProblemNodes.OfType<ILookUpType>().Except(lookUps.Keys).ToArray();

            // overlay generics
            while (toLookUp.Any())
            {
                foreach (var node in toLookUp)
                {
                    lookUps[node] = LookUpOrOverlayOrThrow(lookUpTypeContext[node], lookUpTypeKey[node]);
                }
                toLookUp = typeProblemNodes.OfType<ILookUpType>().Except(lookUps.Keys).ToArray();
            }

            // members that might be on parents 
            var defersTo = new Dictionary<Tpn.IType, Tpn.IType>();

            foreach (var item in possibleMembers)
            {
                foreach (var pair in item.Value)
                {
                    if (TryGetMember(item.Key, pair.Key, out var member))
                    {
                        defersTo[GetType(pair.Value)] = GetType(member);
                    }
                    else {
                        HasMember(item.Key, pair.Key, pair.Value);
                    }
                }
            }

            // hopeful members 

            foreach (var hopeful in hopefulMembers)
            {
                foreach (var pair in hopeful.Value)
                {
                    if (members[GetType(hopeful.Key)].TryGetValue(pair.Key, out var member))
                    {
                        defersTo[GetType(pair.Value)] = GetType(member);
                    }
                    else {
                        HasMember(GetType(hopeful.Key), pair.Key, pair.Value);
                    }
                }
            }

            // flow upstream
            foreach (var (from,to) in assignments)
            {
                Flow(GetType(from), GetType(to));
            }

            // flow downstream
            // we can't flow through convergences, since it might be an or-type
            foreach (var (from, to) in assignments.GroupBy(x => x.Item2).Where(x => x.Count() == 1).SelectMany(x => x))
            {
                Flow(GetType(to), GetType(from));
            }

            #region Helpers

            Tpn.IType GetType(ILookUpType value)
            {
                var res = lookUps[value];
                while (true)
                {
                    if (defersTo.TryGetValue(res, out var nextRes))
                    {
                        res = nextRes;
                    }
                    else
                    {
                        return res;
                    }
                }
            }

            void Flow(Tpn.IType from, Tpn.IType to)
            {
                // I think the only thing that "flow" are members
                // but not all types will accept new members
                if (to is InferedType) {
                    foreach (var memberPair in members[from])
                    {
                        if (members[to].TryGetValue(memberPair.Key, out var upstreamMember))
                        {
                            var one = GetType(upstreamMember);
                            var two = GetType(memberPair.Value);
                            if (one is InferedType oneInfered)
                            {
                                Flow(two, oneInfered);
                            }
                            else if (two is InferedType twoInfered)
                            {
                                Flow(one, twoInfered);
                            }
                            else {
                                throw new Exception("these types are not compatible... right?");
                            }

                        }
                        else {
                            members[to].Add(memberPair.Key, memberPair.Value);
                        }
                    }
                }
            }

            #endregion

        }

        private bool TryGetMember(Tpn.IScope context, IKey key, out Tpn.IMember member)
        {
            while (true)
            {
                if (members[context].TryGetValue(key, out member))
                {
                    return true;
                }
                if (!kidParent.TryGetValue(context, out context)) {
                    return false;
                }
            }
        }

        private Tpn.IType LookUpOrOverlayOrThrow(Tpn.IScope from, ILookUpType lookUp)
        {
            if (!TryLookUpOrOverlay(from, lookUp, out var res))
            {
                throw new Exception("could not find type");
            }
            return res;
        }

        private Tpn.IType LookUpOrOverlayOrThrow(Tpn.IScope from, IKey key)
        {
            if (!TryLookUpOrOverlay(from, key, out var res))
            {
                throw new Exception("could not find type");
            }
            return res;
        }

        // or maybe I just need to make we get the same outcome requardless of what order references are processed in'
        private Dictionary<ILookUpType, Tpn.IType> lookUps = new Dictionary<ILookUpType, Tpn.IType>();

        public TypeProblem2()
        {
            Root = new Scope(this);
        }

        private bool TryLookUpOrOverlay(Tpn.IScope from, ILookUpType lookUp, out Tpn.IType res)
        {

            if (lookUps.TryGetValue(lookUp, out res))
            {
                return true;
            }

            var key = lookUpTypeKey[lookUp];
            if (TryLookUpOrOverlay(from, key, out res))
            {

                return true;
            }
            return false;
        }

        private bool TryLookUpOrOverlay(Tpn.IScope from, IKey key, out Tpn.IType res)
        {

            if (key is GenericNameKey genericNameKey)
            {

                var types = genericNameKey.Types.Select(typeKey => (typeKey, LookUpOrOverlayOrThrow(from, typeKey))).ToArray();
                var lookedUp = LookUpOrOverlayOrThrow(from, genericNameKey.name);
                var genericTypeKey = new GenericTypeKey(lookedUp, types.Select(x => x.Item2).ToArray());

                if (realizedGeneric.TryGetValue(genericTypeKey, out res))
                {
                    return true;
                }

                var to = new Type(this);
                foreach (var type in types)
                {
                    HasPlaceholderType(to, type.typeKey, type.Item2);
                }

                res = CopyTree(lookedUp, to);
                realizedGeneric.Add(genericTypeKey, res);
                return true;
            }
            else
            if (TryLookUp(from, key, out res))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Tpn.IScope DefinedOn(Tpn.IScope haveTypes, IKey key)
        {
            while (true)
            {
                if (types[haveTypes].TryGetValue(key, out var _))
                {
                    return haveTypes;
                }
                if (!kidParent.TryGetValue(haveTypes, out haveTypes))
                {
                    throw new Exception("uhh, should have found that");
                }
            }
        }

        private bool TryLookUp(Tpn.IScope haveTypes, IKey key, out Tpn.IType result)
        {
            while (true)
            {
                if (types[haveTypes].TryGetValue(key, out var res))
                {
                    result = res;
                    return true;
                }
                if (!kidParent.TryGetValue(haveTypes, out haveTypes))
                {
                    result = null;
                    return false;
                }
            }
        }

        private Tpn.IType CopyTree(Tpn.IType from, Tpn.IType to)
        {

            var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();
            Copy(from, to);

            foreach (var pair in map)
            {
                if (pair.Key is Tpn.IScope fromScope && to is Tpn.IScope toScope)
                {
                    kidParent[toScope] = kidParent[CopiedToOrSelf(fromScope)];
                }
            }

            var oldAssignments = assignments.ToArray();
            foreach (var pair in map)
            {
                if (pair.Key is ICanBeAssignedTo assignedToFrom && pair.Value is ICanBeAssignedTo assignedToTo)
                {
                    foreach (var item in oldAssignments)
                    {
                        if (item.Item2 == assignedToFrom)
                        {
                            assignments.Add((CopiedToOrSelf(item.Item1), assignedToTo));
                        }
                    }
                }

                if (pair.Value is ICanAssignFromMe assignFromFrom && pair.Value is ICanAssignFromMe assignFromTo)
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
                if (pair.Key is ILookUpType lookUpFrom && pair.Value is ILookUpType lookUpTo)
                {
                    if (lookUpTypeKey.TryGetValue(lookUpFrom, out var key))
                    {
                        lookUpTypeKey.Add(lookUpTo, key);
                    }

                    if (lookUpTypeContext.TryGetValue(lookUpFrom, out var context))
                    {
                        lookUpTypeContext.Add(lookUpTo, CopiedToOrSelf(context));
                    }
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

            T Copy<T>(T innerFrom, T innerTo)
                where T : ITypeProblemNode
            {
                map.Add(innerFrom, innerTo);

                if (innerFrom is Tpn.IScope innerFromScope && innerTo is Tpn.IScope innerFromTo)
                {

                    foreach (var item in values[innerFromScope])
                    {
                        var newValue = Copy(item, new Value(this));
                        HasValue(innerFromTo, newValue);
                    }

                    foreach (var item in refs[innerFromScope])
                    {
                        var newValue = Copy(item, new TypeReference(this));
                        HasReference(innerFromTo, newValue);
                    }

                    foreach (var member in members[innerFromScope])
                    {
                        var newValue = Copy(member.Value, new Member(this));
                        HasMember(innerFromTo, member.Key, newValue);
                    }

                    foreach (var type in types[innerFromScope])
                    {
                        var newValue = Copy(type.Value, new Type(this));
                        HasType(innerFromTo, type.Key, newValue);
                    }

                    foreach (var type in genericOverlays[innerFromScope])
                    {
                        if (!genericOverlays[innerFromTo].ContainsKey(type.Key))
                        {
                            HasPlaceholderType(innerFromTo, type.Key, type.Value);
                        }
                    }
                }

                if (innerFrom is IHaveMembersPossiblyOnParent innerFromPossible && innerTo is IHaveMembersPossiblyOnParent innerToPossible)
                {

                    foreach (var possible in possibleMembers[innerFromPossible])
                    {
                        var newValue = Copy(possible.Value, new Member(this));
                        HasMembersPossiblyOnParent(innerToPossible, possible.Key, newValue);
                    }
                }

                if (innerFrom is IHaveHopefulMembers innerFromHopeful && innerTo is IHaveHopefulMembers innerToHopeful)
                {
                    foreach (var possible in hopefulMembers[innerFromHopeful])
                    {
                        var newValue = Copy(possible.Value, new Member(this));
                        HasHopefulMember(innerToHopeful, possible.Key, newValue);
                    }
                }

                return innerTo;
            }
        }

        private class GenericTypeKey : IEquatable<GenericTypeKey>
        {
            private readonly Tpn.IType primary;
            private readonly Tpn.IType[] parameters;

            public GenericTypeKey(Tpn.IType primary, Tpn.IType[] parameters)
            {
                this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
                this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as GenericTypeKey);
            }

            public bool Equals(GenericTypeKey other)
            {
                return other != null &&
                       EqualityComparer<Tpn.IType>.Default.Equals(primary, other.primary) &&
                       EqualityComparer<Tpn.IType[]>.Default.Equals(parameters, other.parameters);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(primary, parameters);
            }
        }

    }
}
