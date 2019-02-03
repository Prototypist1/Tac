using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{

    public class FactorialTest
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 6)]
        [InlineData(4, 24)]
        public void Test(double input, double output)
        {
            Assert.Equal(output, Fac(input));
        }
        
        private double Fac(double d)
        {
            var testCase = new Factorial();
            var conversionContext = new Definitions();
            var module = testCase.Module.Convert(conversionContext);

            var res = module.Interpet(InterpetedContext.Root());

            var scope = res.Get<IInterpetedScope>();
            var method = scope.GetMember(new NameKey("fac")).Value.Cast<InterpetedMethod>();
 
            return method.Invoke(new RuntimeNumber(d)).Get<RuntimeNumber>().d;
        }

    }
}
