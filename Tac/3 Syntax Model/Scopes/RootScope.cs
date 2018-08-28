using System;
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

        public bool TryGet(IReferance key, out IReferanced item) {
            throw new NotImplementedException();
        }

    }

}