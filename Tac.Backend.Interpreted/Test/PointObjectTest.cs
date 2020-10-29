using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Tests.Samples;
using Xunit;

namespace Tac.Backend.Interpreted.Test
{
    public class PointObjectTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new PointObject();
            var conversionContext = new Definitions();

            var module = testCase.RootScope.Convert(conversionContext).SafeCastTo(out Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements.InterpetedRootScope _);

            var (scope, value) = module.InterpetWithExposedScope(InterpetedContext.Root());

            Assert.False(value.IsReturn(out var _, out var res));

            var pointScope = scope.GetMember(new NameKey("point")).Value;

            Assert.True(pointScope.CastTo<IInterpetedScope>().ContainsMember(new NameKey("x")));
            Assert.True(pointScope.CastTo<IInterpetedScope>().ContainsMember(new NameKey("y")));
        }
    }

    public class OrTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new OrTypeSample();
            var conversionContext = new Definitions();

            var module = testCase.RootScope.Convert(conversionContext).SafeCastTo(out Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements.InterpetedRootScope _);

            var (scope, value) = module.InterpetWithExposedScope(InterpetedContext.Root());

            Assert.False(value.IsReturn(out var _, out var res));

            Assert.Equal(5, scope.GetMember(new NameKey("x")).Value.Has<IBoxedDouble>().Value);
            Assert.False(scope.GetMember(new NameKey("y")).Value.Has<IBoxedBool>().Value);
        }
    }
}
