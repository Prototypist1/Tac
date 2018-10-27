using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class InterpetedBooleanType : IInterpetedPrimitiveType
    {
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeBoolean(false);
        }
    }

    public class RunTimeBoolean: IRunTime
    {
        public readonly bool b;

        public RunTimeBoolean(bool b)
        {
            this.b = b;
        }
    }
}
