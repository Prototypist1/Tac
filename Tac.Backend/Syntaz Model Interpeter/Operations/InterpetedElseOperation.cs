using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedElseOperation : InterpetedBinaryOperation<bool, IInterpedEmpty, bool>
    {
        public override IInterpetedResult<IInterpetedMember<bool>> Interpet(InterpetedContext interpetedContext) {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<bool>>(leftReturned);
            }

            if (leftValue.Value)
            {
                return InterpetedResult.Create(new InterpetedMember<bool>(false));
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<bool>>(rightReturned);
            }
            
            return InterpetedResult.Create(new InterpetedMember<bool>(true));
        }
    }
}