using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class FactorialTest
    {
        [Theory]
        [InlineData(0,1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 6)]
        [InlineData(4, 24)]
        public void Test(double input,double output)
        {
            Assert.Equal(output, Fac(input));
        }

        // TODO you are here
        // the issue now is with finialzed scope
        // it does not carry everything forward
        // 

        private double Fac(double d) {
            var testCase = new Factorial();
            var conversionContext = new Definitions();
            var lines = testCase.CodeElements.Select(x => x.Convert(conversionContext)).ToArray();

            var rootScope = InterpetedContext.Root();

            var method = Assert.Single(lines).Interpet(rootScope).Get<InterpetedMethod>();

            var scope = rootScope.Child(InterpetedInstanceScope.Make((new NameKey("fac"), new InterpetedMember(method))));

            return method.Invoke(scope, new RuntimeNumber(d)).Get<RuntimeNumber>().d;
        }

    }
}
