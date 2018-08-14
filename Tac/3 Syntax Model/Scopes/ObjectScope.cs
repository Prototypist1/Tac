namespace Tac.Semantic_Model
{
    public class ObjectScope : Scope
    {
        public bool TryAddLocalMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    
}