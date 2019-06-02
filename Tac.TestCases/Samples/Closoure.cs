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
            var x = MemberDefinition.CreateAndBuild(xKey, new NumberType(), false);

            var yKey = new NameKey("y");
            var y = MemberDefinition.CreateAndBuild(yKey, new NumberType(), false);

            var methodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(x ,false) });
            var innerMethodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(y ,false) }, methodScope);

            Module = ModuleDefinition.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<Scope.IsStatic>() {
                        new Scope.IsStatic(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), new AnyType(), false) ,false) }),
                new[]{
                    AssignOperation.CreateAndBuild(
                        MethodDefinition.CreateAndBuild(
                            new NumberType(),
                            MethodType.CreateAndBuild(
                                new NumberType(),
                                new NumberType()),
                            x,
                            methodScope,
                            new ICodeElement[]{
                                ReturnOperation.CreateAndBuild(
                                    MethodDefinition.CreateAndBuild(
                                        new NumberType(),
                                        new NumberType(),
                                        y,
                                        innerMethodScope,
                                        new ICodeElement[]{
                                            AssignOperation.CreateAndBuild(
                                                AddOperation.CreateAndBuild(
                                                    MemberReference.CreateAndBuild(x),
                                                    MemberReference.CreateAndBuild(y)),
                                                MemberReference.CreateAndBuild(x)),
                                            ReturnOperation.CreateAndBuild(
                                                MemberReference.CreateAndBuild(x))
                                        },
                                        new ICodeElement[0],
                                        false)
                                    )},
                            new ICodeElement[0],
                            false),
                        MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), new AnyType(),false)))
                 },
                new NameKey("closoure"));
        }

        public string Text
        {
            get
            {
                return
@"
module closoure {
    method [ int ; method [ int ; int ; ] ; ] x {
        method [ int ; int ; ] y {
            x + y =: x ;
            x return ;
        } return ;
    } =: create-accululator  ;
} ; ";
            }
        }

        public IModuleDefinition Module { get; }
    }
}
