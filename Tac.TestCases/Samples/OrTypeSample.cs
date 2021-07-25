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
5 =: ( bool | number ) x ;
false =: ( bool | number ) y ; 
";

        public IRootScope RootScope => Model.Instantiated.RootScope.CreateAndBuild(
             Scope.CreateAndBuild(
                new List<IsStatic> {
                    new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("x"), TypeOr.CreateAndBuild(new BooleanType(),new NumberType(),new IMemberDefinition[]{ }, Possibly.IsNot<IVerifiableType>(), Possibly.IsNot<IVerifiableType>() ), Access.ReadWrite), false),
                    new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("y"), TypeOr.CreateAndBuild(new BooleanType(),new NumberType(),new IMemberDefinition[]{ }, Possibly.IsNot<IVerifiableType>(), Possibly.IsNot<IVerifiableType>()), Access.ReadWrite), false)}),
            new[] {
                AssignOperation.CreateAndBuild(
                    ConstantNumber.CreateAndBuild(5),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),TypeOr.CreateAndBuild(new BooleanType(),new NumberType(),new IMemberDefinition[]{ }, Possibly.IsNot<IVerifiableType>(), Possibly.IsNot<IVerifiableType>()), Access.ReadWrite))),
                AssignOperation.CreateAndBuild(
                    ConstantBool.CreateAndBuild(false),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("y"),TypeOr.CreateAndBuild(new BooleanType(),new NumberType(),new IMemberDefinition[]{ }, Possibly.IsNot<IVerifiableType>(), Possibly.IsNot<IVerifiableType>()), Access.ReadWrite)))},
            EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
    }
}
