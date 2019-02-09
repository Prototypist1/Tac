using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedLessThanOperation : InterpetedBinaryOperation<double, double, bool>
    {
        public override IInterpetedResult<IInterpetedMember<bool>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<IInterpetedMember<bool>>.Create(new RunTimeBoolean(
                Left.Interpet(interpetedContext).Value.Value <
                Right.Interpet(interpetedContext).Value.Value));
        }
    }
}