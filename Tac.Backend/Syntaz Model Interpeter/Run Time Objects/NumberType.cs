using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    //internal class InterpetedNumberType : IInterpetedType
    //{
    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
    //    {
    //        return new RunTimeNumber(0);
    //    }
    //}

    internal class RuntimeNumber: IRunTime
    {
        public readonly double d;

        public RuntimeNumber(double d)
        {
            this.d = d;
        }
    }
}
