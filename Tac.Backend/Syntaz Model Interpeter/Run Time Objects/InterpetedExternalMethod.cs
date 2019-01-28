using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedExternalMethod : IRunTime
    {
        public InterpetedExternalMethod(
            InterpetedMemberDefinition parameterDefinition,
            Func<IRunTime, InterpetedResult> backing)
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        private InterpetedMemberDefinition ParameterDefinition { get; }
        private Func<IRunTime, InterpetedResult> Backing { get; }
        
        public InterpetedResult Invoke(IRunTime input)
        {
            return Backing(input);
        }
    }
}