using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class ArithmeticTest
    {
        [Fact]
        public void Test() {
            var testCase = new Arithmetic();
            var conversionContext = new Definitions();
            var lines = testCase.CodeElements.Select(x => x.Convert(conversionContext)).ToArray();

            var line = Assert.Single(lines);

            var res = line.Interpet(InterpetedContext.Root());

            Assert.True(res.HasValue);
            Assert.Equal(63.0, res.Get<RunTimeNumber>().d);
        }
    }
}
