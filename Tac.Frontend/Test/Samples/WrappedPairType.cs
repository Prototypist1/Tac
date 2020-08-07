using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    internal class WrappedPairType : PairType, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Atom("module"),
                        TokenHelp.Atom("pair-type"),
                        TokenHelp.Curl(
                            TokenHelp.Line(
                                TokenHelp.Atom("type"),
                                TokenHelp.Square(
                                    TokenHelp.Line(
                                        TokenHelp.Atom("T"))),
                                TokenHelp.Atom("pair"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Atom("T"),
                                        TokenHelp.Atom("x")),
                                    TokenHelp.Line(
                                        TokenHelp.Atom("T"),
                                        TokenHelp.Atom("y")))),
                            TokenHelp.Line(
                                TokenHelp.Atom("method"),
                                TokenHelp.Square(
                                    TokenHelp.Line(TokenHelp.Atom("number")),
                                    TokenHelp.Line(
                                        TokenHelp.Atom("pair"),
                                        TokenHelp.Square(
                                            TokenHelp.Line(TokenHelp.Atom("number"))))),
                                TokenHelp.Atom("input"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                            TokenHelp.Atom("object"),
                                            TokenHelp.Curl(
                                                TokenHelp.Line(
                                                    TokenHelp.Atom("input"),
                                                    TokenHelp.Atom("=:"),
                                                    TokenHelp.Atom("x")),
                                                TokenHelp.Line(
                                                    TokenHelp.Atom("input"),
                                                    TokenHelp.Atom("=:"),
                                                    TokenHelp.Atom("y"))),
                                        TokenHelp.Atom("return"))),
                                TokenHelp.Atom("=:"),
                                    TokenHelp.Atom("pairify")))));
            }
        }
    }
}