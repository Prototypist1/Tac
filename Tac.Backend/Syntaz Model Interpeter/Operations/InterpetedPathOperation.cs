using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation<IInterpetedData, IInterpetedData,IInterpetedMember<IInterpetedData>>
    {
        public override IInterpetedResult<IInterpetedMember<IInterpetedData>> Interpet(InterpetedContext interpetedContext)
        {

            IInterpetedScope ToMember()
            {
                var leftResult = Left.Interpet(interpetedContext).Value;
                if (Left is IInterpetedScope innerScope) {
                    return innerScope;
                }
                if (leftResult is IInterpetedMember<IInterpetedData> member)
                {
                    return member.Value.Cast<IInterpetedScope>();
                    }
                if (leftResult is InterpetedMemberDefinition interpetedMemberDefinition)
                {
                    return interpetedContext.GetMember(interpetedMemberDefinition.Key).Value.Cast<IInterpetedScope>();
                }
                throw new Exception();
            }

            var scope = ToMember();
            
            return InterpetedResult<IInterpetedData>.Create(scope.GetMember(Right.Interpet(interpetedContext).Get<InterpetedMemberDefinition>().Key));
        }
    }
}