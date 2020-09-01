using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedBinaryOperation: IInterpetedOperation
    {
        public void Init(IInterpetedOperation left, IInterpetedOperation right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public abstract IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext);

        private IInterpetedOperation? left;
        public IInterpetedOperation Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedOperation? right;
        public IInterpetedOperation Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }
    }
}