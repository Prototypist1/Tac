using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedSubtractOperation : SubtractOperation, IInterpeted
    {
        public InterpetedSubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new NumberType(
                left.Cast<IInterpeted>().Interpet(interpetedContext).Get<NumberType>().d -
                right.Cast<IInterpeted>().Interpet(interpetedContext).Get<NumberType>().d));
        }

        internal static SubtractOperation MakeNew(ICodeElement left, ICodeElement right)
        {
            return new InterpetedSubtractOperation(left, right);
        }
    }
}