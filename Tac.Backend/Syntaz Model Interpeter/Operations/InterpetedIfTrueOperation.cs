using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedIfTrueOperation : InterpetedBinaryOperation<IBoxedBool, IInterpedEmpty, IBoxedBool>
    {
        // ugh! or types are killing me!
        // this return bool
        // but sometimes it returns a return

        // this takes a bool or a member bool
        // until I solve this we are a hot mess
        // probably the solution is give up on types!!

        public override IInterpetedResult<IInterpetedMember<IBoxedBool>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(leftReturned);
            }

            if (!leftValue.Value.Value)
            {
                return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(false)));
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var _))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(rightReturned);
            }

            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(true)));
        }
    }
}