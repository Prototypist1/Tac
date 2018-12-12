using System;
using Tac.Model.Elements;

namespace Tac.Model.instantiated
{
    public class TestMemberDefinition : IMemberDefinition
    {
        public TestMemberDefinition(IKey key, ITypeReferance type, bool readOnly)
        {
            Key = key ;
            Type = type;
            ReadOnly = readOnly;
        }

        public IKey Key { get; set; }
        public ITypeReferance Type { get; set; }
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