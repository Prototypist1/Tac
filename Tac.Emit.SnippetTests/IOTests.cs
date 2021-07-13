using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tac.Backend.Emit.Walkers;
using Tac.Emit.Runner;
using Xunit;

namespace Tac.Emit.SnippetTests
{
    public class IOTests
    {
        [Fact]
        public void StringInput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { "test" });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToInput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
entry-point [empty; empty;] input {
    new-empty > (in.read-string);
    new-empty return;
};", new Empty(), new[] {
                BasicInputOutput.Input(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point [empty; empty;] input {
    new-empty > (in.read-number);
    new-empty return;
};", new Empty(),
                new[] { BasicInputOutput.Input(intIn, stringIn, boolIn) });

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }



        [Fact]
        public void BoolInputOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToInput(new bool[] { true });

            var (intOut, verifyIntOut) = BasicInputOutput.ToOutput(new double[] { });
            var (stringOut, verifyStringOut) = BasicInputOutput.ToOutput(new string[] { });
            var (boolOut, verifyBoolOut) = BasicInputOutput.ToOutput(new bool[] { true });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point [empty; empty;] input {
    new-empty > (in.read-bool) > (out.write-bool);
    new-empty return;
};", new Empty(),
                new[] {
                    BasicInputOutput.Input(intIn, stringIn, boolIn),
                    BasicInputOutput.Output(intOut, stringOut, boolOut) });

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
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 4.0, 6.0, 8.0, 10.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    2 * 2 > (out.write-number);
    3 * 2 > (out.write-number);
    4 * 2 > (out.write-number);
    5 * 2 > (out.write-number);
    new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
                @"entry-point[empty; empty;] input {2*2>(out.write-number);3*2>(out.write-number);4*2>(out.write-number);5*2>(out.write-number);new-empty return;};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    2 - 2 > (out.write-number);
    3 - 2 > (out.write-number);
    4 - 2 > (out.write-number);
    5 - 2 > (out.write-number);
    new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void Simple()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input { new-empty return; };", new Empty(), Array.Empty<AsseblyPendingType>());

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

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point [empty; empty;] input {
    2 + 2 > (out.write-number);
    new-empty return;
};", new Empty(), new[] { 
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        // good evidence that in/out are breaking it 
        [Fact]
        public void SimplistAdd()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    2 + 2;
    new-empty return;
 };", new Empty(), Array.Empty<AsseblyPendingType>());

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    ((((((((((2)) + ((2))))))) > (((out.write-number))))));
    new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    2 + 2 > (out . write-number);
    3 + 2 > (out . write-number);
    4 + 2 > (out . write-number);
    5 + 2 > (out . write-number);
    new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void LessThen()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true, false, false });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
    1 <? 2 > (out.write-bool);
    2 <? 2 > (out.write-bool);
    3 <? 2 > (out.write-bool);
    new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
     2 > (out.write-number);
     new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
     true then { 2.0 > (out.write-number); };
     false then { 1.0 > (out.write-number); };
     new-empty return;
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
 entry-point [empty; empty;] input {
     true else { 1.0 > (out.write-number); };
     false else { 2.0 > (out.write-number); };
 };", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void StringOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { "hello world" });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
 @"
entry-point [empty; empty;] input {
   ""hello world"" > (out.write-string);
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
entry-point  [empty; empty;] input{
    5 =: bool | number x;

    x is number y { y > (out.write-number) };
    x is bool y { y > (out.write-bool) };
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

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

            Run.CompileAndRun<Empty, Empty>("test",
 @"
entry-point [empty; empty;] input {
    true =: (bool|number) x;

    x is number y { y > (out.write-number) };
    x is bool z { z > (out.write-bool) } ;
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void OrType3()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point [empty; empty;] input {
    true =: bool | bool x;

    x  > (out.write-bool) ;
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void OrType4()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 5 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point  [empty; empty;] input{
    object { true =: bool b; 5 =: number a } =: type { bool b; number a;} | type { bool b; number num;} x;

    x.b  > (out.write-bool) ;
    x is type { bool b; number a;} z { z.a > (out.write-number) } ;
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void OrType5()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { true });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point  [empty; empty;] input{
    true =: bool | ( bool| bool) | bool x;

    x  > (out.write-bool) ;
};", new Empty(), new[] {
                    BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }

        [Fact]
        public void OrType6()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 5 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point  [empty; empty;] input{
    object { true =: bool b; 5 =: number a } =: type { bool b; number a;} | bool x;

    x is type { bool b; number a;} z { z.a > (out.write-number) } ;
};", new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }


        [Fact]
        public void OrType7()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 5 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });
            var (boolIn, verifyBoolIn) = BasicInputOutput.ToOutput(new bool[] { });

            Run.CompileAndRun<Empty, Empty>("test",
@"
entry-point [empty; empty;] input {
    object { true =: bool b; 5 =: number a } =: type { bool b; number a;} | type { bool b; number a;} x;

    x.a is number a { a > out.write-number };
};",
new Empty(), new[] {
                BasicInputOutput.Output(intIn ,stringIn,boolIn)});

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }
    }
}
