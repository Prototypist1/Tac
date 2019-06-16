using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
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

            var res = testCase.Module.Convert(conversionContext).Interpet(InterpetedContext.Root());

            Assert.False(res.IsReturn(out var _, out var scope));

            var method = scope.Cast<IInterpetedMember<IInterpetedScope>>().Value.GetMember<IInterpetedMethod<IBoxedDouble,IInterpetedScope>>(new NameKey("pairify"));

            Assert.False( method.Value.Invoke(TypeManager.NumberMember(TypeManager.Double(d))).IsReturn(out var _, out var methodResult));

            Assert.Equal(d, methodResult.Value.GetMember<IBoxedDouble>(new NameKey("x")).Value.Value);
            Assert.Equal(d, methodResult.Value.GetMember<IBoxedDouble>(new NameKey("y")).Value.Value);
        }
    }
}
