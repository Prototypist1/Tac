using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal abstract class InterpetedTrailingOperation : IInterpetedOperation
    {
        public void Init(IInterpetedOperation argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext);

        private IInterpetedOperation? argument;
        public IInterpetedOperation Argument { get => argument ?? throw new NullReferenceException(nameof(argument)); private set => argument = value ?? throw new NullReferenceException(nameof(value)); }

    }
}