using Newtonsoft.Json;
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
        1 return ;
    } else {
        input minus 1 next-call fac times input return ;      
    } ;
} ;";
            var tokenizer = new Tac.Parser.Tokenizer();
            var res = tokenizer.Tokenize(text);

            var target =
                TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("var"),
                            TokenHelp.Atom("fac")),
                        TokenHelp.Atom("is-static"),
                        TokenHelp.Ele(
                            TokenHelp.Atom("method|int|int"),
                            TokenHelp.Atom("input"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                    TokenHelp.Atom("less-then"),
                                    TokenHelp.Ele(TokenHelp.Atom("2")),
                                    TokenHelp.Atom("if-true"),
                                    TokenHelp.Ele(
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Ele(TokenHelp.Atom("1")),
                                                TokenHelp.Atom("return")))),
                                    TokenHelp.Atom("else"),
                                    TokenHelp.Ele(
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Ele(TokenHelp.Atom("input")),
                                                TokenHelp.Atom("minus"),
                                                TokenHelp.Ele(TokenHelp.Atom("1")),
                                                TokenHelp.Atom("next-call"),
                                                TokenHelp.Ele(TokenHelp.Atom("fac")),
                                                TokenHelp.Atom("times"),
                                                TokenHelp.Ele(TokenHelp.Atom("input")),
                                                TokenHelp.Atom("return")))))))));
            
            Assert.Equal(target, res);
        }

        [Fact]
        public void Test2()
        {
            var text = @"( 2 plus 5 ) times ( 2 plus 7 ) ;";
            var tokenizer = new Tac.Parser.Tokenizer();
            var res = tokenizer.Tokenize(text);

            var target =
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

            Assert.Equal(target, res);
        }
    }
}
