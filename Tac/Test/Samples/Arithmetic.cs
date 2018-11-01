using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class Arithmetic 
    {

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
    }
}
