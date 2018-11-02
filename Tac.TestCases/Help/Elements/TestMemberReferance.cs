using System;

namespace Tac.Model.Elements
{
    public class TestMemberReferance : IMemberReferance
    {
        public TestMemberReferance(IMemberDefinition memberDefinition)
        {
            MemberDefinition = memberDefinition;
        }

        public IMemberDefinition MemberDefinition { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }
}