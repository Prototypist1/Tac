using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public interface IInterpetedNumber {
        double Value { get; }
    }

    internal class RuntimeNumber: IInterpeted
    {
        public double Value { get; }

        public RuntimeNumber(double d)
        {
            this.Value = d;
        }
    }
}
