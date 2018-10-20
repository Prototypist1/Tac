using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class InterpetedNumberType : IInterpetedPrimitiveType {

    }

    class RunTimeNumber: IRunTime
    {
        public readonly double d;

        public RunTimeNumber(double d)
        {
            this.d = d;
        }
    }
}
