using System;
using Prototypist.LeftToRight;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedReturnOperation<T> : InterpetedTrailingOperation<T,T>
        where T: class,IInterpetedData
    {
        public override IInterpetedResult<T> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult<T>.Return(Argument.Interpet(interpetedContext).Value);
        }
        
    }
}