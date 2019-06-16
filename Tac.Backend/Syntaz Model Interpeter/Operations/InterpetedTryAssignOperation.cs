using Prototypist.LeftToRight;
using System;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedTryAssignOperation : IInterpetedOperation<IBoxedBool>
    {

    }

    internal interface IInterpetedTryAssignOperation<out TLeft, in TRight> : IInterpetedTryAssignOperation
        where TLeft : IInterpetedAnyType
        where TRight : IInterpetedAnyType
    { }

    internal class InterpetedTryAssignOperation<TLeft, TRight> : InterpetedBinaryOperation<TLeft, TRight, IBoxedBool>, IInterpetedTryAssignOperation<TLeft, TRight>
    where TLeft : IInterpetedAnyType
    where TRight : IInterpetedAnyType
    {
        public override IInterpetedResult<IInterpetedMember<IBoxedBool>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(rightReturned);
            }

            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(rightValue.Cast<IInterpetedMemberSet<TRight>>().TrySet(leftValue.Value))));
        }
    }
}