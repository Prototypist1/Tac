using Tac.Semantic_Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathPart : MemberReferance, IInterpeted
    {
        internal static readonly MemberReferance.Make MakeNew = (memberDefinition) => new InterpetedPathPart(memberDefinition);

        public InterpetedPathPart(IBox<MemberDefinition> memberDefinition) : base(memberDefinition)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(MemberDefinition.GetValue());
        }
    }
}