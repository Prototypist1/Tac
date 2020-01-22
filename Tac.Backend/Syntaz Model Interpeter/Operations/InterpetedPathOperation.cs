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

        private IInterpetedOperation<IInterpetedScope>? left;
        public IInterpetedOperation<IInterpetedScope> Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedMemberReferance<T>? right;
        public IInterpetedMemberReferance<T> Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }

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