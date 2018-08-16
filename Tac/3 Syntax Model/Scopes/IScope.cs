using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        bool TryGet<TReferanced>(NamePath key, out TReferanced item) where TReferanced : IReferanced;
    }
}