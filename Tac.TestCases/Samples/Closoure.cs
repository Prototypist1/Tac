﻿using Prototypist.Toolbox;
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
            var x = MemberDefinition.CreateAndBuild(xKey, new NumberType(), Access.ReadWrite);

            var yKey = new NameKey("y");
            var y = MemberDefinition.CreateAndBuild(yKey, new NumberType(), Access.ReadWrite);

            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(x ,false) });
            var innerMethodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(y ,false) });

            RootScope = Model.Instantiated.RootScope.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<IsStatic>() {
                        new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), new AnyType(), Access.ReadWrite) ,false) }),
                new []{
                    AssignOperation.CreateAndBuild(
                        MethodDefinition.CreateAndBuild(
                            MethodType.CreateAndBuild(
                                new NumberType(),
                                new NumberType()),
                            x,
                            methodScope,
                            new ICodeElement[]{
                                ReturnOperation.CreateAndBuild(
                                    MethodDefinition.CreateAndBuild(
                                        new NumberType(),
                                        y,
                                        innerMethodScope,
                                        new ICodeElement[]{
                                            AssignOperation.CreateAndBuild(
                                                AddOperation.CreateAndBuild(
                                                    MemberReference.CreateAndBuild(x),
                                                    MemberReference.CreateAndBuild(y)),
                                                MemberReference.CreateAndBuild(x)),
                                           ReturnOperation.CreateAndBuild(MemberReference.CreateAndBuild(x))
                                        },
                                        Array.Empty<ICodeElement>())
                                    )},
                            Array.Empty<ICodeElement>()),
                        MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), new AnyType(),Access.ReadWrite)))
                 },
                EntryPointDefinition.CreateAndBuild(new EmptyType(), MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite), Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
        }

        public string Text
        {
            get
            {
                return
@"
method [ number ; method [ number ; number ; ] ; ] x {
    method [ number ; number ; ] y {
        x + y =: x ;
        x return ;
    } return ;
} =: create-accululator  ;
";
            }
        }

        public IRootScope RootScope { get; }
    }
}
