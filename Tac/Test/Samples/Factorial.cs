using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class Factorial 
    {


        public IToken Token
        {
            get
            {
                return 
                    
                    TokenHelp.File(
                        TokenHelp.Line(
                            TokenHelp.Ele(
                                TokenHelp.Atom("method"),
                                TokenHelp.Square(
                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int"))),
                                    TokenHelp.Line(TokenHelp.Ele(TokenHelp.Atom("int")))),
                                TokenHelp.Atom("input"),
                                TokenHelp.Curl(
                                    TokenHelp.Line(
                                        TokenHelp.Ele(TokenHelp.Atom("input")),
                                        TokenHelp.Atom("<?"),
                                        TokenHelp.Ele(TokenHelp.Atom("2")),
                                        TokenHelp.Atom("if"),
                                        TokenHelp.Ele(
                                            TokenHelp.Curl(
                                                TokenHelp.Line(
                                                    TokenHelp.Ele(TokenHelp.Atom("1")),
                                                    TokenHelp.Atom("return")))),
                                        TokenHelp.Atom("else"),
                                        TokenHelp.Ele(
                                            TokenHelp.Curl(
                                                TokenHelp.Line(
                                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                                    TokenHelp.Atom("-"),
                                                    TokenHelp.Ele(TokenHelp.Atom("1")),
                                                    TokenHelp.Atom(">"),
                                                    TokenHelp.Ele(TokenHelp.Atom("fac")),
                                                    TokenHelp.Atom("*"),
                                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                                    TokenHelp.Atom("return"))))))),
                            TokenHelp.Atom("=:"),
                            TokenHelp.Ele(TokenHelp.Atom("fac"))));
            }
        }
    }
}
