using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal class EqualableHashSet<T> : IEnumerable<T>, IReadOnlySet<T>
    {
        public readonly HashSet<T> backing;

        public int Count => ((IReadOnlyCollection<T>)backing).Count;

        public EqualableHashSet(HashSet<T> backing)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public override bool Equals(object? obj)
        {
            return obj is EqualableHashSet<T> set &&
                set.backing.SetEquals(backing);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backing.GetEnumerator();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var res = 0;
                foreach (var item in backing)
                {
                    res += item.GetHashCode();
                }
                return res;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // I kind of wish I could remove this...
        public bool Add(T t)
        {
            return backing.Add(t);
        }

        public override string? ToString()
        {
            return $"EqualibleHashSet<{typeof(T).Name}>{{{string.Join(", ", backing.Take(10).Select(x => x.ToString())) + ((backing.Count > 10) ? "..." : "")}}}";
        }

        public bool Contains(T item)
        {
            return ((IReadOnlySet<T>)backing).Contains(item);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return ((IReadOnlySet<T>)backing).SetEquals(other);
        }
    }
}