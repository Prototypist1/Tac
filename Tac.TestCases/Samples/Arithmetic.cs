using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class Arithmetic : ITestCase
    {
        public string Text => "module math-module { ( 2 + 5 ) * ( 2 + 7 ) =: x ; } ;";

        public IModuleDefinition ModuleDefinition => Model.Instantiated.ModuleDefinition.CreateAndBuild(
             Scope.CreateAndBuild(
                new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), false), false) }),
            new[] {
                new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                    new OrType<ICodeElement, IError>(MultiplyOperation.CreateAndBuild(
                        new OrType<ICodeElement, IError>(AddOperation.CreateAndBuild(
                            new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(2)),
                            new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(5)))),
                        new OrType<ICodeElement, IError>(AddOperation.CreateAndBuild(
                            new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(2)),
                            new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(7)))))),
                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),new AnyType(), false)))))},
            new NameKey("math-module"),
            EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement,IError>>(), Array.Empty<ICodeElement>()));

    }
}
