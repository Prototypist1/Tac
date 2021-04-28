//using System;
//using Tac.Backent.Emit._2;
//using Xunit;
//using Tac.Backend.Emit._2.Test;


//namespace Tac.Backend.Emit._2.Test
//{
//    public interface ITest
//    {

//    }

//    public class UnitTest1
//    {

//        public partial class Test
//        {

//        }





//        [Fact]
//        public void Test1()
//        {
//            var x = new Test();
            
//            var y = InteraceEmmiter.EmitOrCast<Test, ITest>(x);

//            Assert.NotNull(y);
//        }


//        [Fact]
//        public void Test2()
//        {
//            var x = new CanI();

//            var y = x as ITest;

//            Assert.NotNull(y);
//        }
//    }
//}

//namespace Tac.Backent.Emit._2
//{

//    public partial class CanI: ITest
//    {

//    }
//}

