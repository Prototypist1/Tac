using System;
using Tac.Model.Elements;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedConstantBool : IAssembledOperationRequiresGenerator
    {
        public void Init(bool value)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }

        public void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            if (Value)
            {
                interpetedContext.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
            }
            else
            {
                interpetedContext.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
            }
        }
    }
}