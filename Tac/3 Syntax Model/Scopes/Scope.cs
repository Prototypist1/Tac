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
        protected bool TryAdd(DefintionLifetime defintionLifetime, MemberDefinition definition)
        {
            var list = members.GetOrAdd(definition.Key.Key, new ConcurrentSet<Visiblity<MemberDefinition>>());
            var visiblity = new Visiblity<MemberDefinition>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        protected bool TryAdd(DefintionLifetime defintionLifetime,IKey key, TypeDefinition definition)
        {
            var list = types.GetOrAdd(key, new ConcurrentSet<Visiblity<TypeDefinition>>());
            var visiblity = new Visiblity<TypeDefinition>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }
        
        protected bool TryAddGeneric(DefintionLifetime defintionLifetime, GenericTypeDefinition definition)
        {
            var list = genericTypes.GetOrAdd(definition.Key.Key, new ConcurrentSet<Visiblity<GenericTypeDefinition>>());
            var visiblity = new Visiblity<GenericTypeDefinition>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<MemberDefinition>>> members
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<MemberDefinition>>>();

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<TypeDefinition>>> types
            = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<TypeDefinition>>>();

        private readonly ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<GenericTypeDefinition>>> genericTypes = new ConcurrentDictionary<IKey, ConcurrentSet<Visiblity<GenericTypeDefinition>>>();
        
        public bool TryGetMember(ExplicitMemberName name, bool staticOnly, out MemberDefinition member) {
            if (!members.TryGetValue(name.Key, out var items)) {
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

        public bool TryGetGenericType(ExplicitTypeName name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition)
        {
            if (!genericTypes.TryGetValue(name.Key, out var genericItems))
            {
                typeDefinition = default;
                return false;
            }

            var thing = genericItems.SingleOrDefault(x => x.Definition.TypeParameterDefinitions.Zip(genericTypeParameters, (a, b) => {
                return a.Accepts(b);
            }).All(a => a));

            if (thing == default)
            {
                typeDefinition = default;
                return false;
            }

            if (!thing.Definition.TryCreateConcrete(thing.Definition.TypeParameterDefinitions.Zip(genericTypeParameters, (a, b) => new GenericTypeParameter(b, a)), out var yay)){

                typeDefinition = default;
                return false;
            }

            typeDefinition = yay;
            return true;
        }

        public bool TryGetType(ExplicitTypeName name, out ITypeDefinition type)
        {
            if (!types.TryGetValue(name.Key, out var items))
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
            return false;
        }

        public bool TryGetType(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition> item)
        {
            item = default;
            return false;
        }
    }

}