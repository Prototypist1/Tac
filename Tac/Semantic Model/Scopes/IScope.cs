using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        IScope EnclosingScope { get; }
        TReferanced Get<TReferanced>(NamePath key) where TReferanced : IReferanced;
    }
}