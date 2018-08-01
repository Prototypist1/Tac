namespace Tac.Semantic_Model
{
    public sealed class ParameterDefinition: IReferanced<ParameterName>
    {
        public bool ReadOnly { get; }
        public ParameterName Key { get; }
        public Referance<TypeDefinition> Type { get;  }
    }
}