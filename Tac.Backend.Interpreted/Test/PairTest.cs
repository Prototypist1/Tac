using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Interpreted.Test
{
    public class PairTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Test(double input)
        {
            Pairify(input);
        }

        private void Pairify(double d)
        {
            var testCase = new PairType();
            var conversionContext = new Definitions();

            var res = testCase.RootScope.Convert(conversionContext).Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var scope));

            var method = scope!.Value.Has<IInterpetedScope>().GetMember(new NameKey("pairify"));

            Assert.False( method.Value.Has<IInterpetedMethod>().Invoke(TypeManager.NumberMember(TypeManager.Double(d))).IsReturn(out var _, out var methodResult));

            Assert.Equal(d, methodResult!.Value.Has<IInterpetedScope>().GetMember(new NameKey("x")).Value.Has<IBoxedDouble>().Value);
            Assert.Equal(d, methodResult!.Value.Has<IInterpetedScope>().GetMember(new NameKey("y")).Value.Has<IBoxedDouble>().Value);
        }
    }
}
