using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Instantiated.Elements;
using Tac.Model.Operations;
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
        public void AnyIsNumber()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), Model.Elements.Access.ReadWrite);


            var innerMemberDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            TryAssignOperation.CreateAndBuild(
                                Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition),
                                Tac.Model.Instantiated.MemberReference.CreateAndBuild(innerMemberDefinition),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                    }),
                                    new List<ICodeElement>{
                                    },
                                    Array.Empty<ICodeElement>()),
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(innerMemberDefinition, false)})),
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
                });
        }


        [Fact]
        public void CreateObject() {

            var xDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);
            var yDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{ }),
                        new List<ICodeElement> {
                            ObjectDefiniton.CreateAndBuild( 
                                Scope.CreateAndBuild(
                                    new List<IsStatic>{
                                        new IsStatic(xDefinition,false),
                                        new IsStatic(yDefinition,false)
                                    }
                                ),
                                new List<IAssignOperation>{ 
                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(0),Model.Instantiated.MemberReference.CreateAndBuild(xDefinition) ),
                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1),Model.Instantiated.MemberReference.CreateAndBuild(yDefinition))
                                }
                            ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()
                    )
                }
           );
        }

        // (object{ number x = 0, number y = 1} =: type{ number x , number y} obj).x =: number z;
        [Fact]
        public void CreateObjectAndReadMember()
        {
            var xDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);
            var yDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            var objectDefiniton = ObjectDefiniton.CreateAndBuild(
                                Scope.CreateAndBuild(
                                    new List<IsStatic>{
                                        new IsStatic(xDefinition,false),
                                        new IsStatic(yDefinition,false)
                                    }
                                ),
                                new List<IAssignOperation>{
                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(0),Model.Instantiated.MemberReference.CreateAndBuild(xDefinition) ),
                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1),Model.Instantiated.MemberReference.CreateAndBuild(yDefinition))
                                }
                            );

            var objectType = objectDefiniton.Returns();

            var objectMember = MemberDefinition.CreateAndBuild(new NameKey("obj"), objectType, Model.Elements.Access.ReadWrite);
            var zMember = MemberDefinition.CreateAndBuild(new NameKey("z"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(objectMember,false),
                            new IsStatic(zMember,false),
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
                                PathOperation.CreateAndBuild(
                                    AssignOperation.CreateAndBuild( 
                                        objectDefiniton,
                                        Model.Instantiated.MemberReference.CreateAndBuild(objectMember)
                                    ),
                                    Model.Instantiated.MemberReference.CreateAndBuild(xDefinition)
                                 ),
                                 Model.Instantiated.MemberReference.CreateAndBuild(zMember)
                            ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()
                    )
                }
           );
        }
    }
}
