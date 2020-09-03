using System;
using Prototypist.Toolbox;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedLessThanOperation : InterpetedBinaryOperation
    {
        public override void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            var leftResult = Left.Assemble(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            var rightResult = Right.Assemble(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(rightReturned!);
            }

            return InterpetedResult.Create(TypeManager.BoolMember(TypeManager.Bool(
                leftValue!.Value.Has<IBoxedDouble>().Value <
                rightValue!.Value.Has<IBoxedDouble>().Value)));
        }
    }
}