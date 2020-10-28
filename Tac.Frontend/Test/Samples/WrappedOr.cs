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
                                TokenHelp.Atom("5"),
                                TokenHelp.Atom("=:"),
                                TokenHelp.Par(
                                    TokenHelp.Line(new[] {
                                        TokenHelp.Atom("bool"),
                                        TokenHelp.Atom("|"),
                                        TokenHelp.Atom("number") })),
                                TokenHelp.Atom("x")), 
                        TokenHelp.Line(
                                TokenHelp.Atom("false"),
                                TokenHelp.Atom("=:"),
                                    TokenHelp.Par(
                                        TokenHelp.Line(new[] {
                                            TokenHelp.Atom("bool"),
                                            TokenHelp.Atom("|"),
                                            TokenHelp.Atom("number")})),
                                TokenHelp.Atom("y")));
            }
        }
    }
}
