using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMultiplyOperation : InterpetedBinaryOperation<IInterpetedNumber,IInterpetedNumber,IInterpetedBoolean>
    {
        public override IInterpetedResult<IInterpetedBoolean> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<IInterpetedBoolean>.Create(new RuntimeNumber(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value *
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value));
        }
    }
}