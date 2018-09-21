using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class InstanceScope : LocalStaticScope
    {
        public bool TryAddInstanceMember(IKey key,IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Instance, key, definition);
        }
    }

}