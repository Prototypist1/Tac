using Tac.Parser;
using Tac.TestCases.Samples;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    internal class WrappedMirrorPointImplementation : MirrorPointImplementation, IWrappedTestCase {
        public IToken Token => TokenHelp.File(
                        TokenHelp.Line(
                            TokenHelp.Atom("module"),
                            TokenHelp.Atom("mirror-module"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Atom("implementation"),
                                        TokenHelp.Square(
                                            TokenHelp.Line(
                                                TokenHelp.Atom("type"),
                                                TokenHelp.Curl(
                                                    TokenHelp.Line(
                                                        TokenHelp.Atom("x")),
                                                    TokenHelp.Line(
                                                        TokenHelp.Atom("y")))),
                                            TokenHelp.Line(TokenHelp.Atom("empty")),
                                            TokenHelp.Line(TokenHelp.Atom("empty"))),
                                        TokenHelp.Atom("context"),
                                        TokenHelp.Atom("input"),
                                        TokenHelp.Curl(
                                            TokenHelp.Line(
                                                TokenHelp.Atom("context"),
                                                TokenHelp.Atom("."),
                                                TokenHelp.Atom("x"),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Atom("temp")),
                                            TokenHelp.Line(
                                                TokenHelp.Atom("context"),
                                                TokenHelp.Atom("."),
                                                TokenHelp.Atom("y"),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Par(
                                                    TokenHelp.Line(new[] {
                                                        TokenHelp.Atom("context"),
                                                        TokenHelp.Atom("."),
                                                        TokenHelp.Atom("x") }))),
                                            TokenHelp.Line(
                                                TokenHelp.Atom("temp"),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Par(
                                                    TokenHelp.Line(new[] {
                                                        TokenHelp.Atom("context"),
                                                        TokenHelp.Atom("."),
                                                        TokenHelp.Atom("y") })))),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Atom("mirror")))));
    }
}