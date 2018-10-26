using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class Arithmetic : ISample
    {
        public string Text
        {
            get
            {
                return @"( 2 + 5 ) * ( 2 + 7 ) ;";
            }
        }

        public IToken Token
        {
            get
            {
                return TokenHelp.File(
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
                                           TokenHelp.Ele(TokenHelp.Atom("7"))))));
            }
        }

        public IEnumerable<IWeakCodeElement> CodeElements
        {
            get
            {
                return new[] {
                    new InterpetedMultiplyOperation(
                        new InterpetedAddOperation(
                            new InterpetedConstantNumber(2),
                            new InterpetedConstantNumber(5)),
                        new InterpetedAddOperation(
                            new InterpetedConstantNumber(2),
                            new InterpetedConstantNumber(7)))};
            }
        }

        public InterpetedResult Result {
            get {
                return InterpetedResult.Create(140);
            }
        }

    }
}
