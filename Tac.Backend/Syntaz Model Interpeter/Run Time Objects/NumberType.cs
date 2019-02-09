using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public interface IInterpetedNumber : IInterpetedData
    {
        double Value { get; }
    }

    internal class RuntimeNumber: IInterpetedNumber
    {
        public double Value { get; }

        public RuntimeNumber(double d)
        {
            this.Value = d;
        }
    }
}
