using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedTrailingOperation<TIn,TOut> : IInterpetedOperation<TOut>
        where TOut : IInterpetedAnyType
        where TIn : IInterpetedAnyType
    {
        public void Init(IInterpetedOperation<TIn> argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract IInterpetedResult<IInterpetedMember<TOut>> Interpet(InterpetedContext interpetedContext);

        private IInterpetedOperation<TIn>? argument;
        public IInterpetedOperation<TIn> Argument { get => argument ?? throw new NullReferenceException(nameof(argument)); private set => argument = value ?? throw new NullReferenceException(nameof(value)); }

    }
}