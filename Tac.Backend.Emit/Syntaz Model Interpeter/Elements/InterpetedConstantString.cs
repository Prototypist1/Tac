using System;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedConstantString : IAssembledOperationRequiresGenerator
    {
        public void Init(string value)
        {
            this.value = value;
        }

        private string? value;
        public string Value { get => value ?? throw new NullReferenceException(nameof(value)); private set => this.value = value; }

        public void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            interpetedContext.generator.Emit(System.Reflection.Emit.OpCodes.Ldstr, Value);
        }
    }
}