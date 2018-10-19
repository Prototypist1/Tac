using System;
using Tac.Semantic_Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMember: IRunTime
    {
        public IRunTime Value { get; set; }

    }
}