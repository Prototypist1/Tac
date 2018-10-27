using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class InterpetedStringType : IInterpetedPrimitiveType
    {
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeString("");
        }
    }

    class RunTimeString: IRunTime
    {
        public readonly string s;

        public RunTimeString(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }
    }
}
