namespace Tac.Semantic_Model
{
    public class MethodScope : InstanceScope
    {
        public MethodScope(IScope enclosingScope) : base(enclosingScope)
        {
        }

        public override bool Equals(object obj) => obj is MethodScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddParameter(ParameterDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }
    
}