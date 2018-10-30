using System;
using Prototypist.LeftToRight;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = Left.Interpet(interpetedContext).Cast<InterpetedMember>().Value.Cast<IInterpetedScope>();
            
            return  InterpetedResult.Create(scope.GetMember(Right.Cast<InterpetedMemberReferance>().MemberDefinition.Key));
        }
    }
}