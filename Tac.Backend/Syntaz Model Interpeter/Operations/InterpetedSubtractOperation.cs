using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedSubtractOperation : InterpetedBinaryOperation<double, double, double>
    {
        public override IInterpetedResult<IInterpetedMember<double>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<IInterpetedMember<double>>.Create(new RuntimeNumber(
                Left.Interpet(interpetedContext).Value.Value -
                Right.Interpet(interpetedContext).Value.Value));
        }
    }
}