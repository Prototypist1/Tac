using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tac.Tests.Tokenizer
{
    public class TokenizerTest
    {
        [Fact]
        public void Test1() {
            var text =
@"var fac is-static method|int|int input {
    input less-then 2 if-true {
        1 return;
    } else {
        input minus 1 next-call fac times input return;      
    };
};";
            var operations = Tac.Parser.Operations.StandardOperations.Value;
            var tokenizer = new Tac.Parser.Tokenizer();
            var res = tokenizer.SmartSplitBase(text, operations.AllOperationKeys);
        }
    }
}
