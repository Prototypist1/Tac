using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class PointObject : ISample
    {
        public string Text => @"object {
    x is 5 ;
    y is 2 ;
}";

        public IToken Token => TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("plus"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Ele(
                                        TokenHelp.Atom("x"),
                                        TokenHelp.Atom("is"),
                                        TokenHelp.Atom("5"))),
                                TokenHelp.Line(
                                    TokenHelp.Ele(
                                        TokenHelp.Atom("y"),
                                        TokenHelp.Atom("is"),
                                        TokenHelp.Atom("2")))))));

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {
                var objectScope = new ObjectScope(RootScope.Root);

                objectScope.TryAddLocalMember(new AbstractMemberDefinition(false, false, new ImplicitTypeReferance(), new ExplicitMemberName("x")));
                objectScope.TryAddLocalMember(new AbstractMemberDefinition(false, false, new ImplicitTypeReferance(), new ExplicitMemberName("y")));

                return new ICodeElement[] {
                    new ObjectDefinition(objectScope,new ICodeElement[]{
                        
                    })
                };
            }
        }
    }
}
