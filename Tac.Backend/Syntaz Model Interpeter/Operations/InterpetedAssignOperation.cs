using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = interpetedContext.GetMember(Right.Interpet(interpetedContext).Get().Cast<InterpetedMemberDefinition>().Key);

            res.Value = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>(interpetedContext);
            return InterpetedResult.Create(res);
        }
    }
}