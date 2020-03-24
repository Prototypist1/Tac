using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class OrTypeSample : ITestCase
    {
        public string Text => @"
module or-test 
{ 
    5 =: ( bool | number ) x ;
    false =: ( bool | number ) y ; 
} ;";

        public IModuleDefinition ModuleDefinition => Model.Instantiated.ModuleDefinition.CreateAndBuild(
             Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("x"), TypeOr.CreateAndBuild(new BooleanType(),new NumberType()), false), false),
                    new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("y"), TypeOr.CreateAndBuild(new BooleanType(),new NumberType()), false), false)}),
            new[] {
                new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                    new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(5)),
                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),TypeOr.CreateAndBuild(new BooleanType(),new NumberType()), false))))),
                new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                    new OrType<ICodeElement, IError>(ConstantBool.CreateAndBuild(false)),
                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("y"),TypeOr.CreateAndBuild(new BooleanType(),new NumberType()), false)))))},
            new NameKey("or-test"),
            EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
    }
}
