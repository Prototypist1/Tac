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
                            TokenHelp.Ele(
                                TokenHelp.Atom("module"),
                                TokenHelp.Atom("closoure"),
                                    TokenHelp.Curl(
                                        TokenHelp.Line(
                                            TokenHelp.Ele(
                                                TokenHelp.Atom("method"),
                                                TokenHelp.Square(
                                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))),
                                                    TokenHelp.Line(TokenHelp.Ele(
                                                        TokenHelp.Atom("method"),
                                                        TokenHelp.Square(
                                                            TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))),
                                                            TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int")))
                                                        )))),
                                                TokenHelp.Atom("x"),
                                                TokenHelp.Curl(
                                                    TokenHelp.Line(
                                                        TokenHelp.Ele(
                                                            TokenHelp.Atom("method"),
                                                            TokenHelp.Square(
                                                                TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))),
                                                                TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int")))),
                                                            TokenHelp.Atom("y"),
                                                            TokenHelp.Curl(
                                                                TokenHelp.Line(
                                                                    TokenHelp.Ele(TokenHelp.Atom("x")),
                                                                    TokenHelp.Atom("+"),
                                                                    TokenHelp.Ele(TokenHelp.Atom("y")),
                                                                    TokenHelp.Atom("=:"),
                                                                    TokenHelp.Ele(TokenHelp.Atom("x"))),
                                                                TokenHelp.Line(
                                                                    TokenHelp.Ele(TokenHelp.Atom("x")),
                                                                    TokenHelp.Atom("return")))),
                                                         TokenHelp.Atom("return")))),
                                            TokenHelp.Atom("=:"),
                                            TokenHelp.Ele(TokenHelp.Atom("create-accululator")))))));
            }
        }
    }
}
