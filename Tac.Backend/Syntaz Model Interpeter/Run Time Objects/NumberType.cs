using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    internal class RuntimeNumber: IInterpetedMember<double>
    {
        public double Value { get; set; }

        public RuntimeNumber(double d)
        {
            this.Value = d;
        }
    }
}
