using System;
using Prototypist.Toolbox;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    internal class InterpetedReturnOperation : InterpetedTrailingOperation
    {
        public override IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            var argumentResult = Argument.Interpet(interpetedContext);

            if (argumentResult.IsReturn(out var argumentReturned, out var argumentValue))
            {
                return InterpetedResult.Return<IInterpetedMember>(argumentReturned!);
            }

            return InterpetedResult.Return<IInterpetedMember>(argumentValue!);
        }
        
    }
}