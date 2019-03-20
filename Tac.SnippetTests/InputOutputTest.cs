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

            Tac.Runner.Runner.Run(new[] {
                BasicInputOutput.Input(intIn ,stringIn)},
 @"
entry-point [ empty ; empty ] _ {
    object {} > ( input . read-string ) ;
} ;");

            verifyIntIn();
            verifyStringIn();
        }


        [Fact]
        public void NumberInput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToInput(new double[] { 1.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToInput(new string[] { });

            Tac.Runner.Runner.Run(new[] {
                BasicInputOutput.Input(intIn ,stringIn)},
@"
entry-point [ empty ; empty ] _ {
    object {} > ( input . read-number ) ;
} ;");

            verifyIntIn();
            verifyStringIn();
        }


        [Fact]
        public void NumberOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { 2.0 });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { });

            Tac.Runner.Runner.Run(new[] {
                BasicInputOutput.Output(intIn ,stringIn)},
 @"
 entry-point [ empty ; empty ] _ {
     2 > ( output . write-number ) ;
 } ;");

            verifyIntIn();
            verifyStringIn();
        }

        [Fact]
        public void StringOutput()
        {
            var (intIn, verifyIntIn) = BasicInputOutput.ToOutput(new double[] { });
            var (stringIn, verifyStringIn) = BasicInputOutput.ToOutput(new string[] { "hello world"  });

            Tac.Runner.Runner.Run(new[] {
                BasicInputOutput.Output(intIn ,stringIn)},
 @"
entry-point [ empty ; empty ] _ {
   ""hello world"" > ( output . write-string ) ;
};");

            verifyIntIn();
            verifyStringIn();
        }

    }
}
