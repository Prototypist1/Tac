using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedIfTrueOperation : InterpetedBinaryOperation<bool, IInterpedEmpty, bool>
    {
        // ugh! or types are killing me!
        // this return bool
        // but sometimes it returns a return

        // this takes a bool or a member bool
        // until I solve this we are a hot mess
        // probably the solution is give up on types!!

        public override IInterpetedResult<IInterpetedMember<bool>> Interpet(InterpetedContext interpetedContext)
        {
            var leftRes = Left.Interpet(interpetedContext);
            if (leftRes.IsReturn)
            {
                return leftRes;
            }

            if (!leftRes.Value.Value)
            {
                return InterpetedResult<IInterpetedMember<bool>>.Create(new RunTimeBoolean(false));
            }

            var rightRes = Right.Interpet(interpetedContext);
            if (rightRes.IsReturn)
            {
                return rightRes;
            }

            return InterpetedResult<IInterpetedMember<bool>>.Create(new RunTimeBoolean(true));
        }
    }
}