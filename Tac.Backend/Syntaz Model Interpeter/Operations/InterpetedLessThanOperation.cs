using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLessThanOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeBoolean(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value <
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RuntimeNumber>(interpetedContext).Value));
        }
    }
}