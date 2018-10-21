using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
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

                var keyX = new NameKey("x");
                var localX = new MemberDefinition(false, keyX, new Box<IReturnable>(new InterpetedAnyType()));
                var keyY = new NameKey("y");
                var localY = new MemberDefinition(false, keyY, new Box<IReturnable>(new InterpetedAnyType()));
                                
                return new ICodeElement[] {
                    new InterpetedObjectDefinition(
                        new TestScope(
                        new Dictionary<IKey, (bool, MemberDefinition)> {
                            { keyX, (false,localX) },
                            { keyY, (false,localY) }
                        }),
                        new InterpetedAssignOperation[]{
                            new InterpetedAssignOperation(new InterpetedConstantNumber(5),localX),
                            new InterpetedAssignOperation(new InterpetedConstantNumber(2),localY)
                        },
                        new ImplicitKey())
                };
            }
        }
    }
}
