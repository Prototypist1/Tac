using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    internal class WrappedPointObject: PointObject, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("module"),
                            TokenHelp.Atom("point-module"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Ele(
                                           TokenHelp.Atom("object"),
                                           TokenHelp.Curl(
                                               TokenHelp.Line(
                                                    TokenHelp.Ele(TokenHelp.Atom("5")),
                                                    TokenHelp.Atom("=:"),
                                                    TokenHelp.Ele(TokenHelp.Atom("x"))),
                                               TokenHelp.Line(
                                                    TokenHelp.Ele(TokenHelp.Atom("2")),
                                                    TokenHelp.Atom("=:"),
                                                    TokenHelp.Ele(TokenHelp.Atom("y"))))),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("point")))))));
            }
        }
    }


}