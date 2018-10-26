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

        public IEnumerable<IWeakCodeElement> CodeElements
        {
            get
            {
                
                var ifBlock = new FinalizedScope(new Dictionary<IKey, IBox<WeakMemberDefinition>> {});
                var elseBlock = new FinalizedScope(new Dictionary<IKey, IBox<WeakMemberDefinition>> { });

                var inputKey = new NameKey("input");
                var input = new WeakMemberDefinition(
                                false,
                                inputKey,
                                new Box<IWeakReturnable>(new InterpetedNumberType()));
                var inputBox = new Box<WeakMemberDefinition>(input);
                var facBox = new Box<WeakMemberDefinition>();
                var facKey = new NameKey("fac");
                var fac = new WeakMemberDefinition(
                        false,
                        facKey,
                        facBox);
                facBox.Fill(fac);

                var methodScope = new FinalizedScope(new Dictionary<IKey, IBox<WeakMemberDefinition>> { { inputKey, inputBox } });
                
                var rootScope = new FinalizedScope(new Dictionary<IKey, IBox<WeakMemberDefinition>> { { facKey, facBox }});
                
                return new IWeakCodeElement[] {
                    new InterpetedAssignOperation(
                        new InterpetedMethodDefinition(
                            new Box<IWeakReturnable>(new InterpetedNumberType()),
                            inputBox,
                            new IWeakCodeElement[]{
                                new InterpetedElseOperation(
                                    new InterpetedIfTrueOperation(
                                        new InterpetedLessThanOperation(
                                            new InterpetedMemberReferance(inputBox),
                                            new InterpetedConstantNumber(2)),
                                        new InterpetedBlockDefinition(
                                            new IWeakCodeElement[]{
                                                new InterpetedReturnOperation(
                                                    new InterpetedConstantNumber(1))},
                                            ifBlock,
                                            new IWeakCodeElement[0])),
                                    new InterpetedBlockDefinition(
                                        new IWeakCodeElement[]{
                                            new InterpetedReturnOperation(
                                                new InterpetedMultiplyOperation(
                                                    new InterpetedNextCallOperation(
                                                        new InterpetedSubtractOperation(
                                                            new InterpetedMemberReferance(inputBox),
                                                            new InterpetedConstantNumber(1)),
                                                        new InterpetedMemberReferance(facBox)),
                                                    new InterpetedMemberReferance(inputBox)))},
                                        elseBlock,
                                        new IWeakCodeElement[0]))},
                            methodScope,
                            new IWeakCodeElement[0]),
                        new InterpetedMemberReferance(
                            facBox)),
                };
            }
        }
    }
}
