using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLastCallOperation<TIn,TOut> : InterpetedBinaryOperation<IInterpetedCallable<TIn,TOut>, TIn,TOut>
    {
        public override IInterpetedResult<IInterpetedMember<TOut>> Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Left.Interpet(interpetedContext).Value;
            var parameter = Right.Interpet(interpetedContext).Value;

            return toCall.Value.Invoke(parameter);
        }
    }

    internal class InterpetedNextCallOperation<TIn, TOut> : InterpetedBinaryOperation<TIn, IInterpetedCallable<TIn, TOut>, TOut>
    {
        public override IInterpetedResult<IInterpetedMember<TOut>> Interpet(InterpetedContext interpetedContext)
        {
            var toCall = Right.Interpet(interpetedContext).Value;
            var parameter = Left.Interpet(interpetedContext).Value;

            return toCall.Value.Invoke(parameter);
        }
    }
}