using System;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedConstantNumber : IAssembledOperationRequiresGenerator
    {
        public void Init(double value) {
            this.Value = value;
        }

        public double Value { get; private set; }

        public void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            interpetedContext.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, Value);
        }
    }
}