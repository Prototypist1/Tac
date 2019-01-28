using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Elements
{
    internal class InterpetedExternalMethodDefinition : IInterpeted
    {
        public void Init(InterpetedMemberDefinition parameterDefinition, Func<IRunTime, InterpetedResult> backing)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedExternalMethod(ParameterDefinition,Backing));
        }

        public InterpetedMemberDefinition ParameterDefinition { get; private set; }
        public Func<IRunTime, InterpetedResult> Backing { get; private set; }

    }

}
