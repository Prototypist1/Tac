using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedBinaryOperation<TLeft,TRight,TRes>: IInterpetedOperation<TRes>
        where TRes : IInterpetedAnyType
        where TLeft :  IInterpetedAnyType
        where TRight :  IInterpetedAnyType
    {
        public void Init(IInterpetedOperation<TLeft> left, IInterpetedOperation<TRight> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public abstract IInterpetedResult<IInterpetedMember<TRes>> Interpet(InterpetedContext interpetedContext);

        private IInterpetedOperation<TLeft>? left;
        public IInterpetedOperation<TLeft> Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedOperation<TRight>? right;
        public IInterpetedOperation<TRight> Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }
    }
}