using System;
using Prototypist.LeftToRight;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : InterpetedBinaryOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {

            IInterpetedScope ToMember()
            {
                var leftResult = Left.Interpet(interpetedContext).Get();
                if (Left is IInterpetedScope innerScope) {
                    return innerScope;
                }
                if (leftResult is InterpetedMember member)
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
            
            return InterpetedResult.Create(scope.GetMember(Right.Interpet(interpetedContext).Get<InterpetedMemberDefinition>().Key));
        }
    }
}