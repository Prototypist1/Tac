using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Xunit;

namespace Tac.Tests.Tokenizer
{
    public class TokenizerTest
    {
        private static IToken CT(params IToken[] tokens) {

            return new CompositToken(tokens);
        }

        private static IToken AT(string s)
        {

            return new AtomicToken(s);
        }

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

            var target =
                CT(
                    CT(
                        AT("var"),
                        AT("fac")),
                    AT("is-static"),
                    CT(
                        AT("method|int|int"),
                        AT("input"),
                        CT(
                            AT("{"),
                            CT(
                                CT(AT("input")),
                                AT("less-then"),
                                CT(AT("2")),
                                AT("if-true"),
                                CT(
                                    AT("{"),
                                    CT(
                                        CT(AT("1")),
                                        AT("return"),
                                        AT(";")),
                                    AT("}")),
                                AT("else"),
                                CT(
                                    AT("{"),
                                    CT(
                                        CT(AT("input")),
                                        AT("minus"),
                                        CT(AT("1")),
                                        AT("next-call"),
                                        CT(AT("fac")),
                                        AT("times"),
                                        CT(AT("input")),
                                        AT("return"),
                                        AT(";")),
                                    AT("}")),
                                AT(";")),
                            AT("}"))),
                    AT(";"));
            
            Assert.Equal(target, res);
        }
    }
}
