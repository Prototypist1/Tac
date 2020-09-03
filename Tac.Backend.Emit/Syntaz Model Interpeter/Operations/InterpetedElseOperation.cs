using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{

    internal class InterpetedElseOperation : InterpetedBinaryOperation
    {
        public override void Assemble(AssemblyContextWithGenerator interpetedContext) {
            var leftResult = Left.Assemble(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            if (leftValue!.Value.Has<IBoxedBool>().Value)
            {
                return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(false)));
            }

            var rightResult = Right.Assemble(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var _))
            {
                return InterpetedResult.Return<IInterpetedMember>(rightReturned!);
            }
            
            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(true)));
        }
    }
}