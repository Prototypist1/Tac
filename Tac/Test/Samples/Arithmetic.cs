using Tac.Parser;
using Tac.TestCases;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public interface IWrappedTestCase: ITestCase {
        IToken Token { get; }
    }

    public class WrappedArithmetic : Arithmetic, IWrappedTestCase
    {

        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("module"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Ele(
                                            TokenHelp.Par(
                                                TokenHelp.Ele(TokenHelp.Atom("2")),
                                                TokenHelp.Atom("+"),
                                                TokenHelp.Ele(TokenHelp.Atom("5")))),
                                        TokenHelp.Atom("*"),
                                        TokenHelp.Ele(
                                            TokenHelp.Par(
                                                TokenHelp.Ele(TokenHelp.Atom("2")),
                                                TokenHelp.Atom("+"),
                                                TokenHelp.Ele(TokenHelp.Atom("7")))),
                                        TokenHelp.Atom("=:"),
                                        TokenHelp.Ele(TokenHelp.Atom("x")))))));
            }
        }
    }
}
