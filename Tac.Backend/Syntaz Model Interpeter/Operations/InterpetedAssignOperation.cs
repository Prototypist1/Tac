using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedAssignOperation : IInterpetedOperation{ }

    internal class InterpetedAssignOperation<T> : InterpetedBinaryOperation<T, T, T>, IInterpetedAssignOperation
    {
        public override IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<T>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<T>>(rightReturned);
            }

            rightValue.Value = leftValue.Value;
            return InterpetedResult.Create(rightValue);
        }
    }
}