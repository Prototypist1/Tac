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
            var conversionContext = new Definitions(new ExternalMethodSource(new Dictionary<Guid, InterpetedExternalMethodDefinition>()));
            
            var res = testCase.Module.Convert(conversionContext).Interpet(InterpetedContext.Root());
            
            var scope = res.Get<IInterpetedScope>().GetMember(new NameKey("point")).Value.Cast<InterpetedInstanceScope>();

            
            Assert.True(scope.ContainsMember(new NameKey("x")));
            Assert.True(scope.ContainsMember(new NameKey("y")));
        }
    }
}
