using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    class StringType: IRunTime
    {
        public readonly string s;

        public StringType(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }
    }
}
