using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal class EqualableReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly T[] array;

        public EqualableReadOnlyList(T[] array)
        {
            this.array = array ?? throw new ArgumentNullException(nameof(array));
        }

        public T this[int index] => ((IReadOnlyList<T>)array)[index];

        public int Count => ((IReadOnlyCollection<T>)array).Count;


        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var res = 0;
                var i = 1;
                foreach (var item in array)
                {
                    res += item.GetHashCode() * i;
                    i++;
                }
                return res;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is EqualableReadOnlyList<T> list &&
                  array.SequenceEqual(list.array);
        }

    }
}