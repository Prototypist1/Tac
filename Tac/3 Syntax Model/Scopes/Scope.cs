using System;
using System.Collections.Concurrent;
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

        public IScope EnclosingScope { get; }

        public TReferanced Get<TReferanced>(NamePath key) where TReferanced : IReferanced {
            throw new NotImplementedException();
        }
    }

}