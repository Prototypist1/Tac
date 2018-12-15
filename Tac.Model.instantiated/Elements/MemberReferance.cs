using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class MemberReferance : IMemberReferance, IMemberReferanceBuilder
    {
        private readonly Buildable<IMemberDefinition> buildableMemberDefinition = new Buildable<IMemberDefinition>();

        private MemberReferance()
        {
        }

        public IMemberDefinition MemberDefinition => buildableMemberDefinition.Get(); 

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }

        public void Build(IMemberDefinition memberDefinition)
        {
            buildableMemberDefinition.Set(memberDefinition);
        }
        
        public static (IMemberReferance, IMemberReferanceBuilder) Create()
        {
            var res = new MemberReferance();
            return (res, res);
        }
    }

    public interface IMemberReferanceBuilder
    {
        void Build(IMemberDefinition memberDefinition);
    }
}