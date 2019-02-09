using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedElseOperation : InterpetedBinaryOperation<IInterpetedData,IInterpetedData, IInterpetedBoolean>
    {
        public override IInterpetedResult<IInterpetedBoolean> Interpet(InterpetedContext interpetedContext) {
            var leftRes = Left.Interpet(interpetedContext);
            if (leftRes.IsReturn) {
                return leftRes;
            }

            if (!leftRes.GetAndUnwrapMemberWhenNeeded<RunTimeBoolean>(interpetedContext).Value)
            {
                var rightRes = Right.Interpet(interpetedContext);
                if (rightRes.IsReturn) {
                    return rightRes;
                }
                return InterpetedResult<IInterpetedBoolean>.Create(new RunTimeBoolean(true));
            }
            return InterpetedResult<IInterpetedBoolean>.Create(new RunTimeBoolean(false));
        }
    }
}