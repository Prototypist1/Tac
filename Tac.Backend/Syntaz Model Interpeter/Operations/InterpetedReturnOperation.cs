using System;
using Prototypist.LeftToRight;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedReturnOperation : InterpetedTrailingOperation
    {
        public override InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Return(Argument.Interpet(interpetedContext).GetAndUnwrapMemberWhenNeeded());
        }
        
    }
}