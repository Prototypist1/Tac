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
entry-point [ empty ; empty ] _ {
    new-empty > ( input . read-string ) ;
} ;");

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
entry-point [ empty ; empty ] _ {
    new-empty > ( input . read-number ) ;
} ;");

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
entry-point [ empty ; empty ] _ {
    new-empty > ( input . read-bool ) > ( output . write-bool ) ;
} ;");

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
 entry-point [ empty ; empty ] _ {
     2 * 2 > ( output . write-number ) ;
     3 * 2 > ( output . write-number ) ;
     4 * 2 > ( output . write-number ) ;
     5 * 2 > ( output . write-number ) ;
 } ;");

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
 entry-point [ empty ; empty ] _ {
     2 - 2 > ( output . write-number ) ;
     3 - 2 > ( output . write-number ) ;
     4 - 2 > ( output . write-number ) ;
     5 - 2 > ( output . write-number ) ;
 } ;");

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
 entry-point [ empty ; empty ] _ {
     2 + 2 > ( output . write-number ) ;
     3 + 2 > ( output . write-number ) ;
     4 + 2 > ( output . write-number ) ;
     5 + 2 > ( output . write-number ) ;
 } ;");

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
 entry-point [ empty ; empty ] _ {
     1 <? 2 > ( output . write-bool ) ;
     2 <? 2 > ( output . write-bool ) ;
     3 <? 2 > ( output . write-bool ) ;
 } ;");

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
 entry-point [ empty ; empty ] _ {
     2 > ( output . write-number ) ;
 } ;");

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
entry-point [ empty ; empty ] _ {
   ""hello world"" > ( output . write-string ) ;
};");

            verifyIntIn();
            verifyStringIn();
            verifyBoolIn();
        }
    }
}
