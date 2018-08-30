using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class Scope : IScope
    {
        protected bool TryAdd(DefintionLifetime defintionLifetime, IReferanced definition)
        {
            var list = referanced.GetOrAdd(definition.Key, new ConcurrentSet<Visiblity>());
            var visiblity = new Visiblity(defintionLifetime, definition);
            return list.TryAdd(visiblity);
        }

        protected enum DefintionLifetime {
            Static,
            Instance,
            Local,
        }

        private class Visiblity
        {
            public Visiblity(DefintionLifetime defintionLifeTime, IReferanced definition)
            {
                DefintionLifeTime = defintionLifeTime;
                Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            }

            public DefintionLifetime DefintionLifeTime { get; }
            public IReferanced Definition { get; }
        }

        private readonly ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity>> referanced = new ConcurrentDictionary<AbstractName, ConcurrentSet<Visiblity>>();

        public Scope(IScope enclosingScope) => EnclosingScope = enclosingScope ?? throw new ArgumentNullException(nameof(enclosingScope));

        private IScope EnclosingScope { get; }
        
        public override bool Equals(object obj)
        {
            return obj is Scope scope &&
                   referanced.Count == scope.referanced.Count && !referanced.Except(scope.referanced).Any() &&
                   EqualityComparer<IScope>.Default.Equals(EnclosingScope, scope.EnclosingScope);
        }

        public override int GetHashCode()
        {
            var hashCode = -1365954579;
            hashCode = hashCode * -1521134295 + referanced.Sum(x=>x.GetHashCode());
            hashCode = hashCode * -1521134295 + EqualityComparer<IScope>.Default.GetHashCode(EnclosingScope);
            return hashCode;
        }

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item) {
            if (referanced.TryGetValue(names.First(), out var items)) {
                var thing = items.Single();
                if (names.Count() == 1) {
                    item = thing.Definition;
                    return true;
                }
                if (thing.Definition is IScoped<IScope> scoped)
                {
                    return scoped.Scope.TryGet(names.Skip(1), out item);
                }
                else {
                    throw new Exception($"{thing.Definition} should be a {typeof(IScoped<IScope>)} instead it is {thing.Definition.GetType()}");
                }
            }
            item = null;
            return false;
        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item) {
            item = null;
            return false;
        }
    }

}