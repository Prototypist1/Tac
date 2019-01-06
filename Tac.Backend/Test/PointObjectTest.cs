using System.Linq;
using Tac.Backend.Syntaz_Model_Interpeter;
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
            var lines = testCase.CodeElements.Select(x => x.Convert(conversionContext)).ToArray();

            var line = Assert.Single(lines);

            var res = line.Interpet(InterpetedContext.Root());

            Assert.True(res.HasValue);
            Assert.True(res.Get<InterpetedInstanceScope>().ContainsMember(new NameKey("x")));
            Assert.True(res.Get<InterpetedInstanceScope>().ContainsMember(new NameKey("y")));
        }
    }
}
