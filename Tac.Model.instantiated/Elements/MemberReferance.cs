using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class MemberReferance : IMemberReferance
    {
        public MemberReferance(IMemberDefinition memberDefinition)
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