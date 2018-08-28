using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        bool TryGet(IReferance key, out IReferanced item);
    }
}