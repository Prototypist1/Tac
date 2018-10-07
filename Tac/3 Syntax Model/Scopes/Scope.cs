using Prototypist.LeftToRight;
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

    public class Scope : IPopulatableScope, IResolvableScope, 
        IInstanceScope
    {

        public static IStaticScope StaticScope()
        {
            return new Scope();
        }

        public static ILocalStaticScope LocalStaticScope()
        {
            return new Scope();
        }

        public bool TryAddInstanceMember(IKey key, IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Instance, key, definition);
        }

        public bool TryAddLocal(IKey key, IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Local, key, definition);
        }

        public bool TryAddStaticMember(IKey key, IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Static, key, definition);
        }

        public bool TryAddStaticType(IKey key, IBox<ITypeDefinition> definition)
        {
            return TryAddType(DefintionLifetime.Static, key, definition);
        }

        public bool TryAddStaticGenericType(IKey key, IBox<GenericTypeDefinition> definition)
        {
            return TryAddGeneric(DefintionLifetime.Static, key, definition);
        }

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>> members
    = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<MemberDefinition>>>>();

        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<ITypeDefinition>>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<ITypeDefinition>>>>();


        protected readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>> genericTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>>();
        
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

        protected bool TryAddGeneric(DefintionLifetime defintionLifetime, IKey key, IBox<GenericTypeDefinition> definition)
        {
            var list = genericTypes.GetOrAdd(key, new ConcurrentSet<Visiblity<IBox<GenericTypeDefinition>>>());
            var visiblity = new Visiblity<IBox<GenericTypeDefinition>>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }
        
        public bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member)
        {
            if (!members.TryGetValue(name, out var items))
            {
                member = default;
                return false;
            }

            var thing = items.SingleOrDefault();

            if (thing == default)
            {
                member = default;
                return false;
            }

            member = thing.Definition;
            return true;
        }

        public bool TryGetType(IKey name, out IBox<ITypeDefinition> type)
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

        public IResolvableScope ToResolvable()
        {
            return this;
        }
        
        public IReadOnlyList<IBox<ITypeDefinition>> Types
        {
            get
            {
                return types.Select(x => x.Value.Single().Definition).ToArray();
            }
        }
        
        public IReadOnlyList<IBox<MemberDefinition>> Members
        {
            get
            {
                return members.Select(x => x.Value.Single().Definition).ToArray();
            }
        }
    }
}