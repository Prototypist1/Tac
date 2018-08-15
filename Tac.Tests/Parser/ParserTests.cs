using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Tests.Tokenizer;
using Xunit;

namespace Tac.Tests.Parser
{
    public class ParserTests
    {
        [Fact]
        public void Test1()
        {
            var token =
                TokenHelp.File(
                    TokenHelp.Line(
                        TokenHelp.Ele(
                            TokenHelp.Atom("var"),
                            TokenHelp.Atom("fac")),
                        TokenHelp.Atom("is-static"),
                        TokenHelp.Ele(
                            TokenHelp.Atom("method|int|int"),
                            TokenHelp.Atom("input"),
                            TokenHelp.Curl(
                                TokenHelp.Line(
                                    TokenHelp.Ele(TokenHelp.Atom("input")),
                                    TokenHelp.Atom("less-then"),
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
                                                TokenHelp.Atom("return")))))))));

            var res = TokenParser.ParseFile(token);

            var target = new[] {
                new IsStaticOperation(
                    new LocalDefinition(
                        false,
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
                                                new ConstantNumber(1))})),
                                new BlockDefinition(
                                    new ICodeElement[]{
                                        new ReturnOperation(
                                            new MultiplyOperation(
                                                new NextCallOperation(
                                                    new SubtractOperation(
                                                        new Referance("input"),
                                                        new ConstantNumber(1)),
                                                    new Referance("fac")),
                                                new Referance("input")))}))}))};

            Assert.Equal(target.Length, target.Length);
            for (int i = 0; i < target.Length; i++)
            {
                Assert.Equal(res[i], target[i]);
            }
        }

        [Fact]
        public void Test2()
        {
            var token =
                TokenHelp.File(
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

            var res = TokenParser.ParseFile(token);

            var target = new[] { new MultiplyOperation(new AddOperation(new ConstantNumber(2), new ConstantNumber(5)), new AddOperation(new ConstantNumber(2), new ConstantNumber(7))) };

            Assert.Equal(target.Length, target.Length);
            for (int i = 0; i < target.Length; i++)
            {
                Assert.Equal(res[i], target[i]);
            }
        }

    }
}
