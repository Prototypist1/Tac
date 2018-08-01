using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public class Referance<TKey,TReferanced>
        where TReferanced : IReferanced<TKey>
    {
        public TKey Key { get; }

        public TReferanced GetOrThrow(Scope compilation) {
            return compilation.Get<TKey, TReferanced>(Key);
        }
    }

    public class TypeReferance: Referance<TypeName, TypeDefinition>
    {
    }
}
