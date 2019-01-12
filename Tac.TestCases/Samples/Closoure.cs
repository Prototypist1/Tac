using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;
using Tac.TestCases.Help;

namespace Tac.Tests.Samples
{
    public class Closoure : ITestCase
    {
        public Closoure()
        {
            var xKey = new NameKey("x");
            var x = MemberDefinition.CreateAndBuild(xKey, TypeReference.CreateAndBuild(new NumberType()), false);

            var yKey = new NameKey("y");
            var y = MemberDefinition.CreateAndBuild(yKey, TypeReference.CreateAndBuild(new NumberType()), false);

            var methodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { xKey, x } },
                new Dictionary<IKey, IVerifiableType>());
            var innerMethodScope = new FinalizedScope(new Dictionary<IKey, IMemberDefinition> { { yKey, y } },
                new Dictionary<IKey, IVerifiableType>(),
                methodScope);

            Module = ModuleDefinition.CreateAndBuild(
                 new FinalizedScope(
                     new Dictionary<IKey, IMemberDefinition>() { { new NameKey("create-accululator"), MemberDefinition.CreateAndBuild(new NameKey("create-accululator"), TypeReference.CreateAndBuild(new AnyType()), false) } },
                new Dictionary<IKey, IVerifiableType>()),
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
                                        new ICodeElement[0]
                                        )
                                    )},
                            new ICodeElement[0]),
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
