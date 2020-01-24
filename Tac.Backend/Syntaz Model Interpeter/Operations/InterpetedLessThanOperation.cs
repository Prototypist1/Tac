using System;
using Prototypist.Toolbox;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLessThanOperation : InterpetedBinaryOperation<IBoxedDouble, IBoxedDouble, IBoxedBool>
    {
        public override IInterpetedResult<IInterpetedMember<IBoxedBool>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(leftReturned!);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedBool>>(rightReturned!);
            }

            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(
                leftValue!.Value.Value <
                rightValue!.Value.Value)));
        }
    }
}