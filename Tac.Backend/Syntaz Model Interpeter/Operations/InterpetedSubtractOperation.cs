using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedSubtractOperation : InterpetedBinaryOperation<IInterpetedData,IInterpetedData,IInterpetedNumber>
    {
        public override IInterpetedResult<IInterpetedNumber> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<IInterpetedNumber>.Create(new RuntimeNumber(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value -
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value));
        }
    }
}