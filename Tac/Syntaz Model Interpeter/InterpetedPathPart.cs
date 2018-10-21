using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMemberReferance : MemberReferance, IInterpeted
    {
        public static readonly MemberReferance.Make MakeNew = (memberDefinition) => new InterpetedMemberReferance(memberDefinition);

        public InterpetedMemberReferance(IBox<MemberDefinition> memberDefinition) : base(memberDefinition)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(MemberDefinition.GetValue());
        }
    }
}