using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac
{
    class PlayGround
    {
        public static void Whatever() {
            /*
             
            Fib = Method<int,int> (x){
                x < 2 if {
                    1 return;
                };
                x - 1 -> Fib + x - 2 -> Fib return;
            } 
             
             */


            var fib = new StaticMethodDefinition(
                new TypeReferance(
                    new NamePath(
                        new[] { new ExplicitName("int") })),
                new ParameterDefinition(
                    false,
                    new TypeReferance(
                        new NamePath(
                            new[] { new ExplicitName("int") })),
                    new ExplicitName("x")),
                new CodeElement [] {
                    new IfTrueOperation(
                        new LessThanOperation(
                            new ParameterReferance(
                                new NamePath(
                                    new[] {new ExplicitName("x") })),
                            new Constant("2")),
                        new BlockDefinition(
                            new []{
                                new ReturnOperation(
                                    new Constant("1"))
                            })),
                    new ReturnOperation(
                        new AddOperation(
                            new LeftCallOperation(
                                new SubtractOperation(
                                    new ParameterReferance(
                                        new NamePath(
                                            new[] {new ExplicitName("x") })),
                                    new Constant("1")),
                                new StaticMethodReferance(
                                    new NamePath(
                                        new[] {
                                            new ExplicitName("Fib") }))),
                            new LeftCallOperation(
                                new SubtractOperation(
                                    new ParameterReferance(
                                        new NamePath(
                                            new[] {new ExplicitName("x") })),
                                    new Constant("2")),
                                new StaticMethodReferance(
                                    new NamePath(
                                        new[] {
                                            new ExplicitName("Fib") })))))
                },
                new ExplicitName("Fib"));
            
        }
    }
}
