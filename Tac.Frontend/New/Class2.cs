using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Semantic_Model;

namespace Tac.Frontend.New
{


    internal class ThingWebBuilder
    {

        private class AssignedToCall
        {
            public readonly IThingWebEntry typeWebEntry1;
            public readonly IThingWebEntry typeWebEntry2;

            public AssignedToCall(IThingWebEntry typeWebEntry1, IThingWebEntry typeWebEntry2)
            {
                this.typeWebEntry1 = typeWebEntry1 ?? throw new ArgumentNullException(nameof(typeWebEntry1));
                this.typeWebEntry2 = typeWebEntry2 ?? throw new ArgumentNullException(nameof(typeWebEntry2));
            }
        }
        private readonly List<AssignedToCall> assignedToCalls = new List<AssignedToCall>();
        private void IsAssignedTo(IThingWebEntry typeWebEntry1, IThingWebEntry typeWebEntry2)
        {
            assignedToCalls.Add(new AssignedToCall(typeWebEntry1, typeWebEntry2));
        }

        private readonly List<UnderConstructionType> entries = new List<UnderConstructionType>();
        public IThingWebEntry Entry(ThingWebEntry parent)
        {
            var res = new ThingWebEntry(this, Possibly.Is(parent));
            entries.Add(new UnderConstructionType(res));
            return res;
        }

        private class UnderConstructionType {
            public readonly ThingWebEntry thing;
            public readonly Dictionary<IKey, UnderConstructionType> members = new Dictionary<IKey, UnderConstructionType>();
            public readonly Dictionary<IKey, UnderConstructionType> types = new Dictionary<IKey, UnderConstructionType>();

            public UnderConstructionType(ThingWebEntry thing)
            {
                this.thing = thing ?? throw new ArgumentNullException(nameof(thing));
            }
        }

        private UnderConstructionType Lookup(IThingWebEntry thing) {
            foreach (var item in entries)
            {
                if (item.thing == thing) {
                    return item;
                }
            }
            throw new Exception("Not Found");
        }

        void Solve()
        {
            // we add types
            foreach (var entry in entries)
            {
                foreach (var typeCall in entry.thing.typeCalls)
                {
                    if (!entry.types.TryAdd(typeCall.key, Lookup(typeCall.type))) {
                        throw new NotImplementedException("two types with the same name. sad");
                    }
                }
            }

            // we add members with type look ups and members with included type
            foreach (var entry in entries)
            {
                foreach (var memberCall in entry.thing.memberCalls)
                {
                    if (!entry.members.TryAdd(memberCall.key, Lookup(memberCall.typeWebEntry)))
                    {
                        throw new NotImplementedException("two members with the same name. sad");

                    }
                }
            }

            foreach (var entry in entries)
            {
                foreach (var keyedMemberCall in entry.thing.keyedMemberCalls)
                {
                    if (!entry.members.TryGetValue(keyedMemberCall.typeKey, out var underConstructionType))
                    {
                        throw new NotImplementedException("two members with the same name. sad");
                    }

                    if (!entry.members.TryAdd(keyedMemberCall.key, underConstructionType))
                    {
                        throw new NotImplementedException("two members with the same name. sad");
                    }
                }
            }

            // than members that might be on the enclosing scope 
            foreach (var entry in entries)
            {
                foreach (var memberPossiblyInEnclosingScopeCall in entry.thing.memberPossiblyInEnclosingScopeCalls)
                {
                    var at = entry;
                    while (true)
                    {
                        if (entry.members.TryGetValue(memberPossiblyInEnclosingScopeCall.key, out var underConstructionType))
                        {
                            IsAssignedTo(at.thing, memberPossiblyInEnclosingScopeCall.typeWebEntry);
                            continue;
                        }
                        if (at.thing.enclosingScope.IsDefinately<ThingWebEntry>(out var nextAt, out var _))
                        {
                            at = Lookup(nextAt.Value);
                        }
                        else
                        {
                            // we can not add to any thing
                            // there are types of thing
                            // we can add to scopes
                            // but not to objects
                            // I am not sure I need to enforce that here, now
                            entry.members.TryAdd(memberPossiblyInEnclosingScopeCall.key, Lookup(memberPossiblyInEnclosingScopeCall.typeWebEntry));
                        }
                    }
                }
            }

            // hopefull members  

            // merge
            // should naturally form groups
            // hopefully everything in the group will merge
            // 
            // really it is a tree 




        }

        private class ThingWebEntry : IThingWebEntry
        {
            public readonly ThingWebBuilder parent;

            private IThingWebEntry returns;// 
            public readonly IIsPossibly<ThingWebEntry> enclosingScope;

            public ThingWebEntry(ThingWebBuilder parent, IIsPossibly<ThingWebEntry> enclosingScope)
            {
                this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
                this.enclosingScope = enclosingScope ?? throw new ArgumentNullException(nameof(enclosingScope));
            }


            private readonly List<IKey> types = new List<IKey>();
            public void IsType(IKey type)
            {
                types.Add(type);
            }

            public void IsAssignedTo(IThingWebEntry typeWebEntry)
            {
                parent.IsAssignedTo(this, typeWebEntry);
            }

            public class TypeCall
            {
                public readonly IKey key;
                public readonly IThingWebEntry type;

                public TypeCall(IKey key, IThingWebEntry type)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                    this.type = type ?? throw new ArgumentNullException(nameof(type));
                }
            }
            public readonly List<TypeCall> typeCalls = new List<TypeCall>();
            public void Type(IKey key, IThingWebEntry type)
            {
                typeCalls.Add(new TypeCall(key, type));
            }


            public class KeyedMemberCall
            {
                public readonly IKey key;
                public readonly IKey typeKey;

                public KeyedMemberCall(IKey key, IKey typeKey)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                    this.typeKey = typeKey ?? throw new ArgumentNullException(nameof(typeKey));
                }
            }
            public readonly List<KeyedMemberCall> keyedMemberCalls = new List<KeyedMemberCall>();
            public void Member(IKey key, IKey typeKey)
            {
                keyedMemberCalls.Add(new KeyedMemberCall(key, typeKey));
            }


            public class MemberCall
            {
                public readonly IKey key;
                public readonly IThingWebEntry typeWebEntry;

                public MemberCall(IKey key, IThingWebEntry typeWebEntry)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                    this.typeWebEntry = typeWebEntry ?? throw new ArgumentNullException(nameof(typeWebEntry));
                }
            }
            public readonly List<MemberCall> memberCalls = new List<MemberCall>();
            public void Member(IKey key, IThingWebEntry typeWebEntry)
            {
                memberCalls.Add(new MemberCall(key, typeWebEntry));
            }

            public class MemberPossiblyInEnclosingScopeCall
            {
                public readonly IKey key;
                public readonly IThingWebEntry typeWebEntry;

                public MemberPossiblyInEnclosingScopeCall(IKey key, IThingWebEntry typeWebEntry)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                    this.typeWebEntry = typeWebEntry ?? throw new ArgumentNullException(nameof(typeWebEntry));
                }
            }
            public readonly List<MemberPossiblyInEnclosingScopeCall> memberPossiblyInEnclosingScopeCalls = new List<MemberPossiblyInEnclosingScopeCall>();
            public void MemberPossiblyInEnclosingScope(IKey key, IThingWebEntry typeWebEntry)
            {
                memberPossiblyInEnclosingScopeCalls.Add(new MemberPossiblyInEnclosingScopeCall(key, typeWebEntry));
            }

            public void SetReturns(IThingWebEntry returnType)
            {

                parent.IsAssignedTo(returnType, GetReturns());
            }


            public IThingWebEntry GetReturns()
            {
                if (returns == null)
                {
                    returns = new ThingWebEntry(parent, Possibly.Is( this));
                }
                return returns;
            }

        }

    }

    internal interface IThingWebEntry
    {
        // int this;
        void IsType(IKey typeKey);
        // type x {a;b;}
        void Type(IKey key, IThingWebEntry type);
        // int x;
        void Member(IKey key, IKey typeKey);
        // type {a;b;} x;
        void Member(IKey key, IThingWebEntry typeWebEntry);
        // x;
        void MemberPossiblyInEnclosingScope(IKey key, IThingWebEntry typeWebEntry);
        // 5 =: (a.x);
        void HopefullyMember(IKey key, IThingWebEntry typeWebEntry);
        // return x;
        void SetReturns(IThingWebEntry returnType);
        // 1 > f =: x
        IThingWebEntry GetReturns();
        // a =: x
        void IsAssignedTo(IThingWebEntry typeWebEntry);

        IThing GetSolved();

        // oh god generics
    }

    internal interface IThing
    {

    }


}
