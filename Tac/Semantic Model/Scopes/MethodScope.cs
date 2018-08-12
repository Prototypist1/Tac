namespace Tac.Semantic_Model
{
    public class MethodScope : InstanceScope
    {
        public bool TryAddParameter(ParameterDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }
    
}