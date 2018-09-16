namespace Tac.Semantic_Model
{
    public class LocalStaticScope : StaticScope
    {
        public bool TryAddLocal(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    
}