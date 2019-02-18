using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedMemberReferance : IInterpetedOperation { }

    internal class InterpetedMemberReferance<T> :  IInterpetedOperation<T>, IInterpetedMemberReferance
    {
        public InterpetedMemberReferance<T> Init(InterpetedMemberDefinition<T> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public InterpetedMemberDefinition<T> MemberDefinition { get; private set; }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(interpetedContext.GetMember<T>(MemberDefinition.Key));
        }
        
        void IInterpetedOperation.Interpet(InterpetedContext interpetedContext)
        {
            Interpet(interpetedContext);
        }
    }
}