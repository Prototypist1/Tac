using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Semantic_Model
{
    public abstract class AbstractScope : IScope
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

        private readonly ConcurrentDictionary<IName, ConcurrentSet<Visiblity>> referanced = new ConcurrentDictionary<IName, ConcurrentSet<Visiblity>>();

        public IScope EnclosingScope { get; }

        public abstract TReferanced Get<TReferanced>(IName key) where TReferanced : IReferanced;
    }
    
    // TODO, this does not go here
    // and I would like to use parallel flow
    public class ConcurrentSet<T> : IEnumerable<T> {
        protected readonly ConcurrentDictionary<T, T> backing = new ConcurrentDictionary<T, T>();
        public int Count => backing.Count;
        public bool TryAdd(T item) => backing.TryAdd(item,item);
        public bool Contains(T item) => backing.ContainsKey(item);
        public IEnumerator<T> GetEnumerator() => backing.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => backing.Keys.GetEnumerator();
    }

}