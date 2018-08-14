namespace Tac.Semantic_Model
{
    public class LocalStaticScope : StaticScope
    {
        public bool TryAddLocal(LocalDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    
}