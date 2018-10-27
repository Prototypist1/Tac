using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMemberReferance : WeakMemberReferance, IInterpeted
    {
        public static readonly WeakMemberReferance.Make MakeNew = (memberDefinition) => new InterpetedMemberReferance(memberDefinition);

        public InterpetedMemberReferance(IBox<WeakMemberDefinition> memberDefinition) : base(memberDefinition)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(MemberDefinition.GetValue());
        }
    }
}