using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal abstract class InterpetedTrailingOperation : IInterpeted
    {
        public void Init(IInterpeted argument)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public abstract InterpetedResult Interpet(InterpetedContext interpetedContext);

        public IInterpeted Argument { get; private set; }
    }
}