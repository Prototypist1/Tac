﻿using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class Arithmetic : ITestCase
    {
        public string Text => "module math-module { ( 2 + 5 ) * ( 2 + 7 ) =: x ; } ;";

        public IModuleDefinition Module => ModuleDefinition.CreateAndBuild(
            new FinalizedScope(new Dictionary<IKey, IMemberDefinition>() { { new NameKey("x"), MemberDefinition.CreateAndBuild(new NameKey("x"), TypeReference.CreateAndBuild(new AnyType()), false) } }),
            new[] {
                AssignOperation.CreateAndBuild(
                    MultiplyOperation.CreateAndBuild(
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(5)),
                        AddOperation.CreateAndBuild(
                            ConstantNumber.CreateAndBuild(2),
                            ConstantNumber.CreateAndBuild(7))),
                    MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("x"),TypeReference.CreateAndBuild(new AnyType()), false)))},
            new NameKey("math-module"));
            
    }
}
