using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class MemberReference : IMemberReferance, IMemberReferanceBuilder
    {
        private readonly Buildable<IMemberDefinition> buildableMemberDefinition = new Buildable<IMemberDefinition>();

        private MemberReference()
        {
        }

        public IMemberDefinition MemberDefinition => buildableMemberDefinition.Get(); 

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(this);
        }

        public IVerifiableType Returns()
        {
            return this;
        }

        public void Build(IMemberDefinition memberDefinition)
        {
            buildableMemberDefinition.Set(memberDefinition);
        }
        
        public static (IMemberReferance, IMemberReferanceBuilder) Create()
        {
            var res = new MemberReference();
            return (res, res);
        }

        public static IMemberReferance CreateAndBuild(IMemberDefinition memberDefinition) {
            var (x, y) = Create();
            y.Build(memberDefinition);
            return x;
        }
    }

    public interface IMemberReferanceBuilder
    {
        void Build(IMemberDefinition memberDefinition);
    }
}