//using System;
//using System.Collections.Generic;
//using System.Text;

using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedAnyType : IInterpeted { }


    public abstract class RunTimeAny: IInterpetedAnyType { }
}

//namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
//{
//    //internal class InterpetedAnyType : IInterpetedType
//    //{
//    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
//    //    {
//    //        return new RuntimeAnyType();
//    //    }
//    //}



//    internal class RuntimeAnyType : IInterpeted
//    {
//    }
//}
