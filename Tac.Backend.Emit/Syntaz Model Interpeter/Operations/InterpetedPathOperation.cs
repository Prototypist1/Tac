using System;
using Prototypist.Toolbox;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedPathOperation : IAssembledOperation
    {

        public void Init(IAssembledOperation left, IInterpetedMemberReferance right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private IAssembledOperation? left;
        public IAssembledOperation Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IInterpetedMemberReferance? right;
        public IInterpetedMemberReferance Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }

        public void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            var leftResult = Left.Assemble(interpetedContext);

            if (leftResult.IsReturn(out var leftReturned, out var leftValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(leftReturned!);
            }

            return InterpetedResult.Create(leftValue!.Value.Has<IInterpetedScope>().GetMember(Right.Key));
        }
    }
}