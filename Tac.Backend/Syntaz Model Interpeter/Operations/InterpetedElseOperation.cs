using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedElseOperation : InterpetedBinaryOperation<IInterpetedMember<bool>, IInterpedEmpty, IInterpetedMember<bool>>
    {
        public override IInterpetedResult<IInterpetedMember<bool>> Interpet(InterpetedContext interpetedContext) {
            var leftRes = Left.Interpet(interpetedContext);
            if (leftRes.IsReturn) {
                return leftRes;
            }

            if (leftRes.Value.Value)
            {
                return InterpetedResult<IInterpetedMember<bool>>.Create(new RunTimeBoolean(false));
            }
            
            var rightRes = Right.Interpet(interpetedContext);
            if (rightRes.IsReturn) {
                return rightRes;
            }

            return InterpetedResult<IInterpetedMember<bool>>.Create(new RunTimeBoolean(true));
        }
    }
}