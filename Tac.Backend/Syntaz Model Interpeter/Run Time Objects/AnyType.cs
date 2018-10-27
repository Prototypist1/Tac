using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class InterpetedAnyType : IInterpetedPrimitiveType
    {
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RuntimeAnyType();
        }
    }

    public class RuntimeAnyType: IRunTime
    {
    }
}
