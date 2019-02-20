using System;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedMemberReferance
    {
        IKey Key { get; }
    }

    internal class InterpetedMemberReferance<T> : IInterpetedOperation<T>, IInterpetedMemberReferance
    {
        public InterpetedMemberReferance<T> Init(InterpetedMemberDefinition<T> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public IKey Key => MemberDefinition.Key;

        public InterpetedMemberDefinition<T> MemberDefinition { get; private set; }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(interpetedContext.GetMember<T>(MemberDefinition.Key));
        }
    }
}