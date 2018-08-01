namespace Tac.Semantic_Model
{
    // is this really IReferanced??
    public sealed class ParameterDefinition: IReferanced<ParameterName>
    {
        public bool ReadOnly { get; }
        public ParameterName Key { get; }
        public TypeReferance Type { get;  }
    }
}