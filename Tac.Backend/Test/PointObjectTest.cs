using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Test
{
    public class PointObjectTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new PointObject();
            var conversionContext = new Definitions();
            
            Assert.False( testCase.Module.Convert(conversionContext).Interpet(InterpetedContext.Root()).IsReturn(out var _, out var res));

            var scope = res.Value.Cast<IInterpetedScope>().GetMember<IInterpetedAnyType>(new NameKey("point")).Value;

            Assert.True(scope.Cast<IInterpetedScope>().ContainsMember(new NameKey("x")));
            Assert.True(scope.Cast<IInterpetedScope>().ContainsMember(new NameKey("y")));
        }
    }

    public class OrTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new OrTypeSample();
            var conversionContext = new Definitions();

            Assert.False(testCase.Module.Convert(conversionContext).Interpet(InterpetedContext.Root()).IsReturn(out var _, out var res));

            Assert.Equal(5, res.Value.Cast<IInterpetedScope>().GetMember<>(new NameKey("x")).Value);

            var y = res.Value.Cast<IInterpetedScope>().GetMember<IInterpetedAnyType>(new NameKey("y")).Value;
        }
    }
}
