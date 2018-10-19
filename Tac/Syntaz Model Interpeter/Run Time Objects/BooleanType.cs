using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class BooleanType: IRunTime
    {
        public readonly bool b;

        public BooleanType(bool b)
        {
            this.b = b;
        }
    }
}
