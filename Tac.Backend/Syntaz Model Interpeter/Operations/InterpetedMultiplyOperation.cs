using System;
using Prototypist.Toolbox;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMultiplyOperation : InterpetedBinaryOperation
    {
        public override IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(rightReturned!);
            }

            return InterpetedResult.Create(TypeManager.NumberMember(TypeManager.Double(
                leftValue!.Value.Value *
                rightValue!.Value.Value)));
        }
    }
}