using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class MemberDefinition: IReferanced<MemberName>
    {
        public MemberName Key { get; }

        public bool ReadOnly { get; }
        public Referance<TypeDefinition> Type { get; }
    }
}