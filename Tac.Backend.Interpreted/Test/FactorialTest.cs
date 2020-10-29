using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Interpreted.Test
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
            var module = testCase.RootScope.Convert(conversionContext).SafeCastTo(out Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements.InterpetedRootScope _);

            var (scope, res) = module.InterpetWithExposedScope(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var value));

            // no way this cast works...
            var method = scope.GetMember(new NameKey("fac"));

            Assert.False(method.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(d))).IsReturn(out var _, out var methodReturn));

            return methodReturn!.Value.CastTo<IBoxedDouble>().Value;
        }

    }
}
