using System;
using Prototypist.Toolbox;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation<T> : IInterpetedOperation<T>
        where T : IInterpetedAnyType
    {

        public void Init(IInterpetedOperation<IInterpetedScope> left, IInterpetedMemberReferance<T> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public IInterpetedOperation<IInterpetedScope> Left { get; private set; }
        public IInterpetedMemberReferance<T> Right { get; private set; }

        public IInterpetedResult<IInterpetedMember<T>> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember<T>>(leftReturned);
            }

            return InterpetedResult.Create(leftValue.Value.GetMember<T>(Right.Key));
        }
    }
}