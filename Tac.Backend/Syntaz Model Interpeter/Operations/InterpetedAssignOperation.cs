using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedAssignOperation : InterpetedBinaryOperation<IInterpetedData,IInterpetedData,IInterpetedMember<IInterpetedData>>
    {
        public override IInterpetedResult<IInterpetedMember<IInterpetedData>> Interpet(InterpetedContext interpetedContext)
        {
            IInterpetedMember<IInterpetedData> GetMember()
            {
                var rightRes = Right.Interpet(interpetedContext).Get();
                if (rightRes is InterpetedMember<IInterpetedData> innerMember)
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

            res.Value = Left.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded<IInterpetedData>(interpetedContext);
            return InterpetedResult<IInterpetedMember<IInterpetedData>>.Create(res);
        }
    }
}