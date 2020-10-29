using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Interpreted.Test
{
    public class ArithmeticTest
    {
        [Fact]
        public void Test() {
            var testCase = new Arithmetic();
            var conversionContext = new Definitions();
            var module = testCase.RootScope.Convert(conversionContext).SafeCastTo(out Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements.InterpetedRootScope _);

            var (scope, _) = module.InterpetWithExposedScope(InterpetedContext.Root());

            var x = scope.GetMember(new NameKey("x"));

            Assert.Equal(63.0, x.Value.CastTo<IBoxedDouble>().Value);
        }
    }
}
