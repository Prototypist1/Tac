using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedAssignOperation<out T> : IInterpetedOperation<T>
        where T : class, IInterpetedAnyType
    { }

    internal class InterpetedAssignOperation<T> : InterpetedBinaryOperation<T, T, T>, IInterpetedAssignOperation<T>
        where T : class, IInterpetedAnyType
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

            if (!rightValue.TrySet(leftValue.Value)){
                throw new Exception("type error!");
            }
            return InterpetedResult.Create(rightValue);
        }
    }
}