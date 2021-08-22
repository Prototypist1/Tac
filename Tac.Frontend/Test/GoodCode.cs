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
@"entry-point [empty;empty;] unused {
    true =: ((((bool)|number)| type { number x; })) x;
}");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);
        }

        [Fact]
        public void Method()
        {
            var res = TestSupport.Tokenize(
    @"entry-point [empty;empty;] unused {
        true =: bool | method[bool;bool;] x;
    }");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);

        }


        [Fact]
        public void NoEntryPoint()
        {
            var res = TestSupport.Tokenize(
@"
2 =: x;
");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);

        }

        [Fact]
        public void ReturnFromRootScope()
        {
            var res = TestSupport.Tokenize(
@"
2 =: x;
entry-point [empty;number;] unused {
       x return;
    }
");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);

        }

        [Fact]
        public void GenericMethod() {
            var res = TestSupport.Tokenize(
@"
method [T] [T;T;] t { t return; } =: x;
");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);
        }

        [Fact]
        public void ComplexMethod()
        {
            var res = TestSupport.Tokenize(
    @"entry-point [empty;empty;] unused {
        true =: bool | method[method[bool;bool;];method[bool;bool;];] x;
    }");
            var converted = TestSupport.ConvertToWeak(res);

            var errors = converted.Validate().ToArray();

            Assert.Empty(errors);

        }
    }
}
