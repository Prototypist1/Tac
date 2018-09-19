using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Tests.Tokenizer;

namespace Tac.Tests.Samples
{
    public class Factorial : ISample
    {
        public string Text
        {
            get
            {
                return @"
module {
    method [ int ; int ] input {
        input <? 2 if {
            1 return ;
        } else {
            input - 1 > fac * input return ;      
        } ;
    } =: fac ;
}";
            }
        }

        public IToken Token
        {
            get
            {
                return 
                    
                    TokenHelp.File(
                         TokenHelp.Ele(
                              TokenHelp.Atom("module"),
                                   TokenHelp.Curl(
                                       TokenHelp.Line(
                                           TokenHelp.Ele(
                                               TokenHelp.Atom("method"),
                                               TokenHelp.Square(
                                                   TokenHelp.Ele(TokenHelp.Atom("int")),
                                                   TokenHelp.Ele(TokenHelp.Atom("int"))),
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
                                           TokenHelp.Ele(
                                               TokenHelp.Atom("static"),
                                               TokenHelp.Atom("fac"))))));
            }
        }

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {
                var rootScope = new StaticScope();
                var methodScope = new MethodScope();
                var ifBlock = new LocalStaticScope();
                var elseBlock = new LocalStaticScope();

                return new[] {
                    new InterpetedAssignOperation(
                        new InterpetedMemberPath(0,
                            new InterpetedMemberDefinition(),
                        new MethodDefinition(
                            new TypeReferance("int"),
                            new ParameterDefinition(
                                false,
                                new TypeReferance("int"),
                                new ExplicitMemberName("input")),
                            new ICodeElement[]{
                                new ElseOperation(
                                    new IfTrueOperation(
                                        new LessThanOperation(
                                            new Referance("input"),
                                            new ConstantNumber(2)),
                                        new BlockDefinition(
                                            new ICodeElement[]{
                                                new ReturnOperation(
                                                    new ConstantNumber(1))},
                                            ifBlock)),
                                    new BlockDefinition(
                                        new ICodeElement[]{
                                            new ReturnOperation(
                                                new MultiplyOperation(
                                                    new NextCallOperation(
                                                        new SubtractOperation(
                                                            new Referance("input"),
                                                            new ConstantNumber(1)),
                                                        new Referance("fac")),
                                                    new Referance("input")))},
                                        elseBlock))},
                            methodScope))
                };
            }
        }
    }
}
