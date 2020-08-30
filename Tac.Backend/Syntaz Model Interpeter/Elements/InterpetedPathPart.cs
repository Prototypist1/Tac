using System;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedMemberReferance
    {
        IKey Key { get; }
    }

    internal class InterpetedMemberReferance : IInterpetedOperation, IInterpetedMemberReferance
    {
        public InterpetedMemberReferance Init(InterpetedMemberDefinition memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public IKey Key => MemberDefinition.Key;

        private InterpetedMemberDefinition? memberDefinition;
        public InterpetedMemberDefinition MemberDefinition { get => memberDefinition ?? throw new NullReferenceException(nameof(memberDefinition)); private set => memberDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(interpetedContext.GetMember(MemberDefinition.Key));
        }
    }
}