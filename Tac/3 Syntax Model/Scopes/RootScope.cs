using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed partial class RootScope : IScope
    {
        public override bool Equals(object obj) => obj is RootScope;
        public override int GetHashCode() => 1522345295;

        private RootScope(){}

        public static RootScope Root = new RootScope();

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item) {

        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item) {
            item = default;
            return false;
        }
    }
}