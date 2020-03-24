using Prototypist.Toolbox;
using System;
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
    method [ number ; pair [ number ; ] ] input {
        object {
            input =: x ;
            input =: y ;
        } return ;      
    } =: pairify ;
}";

        public IModuleDefinition ModuleDefinition { get; }

        public PairType()
        {
            var pairTypeNumber =
                InterfaceType.CreateAndBuild(
                        new List<IMemberDefinition>{
                                MemberDefinition.CreateAndBuild(new NameKey("x"),new NumberType(), false) ,
                                MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), false)
                        });

            var inputKey = new NameKey("input");
            var input = MemberDefinition.CreateAndBuild(inputKey, new NumberType(), false);
            
            var methodScope = Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input, false) });

            var localX = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), false);
            var localY = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), false);

            var pairifyKey = new NameKey("pairify");
            var pairify = MemberDefinition.CreateAndBuild(pairifyKey, MethodType.CreateAndBuild(new NumberType(), pairTypeNumber), false);

            ModuleDefinition = Model.Instantiated.ModuleDefinition.CreateAndBuild(
                Scope.CreateAndBuild(
                    new List<IsStatic> { new IsStatic(MemberDefinition.CreateAndBuild(pairifyKey, MethodType.CreateAndBuild(new NumberType(), pairTypeNumber), false), false) }),
                new [] {
                    new OrType<ICodeElement, IError>(AssignOperation.CreateAndBuild(
                        new OrType<ICodeElement, IError>(MethodDefinition.CreateAndBuild(
                            new NumberType(),
                            pairTypeNumber,
                            input,
                            methodScope,
                            new OrType<ICodeElement,IError>[]{
                                new OrType<ICodeElement,IError>(
                                    ReturnOperation.CreateAndBuild(new OrType<ICodeElement, IError>(
                                        ObjectDefiniton.CreateAndBuild(
                                                Scope.CreateAndBuild(
                                                new List<IsStatic> {
                                                    new IsStatic(localX ,false),
                                                    new IsStatic(localY, false)
                                                }),
                                            new IAssignOperation[]{
                                                AssignOperation.CreateAndBuild(
                                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localX))),
                                                AssignOperation.CreateAndBuild(
                                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(input)),
                                                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(localY)))
                                            }))))},
                            Array.Empty<ICodeElement>())),
                    new OrType<ICodeElement, IError>(MemberReference.CreateAndBuild(pairify))))},
                new NameKey("pair-type"),
                EntryPointDefinition.CreateAndBuild(Scope.CreateAndBuild(Array.Empty<IsStatic>()), Array.Empty<IOrType<ICodeElement, IError>>(), Array.Empty<ICodeElement>()));
        }
    }
}