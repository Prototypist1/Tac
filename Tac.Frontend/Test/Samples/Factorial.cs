using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    internal class WrappedFactorial: Factorial, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return 
                    TokenHelp.File(
                        TokenHelp.Line(
                            TokenHelp.Ele(
                                TokenHelp.Atom("module"),
                                TokenHelp.Atom("factorial"),
                                    TokenHelp.Curl(
                                        TokenHelp.Line(
                                            TokenHelp.Ele(
                                                TokenHelp.Atom("method"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("number"))),
                                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("number")))),
                                                TokenHelp.Atom("input"),
                                                TokenHelp.Curl(
                                                    TokenHelp.Line(
                                                        TokenHelp.Ele(TokenHelp.Atom("input")),
                                                        TokenHelp.Atom("<?"),
                                                        TokenHelp.Ele(TokenHelp.Atom("2")),
                                                        TokenHelp.Atom("then"),
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
                                                                    TokenHelp.Atom(" - "),
                                                                    TokenHelp.Ele(TokenHelp.Atom("1")),
                                                                    TokenHelp.Atom(">"),
                                                                    TokenHelp.Ele(TokenHelp.Atom("fac")),
                                                                    TokenHelp.Atom("*"),
                                                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                                                    TokenHelp.Atom("return"))))))),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Ele(TokenHelp.Atom("fac")))))));
            }
        }
    }
}
