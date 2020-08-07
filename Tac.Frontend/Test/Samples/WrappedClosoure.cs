using Tac.Parser;
using Tac.Tests.Samples;
using Tac.Tests.Tokenizer;

namespace Tac.Frontend.Test.Samples
{
    internal class WrappedClosoure : Closoure, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return
                    TokenHelp.File(
                        TokenHelp.Line(
                            TokenHelp.Atom("module"),
                            TokenHelp.Atom("closoure"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Atom("method"),
                                        TokenHelp.Square(
                                            TokenHelp.Line(TokenHelp.Atom("number")),
                                            TokenHelp.Line(
                                                TokenHelp.Atom("method"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Atom("number")),
                                                    TokenHelp.Line(TokenHelp.Atom("number"))
                                                ))),
                                        TokenHelp.Atom("x"),
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Atom("method"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Atom("number")),
                                                    TokenHelp.Line(TokenHelp.Atom("number"))),
                                                TokenHelp.Atom("y"),
                                                TokenHelp.Curl(
                                                    TokenHelp.Line(
                                                        TokenHelp.Atom("x"),
                                                        TokenHelp.Atom("+"),
                                                        TokenHelp.Atom("y"),
                                                        TokenHelp.Atom("=:"),
                                                        TokenHelp.Atom("x")),
                                                    TokenHelp.Line(
                                                        TokenHelp.Atom("x"),
                                                        TokenHelp.Atom("return"))),
                                                    TokenHelp.Atom("return"))),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Atom("create-accululator")))));
            }
        }
    }
}
