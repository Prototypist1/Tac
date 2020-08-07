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
                                TokenHelp.Atom("module"),
                                TokenHelp.Atom("factorial"),
                                    TokenHelp.Curl(
                                        TokenHelp.Line(
                                                TokenHelp.Atom("method"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Atom("number")),
                                                    TokenHelp.Line(TokenHelp.Atom("number"))),
                                                TokenHelp.Atom("input"),
                                                TokenHelp.Curl(
                                                    TokenHelp.Line(
                                                        TokenHelp.Atom("input"),
                                                        TokenHelp.Atom("<?"),
                                                        TokenHelp.Atom("2"),
                                                        TokenHelp.Atom("then"),
                                                            TokenHelp.Curl(
                                                                TokenHelp.Line(
                                                                    TokenHelp.Atom("1"),
                                                                    TokenHelp.Atom("return"))),
                                                        TokenHelp.Atom("else"),
                                                            TokenHelp.Curl(
                                                                TokenHelp.Line(
                                                                    TokenHelp.Atom("input"),
                                                                    TokenHelp.Atom(" - "),
                                                                    TokenHelp.Atom("1"),
                                                                    TokenHelp.Atom(">"),
                                                                    TokenHelp.Atom("fac"),
                                                                    TokenHelp.Atom("*"),
                                                                    TokenHelp.Atom("input"),
                                                                    TokenHelp.Atom("return"))))),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Atom("fac")))));
            }
        }
    }
}
