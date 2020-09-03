using System;
using Prototypist.Toolbox;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedReturnOperation : InterpetedTrailingOperation
    {
        public override void Assemble(AssemblyContextWithGenerator interpetedContext)
        {
            var argumentResult = Argument.Assemble(interpetedContext);

            if (argumentResult.IsReturn(out var argumentReturned, out var argumentValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(argumentReturned!);
            }

            return InterpetedResult.Return<IInterpetedMember>(argumentValue!);
        }
        
    }
}