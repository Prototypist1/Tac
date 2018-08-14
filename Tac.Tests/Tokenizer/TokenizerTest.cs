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
        private static IToken Ele(params IToken[] tokens)
        {
            return new ElementToken(tokens);
        }
        private static IToken Par(params IToken[] tokens)
        {
            return new ParenthesisToken(tokens);
        }
        private static IToken Broke(params IToken[] tokens)
        {
            return new BrokenBracketToken(tokens);
        }
        private static IToken Curl(params IToken[] tokens)
        {
            return new CurleyBacketToken(tokens);
        }
        private static IToken File(params IToken[] tokens)
        {
            return new FileToken(tokens);
        }
        private static IToken Line(params IToken[] tokens)
        {
            return new LineToken(tokens);
        }
        private static IToken Square(params IToken[] tokens)
        {
            return new SquareBacketToken(tokens);
        }
        private static IToken Atom(string s)
        {
            return new AtomicToken(s);
        }

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
                File(
                    Line(
                        Ele(
                            Atom("var"),
                            Atom("fac")),
                        Atom("is-static"),
                        Ele(
                            Atom("method|int|int"),
                            Atom("input"),
                            Curl(
                                Line(
                                    Ele(Atom("input")),
                                    Atom("less-then"),
                                    Ele(Atom("2")),
                                    Atom("if-true"),
                                    Ele(
                                        Curl(
                                            Line(
                                                Ele(Atom("1")),
                                                Atom("return")))),
                                    Atom("else"),
                                    Ele(
                                        Curl(
                                            Line(
                                                Ele(Atom("input")),
                                                Atom("minus"),
                                                Ele(Atom("1")),
                                                Atom("next-call"),
                                                Ele(Atom("fac")),
                                                Atom("times"),
                                                Ele(Atom("input")),
                                                Atom("return")))))))));
            
            Assert.Equal(target, res);
        }

        [Fact]
        public void Test2()
        {
            var text = @"( 2 plus 5 ) times ( 2 plus 7 ) ;";
            var tokenizer = new Tac.Parser.Tokenizer();
            var res = tokenizer.Tokenize(text);

            var target =
                File(
                    Line(
                        Ele(
                            Par(
                                Line(
                                    Ele(Atom("2")),
                                    Atom("plus"),
                                    Ele(Atom("5"))))),
                        Atom("times"),
                         Ele(
                            Par(
                                Line(
                                    Ele(Atom("2")),
                                    Atom("plus"),
                                    Ele(Atom("7")))))));

            Assert.Equal(target, res);
        }
    }
}
