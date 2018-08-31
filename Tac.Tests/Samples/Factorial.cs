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
    public class Factorial : ISample
    {
        public string Text =>
@"method ( int ; int ) input {
    input less-than 2 if-true {
        1 return ;
    } else {
        input minus 1 next-call fac times input return ;      
    } ;
} assign static fac ;";

        public IToken Token => TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("method|int|int"),
                            TokenHelp.Atom("input"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                    TokenHelp.Atom("less-than"),
                                    TokenHelp.Ele(TokenHelp.Atom("2")),
                                    TokenHelp.Atom("if-true"),
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
                                                TokenHelp.Atom("minus"),
                                                TokenHelp.Ele(TokenHelp.Atom("1")),
                                                TokenHelp.Atom("next-call"),
                                                TokenHelp.Ele(TokenHelp.Atom("fac")),
                                                TokenHelp.Atom("times"),
                                                TokenHelp.Ele(TokenHelp.Atom("input")),
                                                TokenHelp.Atom("return"))))))),
                        TokenHelp.Atom("assign"),
                        TokenHelp.Ele(
                            TokenHelp.Atom("static"),
                            TokenHelp.Atom("fac"))
                        ));

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {
                var rootScope = new StaticScope(RootScope.Root);
                var methodScope = new MethodScope(rootScope);
                var ifBlock = new LocalStaticScope(methodScope);
                var elseBlock = new LocalStaticScope(methodScope);

                return new[] {
                    new IsDefininition(
                        new AbstractMemberDefinition(
                            false,
                            true,
                            new ImplicitTypeReferance(),
                            new ExplicitName("fac") ),
                        new MethodDefinition(
                            new TypeReferance("int"),
                            new ParameterDefinition(
                                false,
                                new TypeReferance("int"),
                                new ExplicitName("input")),
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
