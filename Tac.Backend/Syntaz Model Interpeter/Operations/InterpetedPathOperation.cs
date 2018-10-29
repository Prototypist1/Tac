using System;
using Prototypist.LeftToRight;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = Left.Interpet(interpetedContext).Cast<InterpetedMember>().Value.Cast<IInterpetedScope>();
            
            return  InterpetedResult.Create(scope.GetMember(Right.Cast<InterpetedMemberReferance>().MemberDefinition.GetValue().Key));
        }
    }
}