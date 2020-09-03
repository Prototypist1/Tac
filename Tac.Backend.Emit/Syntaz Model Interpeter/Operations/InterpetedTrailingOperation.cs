using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal abstract class InterpetedTrailingOperation : IAssembledOperationRequiresGenerator
    {
        public void Init(IAssembledOperationRequiresGenerator argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract void Assemble(AssemblyContextWithGenerator interpetedContext);

        private IAssembledOperationRequiresGenerator? argument;
        public IAssembledOperationRequiresGenerator Argument { get => argument ?? throw new NullReferenceException(nameof(argument)); private set => argument = value ?? throw new NullReferenceException(nameof(value)); }

    }
}