using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation<T> : InterpetedBinaryOperation<T, T, T>
    {
        public override IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            var res = Right.Interpet(interpetedContext).Value;

            res.Value = Left.Interpet(interpetedContext).Value.Value;
            return InterpetedResult<IInterpetedMember<T>>.Create(res);
        }
    }
}