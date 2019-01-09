using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
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
            var input = MemberDefinition.CreateAndBuild(inputKey, TypeReference.CreateAndBuild(new NumberType()), false);

            var facKey = new NameKey("fac");
            var fac = MemberDefinition.CreateAndBuild(facKey, TypeReference.CreateAndBuild(MethodType.CreateAndBuild(new NumberType(), new NumberType())), false);


            var methodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { inputKey, input } });

            var rootScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { facKey, fac } });

            Module =
                ModuleDefinition.CreateAndBuild(
                    new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { facKey, MemberDefinition.CreateAndBuild(facKey, TypeReference.CreateAndBuild(MethodType.CreateAndBuild(new NumberType(), new NumberType())), false) } }),
                    new ICodeElement[]{
                        AssignOperation.CreateAndBuild(
                                MethodDefinition.CreateAndBuild(
                                    TypeReference.CreateAndBuild(new NumberType()),
                                    TypeReference.CreateAndBuild(new NumberType()),
                                    input,
                                    methodScope,
                                    new ICodeElement[]{
                                            ElseOperation.CreateAndBuild(
                                                IfOperation.CreateAndBuild(
                                                    LessThanOperation.CreateAndBuild(
                                                        MemberReference.CreateAndBuild(input),
                                                        ConstantNumber.CreateAndBuild(2)),
                                                    BlockDefinition.CreateAndBuild(
                                                        ifBlockScope,
                                                        new ICodeElement[]{
                                                            ReturnOperation.CreateAndBuild(
                                                                ConstantNumber.CreateAndBuild(1))},
                                                        new ICodeElement[0])),
                                                BlockDefinition.CreateAndBuild(
                                                    elseBlock,
                                                    new ICodeElement[]{
                                                        ReturnOperation.CreateAndBuild(
                                                            MultiplyOperation.CreateAndBuild(
                                                                NextCallOperation.CreateAndBuild(
                                                                    SubtractOperation.CreateAndBuild(
                                                                        MemberReference.CreateAndBuild(input),
                                                                        ConstantNumber.CreateAndBuild(1)),
                                                                    MemberReference.CreateAndBuild(fac)),
                                                                MemberReference.CreateAndBuild(input)))},
                                                    new ICodeElement[0]))},
                                    new ICodeElement[0]),
                                MemberReference.CreateAndBuild(fac)
                        ) },
                    new NameKey("factorial")
                    );
        }

        public string Text
        {
            get
            {
                return
@"
module factorial {
    method [ int ; int ; ] input {
        input <? 2 then {
            1 return ;
        } else {
            input - 1 > fac * input return ;      
        } ;
    } =: fac ;
}";
            }
        }
        
        public IModuleDefinition Module { get; }
    }

}
