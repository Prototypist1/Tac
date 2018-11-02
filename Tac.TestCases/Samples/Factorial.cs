using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.TestCases;
namespace Tac.Tests.Samples
{
    public class Factorial : ITestCase
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
        
        public ICodeElement[] CodeElements
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
