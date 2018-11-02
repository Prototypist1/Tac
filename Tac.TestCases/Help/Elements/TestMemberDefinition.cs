using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public class TestMemberDefinition : IMemberDefinition
    {
        public IKey Key { get; }
        public IVarifiableType Type { get; }
        public bool ReadOnly { get; }
    }
}