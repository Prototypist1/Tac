using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Model;
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
            var module = testCase.Module.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            var scope = res.Get<IInterpetedScope>();
            var x = scope.GetMember(new NameKey("x"));

            Assert.True(res.HasValue);
            Assert.Equal(63.0, x.Value.Cast<RuntimeNumber>().d);
        }
    }
}
