using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class PairType : ITestCase
    {
        public string Text =>
            @"
type [ T ; ] pair { T x ; T y ; } ; 
method [ number ; pair [ number ; ] ] input {
    object {
        input =: x ;
        input =: y ;
    } return ;      
} =: pairify ;
";

        public IRootScope RootScope { get; }

        public PairType()
        {
            var pairTypeNumber =
                InterfaceType.CreateAndBuild(
                        new List<IMemberDefinition>{
                                MemberDefinition.CreateAndBuild(new NameKey("x"),new NumberType(), Access.ReadWrite) ,
                                MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Access.ReadWrite)
                        });

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new NumberType(), Access.ReadWrite);
            
            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input, false) });

            var localX = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Access.ReadWrite);
            var localY = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Access.ReadWrite);

            var pairifyKey = new NameKey("pairify");
            var pairify = MemberDefinition.CreateAndBuild(pairifyKey, MethodType.CreateAndBuild(new NumberType(), pairTypeNumber), Access.ReadWrite);

            RootScope = Model.Instantiated.RootScope.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(pairifyKey, MethodType.CreateAndBuild(new NumberType(), pairTypeNumber), Access.ReadWrite), false) }),
                new [] {
                    AssignOperation.CreateAndBuild(
                        MethodDefinition.CreateAndBuild(
                            pairTypeNumber,
                            input,
                            methodScope,
                            new ICodeElement[]{
                                ReturnOperation.CreateAndBuild(
                                    ObjectDefiniton.CreateAndBuild(
                                            Scope.CreateAndBuild(
                                            new List<IsStatic> {
                                                new IsStatic(localX ,false),
                                                new IsStatic(localY, false)
                                            }),
                                        new IAssignOperation[]{
                                            AssignOperation.CreateAndBuild(
                                                MemberReference.CreateAndBuild(input),
                                                MemberReference.CreateAndBuild(localX)),
                                            AssignOperation.CreateAndBuild(
                                                MemberReference.CreateAndBuild(input),
                                                MemberReference.CreateAndBuild(localY))
                                        }))},
                            Array.Empty<ICodeElement>()),
                    MemberReference.CreateAndBuild(pairify))},
                EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
        }
    }
}