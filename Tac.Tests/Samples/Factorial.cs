using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
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
    method [ int ; int ; ] input {
        input <? 2 if {
            1 return ;
        } else {
            input - 1 > fac * input return ;      
        } ;
    } =: fac ;
";
            }
        }

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

        public IEnumerable<ICodeElement> CodeElements
        {
            get
            {
                
                var ifBlock = new FinalizedScope(new Dictionary<IKey, IBox<MemberDefinition>> {});
                var elseBlock = new FinalizedScope(new Dictionary<IKey, IBox<MemberDefinition>> { });

                var inputKey = new NameKey("input");
                var input = new MemberDefinition(
                                false,
                                inputKey,
                                new Box<IReturnable>(new InterpetedNumberType()));
                var inputBox = new Box<MemberDefinition>(input);
                var facBox = new Box<MemberDefinition>();
                var facKey = new NameKey("fac");
                var fac = new MemberDefinition(
                        false,
                        facKey,
                        facBox);
                facBox.Fill(fac);

                var methodScope = new FinalizedScope(new Dictionary<IKey, IBox<MemberDefinition>> { { inputKey, inputBox } });
                
                var rootScope = new FinalizedScope(new Dictionary<IKey, IBox<MemberDefinition>> { { facKey, facBox }});
                
                return new ICodeElement[] {
                    new InterpetedAssignOperation(
                        new InterpetedMethodDefinition(
                            new Box<IReturnable>(new InterpetedNumberType()),
                            inputBox,
                            new ICodeElement[]{
                                new InterpetedElseOperation(
                                    new InterpetedIfTrueOperation(
                                        new InterpetedLessThanOperation(
                                            new InterpetedMemberReferance(inputBox),
                                            new InterpetedConstantNumber(2)),
                                        new InterpetedBlockDefinition(
                                            new ICodeElement[]{
                                                new InterpetedReturnOperation(
                                                    new InterpetedConstantNumber(1))},
                                            ifBlock,
                                            new ICodeElement[0])),
                                    new InterpetedBlockDefinition(
                                        new ICodeElement[]{
                                            new InterpetedReturnOperation(
                                                new InterpetedMultiplyOperation(
                                                    new InterpetedNextCallOperation(
                                                        new InterpetedSubtractOperation(
                                                            new InterpetedMemberReferance(inputBox),
                                                            new InterpetedConstantNumber(1)),
                                                        new InterpetedMemberReferance(facBox)),
                                                    new InterpetedMemberReferance(inputBox)))},
                                        elseBlock,
                                        new ICodeElement[0]))},
                            methodScope,
                            new ICodeElement[0]),
                        new InterpetedMemberReferance(
                            facBox)),
                };
            }
        }
    }
}
