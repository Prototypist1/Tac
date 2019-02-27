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
            var x = MemberDefinition.CreateAndBuild(xKey, TypeReference.CreateAndBuild(new NumberType()), false);

            var yKey = new NameKey("y");
            var y = MemberDefinition.CreateAndBuild(yKey, TypeReference.CreateAndBuild(new EmptyType()), false);

            var methodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(x ,false) },
                new List<Scope.TypeData>(),
                    new List<Scope.GenericTypeData>());
            var innerMethodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(y ,false) },
                new List<Scope.TypeData>(),
                new List<Scope.GenericTypeData>(),
                methodScope);

            Module = ModuleDefinition.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<Scope.IsStatic>() { new Scope.IsStatic(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), TypeReference.CreateAndBuild(new AnyType()), false) ,false) },
                    new List<Scope.TypeData>(),
                    new List<Scope.GenericTypeData>()),
                new[]{
                    AssignOperation.CreateAndBuild(
                        MethodDefinition.CreateAndBuild(
                            TypeReference.CreateAndBuild(new NumberType()),
                            TypeReference.CreateAndBuild(MethodType.CreateAndBuild(
                                new EmptyType(),
                                new NumberType())),
                            x,
                            methodScope,
                            new ICodeElement[]{
                                ReturnOperation.CreateAndBuild(
                                    MethodDefinition.CreateAndBuild(
                                        TypeReference.CreateAndBuild(new EmptyType()),
                                        TypeReference.CreateAndBuild(new NumberType()),
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
                        MemberReference.CreateAndBuild(MemberDefinition.CreateAndBuild(new NameKey("create-accululator"),TypeReference.CreateAndBuild( new AnyType()),false)))
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
    method [ int ; method [ empty ; int ; ] ; ] x {
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
