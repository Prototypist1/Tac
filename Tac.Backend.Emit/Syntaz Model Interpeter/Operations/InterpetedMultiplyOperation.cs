using System;
using Prototypist.Toolbox;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedMultiplyOperation : InterpetedBinaryOperation
    {
        public override void Assemble(AssemblyContextWithGenerator interpetedContext)
        {

            Left.Assemble(interpetedContext);
            Right.Assemble(interpetedContext);
            interpetedContext.generator.Emit(System.Reflection.Emit.OpCodes.Mul_Ovf);
        }
    }
}