using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class RootScope : IScope
    {
        public override bool Equals(object obj) => obj is RootScope;
        public override int GetHashCode() => 1522345295;

        public bool TryGet<TReferanced>(NamePath key, out TReferanced item) where TReferanced : IReferanced
        {
            item = default;
            return false;
        }
    }

}