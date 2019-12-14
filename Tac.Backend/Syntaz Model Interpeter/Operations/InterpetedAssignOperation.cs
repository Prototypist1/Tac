using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedAssignOperation<out TLeft> : IInterpetedOperation<TLeft>
        where TLeft : IInterpetedAnyType
    {

    }

    internal interface IInterpetedAssignOperation<out TLeft, in TRight> : IInterpetedAssignOperation<TLeft>
        where TLeft: TRight
        where TRight : IInterpetedAnyType
    { }

    internal class InterpetedAssignOperation<TLeft, TRight> : InterpetedBinaryOperation<TLeft,TRight,TLeft>, IInterpetedAssignOperation<TLeft,TRight>
        where TLeft : TRight
        where TRight : IInterpetedAnyType
    {
        public override IInterpetedResult<IInterpetedMember<TLeft>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TLeft>>(leftReturned);
            }

            var rightResult = Right.Interpet(interpetedContext);

            if (rightResult.IsReturn(out var rightReturned, out var rightValue))
            {
                return InterpetedResult.Return<IInterpetedMember<TLeft>>(rightReturned);
            }

            rightValue.CastTo<IInterpetedMemberSet<TRight>>().Set(leftValue.Value);
             
            return InterpetedResult.Create(leftValue);
        }
    }
}