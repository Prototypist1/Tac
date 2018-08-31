namespace Tac.Semantic_Model
{

    public class InstanceScope : LocalStaticScope
    {
        public bool TryAddInstanceMember(AbstractMemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
        
        public override bool Equals(object obj) => obj is InstanceScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

    }

}