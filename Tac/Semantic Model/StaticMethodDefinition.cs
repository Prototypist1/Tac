namespace Tac.Semantic_Model
{
    public sealed class StaticMethodDefinition : MethodDefinition, IReferanced<MethodName>
    {
        public MethodName Key { get; }
    }
}
