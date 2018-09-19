using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedSubtractOperation : SubtractOperation, IInterpeted
    {
        public InterpetedSubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                left.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>() +
                right.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>());
        }
    }
}