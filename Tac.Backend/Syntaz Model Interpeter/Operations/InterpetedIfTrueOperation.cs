using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedIfTrueOperation : InterpetedBinaryOperation<IInterpetedData, IInterpetedData,IInterpetedBoolean>
    {
        // ugh! or types are killing me!
        // this return bool
        // but sometimes it returns a return

        // this takes a bool or a member bool
        // until I solve this we are a hot mess
        // probably the solution is give up on types!!

        public override IInterpetedResult<IInterpetedBoolean> Interpet(InterpetedContext interpetedContext)
        {
            if (Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<RunTimeBoolean>(interpetedContext).Value) {
                var rightRes = Right.Interpet(interpetedContext);
                if (rightRes.IsReturn) {
                    return rightRes;
                }
                return InterpetedResult<IInterpetedBoolean>.Create(new RunTimeBoolean(true));
            }
            return InterpetedResult<IInterpetedBoolean>.Create(new RunTimeBoolean(false));
        }
    }
}