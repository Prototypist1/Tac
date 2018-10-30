using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberReferance :  IInterpeted
    {
        public InterpetedMemberReferance Init(InterpetedMemberDefinition memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            return this;
        }

        public InterpetedMemberDefinition MemberDefinition { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(MemberDefinition);
        }
    }
}