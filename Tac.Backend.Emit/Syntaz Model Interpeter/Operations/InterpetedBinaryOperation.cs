using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal abstract class InterpetedBinaryOperation: IAssembledOperationRequiresGenerator
    {
        public void Init(IAssembledOperationRequiresGenerator left, IAssembledOperationRequiresGenerator right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public abstract void Assemble(AssemblyContextWithGenerator interpetedContext);

        private IAssembledOperationRequiresGenerator? left;
        public IAssembledOperationRequiresGenerator Left { get => left ?? throw new NullReferenceException(nameof(left)); private set => left = value ?? throw new NullReferenceException(nameof(value)); }
        private IAssembledOperationRequiresGenerator? right;
        public IAssembledOperationRequiresGenerator Right { get => right ?? throw new NullReferenceException(nameof(right)); private set => right = value ?? throw new NullReferenceException(nameof(value)); }
    }
}