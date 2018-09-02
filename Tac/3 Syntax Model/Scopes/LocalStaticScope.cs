namespace Tac.Semantic_Model
{
    public class LocalStaticScope : StaticScope
    {
        public override bool Equals(object obj) => obj is LocalStaticScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddLocal(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    
}