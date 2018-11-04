using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedIfTrueOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            if (Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeBoolean>(interpetedContext).b) {
                var rightRes = Right.Interpet(interpetedContext);
                if (rightRes.IsReturn) {
                    return rightRes;
                }
                return InterpetedResult.Create(new RunTimeBoolean(true));
            }
            return InterpetedResult.Create(new RunTimeBoolean(false));
        }
    }
}