using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLessThanOperation : LessThanOperation, IInterpeted
    {
        public InterpetedLessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                left.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>() <
                right.Cast<IInterpeted>().Interpet(interpetedContext).Get<double>());
        }
    }
}