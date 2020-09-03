using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal interface IInterpetedAssignOperation : IAssembledOperationRequiresGenerator
    {

    }


    internal class InterpetedAssignOperation: InterpetedBinaryOperation, IInterpetedAssignOperation
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

            rightValue.CastTo<IInterpetedMemberSet>().Set(leftValue!.Value);
             
            return InterpetedResult.Create(leftValue);
        }
    }
}