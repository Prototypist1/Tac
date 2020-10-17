using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
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

        // 2=: any x
        // x is any y {
        //}
        // empty return
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

        [Fact]
        public void ObjectIsType()
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

            var objectMemberX =
                MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            var objectType = InterfaceType.CreateAndBuild(new List<Tac.Model.Elements.IMemberDefinition> {
                objectMemberX
            });

            var isMember = MemberDefinition.CreateAndBuild(new NameKey("is-type"), objectType, Model.Elements.Access.ReadWrite);

            var objectMember = MemberDefinition.CreateAndBuild(new NameKey("obj"), new AnyType(), Model.Elements.Access.ReadWrite);
            var zMember = MemberDefinition.CreateAndBuild(new NameKey("z"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(objectMember,false),
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
                                objectDefiniton,
                                Model.Instantiated.MemberReference.CreateAndBuild(objectMember)
                            ),
                            TryAssignOperation.CreateAndBuild(
                                Model.Instantiated.MemberReference.CreateAndBuild(objectMember),
                                Model.Instantiated.MemberReference.CreateAndBuild(isMember),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(
                                        new List<IsStatic>{
                                            new IsStatic(zMember,false)
                                        }
                                    ),
                                    new List<ICodeElement>{
                                        AssignOperation.CreateAndBuild(
                                            PathOperation.CreateAndBuild(
                                                Model.Instantiated.MemberReference.CreateAndBuild(isMember),
                                                Model.Instantiated.MemberReference.CreateAndBuild(objectMemberX)
                                                ),
                                            Model.Instantiated.MemberReference.CreateAndBuild(zMember))
                                    },
                                    Array.Empty<ICodeElement>()
                                    ),
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(isMember, false),
                                    })
                                ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()
                    )
                }
           );
        }

        // any func = Method[abc-type,ab-type] input { input return;  };
        // func is method [abcd-type, a-type] cast {
        //  obj { 1=: a; 2 =: b; 3 =: c 4=:d } > cast;
        //}
        //
        [Fact]
        public void FunctionIsType() {

            var abcdType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("b"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("c"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("d"),new NumberType(),Access.ReadWrite)
            });

            var abcType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("b"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("c"),new NumberType(),Access.ReadWrite)
            });

            var abType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("b"),new NumberType(),Access.ReadWrite)
            });

            var aType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite)
            });

            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), abcType, Access.ReadWrite);
            var func = MemberDefinition.CreateAndBuild(new NameKey("func"), new AnyType(), Access.ReadWrite);
            var cast = MemberDefinition.CreateAndBuild(new NameKey("cast"), MethodType.CreateAndBuild(abcdType, aType), Access.ReadWrite);


            var objectA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var objectB = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);
            var objectC = MemberDefinition.CreateAndBuild(new NameKey("c"), new NumberType(), Access.ReadWrite);
            var objectD = MemberDefinition.CreateAndBuild(new NameKey("d"), new NumberType(), Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(func,false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
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
                                    Array.Empty<ICodeElement>()),
                                Model.Instantiated.MemberReference.CreateAndBuild(func)),

                            TryAssignOperation.CreateAndBuild(
                                    Model.Instantiated.MemberReference.CreateAndBuild(func),
                                    Model.Instantiated.MemberReference.CreateAndBuild(cast),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(new List<IsStatic>{}),
                                        new List<ICodeElement>{ 
                                            NextCallOperation.CreateAndBuild(
                                            ObjectDefiniton.CreateAndBuild(
                                                Scope.CreateAndBuild(new List<IsStatic>{
                                                    new IsStatic( objectA,false),
                                                    new IsStatic( objectB,false),
                                                    new IsStatic( objectC,false),
                                                    new IsStatic( objectD,false)
                                                }),
                                                new List<IAssignOperation>{ 
                                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1), Model.Instantiated.MemberReference.CreateAndBuild(objectA)),
                                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2), Model.Instantiated.MemberReference.CreateAndBuild(objectB)),
                                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(3), Model.Instantiated.MemberReference.CreateAndBuild(objectC)),
                                                    AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(4), Model.Instantiated.MemberReference.CreateAndBuild(objectD)),
                                                }) ,
                                                Model.Instantiated.MemberReference.CreateAndBuild(cast))
                                        },
                                        Array.Empty<ICodeElement>()
                                        ),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(cast, false),
                                    })
                                ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())},
                    Array.Empty<ICodeElement>())
                });
        }

        // I should test a return from inside a try assign

        // any x := 2
        // x is number num {
        //      num < 10 then {
        //          empty return;
        //      }
        //}
        // empty return
        [Fact]
        public void ReturnInsideBlocks() {
            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), Model.Elements.Access.ReadWrite);

            var innerMemberDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            TryAssignOperation.CreateAndBuild(
                                Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition),
                                Model.Instantiated.MemberReference.CreateAndBuild(innerMemberDefinition),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                    }),
                                    new List<ICodeElement>{
                                        IfOperation.CreateAndBuild(
                                            LessThanOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(innerMemberDefinition), ConstantNumber.CreateAndBuild(10)),
                                            BlockDefinition.CreateAndBuild(
                                                Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                                new List<ICodeElement> {
                                                    ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                                                },
                                                Array.Empty<ICodeElement>()
                                            )),
                                    },
                                    Array.Empty<ICodeElement>()),
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(innerMemberDefinition, false)})),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
            });
        }

        // long path test
        // something with an object that is a circlar reference 
        // something with an or type
        // type T { T | empty next }
        // T t := object { T | empty next := object { T | empty next := object { T | empty next :=  object{ T | empty next := empty }}}}
        // t.next is T t-next { t-next.next is T t-next-next { t-next-next.next is T t-next-next-next { t-next-next.next is empty t-next-next-next-empty {  t-next-next-next-empty return } } } }
        // empty return 
        [Fact]
        public void LinkedList() {
            var (node, nodeBuilder) = InterfaceType.Create();

            var nodeOrNull= TypeOr.CreateAndBuild(node, new EmptyType());

            var next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Model.Elements.Access.ReadWrite);

            nodeBuilder.Build(new List<IMemberDefinition>
            {
                next
            });

            var t = MemberDefinition.CreateAndBuild(new NameKey("t"), node, Access.ReadWrite);
            var tnext = MemberDefinition.CreateAndBuild(new NameKey("t-next"), node, Access.ReadWrite);
            var tnextnext = MemberDefinition.CreateAndBuild(new NameKey("t-next-next"), node, Access.ReadWrite);
            var tnextnextnext = MemberDefinition.CreateAndBuild(new NameKey("t-next-next-next"), node, Access.ReadWrite);
            var tnextnextnextempty = MemberDefinition.CreateAndBuild(new NameKey("t-next-next-next-empty"), new EmptyType(), Access.ReadWrite);

            var object1next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Access.ReadWrite);
            var object2next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Access.ReadWrite);
            var object3next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Access.ReadWrite);
            var object4next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Access.ReadWrite);


            Compiler.BuildAndRun(
                new List<ICodeElement>{
                    EntryPointDefinition.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(t, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
                                ObjectDefiniton.CreateAndBuild(
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(object1next, false)
                                }),
                                new List<IAssignOperation>{
                                    AssignOperation.CreateAndBuild(
                                        ObjectDefiniton.CreateAndBuild(
                                        Scope.CreateAndBuild(new List<IsStatic>{
                                            new IsStatic(object2next, false)
                                        }),
                                        new List<IAssignOperation>{
                                            AssignOperation.CreateAndBuild(
                                                ObjectDefiniton.CreateAndBuild(
                                                Scope.CreateAndBuild(new List<IsStatic>{
                                                    new IsStatic(object3next, false)
                                                }),
                                                new List<IAssignOperation>{
                                                    AssignOperation.CreateAndBuild(
                                                        ObjectDefiniton.CreateAndBuild(
                                                        Scope.CreateAndBuild(new List<IsStatic>{
                                                            new IsStatic(object4next, false)
                                                        }),
                                                        new List<IAssignOperation>{
                                                            AssignOperation.CreateAndBuild(
                                                                EmptyInstance.CreateAndBuild(),
                                                                Model.Instantiated.MemberReference.CreateAndBuild(object4next))
                                                        }),
                                                        Model.Instantiated.MemberReference.CreateAndBuild(object3next))
                                                }),
                                                Model.Instantiated.MemberReference.CreateAndBuild(object2next))
                                        }),
                                        Model.Instantiated.MemberReference.CreateAndBuild(object1next))
                                }),
                                Model.Instantiated.MemberReference.CreateAndBuild(t)),
                                TryAssignOperation.CreateAndBuild(
                                    PathOperation.CreateAndBuild( Model.Instantiated.MemberReference.CreateAndBuild(t), Model.Instantiated.MemberReference.CreateAndBuild(next)),
                                    Model.Instantiated.MemberReference.CreateAndBuild(tnext),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(new List<IsStatic>{
                                        }),
                                        new List<ICodeElement>{
                                            TryAssignOperation.CreateAndBuild(
                                                PathOperation.CreateAndBuild( Model.Instantiated.MemberReference.CreateAndBuild(tnext), Model.Instantiated.MemberReference.CreateAndBuild(next)),
                                                Model.Instantiated.MemberReference.CreateAndBuild(tnextnext),
                                                BlockDefinition.CreateAndBuild(
                                                    Scope.CreateAndBuild(new List<IsStatic>{
                                                    }),
                                                    new List<ICodeElement>{


                                                        TryAssignOperation.CreateAndBuild(
                                                            PathOperation.CreateAndBuild( Model.Instantiated.MemberReference.CreateAndBuild(tnextnext), Model.Instantiated.MemberReference.CreateAndBuild(next)),
                                                            Model.Instantiated.MemberReference.CreateAndBuild(tnextnextnext),
                                                            BlockDefinition.CreateAndBuild(
                                                                Scope.CreateAndBuild(new List<IsStatic>{
                                                                }),
                                                                new List<ICodeElement>{
                                                                    TryAssignOperation.CreateAndBuild(
                                                                        PathOperation.CreateAndBuild( Model.Instantiated.MemberReference.CreateAndBuild(tnextnextnext), Model.Instantiated.MemberReference.CreateAndBuild(next)),
                                                                        Model.Instantiated.MemberReference.CreateAndBuild(tnextnextnextempty),
                                                                        BlockDefinition.CreateAndBuild(
                                                                            Scope.CreateAndBuild(new List<IsStatic>{
                                                                            }),
                                                                            new List<ICodeElement>{
                                                                                ReturnOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(tnextnextnextempty))
                                                                            },
                                                                            Array.Empty<ICodeElement>()),
                                                                        Scope.CreateAndBuild(new List<IsStatic>{
                                                                            new IsStatic(tnextnextnextempty, false)})),
                                                                },
                                                                Array.Empty<ICodeElement>()),
                                                            Scope.CreateAndBuild(new List<IsStatic>{
                                                                new IsStatic(tnextnextnext, false)})),
                                                    },
                                                    Array.Empty<ICodeElement>()),
                                                Scope.CreateAndBuild(new List<IsStatic>{
                                                    new IsStatic(tnextnext, false)})),
                                        },
                                        Array.Empty<ICodeElement>()),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(tnext, false)})),
                                ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>())
            });
        }

        // maybe a simpler circular reference 

    }
}
