using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class ObjectScope : StaticScope
    {
        public bool TryAddLocalMember(IKey key, IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Local, key, definition);
        }
    }
}