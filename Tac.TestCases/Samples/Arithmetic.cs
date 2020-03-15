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
                AssignOperation.CreateAndBuild(
                    MultiplyOperation.CreateAndBuild(
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(5)),
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(7))),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),new AnyType(), false)))},
            new NameKey("math-module"),
            EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));

    }
}
