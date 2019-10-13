using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal interface ITypeProblemNode { }
    internal interface IScope : ITypeProblemNode { }

    internal interface IHaveMembersPossiblyOnParent : ITypeProblemNode { }
    internal interface IHaveHopefulMembers : ITypeProblemNode { }
    internal interface ILookUpType : ITypeProblemNode { }
    internal interface ICanAssignFromMe : ITypeProblemNode { }
    internal interface ICanBeAssignedTo : ITypeProblemNode { }

    public class Yo
    {
        internal class TypeReference : ITypeProblemNode, ILookUpType { }
        internal class Value : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe { }
        internal class Member : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe, ICanBeAssignedTo { }
        internal class Type : ITypeProblemNode, IScope { }
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
            // overlay generics
            var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
            while (toLookUp.Any())
            {
                foreach (var node in toLookUp)
                {
                    LookUpOrOverlayOrThrow(lookUpTypeContext[node], lookUpTypeKey[node]);
                }
                toLookUp = typeProblemNodes.OfType<ILookUpType>().Except(lookUps.Keys).ToArray();
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

                lookUps[lookUp] = res;
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

        #region Solve Side

        private class TypeSolution : ITypeSolution { }

        private class OverlayIntention
        {
            public readonly SolveSideNode copyFrom;
            public readonly SolveSideNode copyTo;
            public readonly Dictionary<SolveSideNode, SolveSideNode> map;

            public OverlayIntention(SolveSideNode copyFrom, SolveSideNode copyTo, Dictionary<SolveSideNode, SolveSideNode> map)
            {
                this.copyFrom = copyFrom ?? throw new ArgumentNullException(nameof(copyFrom));
                this.copyTo = copyTo ?? throw new ArgumentNullException(nameof(copyTo));
                this.map = map ?? throw new ArgumentNullException(nameof(map));
            }

        }

        public ITypeSolution Solve()
        {
            var cache = new Dictionary<ISetUpSideNode, SolveSideNode>();

            // now the impossible part...

            // generate scope tree
            GenerateScopeTree();

            // convert types
            ConvertTypes();

            var toOverlay = new List<OverlayIntention>();

            // then members
            ConvertMembersAndValuesAndTypeRefs();

            // then members that might be on parents
            ConvertMembersThatMightBeOnParents();

            // then hopeful members
            ConvertHopefulMembers();

            // add the assignments 
            foreach (var (from, to) in assignments)
            {
                cache[from].AddAssignedTo(cache[to]);
                cache[to].AddAssignedFrom(cache[from]);
            }

            // do generic overlaying
            OverlayGenerics();
            // note that this makes the list of ITypeProblemNode incomplete 

            // figure out what types members are
            ResolveTypes();

            // realize hopeful members
            foreach (var item in cache.Values)
            {
                item.RealizeHopefulMembers();
            }

            // flow generic relation ships A<T> -> A<int>
            foreach (var overlay in toOverlay)
            {
                foreach (var (key, value) in overlay.copyFrom.Members)
                {
                    overlay.copyTo.FlowUpStream(key, value);
                }
            }

            // flow members upstream and merge them
            foreach (var node in cache.Values)
            {
                foreach (var (key, value) in node.Members)
                {
                    foreach (var assignedFromNode in node.AssignedFrom)
                    {
                        assignedFromNode.FlowUpStream(key, value);
                    }
                }
            }

            // flow members downstream but not pass confluences
            foreach (var node in cache.Values)
            {
                foreach (var (key, value) in node.Members)
                {
                    foreach (var assignedFromNode in node.AssignedTo)
                    {
                        assignedFromNode.FlowDownStream(key, value);
                    }
                }
            }

            return new TypeSolution();

            void GenerateScopeTree()
            {
                foreach (var scope in scopes)
                {
                    ConvertScope(scope);
                }

                foreach (var @object in objects)
                {
                    ConvertScope(@object);
                }


                foreach (var method in methods)
                {
                    ConvertScope(method);
                }

                foreach (var type in types)
                {
                    ConvertScope(type);
                }

                SolveSideNode ConvertScope(IDefineMembers scope)
                {
                    if (scope == null)
                    {
                        throw new ArgumentNullException(nameof(scope));
                    }

                    {
                        if (cache.TryGetValue(scope, out var res))
                        {
                            return res;
                        }
                    }

                    if (scope.ParentOrNull == null)
                    {
                        var res = new SolveSideNode(true, null, null);
                        cache.Add(scope, res);
                        return res;

                    }
                    else
                    {
                        var parent = ConvertScope(scope.ParentOrNull);
                        var res = new SolveSideNode(true, parent, null);
                        cache.Add(scope, res);
                        return res;
                    }

                }

            }

            SolveSideNode GetParent(IDefineMembers defineMembers)
            {
                if (defineMembers.ParentOrNull == null)
                {
                    return null;
                }
                return cache[defineMembers.ParentOrNull];
            }

            void ConvertTypes()
            {
                foreach (var type in types)
                {
                    ConvertType(type);
                }

                SolveSideNode ConvertType(Type type)
                {
                    {
                        if (cache.TryGetValue(type, out var res))
                        {
                            return res;
                        }
                    }
                    {
                        var res = new SolveSideNode(true, GetParent(type), null);

                        if (type.Key != null)
                        {
                            cache[type.ParentOrNull].AddType(type.Key, res);
                        }

                        foreach (var placeholder in type.placeholders)
                        {
                            res.placeholderTypes.Add(ConvertType(placeholder));
                        }

                        cache.Add(type, res);
                        return res;
                    }
                }
            }


            void ConvertMembersAndValuesAndTypeRefs()
            {
                foreach (var scope in scopes)
                {
                    foreach (var member in scope.memberTracker.members)
                    {
                        ConvertMember(scope, member);
                    }
                }

                foreach (var @object in objects)
                {
                    foreach (var member in @object.memberTracker.members)
                    {
                        ConvertMember(@object, member);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var member in method.memberTracker.members)
                    {
                        ConvertMember(method, member);
                    }
                }

                foreach (var type in types)
                {
                    foreach (var member in type.memberTracker.members)
                    {
                        ConvertMember(type, member);
                    }
                }


                foreach (var typeReference in typeReferences)
                {
                    ConvertTypeReference(typeReference);
                }

                foreach (var value in values)
                {
                    ConvertValue(value);
                }


                SolveSideNode ConvertMember(IDefineMembers owner, Member member)
                {
                    {
                        if (cache.TryGetValue(member, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, member.typeKey);
                        cache.Add(member, res);

                        if (member.typeKey is GenericNameKey genericNameKey)
                        {
                            HandleGenericNameKey(owner, genericNameKey);
                        }

                        cache[owner].AddMember(member.Key, res);

                        return res;
                    }


                }

                SolveSideNode ConvertValue(Value value)
                {
                    {
                        if (cache.TryGetValue(value, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, value.Type.Key);
                        cache.Add(value, res);

                        if (value.Type.Key is GenericNameKey genericNameKey)
                        {
                            HandleGenericNameKey(value.Type.Context, genericNameKey);
                        }

                        return res;
                    }
                }


                SolveSideNode ConvertTypeReference(TypeReference typeRef)
                {
                    {
                        if (cache.TryGetValue(typeRef, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, typeRef.Key);
                        cache.Add(typeRef, res);

                        if (typeRef.Key is GenericNameKey genericNameKey)
                        {
                            HandleGenericNameKey(typeRef.Context, genericNameKey);
                        }

                        return res;
                    }
                }

                SolveSideNode HandleGenericNameKey(IDefineMembers owner, GenericNameKey genericNameKey)
                {
                    if (cache[owner].TryGetType(new NameKey(genericNameKey.Name), out var copyFrom))
                    {
                        var list = new List<SolveSideNode>();
                        foreach (var inner in genericNameKey.Types)
                        {
                            if (inner is GenericNameKey innerGernericNameKey)
                            {
                                list.Add(HandleGenericNameKey(owner, innerGernericNameKey));
                            }
                            else if (cache[owner].TryGetType(new NameKey(genericNameKey.Name), out var innerNode))
                            {
                                list.Add(innerNode);
                            }
                            else
                            {
                                throw new Exception("uuhhh, we could not find the type..");
                            }
                        }
                        var copyTo = new SolveSideNode(true, copyFrom.parentOrNull, null);
                        var map = new Dictionary<SolveSideNode, SolveSideNode>();
                        foreach (var (to, from) in list.Zip(copyFrom.placeholderTypes, (x, y) => (x, y)))
                        {
                            map[from] = to;
                        }

                        toOverlay.Add(new OverlayIntention(copyFrom, copyTo, map));
                        return copyTo;
                    }
                    else
                    {
                        throw new Exception("uuhhh, we could not find the type..");
                    }
                }
            }

            void OverlayGenerics()
            {
                foreach (var overlay in toOverlay)
                {
                    // TODO ahhh! do I  need to overlay values and type reference as well?
                    var mapped = new Dictionary<SolveSideNode, SolveSideNode>();
                    overlay.copyFrom.OverlayTo(overlay.copyTo, overlay.map, mapped);
                    overlay.copyFrom.OverlayToRelationshipsTo(overlay.copyTo, mapped);
                }
            }

            void ResolveTypes()
            {

                foreach (var node in cache.Values)
                {
                    foreach (var member in node.Members.Select(x => x.Item2))
                    {
                        Resolve(node, member);
                    }
                }

                foreach (var key in cache.Keys)
                {
                    if (key is Value value)
                    {
                        Resolve(cache[value.Type.Context], cache[value]);
                    }
                    if (key is TypeReference typeReference)
                    {
                        Resolve(cache[typeReference.Context], cache[typeReference]);
                    }
                }

                void Resolve(SolveSideNode owner, SolveSideNode member)
                {
                    if (member.typeKeyOrNull != null)
                    {
                        if (owner.TryGetType(member.typeKeyOrNull, out var node))
                        {
                            member.DefersTo(node);
                        }
                        else
                        {
                            throw new Exception("uuhhh, we could not find the type..");
                        }
                    }
                }

            }

            void ConvertMembersThatMightBeOnParents()
            {

                foreach (var scope in scopes)
                {
                    foreach (var member in scope.mightBeOnParentMemberTracker.members)
                    {
                        ConvertMemberThatMightBeOnParents(scope, member);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var member in method.mightBeOnParentMemberTracker.members)
                    {
                        ConvertMemberThatMightBeOnParents(method, member);
                    }
                }


                SolveSideNode ConvertMemberThatMightBeOnParents(IDefineMembers owner, Member member)
                {
                    {
                        if (cache.TryGetValue(member, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, null);
                        cache.Add(member, res);

                        if (!(cache[owner].TryGetMember(member.Key, out SolveSideNode node)))
                        {
                            res.DefersTo(node);
                        }
                        else
                        {
                            // add it to owner
                            cache[owner].AddMember(member.Key, res);
                        }

                        return res;
                    }
                }
            }

            void ConvertHopefulMembers()
            {
                foreach (var value in values)
                {
                    foreach (var hopefullMember in value.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(value, hopefullMember);
                    }
                }


                foreach (var member in members)
                {
                    foreach (var hopefullMember in member.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(member, hopefullMember);
                    }
                }


                foreach (var @object in objects)
                {
                    foreach (var hopefullMember in @object.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(@object, hopefullMember);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var hopefullMember in method.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(method, hopefullMember);
                    }
                }

                SolveSideNode ConvertHopefulMember(ISetUpSideNode node, Member hopefullMember)
                {
                    {
                        if (cache.TryGetValue(hopefullMember, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, null);
                        cache.Add(hopefullMember, res);

                        cache[node].AddHopefulMember(hopefullMember.Key, res);

                        return res;
                    }
                }
            }
        }


        private class SolveSideNode
        {
            private SolveSideNode inner;
            internal IKey typeKeyOrNull;

            // placeholderTypes also appear in types
            public readonly List<SolveSideNode> placeholderTypes = new List<SolveSideNode>();
            public readonly Dictionary<IKey, SolveSideNode> types = new Dictionary<IKey, SolveSideNode>();
            private readonly Dictionary<IKey, SolveSideNode> members = new Dictionary<IKey, SolveSideNode>();
            private readonly List<(IKey, SolveSideNode)> hopefulMembers = new List<(IKey, SolveSideNode)>();
            private readonly List<SolveSideNode> assignTos = new List<SolveSideNode>();
            private readonly List<SolveSideNode> assignFroms = new List<SolveSideNode>();

            public readonly SolveSideNode parentOrNull;
            private readonly bool explictMembersOnly;


            public IEnumerable<(IKey, SolveSideNode)> Members => members.Select(x => (x.Key, x.Value));

            public IEnumerable<SolveSideNode> AssignedFrom
            {
                get
                {
                    if (inner == null)
                    {
                        return assignFroms;
                    }
                    else
                    {
                        return inner.AssignedFrom;
                    }
                }
            }
            public IEnumerable<SolveSideNode> AssignedTo
            {
                get
                {

                    if (inner == null)
                    {
                        return assignTos;
                    }
                    else
                    {
                        return inner.AssignedTo;
                    }

                }
            }

            internal void AddType(IKey key, SolveSideNode res)
            {
                types.Add(key, res);
            }

            internal bool TryGetType(IKey typeKey, out SolveSideNode node)
            {
                if (types.TryGetValue(typeKey, out node))
                {
                    return true;
                }
                if (parentOrNull == null)
                {
                    return false;
                }
                return parentOrNull.TryGetType(typeKey, out node);
            }

            internal bool TryGetMember(IKey key, out SolveSideNode resNode)
            {
                if (TryGetValue(key, out resNode))
                {
                    return true;
                }
                if (parentOrNull == null)
                {
                    return false;
                }
                return parentOrNull.TryGetMember(key, out resNode);
            }

            public SolveSideNode(bool explictMembersOnly, SolveSideNode parent, IKey typeKeyOrNull)
            {
                this.typeKeyOrNull = typeKeyOrNull;
                this.explictMembersOnly = explictMembersOnly;
                this.parentOrNull = parent;
            }

            public bool TryGetValue(IKey key, out SolveSideNode value)
            {
                if (inner == null)
                {
                    return members.TryGetValue(key, out value);
                }
                else
                {
                    return inner.TryGetValue(key, out value);
                }
            }

            // what does this do if the member is already there?
            // probably merge?
            internal void AddMember(IKey key, SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    if (members.TryGetValue(key, out var current))
                    {
                        solveSideNode.DefersTo(current);
                    }
                    else
                    {
                        members.Add(key, solveSideNode);
                    }
                }
                else
                {
                    inner.AddMember(key, solveSideNode);
                }
            }

            internal void DefersTo(SolveSideNode node)
            {
                while (node.inner != null)
                {
                    node = node.inner;
                }

                if (inner != null)
                {
                    inner.DefersTo(node);
                    return;
                }

                if (this == node)
                {
                    return;
                }

                // add all the members
                foreach (var member in node.members)
                {
                    AddMergeMember(member.Key, member.Value);
                }
                // do we need to clean house?
                node.members.Clear();


                // add all the assignFroms
                foreach (var assignFrom in node.assignFroms)
                {
                    assignFroms.Add(assignFrom);
                }
                // do we need to clean house?
                node.assignFroms.Clear();

                // add all the assignTos
                foreach (var assignTo in node.assignTos)
                {
                    assignTos.Add(assignTo);
                }
                // do we need to clean house?
                node.assignTos.Clear();
            }

            internal void AddHopefulMember(IKey key, SolveSideNode solveSideNode)
            {
                hopefulMembers.Add((key, solveSideNode));
            }

            internal void RealizeHopefulMembers()
            {

                foreach (var (key, solveSideNode) in hopefulMembers)
                {
                    if (inner == null)
                    {
                        if (members.TryGetValue(key, out var current))
                        {
                            solveSideNode.DefersTo(current);
                        }
                        else
                        {
                            if (explictMembersOnly)
                            {
                                throw new Exception("this does not accept hopeful members");
                            }
                            members.Add(key, solveSideNode);
                        }
                    }
                    else
                    {
                        inner.AddHopefulMember(key, solveSideNode);
                    }
                }
            }

            internal void AddMergeMember(IKey key, SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    if (members.TryGetValue(key, out var current))
                    {
                        solveSideNode.DefersTo(current);
                    }
                    else
                    {
                        if (explictMembersOnly)
                        {
                            throw new Exception("this does not accept hopeful members");
                        }
                        members.Add(key, solveSideNode);
                    }
                }
                else
                {
                    inner.AddMergeMember(key, solveSideNode);
                }
            }

            internal void AddAssignedTo(SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    assignTos.Add(solveSideNode);
                }
                else
                {
                    inner.AddAssignedTo(solveSideNode);
                }
            }

            internal void AddAssignedFrom(SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    assignFroms.Add(solveSideNode);
                }
                else
                {
                    inner.AddAssignedFrom(solveSideNode);
                }
            }

            internal void FlowUpStream(IKey key, SolveSideNode value)
            {

                if (members.TryGetValue(key, out var current))
                {
                    value.DefersTo(current);
                    return;
                }

                if (explictMembersOnly)
                {
                    return;
                }

                members.Add(key, value);
                foreach (var node in AssignedFrom)
                {
                    node.FlowUpStream(key, value);
                }

            }

            internal void FlowDownStream(IKey key, SolveSideNode value)
            {


                // don't flow in to nodes that are assigned from more than one place 
                if (AssignedFrom.Count() > 1)
                {
                    return;
                }

                if (members.TryGetValue(key, out var current))
                {
                    value.DefersTo(current);
                    return;
                }

                if (explictMembersOnly)
                {
                    return;
                }

                members.Add(key, value);
                foreach (var node in AssignedFrom)
                {
                    node.FlowUpStream(key, value);
                }
            }

            internal SolveSideNode OverlayTo(SolveSideNode copyTo, Dictionary<SolveSideNode, SolveSideNode> map, Dictionary<SolveSideNode, SolveSideNode> alreadyMapped)
            {
                if (inner != null)
                {
                    throw new Exception("at the time when this is called inner should be null");
                    // well inner would not be null on a "member possibly on parent" when it is on the parent
                    // but that should not be reached by this method 
                }

                if (alreadyMapped.TryGetValue(this, out var res))
                {
                    return res;
                }

                alreadyMapped[this] = copyTo;

                foreach (var typePair in types)
                {
                    if (map.TryGetValue(typePair.Value, out var newValue))
                    {
                        copyTo.AddType(typePair.Key, newValue);
                    }
                    else
                    {
                        copyTo.AddType(typePair.Key, OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), typePair.Value.typeKeyOrNull), map, alreadyMapped));
                    }
                }

                foreach (var placeholder in placeholderTypes)
                {
                    copyTo.placeholderTypes.Add(map[placeholder]);
                }

                foreach (var hopefulMember in hopefulMembers)
                {
                    if (map.TryGetValue(hopefulMember.Item2, out var newValue))
                    {
                        copyTo.AddHopefulMember(hopefulMember.Item1, newValue);
                    }
                    else
                    {
                        copyTo.AddHopefulMember(hopefulMember.Item1, OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), hopefulMember.Item2.typeKeyOrNull), map, alreadyMapped));
                    }
                }


                foreach (var memberPair in members)
                {
                    if (map.TryGetValue(memberPair.Value, out var newValue))
                    {
                        copyTo.AddMember(memberPair.Key, newValue);
                    }
                    else
                    {
                        copyTo.AddMember(memberPair.Key, memberPair.Value.OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), memberPair.Value.typeKeyOrNull), map, alreadyMapped));
                    }
                }

                return copyTo;

                SolveSideNode OverlayParent(SolveSideNode parentOrNull)
                {
                    if (parentOrNull == null)
                    {
                        return null;
                    }
                    if (alreadyMapped.TryGetValue(parentOrNull, out var parent))
                    {
                        return parent;
                    }

                    return parentOrNull;
                }
            }

            internal void OverlayToRelationshipsTo(SolveSideNode copyTo, Dictionary<SolveSideNode, SolveSideNode> alreadyMapped)
            {
                foreach (var assignTo in assignTos)
                {
                    if (alreadyMapped.TryGetValue(assignTo, out var resplace))
                    {
                        copyTo.AddAssignedTo(resplace);
                    }
                    else
                    {
                        copyTo.AddAssignedTo(assignTo);
                    }
                }

                foreach (var assignFrom in assignFroms)
                {
                    if (alreadyMapped.TryGetValue(assignFrom, out var resplace))
                    {
                        copyTo.AddAssignedFrom(resplace);
                    }
                    else
                    {
                        copyTo.AddAssignedFrom(assignFrom);
                    }
                }
            }
        }

        #endregion
    }

}
