using Prototypist.Toolbox;
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
            var ifBlockScope = Scope.CreateAndBuild(new List<IsStatic> { });
            var elseBlock = Scope.CreateAndBuild(new List<IsStatic> { });

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new NumberType(), false);

            var facKey = new NameKey("fac");
            var fac = MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(new NumberType(), new NumberType()), false);

            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input ,false) });


            ModuleDefinition =
                Model.Instantiated.ModuleDefinition.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(new NumberType(), new NumberType()), false), false) }),
                    new OrType<ICodeElement, IError>[]{
                        new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                                new OrType<ICodeElement, IError>(MethodDefinition.CreateAndBuild(
                                    new NumberType(),
                                    new NumberType(),
                                    input,
                                    methodScope,
                                    new OrType<ICodeElement,IError>[]{
                                            new OrType<ICodeElement,IError>(ElseOperation.CreateAndBuild(
                                                new OrType<ICodeElement, IError>(IfOperation.CreateAndBuild(
                                                    new OrType<ICodeElement, IError>(LessThanOperation.CreateAndBuild(
                                                        new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                        new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(2)))),
                                                    new OrType<ICodeElement, IError>(BlockDefinition.CreateAndBuild(
                                                        ifBlockScope,
                                                        new OrType<ICodeElement,IError>[]{
                                                            new OrType<ICodeElement,IError>(
                                                                ReturnOperation.CreateAndBuild(new OrType<ICodeElement, IError>(
                                                                    ConstantNumber.CreateAndBuild(1))))},
                                                        Array.Empty<ICodeElement>())))),
                                                new OrType<ICodeElement, IError>(BlockDefinition.CreateAndBuild(
                                                    elseBlock,
                                                    new OrType<ICodeElement,IError>[]{
                                                       new OrType<ICodeElement,IError>(
                                                        ReturnOperation.CreateAndBuild(new OrType<ICodeElement, IError>(
                                                            MultiplyOperation.CreateAndBuild(
                                                                new OrType<ICodeElement, IError>(NextCallOperation.CreateAndBuild(
                                                                    new OrType<ICodeElement, IError>(SubtractOperation.CreateAndBuild(
                                                                        new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                                        new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(1)))),
                                                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(fac)))),
                                                                new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(input))))))},
                                                    Array.Empty<ICodeElement>()))))},
                                    Array.Empty<ICodeElement>())),
                                new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(fac))
                        )) },
                    new NameKey("factorial"),
                    EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>())
                    );
        }

        public string Text
        {
            get
            {
                return
@"
module factorial {
    method [ number ; number ; ] input {
        input <? 2 then {
            1 return ;
        } else {
            input - 1 > fac * input return ;      
        } ;
    } =: fac ;
}";
            }
        }
        
        public IModuleDefinition ModuleDefinition { get; }
    }

}
