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
                            TokenHelp.Atom("object"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Atom("5"),
                                    TokenHelp.Atom("=:"),
                                    TokenHelp.Atom("x")),
                                TokenHelp.Line(
                                    TokenHelp.Atom("2"),
                                    TokenHelp.Atom("=:"),
                                    TokenHelp.Atom("y"))),
                            TokenHelp.Atom("=:"),
                            TokenHelp.Atom("point")));
            }
        }
    }


}