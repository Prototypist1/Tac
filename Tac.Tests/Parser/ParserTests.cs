using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Tests.Tokenizer;
using Xunit;

namespace Tac.Tests.Parser
{
    public class ParserTests
    {
        [Fact]
        public void Test2(){
            var token =
                TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Par(
                                TokenHelp.Ele(TokenHelp.Atom("2")),
                                TokenHelp.Atom("plus"),
                                TokenHelp.Ele(TokenHelp.Atom("5")))),
                        TokenHelp.Atom("times"),
                        TokenHelp.Ele(
                            TokenHelp.Par(
                                TokenHelp.Ele(TokenHelp.Atom("2")),
                                TokenHelp.Atom("plus"),
                                TokenHelp.Ele(TokenHelp.Atom("7"))))));

            var res = TokenParser.ParseFile(token);
        }
    }
}
