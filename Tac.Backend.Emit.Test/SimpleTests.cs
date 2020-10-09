using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Instantiated.Elements;
using Xunit;

namespace Tac.Backend.Emit.Test
{
    public class SimpleTests
    {
        [Fact]
        public void Simplist() {
            Compiler.BuildAndRun(
                new List<ICodeElement>{ 
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void Add()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            AddOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1),ConstantNumber.CreateAndBuild(1)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void Multiply()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            MultiplyOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2),ConstantNumber.CreateAndBuild(2)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void LessThen() {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                            EntryPointDefinition.CreateAndBuild(
                                Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                new List<ICodeElement> {
                                    LessThanOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2),ConstantNumber.CreateAndBuild(2)),
                                    ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                },
                                Array.Empty<ICodeElement>())
                });
        }


        [Fact]
        public void If()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            IfOperation.CreateAndBuild(
                                ConstantBool.CreateAndBuild(true),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                    new List<ICodeElement> {
                                        ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                    },
                                    Array.Empty<ICodeElement>()
                                )),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void Else()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ElseOperation.CreateAndBuild(
                                ConstantBool.CreateAndBuild(false),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                    new List<ICodeElement> {
                                        ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                    },
                                    Array.Empty<ICodeElement>()
                                )),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void IfElse()
        {
            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ElseOperation.CreateAndBuild(
                                IfOperation.CreateAndBuild(
                                    ConstantBool.CreateAndBuild(true),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                        new List<ICodeElement> {
                                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                        },
                                        Array.Empty<ICodeElement>()
                                    )),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                    new List<ICodeElement> {
                                        ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                    },
                                    Array.Empty<ICodeElement>()
                                )),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
                });
        }

        [Fact]
        public void AssignMemberReference() {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
            });
        }

        [Fact]
        public void AssignMemberReferenceAnyType()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
            });
        }

        [Fact]
        public void AssignMemberAndAddReference()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AddOperation.CreateAndBuild(AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),ConstantNumber.CreateAndBuild(3)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
            });
        }


        [Fact]
        public void PassThroughFunc()
        {
            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(),Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                        }),
                        new List<ICodeElement> {
                            NextCallOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) ,
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
                                    new NumberType(),
                                    new NumberType(),
                                    input,
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic( input,false)
                                    }),
                                    new List<ICodeElement>{
                                        ReturnOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(input))
                                    },
                                    Array.Empty<ICodeElement>())),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())},
                    Array.Empty<ICodeElement>()) 
                });
        }


        // TODO boxing unboxing and any!
        // I don't think I should need boxing tho
        // Func should be <double, double>

        [Fact]
        public void ClosureFunc()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(3) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            NextCallOperation.CreateAndBuild(
                                ConstantNumber.CreateAndBuild(2) ,
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
                                    new NumberType(),
                                    new NumberType(),
                                    input,
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(input,false)
                                    }),
                                    new List<ICodeElement>{
                                        ReturnOperation.CreateAndBuild(
                                            AddOperation.CreateAndBuild(
                                                Model.Instantiated.MemberReference.CreateAndBuild(input),
                                                Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)))
                                    },
                                    Array.Empty<ICodeElement>())),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())},
                    Array.Empty<ICodeElement>())
                }); ; ;
        }
    }
}
