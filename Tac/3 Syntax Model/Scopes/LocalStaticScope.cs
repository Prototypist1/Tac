using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class LocalStaticScope : StaticScope
    {
        public bool TryAddLocal(IKey key ,IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Local, key, definition);
        }
    }
    
}