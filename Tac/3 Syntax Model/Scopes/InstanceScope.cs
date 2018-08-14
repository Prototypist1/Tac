namespace Tac.Semantic_Model
{

    public class InstanceScope : LocalStaticScope
    {
        public bool TryAddInstanceMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }
    
}