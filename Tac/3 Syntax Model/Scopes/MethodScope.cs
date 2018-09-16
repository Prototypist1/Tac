namespace Tac.Semantic_Model
{
    public class MethodScope : LocalStaticScope
    {
        public bool TryAddParameter(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }

}