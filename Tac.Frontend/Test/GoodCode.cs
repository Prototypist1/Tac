using Xunit;
using Tac.SemanticModel;
using System.Linq;

namespace Tac.Tests
{
    public class GoodCode {
        [Fact]
        public void TypeInParns()
        {
            var res = TestSupport.Tokenize(
@"entry-point {
    true =: ((((bool)|number)| type { number x; })) x;
}");
            var converted = TestSupport.ConvertToWeak<WeakEntryPointDefinition>(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);


            //var lineOr = Assert.Single(converted.StaticInitialization);
            //lineOr.Is2OrThrow();
        }
    }
}
