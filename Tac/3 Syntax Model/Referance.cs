using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class Referance: ICodeElement
    {
        public readonly NamePath key;

        public Referance(NamePath key) => this.key = key ?? throw new ArgumentNullException(nameof(key));

        public IReferanced GetOrThrow(IScope compilation)
        {
            return compilation.Get<IReferanced>(key);
        }
    }

    public class Referance<TReferanced>: Referance
        where TReferanced : IReferanced
    {
        public Referance(NamePath key) : base(key)
        {
        }

        public new TReferanced GetOrThrow(IScope compilation) {
            return compilation.Get<TReferanced>(key);
        }
    }
}
