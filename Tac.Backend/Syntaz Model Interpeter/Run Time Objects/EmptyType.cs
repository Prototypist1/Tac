using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
{
    //internal class InterpetedEmptyType : IInterpetedType
    //{
    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
    //    {
    //        return new RunTimeEmpty();
    //    }
    //}

    public interface IInterpedEmpty : IInterpetedAnyType
    {

    }

    internal class RunTimeEmpty : RunTimeAny, IInterpedEmpty
    {
    }
}
