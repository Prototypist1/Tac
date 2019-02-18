using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedLastCallOperation : IInterpetedOperation{ }

    internal class InterpetedLastCallOperation<TIn,TOut> : InterpetedBinaryOperation<IInterpetedCallable<TIn,TOut>, TIn,TOut>, IInterpetedLastCallOperation
    {
        public override IInterpetedResult<IInterpetedMember<TOut>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TOut>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TOut>>(rightReturned);
            }

            if (leftValue.Value.Invoke(rightValue).IsReturn(out var returned, out var _) && returned is IInterpetedMember<TOut> outReturned)
            {
                return InterpetedResult.Create(outReturned);
            }

            throw new Exception("should never get here!");
        }
    }

    internal interface IInterpetedNextCallOperation : IInterpetedOperation { }

    internal class InterpetedNextCallOperation<TIn, TOut> : InterpetedBinaryOperation<TIn, IInterpetedCallable<TIn, TOut>, TOut>, IInterpetedNextCallOperation
    {
        public override IInterpetedResult<IInterpetedMember<TOut>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TOut>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TOut>>(rightReturned);
            }

            if (rightValue.Value.Invoke(leftValue).IsReturn(out var returned, out var _) && returned is IInterpetedMember<TOut> outReturned){
                return InterpetedResult.Create(outReturned);
            }

            throw new Exception("should never get here!");
        }
    }
}