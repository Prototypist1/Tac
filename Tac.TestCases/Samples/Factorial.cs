using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class Factorial : ITestCase
    {
        public Factorial() {
            var ifBlockScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { });
            var elseBlock = Scope.CreateAndBuild(new List<Scope.IsStatic> { });

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new NumberType(), false);

            var facKey = new NameKey("fac");
            var fac = MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(new NumberType(), new NumberType()), false);


            var methodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(input ,false) });


            Module =
                ModuleDefinition.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<Scope.IsStatic> { new Scope.IsStatic(MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(new NumberType(), new NumberType()), false), false) }),
                    new ICodeElement[]{
                        AssignOperation.CreateAndBuild(
                                MethodDefinition.CreateAndBuild(
                                    new NumberType(),
                                    new NumberType(),
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
                                    new ICodeElement[0],
                                    false),
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
