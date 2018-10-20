using System;
using Tac.Semantic_Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMember: IRunTime
    {
        public InterpetedMember(IRunTime value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IRunTime Value { get; set; }

    }
}