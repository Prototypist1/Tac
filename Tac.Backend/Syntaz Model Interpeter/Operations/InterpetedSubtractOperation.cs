using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedSubtractOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new RunTimeNumber(
                Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>(interpetedContext).d -
                Right.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeNumber>(interpetedContext).d));
        }
    }
}