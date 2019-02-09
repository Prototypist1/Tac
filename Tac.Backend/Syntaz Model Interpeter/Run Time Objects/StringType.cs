using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    //internal class InterpetedStringType : IInterpetedType
    //{
    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
    //    {
    //        return new RunTimeString("");
    //    }
    //}

    public interface IInterpetedString : IInterpetedData
    {
        string Value { get; }
    }

    internal class RunTimeString: IInterpetedString
    {
        public string Value { get; }

        public RunTimeString(string s)
        {
            this.Value = s ?? throw new ArgumentNullException(nameof(s));
        }
    }
}
