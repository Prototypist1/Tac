using Prototypist.Toolbox;
using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedLastCallOperation : InterpetedBinaryOperation
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

            if (leftValue!.Value.Has<IInterpetedMethod>().Invoke(rightValue!).IsReturn(out var returned, out var _) && returned is IInterpetedMember outReturned)
            {
                return InterpetedResult.Create(outReturned);
            }

            throw new Exception("should never get here!");
        }
    }

    internal class InterpetedNextCallOperation : InterpetedBinaryOperation
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

            if (!rightValue!.Value.Has<IInterpetedMethod>().Invoke(leftValue!).IsReturn(out var _, out var value)){
                return InterpetedResult.Create(value!);
            }

            throw new Exception("should never get here!");
        }
    }
}