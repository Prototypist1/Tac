using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedTrailingOperation : IInterpetedOperation
    {
        public void Init(IInterpetedOperation argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext);

        private IInterpetedOperation? argument;
        public IInterpetedOperation Argument { get => argument ?? throw new NullReferenceException(nameof(argument)); private set => argument = value ?? throw new NullReferenceException(nameof(value)); }

    }
}