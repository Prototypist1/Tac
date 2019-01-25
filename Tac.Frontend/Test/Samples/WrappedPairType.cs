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
                        TokenHelp.Ele(
                            TokenHelp.Atom("module"),
                            TokenHelp.Atom("pair-type"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Ele(
                                        TokenHelp.Atom("type"),
                                        TokenHelp.Square(
                                            TokenHelp.Line(
                                                TokenHelp.Ele(
                                                    TokenHelp.Atom("T")))),
                                        TokenHelp.Atom("pair"),
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Ele(
                                                    TokenHelp.Atom("T"),
                                                    TokenHelp.Atom("x"))),
                                            TokenHelp.Line(
                                                TokenHelp.Ele(
                                                    TokenHelp.Atom("T"),
                                                    TokenHelp.Atom("y")))))),
                                TokenHelp.Line(
                                    TokenHelp.Ele(
                                        TokenHelp.Atom("method"),
                                        TokenHelp.Square(
                                            TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))),
                                            TokenHelp.Line(
                                                TokenHelp.Ele(TokenHelp.Atom("pair"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))))))),
                                        TokenHelp.Atom("input"),
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Ele(
                                                    TokenHelp.Atom("object"),
                                                    TokenHelp.Curl(
                                                        TokenHelp.Line(
                                                            TokenHelp.Ele(TokenHelp.Atom("input")),
                                                            TokenHelp.Atom("=:"),
                                                            TokenHelp.Ele(TokenHelp.Atom("x"))),
                                                        TokenHelp.Line(
                                                            TokenHelp.Ele(TokenHelp.Atom("input")),
                                                            TokenHelp.Atom("=:"),
                                                            TokenHelp.Ele(TokenHelp.Atom("y"))))),
                                                TokenHelp.Atom("return")))),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("pairify")))))));
            }
        }
    }
}