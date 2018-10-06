using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ILocalStaticScope : IStaticScope
    {
        bool TryAddLocal(IKey key, IBox<MemberDefinition> definition);
    }
    
}