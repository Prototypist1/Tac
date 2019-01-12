using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.TestCases;

namespace Tac.Tests.Samples
{
    public class PairType : ITestCase
    {
        public string Text => @"module pair-type { type [ T ; ] pair {
                            T x ;
                            T y ;
                        } ; }";

        public IModuleDefinition Module { get; }

        public PairType()
        {
                var key = new NameKey("T");
                var type = GemericTypeParameterPlacholder.CreateAndBuild(key);

                var keyX = new NameKey("x");
                var localX = MemberDefinition.CreateAndBuild(keyX, TypeReference.CreateAndBuild(type), false);
                var keyY = new NameKey("y");
                var localY = MemberDefinition.CreateAndBuild(keyY, TypeReference.CreateAndBuild(type), false);

            Module = ModuleDefinition.CreateAndBuild(
                 // huh, FinalizedScope does not hold types??
                 Scope.CreateAndBuild(
                    new List<Scope.IsStatic>(),
                    new List<Scope.TypeData>() {

                    }),
                new[] {
                        GenericInterfaceDefinition.CreateAndBuild(
                                                 Scope.CreateAndBuild(
                                                    new List<Scope.IsStatic>{
                                                        new Scope.IsStatic(localX ,false),
                                                        new Scope.IsStatic( localY,false)
                                                    },
                                                    new List<Scope.TypeData>()),
                                                new TestGenericTypeParameterDefinition[]{
                                                    new TestGenericTypeParameterDefinition(key)
                                                })
                },
                new NameKey("pair-type"));
        }
    }
}