using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedSubtractOperation : InterpetedBinaryOperation<BoxedDouble, BoxedDouble, BoxedDouble>
    {
        public override IInterpetedResult<IInterpetedMember<BoxedDouble>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<BoxedDouble>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<BoxedDouble>>(rightReturned);
            }

            return InterpetedResult.Create(new InterpetedMember<BoxedDouble>(new BoxedDouble(
                leftValue.Value.Value -
                rightValue.Value.Value)));
        }
    }
}