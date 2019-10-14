using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal interface ITypeProblemNode { }
    internal interface IScope : ITypeProblemNode { }

    internal interface IHaveMembersPossiblyOnParent : IScope { }
    internal interface IHaveHopefulMembers : ILookUpType { }
    internal interface ILookUpType : ITypeProblemNode { }
    internal interface ICanAssignFromMe : ILookUpType, ITypeProblemNode { }
    internal interface ICanBeAssignedTo : ILookUpType, ITypeProblemNode { }

    public class Yo
    {
        internal class TypeReference : ITypeProblemNode, ILookUpType { }
        internal class Value : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe { }
        internal class Member : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe, ICanBeAssignedTo { }
        internal class Type : ITypeProblemNode, IScope { }
        internal class InferedType : Type { }
        internal class Scope : ITypeProblemNode, IHaveMembersPossiblyOnParent, IScope { }
        internal class Object : ITypeProblemNode, IHaveHopefulMembers, IHaveMembersPossiblyOnParent, IScope, ICanAssignFromMe { }
        internal class Method : ITypeProblemNode, IHaveHopefulMembers, IHaveMembersPossiblyOnParent, IScope, ICanAssignFromMe { }

    }
    internal class TypeProblem2 : ISetUpTypeProblem
    {
        // basic stuff
        private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();
        private IScope root;
        // relationships
        private readonly Dictionary<IScope, IScope> kidParent = new Dictionary<IScope, IScope>();

        private readonly Dictionary<IScope, List<Yo.Value>> values = new Dictionary<IScope, List<Yo.Value>>();
        private readonly Dictionary<IScope, List<Yo.TypeReference>> refs = new Dictionary<IScope, List<Yo.TypeReference>>();
        private readonly Dictionary<IScope, Dictionary<IKey, Yo.Type>> types = new Dictionary<IScope, Dictionary<IKey, Yo.Type>>();
        private readonly Dictionary<IScope, Dictionary<IKey, Yo.Member>> members = new Dictionary<IScope, Dictionary<IKey, Yo.Member>>();
        private readonly Dictionary<IScope, Dictionary<IKey, Yo.Type>> genericOverlays = new Dictionary<IScope, Dictionary<IKey, Yo.Type>>();


        private readonly Dictionary<IHaveMembersPossiblyOnParent, Dictionary<IKey, Yo.Member>> possibleMembers = new Dictionary<IHaveMembersPossiblyOnParent, Dictionary<IKey, Yo.Member>>();
        private readonly Dictionary<IHaveHopefulMembers, Dictionary<IKey, Yo.Member>> hopefulMembers = new Dictionary<IHaveHopefulMembers, Dictionary<IKey, Yo.Member>>();
        private readonly List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
        // members
        private readonly Dictionary<ILookUpType, IKey> lookUpTypeKey = new Dictionary<ILookUpType, IKey>();
        private readonly Dictionary<ILookUpType, IScope> lookUpTypeContext = new Dictionary<ILookUpType, IScope>();

        private readonly Dictionary<GenericTypeKey, Yo.Type> realizedGeneric = new Dictionary<GenericTypeKey, Yo.Type>();

        #region Building APIs

        public void IsChildOf(IScope parent, IScope kid)
        {
            kidParent.Add(kid, parent);
        }
        public void HasValue(IScope parent, Yo.Value value)
        {
            if (!values.ContainsKey(parent))
            {
                values.Add(parent, new List<Yo.Value>());
            }
            values[parent].Add(value);
        }
        public void HasReference(IScope parent, Yo.TypeReference reference)
        {
            if (!refs.ContainsKey(parent))
            {
                refs.Add(parent, new List<Yo.TypeReference>());
            }
            refs[parent].Add(reference);
        }
        public void HasType(IScope parent, IKey key, Yo.Type type)
        {
            if (!types.ContainsKey(parent))
            {
                types.Add(parent, new Dictionary<IKey, Yo.Type>());
            }
            types[parent].Add(key, type);
        }

        public void HasPlaceholderType(IScope parent, IKey key, Yo.Type type)
        {
            if (!genericOverlays.ContainsKey(parent))
            {
                genericOverlays.Add(parent, new Dictionary<IKey, Yo.Type>());
            }
            genericOverlays[parent].Add(key, type);
        }
        public void HasMember(IScope parent, IKey key, Yo.Member member)
        {
            if (!members.ContainsKey(parent))
            {
                members.Add(parent, new Dictionary<IKey, Yo.Member>());
            }
            members[parent].Add(key, member);
        }
        public void HasMembersPossiblyOnParent(IHaveMembersPossiblyOnParent parent, IKey key, Yo.Member member)
        {
            if (!possibleMembers.ContainsKey(parent))
            {
                possibleMembers.Add(parent, new Dictionary<IKey, Yo.Member>());
            }
            possibleMembers[parent].Add(key, member);
        }
        public void HasHopefulMember(IHaveHopefulMembers parent, IKey key, Yo.Member member)
        {

            if (!hopefulMembers.ContainsKey(parent))
            {
                hopefulMembers.Add(parent, new Dictionary<IKey, Yo.Member>());
            }
            hopefulMembers[parent].Add(key, member);
        }
        public void AssignType(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo)
        {
            assignments.Add((assignedFrom, assignedTo));
        }

        public T Register<T>(T typeProblemNode)
            where T : ITypeProblemNode
        {
            typeProblemNodes.Add(typeProblemNode);
            return typeProblemNode;
        }

        #endregion

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
                var type = Register(new Yo.InferedType());
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
            var defersTo = new Dictionary<Yo.Type, Yo.Type>();

            foreach (var item in possibleMembers)
            {
                foreach (var pair in item.Value)
                {
                    if (TryGetMember(item.Key, pair.Key, out var member))
                    {
                        // TODO is there more to defering ??
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

            Yo.Type GetType(ILookUpType value)
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

            void Flow(Yo.Type from, Yo.Type to)
            {
                // I think the only thing that "flow" are members
                // but not all types will accept new members
                if (to is Yo.InferedType) {
                    foreach (var memberPair in members[from])
                    {
                        if (members[to].TryGetValue(memberPair.Key, out var upstreamMember))
                        {
                            var one = GetType(upstreamMember);
                            var two = GetType(memberPair.Value);
                            if (one is Yo.InferedType oneInfered)
                            {
                                Flow(two, oneInfered);
                            }
                            else if (two is Yo.InferedType twoInfered)
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

        private bool TryGetMember(IScope context, IKey key, out Yo.Member member)
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

        private Yo.Type LookUpOrOverlayOrThrow(IScope from, ILookUpType lookUp)
        {
            if (!TryLookUpOrOverlay(from, lookUp, out var res))
            {
                throw new Exception("could not find type");
            }
            return res;
        }

        private Yo.Type LookUpOrOverlayOrThrow(IScope from, IKey key)
        {
            if (!TryLookUpOrOverlay(from, key, out var res))
            {
                throw new Exception("could not find type");
            }
            return res;
        }

        // or maybe I just need to make we get the same outcome requardless of what order references are processed in'
        private Dictionary<ILookUpType, Yo.Type> lookUps = new Dictionary<ILookUpType, Yo.Type>();
        private bool TryLookUpOrOverlay(IScope from, ILookUpType lookUp, out Yo.Type res)
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

        private bool TryLookUpOrOverlay(IScope from, IKey key, out Yo.Type res)
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

                var to = Register(new Yo.Type());
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

        private IScope DefinedOn(IScope haveTypes, IKey key)
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

        private bool TryLookUp(IScope haveTypes, IKey key, out Yo.Type result)
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

        private Yo.Type CopyTree(Yo.Type from, Yo.Type to)
        {

            var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();
            Copy(from, to);

            foreach (var pair in map)
            {
                if (pair.Key is IScope fromScope && to is IScope toScope)
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

                if (innerFrom is IScope innerFromScope && innerTo is IScope innerFromTo)
                {

                    foreach (var item in values[innerFromScope])
                    {
                        var newValue = Copy(item, Register(new Yo.Value()));
                        HasValue(innerFromTo, newValue);
                    }

                    foreach (var item in refs[innerFromScope])
                    {
                        var newValue = Copy(item, Register(new Yo.TypeReference()));
                        HasReference(innerFromTo, newValue);
                    }

                    foreach (var member in members[innerFromScope])
                    {
                        var newValue = Copy(member.Value, Register(new Yo.Member()));
                        HasMember(innerFromTo, member.Key, newValue);
                    }

                    foreach (var type in types[innerFromScope])
                    {
                        var newValue = Copy(type.Value, Register(new Yo.Type()));
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
                        var newValue = Copy(possible.Value, Register(new Yo.Member()));
                        HasMembersPossiblyOnParent(innerToPossible, possible.Key, newValue);
                    }
                }

                if (innerFrom is IHaveHopefulMembers innerFromHopeful && innerTo is IHaveHopefulMembers innerToHopeful)
                {
                    foreach (var possible in hopefulMembers[innerFromHopeful])
                    {
                        var newValue = Copy(possible.Value, Register(new Yo.Member()));
                        HasHopefulMember(innerToHopeful, possible.Key, newValue);
                    }
                }

                return innerTo;
            }
        }

        private class GenericTypeKey : IEquatable<GenericTypeKey>
        {
            private readonly Yo.Type primary;
            private readonly Yo.Type[] parameters;

            public GenericTypeKey(Yo.Type primary, Yo.Type[] parameters)
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
                       EqualityComparer<Yo.Type>.Default.Equals(primary, other.primary) &&
                       EqualityComparer<Yo.Type[]>.Default.Equals(parameters, other.parameters);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(primary, parameters);
            }
        }

    }
}
