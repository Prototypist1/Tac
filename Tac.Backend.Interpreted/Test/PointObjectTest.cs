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
            
            Assert.False( testCase.RootScope.Convert(conversionContext).Interpet(InterpetedContext.Root()).IsReturn(out var _, out var res));

            var scope = res!.Value.CastTo<IInterpetedScope>().GetMember(new NameKey("point")).Value;

            Assert.True(scope.CastTo<IInterpetedScope>().ContainsMember(new NameKey("x")));
            Assert.True(scope.CastTo<IInterpetedScope>().ContainsMember(new NameKey("y")));
        }
    }

    public class OrTest
    {
        [Fact]
        public void Test()
        {
            var testCase = new OrTypeSample();
            var conversionContext = new Definitions();

            Assert.False(testCase.RootScope.Convert(conversionContext).Interpet(InterpetedContext.Root()).IsReturn(out var _, out var res));

            Assert.Equal(5, res!.Value.CastTo<IInterpetedScope>().GetMember(new NameKey("x")).Value.Has<IBoxedDouble>().Value);
            Assert.False(res.Value.CastTo<IInterpetedScope>().GetMember(new NameKey("y")).Value.Has<IBoxedBool>().Value);
        }
    }
}
