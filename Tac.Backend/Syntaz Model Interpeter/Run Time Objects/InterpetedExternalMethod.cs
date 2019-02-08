using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedExternalMethod : Run_Time_Objects.IInterpeted
    {
        public InterpetedExternalMethod(
            InterpetedMemberDefinition parameterDefinition,
            Func<Run_Time_Objects.IInterpeted, InterpetedResult> backing)
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        private InterpetedMemberDefinition ParameterDefinition { get; }
        private Func<Run_Time_Objects.IInterpeted, InterpetedResult> Backing { get; }
        
        public InterpetedResult Invoke(Run_Time_Objects.IInterpeted input)
        {
            return Backing(input);
        }
    }
}