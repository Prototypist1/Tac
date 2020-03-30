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
                var localX = MemberDefinition.CreateAndBuild(keyX, new AnyType(), false);
                var keyY = new NameKey("y");
                var localY = MemberDefinition.CreateAndBuild(keyY, new AnyType(), false);

            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                     Scope.CreateAndBuild(
                        new List<IsStatic>(){new IsStatic(MemberDefinition.CreateAndBuild(new NameKey("point"), new AnyType(), false),false) } ),
                    new[] {
                        new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                            new OrType<ICodeElement, IError>(ObjectDefiniton.CreateAndBuild(
                                 Scope.CreateAndBuild(
                                    new List<IsStatic> {
                                        new IsStatic( localX ,false),
                                        new IsStatic( localY ,false)
                                    }),
                                new IOrType<IAssignOperation,IError>[]{
                                    new OrType<IAssignOperation,IError>(AssignOperation.CreateAndBuild(
                                        new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(5)),
                                        new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localX)))),
                                    new OrType<IAssignOperation,IError>(AssignOperation.CreateAndBuild(
                                        new OrType<ICodeElement, IError>(ConstantNumber.CreateAndBuild(2)),
                                        new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localY))))
                                })),
                            new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("point"), new AnyType(), false)))))},
                    new NameKey("point-module"),
                    EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
        }
    }
}