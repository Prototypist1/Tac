using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class Arithmetic : ITestCase
    {
        public string Text => @"( 2 + 5 ) * ( 2 + 7 ) ;";
        
        public ICodeElement[] CodeElements
        {
            get
            {
                return new[] {
                    new TestMultiplyOperation(
                        new TestAddOperation(
                            new TestConstantNumber(2),
                            new TestConstantNumber(5)),
                        new TestAddOperation(
                            new TestConstantNumber(2),
                            new TestConstantNumber(7)))};
            }
        }
    }
}
