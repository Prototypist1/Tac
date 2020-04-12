using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class Closoure : ITestCase
    {
        public Closoure()
        {
            var xKey = new NameKey("x");
            var x = MemberDefinition.CreateAndBuild(xKey, OrType.Make<IVerifiableType, IError>(new NumberType()), false);

            var yKey = new NameKey("y");
            var y = MemberDefinition.CreateAndBuild(yKey, OrType.Make<IVerifiableType, IError>(new NumberType()), false);

            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(x ,false) });
            var innerMethodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(y ,false) });

            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), OrType.Make< IVerifiableType ,IError>(new AnyType()), false) ,false) }),
                new []{
                    OrType.Make<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                        OrType.Make<ICodeElement, IError>(MethodDefinition.CreateAndBuild(
                            OrType.Make< IVerifiableType ,IError>(new NumberType()),
                            OrType.Make< IVerifiableType ,IError>(MethodType.CreateAndBuild(
                                OrType.Make< IVerifiableType ,IError>(new NumberType()),
                                OrType.Make< IVerifiableType ,IError>(new NumberType()))),
                            x,
                            methodScope,
                            new IOrType<ICodeElement,IError>[]{
                                OrType.Make<ICodeElement,IError>(ReturnOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(
                                    MethodDefinition.CreateAndBuild(
                                        OrType.Make< IVerifiableType ,IError>(new NumberType()),
                                        OrType.Make< IVerifiableType ,IError>(new NumberType()),
                                        y,
                                        innerMethodScope,
                                        new IOrType<ICodeElement,IError>[]{
                                            OrType.Make<ICodeElement,IError>( AssignOperation.CreateAndBuild(
                                                OrType.Make<ICodeElement, IError>(AddOperation.CreateAndBuild(
                                                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(x)),
                                                    OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(y)))),
                                                OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(x)))),
                                            OrType.Make<ICodeElement,IError>(ReturnOperation.CreateAndBuild(OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(x))))
                                        },
                                        Array.Empty<ICodeElement>()))
                                    ))},
                            Array.Empty<ICodeElement>())),
                        OrType.Make<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), OrType.Make< IVerifiableType ,IError>(new AnyType()),false)))))
                 },
                new NameKey("closoure"),
                EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
        }

        public string Text
        {
            get
            {
                return
@"
module closoure {
    method [ number ; method [ number ; number ; ] ; ] x {
        method [ number ; number ; ] y {
            x + y =: x ;
            x return ;
        } return ;
    } =: create-accululator  ;
} ; ";
            }
        }

        public IModuleDefinition ModuleDefinition { get; }
    }
}
