using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedAddOperation : InterpetedBinaryOperation<IBoxedDouble, IBoxedDouble, IBoxedDouble>
    {
        public override IInterpetedResult<IInterpetedMember<IBoxedDouble>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue)) {
                return InterpetedResult.Return<IInterpetedMember<IBoxedDouble>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<IBoxedDouble>>(rightReturned);
            }

            return InterpetedResult.Create(TypeManager.Member(TypeManager.Double(
                leftValue.Value.Value +
                rightValue.Value.Value
            )));
        }
    }
}