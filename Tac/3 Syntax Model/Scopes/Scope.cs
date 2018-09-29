using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    //why does this live here?
    // where is the right place for it to live??
    // this is really owned by what scope it is on right?
    public enum DefintionLifetime
    {
        Static,
        Instance,
        Local,
    }

    public class Visiblity<TReferanced> where TReferanced : class
    {
        public Visiblity(DefintionLifetime defintionLifeTime, TReferanced definition)
        {
            DefintionLifeTime = defintionLifeTime;
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public DefintionLifetime DefintionLifeTime { get; }
        public TReferanced Definition { get; }
    }

    public class Scope : IScope
    {
        protected bool TryAddMember(DefintionLifetime defintionLifetime, IKey key, IBox<MemberDefinition> definition)
        {
            var list = members.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<MemberDefinition>>>());
            var visiblity = new Visiblity<IBox<MemberDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        protected bool TryAddType(DefintionLifetime defintionLifetime, IKey key, IBox<ITypeDefinition> definition)
        {
            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<ITypeDefinition>>>());
            var visiblity = new Visiblity<IBox<ITypeDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }
        
        protected bool TryAddGeneric(DefintionLifetime defintionLifetime,IKey key, IBox<GenericTypeDefinition> definition)
        {
            var list = genericTypes.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>());
            var visiblity = new Visiblity<IBox<GenericTypeDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>> members
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>>();

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<ITypeDefinition>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<ITypeDefinition>>>>();


        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>> genericTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>>();
        
        private class RealizedGenericKey
        {
            public RealizedGenericKey(IEnumerable<IBox<ITypeDefinition>> boxes, IKey key)
            {
                Boxes = boxes ?? throw new ArgumentNullException(nameof(boxes));
                Key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IEnumerable<IBox<ITypeDefinition>> Boxes { get; }
            public IKey Key { get; }

            public override bool Equals(object obj)
            {
                var key = obj as RealizedGenericKey;
                return key != null &&
                       Boxes.SequenceEqual(key.Boxes) &&
                       EqualityComparer<IKey>.Default.Equals(Key, key.Key);
            }

            public override int GetHashCode()
            {
                var hashCode = -1506854802;
                hashCode = (hashCode * -1521134295) + Boxes.Sum(x=>x.GetHashCode());
                hashCode = (hashCode * -1521134295) + EqualityComparer<IKey>.Default.GetHashCode(Key);
                return hashCode;
            }
        }
        
        private readonly ConcurrentDictionary<RealizedGenericKey, Visiblity<IBox<ITypeDefinition>>> realizedGenericTypes = new ConcurrentDictionary<RealizedGenericKey, Visiblity<IBox<ITypeDefinition>>>();

        public IReadOnlyList<IBox<MemberDefinition>> Members
        {
            get
            {
                return members.Select(x => x.Value.Single().Definition).ToArray();
            }
        }

        public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member) {
            if (!members.TryGetValue(name, out var items)) {
                member = default;
                return false;
            }

            var thing = items.SingleOrDefault();

            if (thing == default) {
                member = default;
                return false;
            }

            member = thing.Definition;
            return true;
        }

        public bool TryGetGenericType(NameKey name, IEnumerable<IBox<ITypeDefinition>> genericTypeParameters, out IBox<ITypeDefinition> typeDefinition)
        {
            var key = new RealizedGenericKey(genericTypeParameters, name);

            if (!genericTypes.TryGetValue(name, out var items))
            {
                typeDefinition = default;
                return false;
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                typeDefinition = default;
                return false;
            }

            var concrete = new GenericBox(thing.Definition,genericTypeParameters); 

            var fallback = new Visiblity<IBox<ITypeDefinition>>(DefintionLifetime.Static, concrete);
            typeDefinition = realizedGenericTypes.GetOrAdd(key, fallback).Definition;
            return true;
        }

        public bool TryGetType(NameKey name, out IBox<ITypeDefinition> type)
        {
            if (!types.TryGetValue(name, out var items))
            {
                type = default;
                return false;
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                type = default;
                return false;
            }

            type = thing.Definition;
            return true;
        }
    }

}