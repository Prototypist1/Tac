using System;
using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMemberReferance :  IInterpeted
    {
        public void Init(InterpetedMemberReferance memberReferance)
        {
            MemberReferance = memberReferance ?? throw new ArgumentNullException(nameof(memberReferance));
        }

        public InterpetedMemberReferance MemberReferance { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(MemberDefinition);
        }
    }
}