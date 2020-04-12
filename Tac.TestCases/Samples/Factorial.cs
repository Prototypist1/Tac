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
            var input = MemberDefinition.CreateAndBuild(inputKey, OrType.Make<IVerifiableType, IError>(new NumberType()), false);

            var facKey = new NameKey("fac");
            var fac = MemberDefinition.CreateAndBuild(facKey, OrType.Make<IVerifiableType, IError>(MethodType.CreateAndBuild(OrType.Make<IVerifiableType, IError>(new NumberType()), OrType.Make<IVerifiableType, IError>(new NumberType()))), false);

            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input ,false) });


            ModuleDefinition =
                Model.Instantiated.ModuleDefinition.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(facKey, OrType.Make< IVerifiableType ,IError>(MethodType.CreateAndBuild(
                            OrType.Make< IVerifiableType ,IError>(new NumberType()),
                            OrType.Make< IVerifiableType ,IError>(new NumberType()))), false), false) }),
                    new IOrType<ICodeElement, IError>[]{
                        OrType.Make<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                                OrType.Make<ICodeElement, IError>(MethodDefinition.CreateAndBuild(
                                    OrType.Make< IVerifiableType ,IError>(new NumberType()),
                                    OrType.Make< IVerifiableType ,IError>(new NumberType()),
                                    input,
                                    methodScope,
                                    new IOrType<ICodeElement,IError>[]{
                                            OrType.Make<ICodeElement,IError>(ElseOperation.CreateAndBuild(
                                                OrType.Make<ICodeElement, IError>(IfOperation.CreateAndBuild(
                                                    OrType.Make<ICodeElement, IError>(LessThanOperation.CreateAndBuild(
                                                        OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                        OrType.Make<ICodeElement, IError>(ConstantNumber.CreateAndBuild(2)))),
                                                    OrType.Make<ICodeElement, IError>(BlockDefinition.CreateAndBuild(
                                                        ifBlockScope,
                                                        new IOrType<ICodeElement,IError>[]{
                                                            OrType.Make<ICodeElement,IError>(
                                                                ReturnOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(
                                                                    ConstantNumber.CreateAndBuild(1))))},
                                                        Array.Empty<ICodeElement>())))),
                                                OrType.Make<ICodeElement, IError>(BlockDefinition.CreateAndBuild(
                                                    elseBlock,
                                                    new IOrType<ICodeElement,IError>[]{
                                                       OrType.Make<ICodeElement,IError>(
                                                        ReturnOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(
                                                            MultiplyOperation.CreateAndBuild(
                                                                OrType.Make<ICodeElement, IError>(NextCallOperation.CreateAndBuild(
                                                                    OrType.Make<ICodeElement, IError>(SubtractOperation.CreateAndBuild(
                                                                        OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                                        OrType.Make<ICodeElement, IError>(ConstantNumber.CreateAndBuild(1)))),
                                                                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(fac)))),
                                                                OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(input))))))},
                                                    Array.Empty<ICodeElement>()))))},
                                    Array.Empty<ICodeElement>())),
                                OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(fac))
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
