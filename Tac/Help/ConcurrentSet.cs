//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;

//namespace Tac.Semantic_Model
//{

//    // TODO, this does not go here
//    // and I would like to use parallel flow
//    public class ConcurrentSet<T> : IEnumerable<T> {
//        protected readonly ConcurrentDictionary<T, T> backing = new ConcurrentDictionary<T, T>();
//        public int Count => backing.Count;
//        public bool TryAdd(T item) => backing.TryAdd(item,item);
//        public bool Contains(T item) => backing.ContainsKey(item);
//        public IEnumerator<T> GetEnumerator() => backing.Keys.GetEnumerator();
//        IEnumerator IEnumerable.GetEnumerator() => backing.Keys.GetEnumerator();
//    }

//}