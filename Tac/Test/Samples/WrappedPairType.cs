using Tac.Parser;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class WrappedPairType : PairType, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return TokenHelp.File(
                           TokenHelp.Line(
                               TokenHelp.Ele(
                                   TokenHelp.Atom("type"),
                                   TokenHelp.Square(
                                       TokenHelp.Line(
                                           TokenHelp.Ele(
                                               TokenHelp.Atom("T")))),
                                   TokenHelp.Atom("pair"),
                                   TokenHelp.Curl(
                                       TokenHelp.Line(
                                            TokenHelp.Ele(
                                                TokenHelp.Atom("T"),
                                                TokenHelp.Atom("x"))),
                                       TokenHelp.Line(
                                            TokenHelp.Ele(
                                                TokenHelp.Atom("T"),
                                                TokenHelp.Atom("y")))))));
            }
        }
    }
}