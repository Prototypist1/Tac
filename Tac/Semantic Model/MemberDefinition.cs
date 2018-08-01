using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    public sealed class MemberDefinition: IReferanced<MemberName>
    {
        public MemberName Key { get; }
        public bool ReadOnly { get; }
        public TypeReferance Type { get; }
    }
}