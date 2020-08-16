using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tac.SnippetTests
{
    public class InputOutputTest
    {
        [Fact]
        public void StringInput(){
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { "test" });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToInput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Input(intIn ,stringIn,boolIn)},
 @"
entry-point {
    new-empty > (in.read-string);
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void NumberInput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] { 1.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToInput(new bool[] { });

            Tac.Runner.Runner.Run("test",
                new[] {BasicInputOutput.Input(intIn ,stringIn, boolIn) },
@"
entry-point {
    new-empty > (in.read-number);
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }



        [Fact]
        public void BoolInputOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] {  });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToInput(new bool[] { true });

            var (intOut, verifyIntOut) = BasicInputOutput.ToOutput(new double[] { });
            var (stringOut, verifyStringOut) = BasicInputOutput.ToOutput(new string[] { });
            var (boolOut, verifyBoolOut) = BasicInputOutput.ToOutput(new bool[] { true });

            Tac.Runner.Runner.Run("test",
                new[] {
                    BasicInputOutput.Input(intIn, stringIn, boolIn),
                    BasicInputOutput.Output(intOut, stringOut, boolOut) },
@"
entry-point {
    new-empty > (in.read-bool) > (out.write-bool);
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
            verifyIntOut();
            verifyStringOut();
            verifyBoolOut();
        }


        [Fact]
        public void Multiply()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0 , 6.0, 8.0, 10.0});
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     2 * 2 > (out.write-number);
     3 * 2 > (out.write-number);
     4 * 2 > (out.write-number);
     5 * 2 > (out.write-number);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void MultiplyCompact()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0, 6.0, 8.0, 10.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
                @"entry-point{2*2>(out.write-number);3*2>(out.write-number);4*2>(out.write-number);5*2>(out.write-number);};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void Subtract()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 0.0, 1.0, 2.0, 3.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     2 - 2 > (out.write-number);
     3 - 2 > (out.write-number);
     4 - 2 > (out.write-number);
     5 - 2 > (out.write-number);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void Simple()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] {  });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point { };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }



        [Fact]
        public void SimpleAdd()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     2 + 2 > (out.write-number);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void NestedParn()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     ((((((((((2)) + ((2))))))) > (((out.write-number))))));
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }



        [Fact]
        public void Add()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0, 5.0, 6.0, 7.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     2 + 2 > (out . write-number);
     3 + 2 > (out . write-number);
     4 + 2 > (out . write-number);
     5 + 2 > (out . write-number);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void LessThen()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true, false, false});

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     1 <? 2 > (out.write-bool);
     2 <? 2 > (out.write-bool);
     3 <? 2 > (out.write-bool);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void NumberOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 2.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     2 > (out.write-number);
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void Then()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 2.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     true then { 2.0 > (out.write-number); };
     false then { 1.0 > (out.write-number); };
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void Else()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 2.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
 entry-point {
     true else { 1.0 > (out.write-number); };
     false else { 2.0 > (out.write-number); };
 };");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void StringOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { "hello world"  });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
entry-point {
   ""hello world"" > (out.write-string);
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        // types in the backend need to be redone
        [Fact]
        public void OrType1()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 5 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
entry-point {
    5 =: bool | number x;

    x is number y { y > (out.write-number) };
    x is bool y { y > (out.write-bool) };
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        // types in the backend need to be redone
        [Fact]
        public void OrType2()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true });

            Tac.Runner.Runner.Run("test", new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)},
 @"
entry-point {
    true =: (bool|number) x;

    x is number y { y > (out.write-number) };
    x is bool z { z > (out.write-bool) } ;
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }
    }
}
