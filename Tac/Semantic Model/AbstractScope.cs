using System.Collections.Generic;

namespace Tac.Semantic_Model
{
    public abstract class AbstractScope : IScope
    {
        protected enum DefintionLifeTime {
            Static,
            Instance,
            Local,
        }

        protected  class Visiblity<T> {
            public DefintionLifeTime DefintionLifeTime { get; }
            public T Definition { get; }
        }
        
        protected readonly Dictionary<IName, ISet<Visiblity<IReferanced>>> referanced = new Dictionary<IName, ISet<Visiblity<IReferanced>>>();

        public IScope EnclosingScope { get; }

        public abstract TReferanced Get<TKey, TReferanced>(TKey key) where TReferanced : IReferanced<TKey>;
    }

    
}