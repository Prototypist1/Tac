//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Tac.Syntaz_Model_Interpeter.Run_Time_Objects
//{
//    //internal class InterpetedStringType : IInterpetedType
//    //{
//    //    public IRunTime GetDefault(InterpetedContext interpetedContext)
//    //    {
//    //        return new RunTimeString("");
//    //    }
//    //}


//    internal class RunTimeString : IInterpetedMember<string>
//    {
//        private string value;

//        public string Value
//        {
//            get
//            {
//                return value;
//            }
//            set
//            {
//                this.value = value ?? throw new ArgumentNullException(nameof(value));
//            }
//        }

//        public RunTimeString(string s)
//        {
//            this.Value = s ?? throw new ArgumentNullException(nameof(s));
//        }
//    }
//}
