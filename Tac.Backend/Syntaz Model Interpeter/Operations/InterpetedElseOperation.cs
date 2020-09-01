using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedElseOperation : InterpetedBinaryOperation
    {
        public override IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext) {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            if (leftValue!.Value.Has<IBoxedBool>().Value)
            {
                return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(false)));
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var _))
            {
                return InterpetedResult.Return<IInterpetedMember>(rightReturned!);
            }
            
            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(true)));
        }
    }
}