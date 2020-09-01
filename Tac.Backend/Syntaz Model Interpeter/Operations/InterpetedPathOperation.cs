using System;
using Prototypist.Toolbox;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedPathOperation : IInterpetedOperation
    {

        public void Init(IInterpetedOperation left, IInterpetedMemberReferance right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private IInterpetedOperation? left;
        public IInterpetedOperation Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedMemberReferance? right;
        public IInterpetedMemberReferance Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var leftResult = Left.Interpet(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            return InterpetedResult.Create(leftValue!.Value.Has<IInterpetedScope>().GetMember(Right.Key));
        }
    }
}