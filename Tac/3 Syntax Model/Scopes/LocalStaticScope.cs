namespace Tac.Semantic_Model
{
    public class LocalStaticScope : StaticScope
    {
        public LocalStaticScope(IScope enclosingScope) : base(enclosingScope)
        {
        }

        public override bool Equals(object obj) => obj is LocalStaticScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddLocal(LocalDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }
    
}