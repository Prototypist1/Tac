using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class Factorial : ITestCase
    {
        public Factorial() {
            var ifBlockScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { });
            var elseBlock = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { });

            var inputKey = new NameKey("input");
            var input = new MemberDefinition(inputKey, new TypeReferance(new NumberType()), false);

            var facKey = new NameKey("fac");
            var fac = new MemberDefinition(facKey, new TypeReferance(new MethodType(new NumberType(), new NumberType())), false);


            var methodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { inputKey, input } });

            var rootScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { facKey, fac } });

            var method = new MethodDefinition(
                        new TypeReferance(new NumberType()),
                        new TypeReferance(new NumberType()),
                        input,
                        methodScope,
                        new ICodeElement[]{
                                new ElseOperation(
                                    new IfOperation(
                                        new LessThanOperation(
                                            new MemberReferance(input),
                                            new ConstantNumber(2)),
                                        new BlockDefinition(
                                            ifBlockScope,
                                            new ICodeElement[]{
                                                new ReturnOperation(
                                                    new ConstantNumber(1))},
                                            new ICodeElement[0])),
                                    new BlockDefinition(
                                        elseBlock,
                                        new ICodeElement[]{
                                            new ReturnOperation(
                                                new MultiplyOperation(
                                                    new NextCallOperation(
                                                        new SubtractOperation(
                                                            new MemberReferance(input),
                                                            new ConstantNumber(1)),
                                                        new MemberReferance(fac)),
                                                    new MemberReferance(input)))},
                                        new ICodeElement[0]))},
                        new ICodeElement[0]);

            CodeElements= new ICodeElement[] {method ,};
            Scope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition>{{facKey,new MemberDefinition(facKey, new TypeReferance(method),false) }});
        }

        public string Text
        {
            get
            {
                return 
@"
method [ int ; int ; ] input {
    input <? 2 then {
        1 return ;
    } else {
        input - 1 > fac * input return ;      
    } ;
} ;
";
            }
        }
        
        public ICodeElement[] CodeElements
        {
            get;
        }

        public IFinalizedScope Scope { get; }
    }

}
