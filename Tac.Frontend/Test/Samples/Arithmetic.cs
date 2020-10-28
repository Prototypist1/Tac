using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{

    internal class WrappedArithmetic : Arithmetic, IWrappedTestCase
    {

        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                            TokenHelp.Line(
                                TokenHelp.Par(
                                    TokenHelp.Line(new[] {
                                        TokenHelp.Atom("2"),
                                        TokenHelp.Atom("+"),
                                        TokenHelp.Atom("5")})),
                                TokenHelp.Atom("*"),
                                TokenHelp.Par(
                                    TokenHelp.Line(new[] {
                                        TokenHelp.Atom("2"),
                                        TokenHelp.Atom("+"),
                                        TokenHelp.Atom("7")})),
                                TokenHelp.Atom("=:"),
                                TokenHelp.Atom("x")));
            }
        }
    }
}
