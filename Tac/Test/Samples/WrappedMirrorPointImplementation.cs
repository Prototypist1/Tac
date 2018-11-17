using Tac.Parser;
using Tac.TestCases.Samples;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class WrappedMirrorPointImplementation : MirrorPointImplementation, IWrappedTestCase {
        public IToken Token => TokenHelp.File(
                        TokenHelp.Line(
                            TokenHelp.Ele(
                                TokenHelp.Atom("implementation"),
                                TokenHelp.Square(
                                    TokenHelp.Line(
                                        TokenHelp.Ele(
                                            TokenHelp.Atom("type"),
                                            TokenHelp.Curl(
                                                TokenHelp.Line(
                                                    TokenHelp.Ele(TokenHelp.Atom("x")),
                                                    TokenHelp.Ele(TokenHelp.Atom("y")))))),
                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("empty"))),
                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("empty")))),
                                TokenHelp.Atom("context"),
                                TokenHelp.Atom("input"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Ele(TokenHelp.Atom("context")),
                                        TokenHelp.Atom("."),
                                        TokenHelp.Ele(TokenHelp.Atom("x")),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("temp"))),
                                    TokenHelp.Line(
                                        TokenHelp.Ele(TokenHelp.Atom("context")),
                                        TokenHelp.Atom("."),
                                        TokenHelp.Ele(TokenHelp.Atom("y")),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("context")),
                                        TokenHelp.Atom("."),
                                        TokenHelp.Ele(TokenHelp.Atom("x"))),
                                    TokenHelp.Line(
                                        TokenHelp.Ele(TokenHelp.Atom("temp")),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("context"))),
                                        TokenHelp.Atom("."),
                                        TokenHelp.Ele(TokenHelp.Atom("y"))))));
    }
}