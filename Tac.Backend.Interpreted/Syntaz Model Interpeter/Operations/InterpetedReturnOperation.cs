using System;
using Prototypist.Toolbox;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
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