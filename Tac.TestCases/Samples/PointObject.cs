using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class PointObject : ITestCase
    {
        public string Text => @"module { object {
                            5 =: x ;
                            2 =: y ;
                        } =: point ; }";

        public IModuleDefinition Module { get; }

        public PointObject()
        {
                var keyX = new NameKey("x");
                var localX = MemberDefinition.CreateAndBuild(keyX, TypeReference.CreateAndBuild(new AnyType()), false);
                var keyY = new NameKey("y");
                var localY = MemberDefinition.CreateAndBuild(keyY, TypeReference.CreateAndBuild(new AnyType()), false);
                                
                 Module = ModuleDefinition.CreateAndBuild(
                    new FinalizedScope(new Dictionary<IKey, IMemberDefinition>() { { new NameKey("point"), MemberDefinition.CreateAndBuild(new NameKey("point"), TypeReference.CreateAndBuild(new AnyType()), false) } }),
                    new[] {
                        ObjectDefiniton.CreateAndBuild(
                            new FinalizedScope(
                            new Dictionary<IKey, IMemberDefinition> {
                                { keyX, localX },
                                { keyY, localY }
                            }),
                            new IAssignOperation[]{
                                AssignOperation.CreateAndBuild(
                                    ConstantNumber.CreateAndBuild(5),
                                    MemberReference.CreateAndBuild(localX)),
                                AssignOperation.CreateAndBuild(
                                    ConstantNumber.CreateAndBuild(2),
                                    MemberReference.CreateAndBuild(localY))
                            })
                    });
        }
    }
}