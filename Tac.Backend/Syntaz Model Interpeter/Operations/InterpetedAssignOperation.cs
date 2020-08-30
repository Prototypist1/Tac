using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedAssignOperation : IInterpetedOperation
    {

    }


    internal class InterpetedAssignOperation: InterpetedBinaryOperation, IInterpetedAssignOperation
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

            rightValue.CastTo<IInterpetedMemberSet>().Set(leftValue!.Value);
             
            return InterpetedResult.Create(leftValue);
        }
    }
}