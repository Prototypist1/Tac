using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    internal class InterpetedBooleanType : IInterpetedType
    {
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeBoolean(false);
        }
    }

    internal class RunTimeBoolean: IRunTime
    {
        public readonly bool b;

        public RunTimeBoolean(bool b)
        {
            this.b = b;
        }
    }
}
