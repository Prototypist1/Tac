namespace Tac.Semantic_Model
{
    public class ObjectScope : StaticScope
    {
        public override bool Equals(object obj) => obj is ObjectScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddLocalMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
}