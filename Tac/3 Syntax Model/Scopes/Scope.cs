using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // dead end!
    public class RootScope : IScope
    {
        public bool TryGet<TReferanced>(NamePath key, out TReferanced item) where TReferanced : IReferanced
        {
            item = default;
            return false;
        }
    }

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

        private  class Visiblity
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

        public bool TryGet<TReferanced>(NamePath key, out TReferanced item) where TReferanced : IReferanced => throw new NotImplementedException();
    }

}