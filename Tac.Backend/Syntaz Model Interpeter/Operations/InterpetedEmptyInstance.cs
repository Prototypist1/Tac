using System;
using System.Collections.Generic;
using System.Text;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Run_Time_Objects
{

    internal class InterpetedEmptyInstance : IInterpetedOperation<RunTimeEmpty>
    {
        public void Init()
        {
        }

        public IInterpetedResult<IInterpetedMember<RunTimeEmpty>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMember<RunTimeEmpty>(new RunTimeEmpty()));
        }
    }
}
