using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedTrailingOperation<TIn,TOut> : IInterpetedOperation<TOut>
        where TIn: class, IInterpetedData
        where TOut : IInterpetedData
    {
        public void Init(TIn argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract IInterpetedResult<TOut> Interpet(InterpetedContext interpetedContext);

        public TIn Argument { get; private set; }
    }
}