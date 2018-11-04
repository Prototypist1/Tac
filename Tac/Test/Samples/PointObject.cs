using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class WrappedPointObject: PointObject, IWrappedTestCase
    {
        public IToken Token
        {
            get
            {
                return TokenHelp.File(
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
                                            TokenHelp.Ele(TokenHelp.Atom("y")))))));
            }
        }
    }
}