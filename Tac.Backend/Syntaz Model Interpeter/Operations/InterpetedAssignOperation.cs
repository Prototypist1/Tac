using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            InterpetedMember GetMember()
            {
                var rightRes = Right.Interpet(interpetedContext).Get();
                if (rightRes is InterpetedMember innerMember)
                {
                    return innerMember;
                }
                if (rightRes is InterpetedMemberDefinition interpetedMemberDefinition)
                {
                    return interpetedContext.GetMember(interpetedMemberDefinition.Key);
                }
                throw new Exception("PoS");
            }

            var res = GetMember();  

            res.Value = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IRunTime>(interpetedContext);
            return InterpetedResult.Create(res);
        }
    }
}