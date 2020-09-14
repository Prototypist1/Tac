using Prototypist.Toolbox;
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
    public class PointObject : ITestCase
    {
        public string Text => @"module point-module { object {
                            5 =: x ;
                            2 =: y ;
                        } =: point ; }";

        public IModuleDefinition ModuleDefinition { get; }

        public PointObject()
        {
                var keyX = new NameKey("x");
                var localX = MemberDefinition.CreateAndBuild(keyX, new AnyType(), Access.ReadWrite);
                var keyY = new NameKey("y");
                var localY = MemberDefinition.CreateAndBuild(keyY, new AnyType(), Access.ReadWrite);

            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<IsStatic>(){new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("point"), new AnyType(), Access.ReadWrite),false) } ),
                    new[] {
                        AssignOperation.CreateAndBuild(
                            ObjectDefiniton.CreateAndBuild(
                                 Scope.CreateAndBuild(
                                    new List<IsStatic> {
                                        new IsStatic( localX ,false),
                                        new IsStatic( localY ,false)
                                    }),
                                new IAssignOperation[]{
                                    AssignOperation.CreateAndBuild(
                                        ConstantNumber.CreateAndBuild(5),
                                        MemberReference.CreateAndBuild(localX)),
                                    AssignOperation.CreateAndBuild(
                                        ConstantNumber.CreateAndBuild(2),
                                        MemberReference.CreateAndBuild(localY))
                                }),
                            MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("point"), new AnyType(), Access.ReadWrite)))},
                    new NameKey("point-module"),
                    EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<ICodeElement>(), Array.Empty<ICodeElement>()));
        }
    }
}