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
module pair-type { 
    type [ T ; ] pair { T x ; T y ; } ; 
    method [ int ; pair [ int ; ] ] input {
        object {
            input =: x ;
            input =: y ;
        } return ;      
    } =: pairify ;
}";

        public IModuleDefinition Module { get; }

        public PairType()
        {
            var key = new NameKey("T");
            var type = GemericTypeParameterPlacholder.CreateAndBuild(key);

            var pairTypeNumber =
                InterfaceType.CreateAndBuild(
                        new List<IMemberDefinition>{
                                MemberDefinition.CreateAndBuild(new NameKey("x"), TypeReference.CreateAndBuild(new NumberType()), false) ,
                                MemberDefinition.CreateAndBuild(new NameKey("y"), TypeReference.CreateAndBuild(new NumberType()), false)
                        });

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, TypeReference.CreateAndBuild(new NumberType()), false);
            
            var methodScope = Scope.CreateAndBuild(new List<Scope.IsStatic> { new Scope.IsStatic(input, false) });

            var localX = MemberDefinition.CreateAndBuild(new NameKey("x"), TypeReference.CreateAndBuild(new NumberType()), false);
            var localY = MemberDefinition.CreateAndBuild(new NameKey("y"), TypeReference.CreateAndBuild(new NumberType()), false);

            var pairifyKey = new NameKey("pairify");
            var pairify = MemberDefinition.CreateAndBuild(pairifyKey, TypeReference.CreateAndBuild(MethodType.CreateAndBuild(new NumberType(), pairTypeNumber)), false);
            
            Module = ModuleDefinition.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<Scope.IsStatic> { new Scope.IsStatic(MemberDefinition.CreateAndBuild(pairifyKey, TypeReference.CreateAndBuild(MethodType.CreateAndBuild(new NumberType(), pairTypeNumber)), false), false) }),
                new ICodeElement[] {
                    AssignOperation.CreateAndBuild(
                        MethodDefinition.CreateAndBuild(
                            TypeReference.CreateAndBuild(new NumberType()),
                            TypeReference.CreateAndBuild(pairTypeNumber),
                            input,
                            methodScope,
                            new ICodeElement[]{
                                ReturnOperation.CreateAndBuild(
                                    ObjectDefiniton.CreateAndBuild(
                                            Scope.CreateAndBuild(
                                            new List<Scope.IsStatic> {
                                                new Scope.IsStatic(localX ,false),
                                                new Scope.IsStatic(localY, false)
                                            }),
                                        new IAssignOperation[]{
                                            AssignOperation.CreateAndBuild(
                                                MemberReference.CreateAndBuild(input),
                                                MemberReference.CreateAndBuild(localX)),
                                            AssignOperation.CreateAndBuild(
                                                MemberReference.CreateAndBuild(input),
                                                MemberReference.CreateAndBuild(localY))
                                        }))},
                            new ICodeElement[0],
                            false),
                    MemberReference.CreateAndBuild(pairify))},
                new NameKey("pair-type"));
        }
    }
}