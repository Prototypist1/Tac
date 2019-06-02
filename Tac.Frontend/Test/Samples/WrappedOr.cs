using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    internal class WrappedOr : OrTypeSample, IWrappedTestCase
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
                                TokenHelp.Atom("or-test"),
                                    TokenHelp.Curl(
                                        TokenHelp.Line(
                                                TokenHelp.Ele(TokenHelp.Atom("5")),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Ele(
                                                    TokenHelp.Par(
                                                        TokenHelp.Ele(TokenHelp.Atom("bool")),
                                                        TokenHelp.Atom("|"),
                                                        TokenHelp.Ele(TokenHelp.Atom("int"))),
                                                    TokenHelp.Atom("x"))), 
                                        TokenHelp.Line(
                                                TokenHelp.Ele(TokenHelp.Atom("false")),
                                                TokenHelp.Atom("=:"),
                                                TokenHelp.Ele(
                                                    TokenHelp.Par(
                                                        TokenHelp.Ele(TokenHelp.Atom("bool")),
                                                        TokenHelp.Atom("|"),
                                                        TokenHelp.Ele(TokenHelp.Atom("int"))),
                                                    TokenHelp.Atom("y")))))));
            }
        }
    }
}
