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
        public string Text => "( 2 + 5 ) * ( 2 + 7 ) =: x ;";

        public IRootScope RootScope => Model.Instantiated.RootScope.CreateAndBuild(
             Scope.CreateAndBuild(
                new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("x"),  new AnyType(), Access.ReadWrite), false) }),
            new[] {
                AssignOperation.CreateAndBuild(
                    MultiplyOperation.CreateAndBuild(
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(5)),
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(7))),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),new AnyType(), Access.ReadWrite)))},
            EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));

    }
}
