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
            var list = members.GetOrAdd(definition.Key, new ConcurrentSet<Visiblity<MemberDefinition>>());
            var visiblity = new Visiblity<MemberDefinition>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        protected bool TryAdd(DefintionLifetime defintionLifetime, TypeDefinition definition)
        {
            var list = types.GetOrAdd(definition.Key, new ConcurrentSet<Visiblity<TypeDefinition>>());
            var visiblity = new Visiblity<TypeDefinition>(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }


        private readonly ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity<MemberDefinition>>> members
            = new ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity<MemberDefinition>>>();

        private readonly ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity<TypeDefinition>>> types
            = new ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity<TypeDefinition>>>();

        private readonly ConcurrentDictionary<ExplicitName, ConcurrentSet<Visiblity>> genericTypes = new ConcurrentDictionary<ExplicitName, ConcurrentSet<Visiblity>>();

    
        public override bool Equals(object obj)
        {
            return obj is Scope scope &&
                   referanced.Count == scope.referanced.Count && !referanced.Except(scope.referanced).Any();
        }

        public override int GetHashCode()
        {
            var hashCode = -1365954579;
            hashCode = hashCode * -1521134295 + referanced.Sum(x=>x.GetHashCode());
            return hashCode;
        }

        public bool TryGetMember(AbstractName name, bool staticOnly, out MemberDefinition member) {
            if (referanced.TryGetValue(name, out var items)) {
                var thing = items.Single();
                if (thing.DefintionLifeTime == DefintionLifetime.Static && thing.Definition is MemberDefinition memberDefinition)
                {
                    member = memberDefinition;
                    return true;
                }
                else
                {
                    throw new Exception($"{thing.Definition} should be a {typeof(IScoped<IScope>)} instead it is {thing.Definition.GetType()}");
                }
            }
            member = default;
            return false;
        }

        public bool TryGetType(AbstractName name, out TypeDefinition type)
        {
            if (name is GenericExplicitName genericExplicitName && genericReferanced.TryGetValue(genericExplicitName, out var genericItems)) {
                
            }
            else if (referanced.TryGetValue(name, out var items))
            {
                var thing = items.Single();
                if (thing.Definition is TypeDefinition typeDefinition)
                {
                    type = typeDefinition;
                    return true;
                }
                else
                {
                    throw new Exception($"{thing.Definition} should be a {typeof(IScoped<IScope>)} instead it is {thing.Definition.GetType()}");
                }
            }
            type = default;
            return false;
        }

        public bool TryGet(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition<IScope>> item)
        {
            item = default;
            return false;
        }
    }

}