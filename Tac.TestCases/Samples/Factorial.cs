﻿using Prototypist.Toolbox;
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
            var input = MemberDefinition.CreateAndBuild(inputKey, new NumberType(), Access.ReadWrite);

            var facKey = new NameKey("fac");
            var fac = MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(new NumberType(), new NumberType()), Access.ReadWrite);

            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input ,false) });

            RootScope =
                Model.Instantiated.RootScope.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(facKey, MethodType.CreateAndBuild(
                            new NumberType(),
                            new NumberType()), Access.ReadWrite), false) }),
                    new []{
                        AssignOperation.CreateAndBuild(
                                MethodDefinition.CreateAndBuild(
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
                                                        Array.Empty<ICodeElement>())),
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
                                                    Array.Empty<ICodeElement>()))},
                                    Array.Empty<ICodeElement>()),
                                MemberReference.CreateAndBuild(fac)
                        ) },
                    EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>())
                    );
        }

        public string Text
        {
            get
            {
                return
@"
method [ number ; number ; ] input {
    input <? 2 then {
        1 return ;
    } else {
        ( input - 1 > fac ) * input return ;      
    } ;
} =: fac ;
";
            }
        }
        
        public IRootScope RootScope { get; }
    }

}