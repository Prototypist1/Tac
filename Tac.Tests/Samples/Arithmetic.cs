using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class Arithmetic : ISample
    {
        public string Text => @"( 2 plus 5 ) times ( 2 plus 7 ) ;";

        public IToken Token => TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Par(
                                    TokenHelp.Ele(TokenHelp.Atom("2")),
                                    TokenHelp.Atom("plus"),
                                    TokenHelp.Ele(TokenHelp.Atom("5")))),
                        TokenHelp.Atom("times"),
                        TokenHelp.Ele(
                            TokenHelp.Par(
                                    TokenHelp.Ele(TokenHelp.Atom("2")),
                                    TokenHelp.Atom("plus"),
                                    TokenHelp.Ele(TokenHelp.Atom("7"))))));

        public IEnumerable<ICodeElement> CodeElements => new[] {
            new MultiplyOperation(
                new AddOperation(
                    new ConstantNumber(2),
                    new ConstantNumber(5)),
                new AddOperation(
                    new ConstantNumber(2),
                    new ConstantNumber(7)))};
    }
}
