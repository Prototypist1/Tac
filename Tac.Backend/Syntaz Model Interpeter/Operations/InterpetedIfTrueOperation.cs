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
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<bool>>(leftReturned);
            }

            if (!leftValue.Value)
            {
                return InterpetedResult.Create(new InterpetedMember<bool>(false));
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<bool>>(rightReturned);
            }

            return InterpetedResult.Create(new InterpetedMember<bool>(true));
        }
    }
}