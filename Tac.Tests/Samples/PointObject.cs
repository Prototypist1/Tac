using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class PointObject : ISample
    {
        public string Text
        {
            get
            {
                return @"object {
                            5 =: x ;
                            2 =: y ;
                        }";
            }
        }

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
                   TokenHelp.Ele(
                       TokenHelp.Ele(TokenHelp.Atom("5")),
                       TokenHelp.Atom("=:"),
                       TokenHelp.Ele(TokenHelp.Atom("x")))),
               TokenHelp.Line(
                   TokenHelp.Ele(
                       TokenHelp.Ele(TokenHelp.Atom("2")),
                       TokenHelp.Atom("=:"),
                       TokenHelp.Ele(TokenHelp.Atom("y"))))))));
            }
        }

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {
                var objectScope = new ObjectScope();

                var intType = default(TypeDefinition);

                var localX = new MemberDefinition(false, new ExplicitMemberName("x"), intType);
                objectScope.TryAddLocalMember(localX);
                var localY = new MemberDefinition(false, new ExplicitMemberName("y"), intType);
                objectScope.TryAddLocalMember(localY);

                return new ICodeElement[] {
                    new ObjectDefinition(objectScope,new InterpetedAssignOperation[]{
                        new InterpetedAssignOperation(new InterpetedConstantNumber(5),new InterpetedMemberPath(0,localX)),
                        new InterpetedAssignOperation(new InterpetedConstantNumber(2),new InterpetedMemberPath(0,localY))
                    })
                };
            }
        }
    }
}
