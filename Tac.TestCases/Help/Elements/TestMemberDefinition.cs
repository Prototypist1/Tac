using System;
using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public class TestMemberDefinition : IMemberDefinition
    {
        public TestMemberDefinition(IKey key, IVarifiableType type, bool readOnly)
        {
            Key = key ;
            Type = type;
            ReadOnly = readOnly;
        }

        public IKey Key { get; set; }
        public IVarifiableType Type { get; set; }
        public bool ReadOnly { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }
}