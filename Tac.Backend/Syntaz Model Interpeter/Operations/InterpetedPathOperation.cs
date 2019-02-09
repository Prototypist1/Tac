using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation<IInterpetedScope, IInterpetedData,IInterpetedData>
    {
        public override IInterpetedResult<IInterpetedMember<IInterpetedData>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = Left.Interpet(interpetedContext).Value;
            
            return InterpetedResult<IInterpetedData>.Create(scope.GetMember(Right.Interpet(interpetedContext).Get<InterpetedMemberDefinition>().Key));
        }
    }
}