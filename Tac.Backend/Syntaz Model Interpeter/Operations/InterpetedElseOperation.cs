using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedElseOperation : InterpetedBinaryOperation<BoxedBool, IInterpedEmpty, BoxedBool>
    {
        public override IInterpetedResult<IInterpetedMember<BoxedBool>> Interpet(InterpetedContext interpetedContext) {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<BoxedBool>>(leftReturned);
            }

            if (leftValue.Value.Value)
            {
                return InterpetedResult.Create(new InterpetedMember<BoxedBool>(new BoxedBool(false)));
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<BoxedBool>>(rightReturned);
            }
            
            return InterpetedResult.Create(new InterpetedMember<BoxedBool>(new BoxedBool(true)));
        }
    }
}