using System;
using System.Collections.Generic;
using System.Text;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Run_Time_Objects
{

    internal class InterpetedEmptyInstance : IInterpetedOperation<IInterpedEmpty>
    {
        public void Init()
        {
        }

        public IInterpetedResult<IInterpetedMember<IInterpedEmpty>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.Member(TypeManager.Empty()));
        }
    }
}
