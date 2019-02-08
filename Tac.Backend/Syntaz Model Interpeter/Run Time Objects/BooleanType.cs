using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    //internal class InterpetedBooleanType : IInterpetedType
    //{
    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
    //    {
    //        return new RunTimeBoolean(false);
    //    }
    //}

    public interface IInterpetedBoolean : IInterpeted{
        bool Value { get; }
    }

    internal class RunTimeBoolean : IInterpetedBoolean
    {
        public bool Value { get;  }

        public RunTimeBoolean(bool b)
        {
            this.Value = b;
        }
    }
}
