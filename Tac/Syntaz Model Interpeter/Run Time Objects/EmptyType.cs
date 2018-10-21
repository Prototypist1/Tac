using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    public class InterpetedEmptyType : IInterpetedPrimitiveType
    {
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new RunTimeEmpty();
        }
    }

    public class RunTimeEmpty: IRunTime
    {
    }
}
