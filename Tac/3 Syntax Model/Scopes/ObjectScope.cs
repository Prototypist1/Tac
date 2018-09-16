namespace Tac.Semantic_Model
{
    public class ObjectScope : StaticScope
    {
        public bool TryAddLocalMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    


}