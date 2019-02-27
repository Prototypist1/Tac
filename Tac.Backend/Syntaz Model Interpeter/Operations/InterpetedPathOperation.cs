using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation<IInterpetedScope, IInterpetedMemberReferance, IInterpetedAnyType>
    {
        public override IInterpetedResult<IInterpetedMember<IInterpetedAnyType>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IInterpetedAnyType>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IInterpetedAnyType>>(rightReturned);
            }
            
            return InterpetedResult.Create(leftValue.Value.GetMember<IInterpetedAnyType>(rightValue.Value.Key));
        }
    }
}