using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.TestCases.Help
{
    public class FinalizedScope: IFinalizedScope
    {
        private readonly IReadOnlyDictionary<IKey, IMemberDefinition> backing;

        public FinalizedScope(IReadOnlyDictionary<IKey, IMemberDefinition> backing)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public IMemberDefinition this[IKey key]
        {
            get
            {
                return backing[key];
            }
        }

        public IEnumerable<IKey> Keys
        {
            get
            {
                return backing.Keys;
            }
        }

        public IEnumerable<IMemberDefinition> Values
        {
            get
            {
                return backing.Values;
            }
        }

        public int Count
        {
            get
            {
                return backing.Count;
            }
        }

        public bool ContainsKey(IKey key)
        {
            return backing.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<IKey, IMemberDefinition>> GetEnumerator()
        {
            return backing.GetEnumerator();
        }

        public bool TryGetValue(IKey key, out IMemberDefinition value)
        {
            return backing.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backing.GetEnumerator();
        }
    }
}
