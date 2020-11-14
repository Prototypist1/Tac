using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using Tac.Backend.Emit.Support;
using Tac.Backend.Emit.Visitors;
using Tac.Backend.Emit.Walkers;
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
        public void Simplist()
        {
            Compiler.BuildAndRun<double, object>(
                    Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
                , 0.0);
        }

        [Fact]
        public void Add()
        {
            var res = Compiler.BuildAndRun<double, double>(
                    Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                            EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                                Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                new List<ICodeElement> {
                                    ReturnOperation.CreateAndBuild(AddOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1),ConstantNumber.CreateAndBuild(1)))
                                },
                                Array.Empty<ICodeElement>()))
                , 0.0);

            Assert.Equal(2.0, res);
        }

        [Fact]
        public void Multiply()
        {
            var res = Compiler.BuildAndRun<double, double>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        new List<ICodeElement> {
                            ReturnOperation.CreateAndBuild(MultiplyOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2),ConstantNumber.CreateAndBuild(2)))
                        },
                        Array.Empty<ICodeElement>()))
                , 0.0);

            Assert.Equal(4.0, res);
        }

        [Fact]
        public void LessThen()
        {
            var res = Compiler.BuildAndRun<double, bool>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                            EntryPointDefinition.CreateAndBuild(new BooleanType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                                Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                new List<ICodeElement> {
                                    ReturnOperation.CreateAndBuild(LessThanOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2),ConstantNumber.CreateAndBuild(2)))
                                },
                                Array.Empty<ICodeElement>()))
                , 0.0);

            Assert.False(res);
        }


        [Fact]
        public void If()
        {
            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
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
                        Array.Empty<ICodeElement>()))
                , 0.0);
        }

        [Fact]
        public void Else()
        {
            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
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
                        Array.Empty<ICodeElement>()))
                , 0.0);
        }

        [Fact]
        public void IfElse()
        {
            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
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
                        Array.Empty<ICodeElement>()))
                , 0.0);
        }

        [Fact]
        public void AssignMemberReference()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
            , 0.0);
        }

        [Fact]
        public void AssignMemberReferenceAnyType()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
            , 0.0);
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

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
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
                        Array.Empty<ICodeElement>()))
            , 0.0);
        }


        // 2 + (2=:x)
        [Fact]
        public void AssignMemberAndAddReference()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite), 
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AddOperation.CreateAndBuild(AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),ConstantNumber.CreateAndBuild(3)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
           , 0.0);
        }


        [Fact]
        public void PassThroughFunc()
        {
            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>
                        {
                        }),
                        new List<ICodeElement> {
                            NextCallOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) ,
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
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
                    Array.Empty<ICodeElement>()))
                , 0.0);
        }


        [Fact]
        public void ClosureFunc()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);

            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(3) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            NextCallOperation.CreateAndBuild(
                                ConstantNumber.CreateAndBuild(2) ,
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
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
                    Array.Empty<ICodeElement>()))
                , 0.0);
        }


        [Fact]
        public void CreateObject()
        {

            var xDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);
            var yDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic> { }),
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
                    ))
           , 0.0);
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

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
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
                    ))
           , 0.0);
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

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
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
                    ))
           , 0.0);
        }

        // any func = Method[abc-type,ab-type] input { input return;  };
        // func is method [abcd-type, a-type] cast {
        //  obj { 1=: a; 2 =: b; 3 =: c 4=:d } > cast;
        //}
        //
        [Fact]
        public void FunctionIsType()
        {

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

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(func,false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
                                    abType,
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
                    Array.Empty<ICodeElement>()))
                , 0.0);
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
        public void ReturnInsideBlocks()
        {
            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new AnyType(), Model.Elements.Access.ReadWrite);

            var innerMemberDefinition = MemberDefinition.CreateAndBuild(new NameKey("y"), new NumberType(), Model.Elements.Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
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
                        Array.Empty<ICodeElement>()))
            , 0.0);
        }

        // long path test
        // something with an object that is a circlar reference 
        // something with an or type
        // type T { T | empty next }
        // T t := object { T | empty next := object { T | empty next := object { T | empty next :=  object{ T | empty next := empty }}}}
        // t.next is T t-next { t-next.next is T t-next-next { t-next-next.next is T t-next-next-next { t-next-next.next is empty t-next-next-next-empty {  t-next-next-next-empty return } } } }
        // empty return 
        [Fact]
        public void LinkedList()
        {
            var (node, nodeBuilder) = InterfaceType.Create();

            var nodeOrNull = TypeOr.CreateAndBuild(node, new EmptyType());

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


            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
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
                        Array.Empty<ICodeElement>()))
            , 0.0);
        }


        // a long path of fields
        [Fact]
        public void LongPath()
        {

            var object4next = MemberDefinition.CreateAndBuild(new NameKey("next"), new NumberType(), Access.ReadWrite);
            var object4 = ObjectDefiniton.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(object4next, false)
                                    }),
                                    new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(5),
                                            Model.Instantiated.MemberReference.CreateAndBuild(object4next))
                                    });

            var object3next = MemberDefinition.CreateAndBuild(new NameKey("next"), object4.Returns(), Access.ReadWrite);
            var object3 = ObjectDefiniton.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(object3next, false)
                                    }),
                                    new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            object4,
                                            Model.Instantiated.MemberReference.CreateAndBuild(object3next))
                                    });

            var object2next = MemberDefinition.CreateAndBuild(new NameKey("next"), object3.Returns(), Access.ReadWrite);
            var object2 = ObjectDefiniton.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(object2next, false)
                                    }),
                                    new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            object3,
                                            Model.Instantiated.MemberReference.CreateAndBuild(object2next))
                                    });

            var object1next = MemberDefinition.CreateAndBuild(new NameKey("next"), object2.Returns(), Access.ReadWrite);
            var object1 = ObjectDefiniton.CreateAndBuild(
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(object1next, false)
                                    }),
                                    new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            object2,
                                            Model.Instantiated.MemberReference.CreateAndBuild(object1next))
                                    });


            var member = MemberDefinition.CreateAndBuild(new NameKey("root"), object1.Returns(), Access.ReadWrite);


            Compiler.BuildAndRun<double, object>(
                      Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(member, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(
                                object1,
                                Model.Instantiated.MemberReference.CreateAndBuild(member)),
                            AssignOperation.CreateAndBuild(
                                ConstantNumber.CreateAndBuild(10),
                                PathOperation.CreateAndBuild(
                                    PathOperation.CreateAndBuild(
                                        PathOperation.CreateAndBuild(
                                            PathOperation.CreateAndBuild(
                                                Model.Instantiated.MemberReference.CreateAndBuild(member),
                                                Model.Instantiated.MemberReference.CreateAndBuild(object1next)),
                                            Model.Instantiated.MemberReference.CreateAndBuild(object2next)),
                                        Model.Instantiated.MemberReference.CreateAndBuild(object3next)),
                                    Model.Instantiated.MemberReference.CreateAndBuild(object4next))),
                            PathOperation.CreateAndBuild(
                                PathOperation.CreateAndBuild(
                                    PathOperation.CreateAndBuild(
                                        PathOperation.CreateAndBuild(
                                            Model.Instantiated.MemberReference.CreateAndBuild(member),
                                            Model.Instantiated.MemberReference.CreateAndBuild(object1next)),
                                        Model.Instantiated.MemberReference.CreateAndBuild(object2next)),
                                    Model.Instantiated.MemberReference.CreateAndBuild(object3next)),
                                Model.Instantiated.MemberReference.CreateAndBuild(object4next)),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
            , 0.0);

        }

        // a method that consumes a method and returns method
        // var m = method [method [number; number]; method [number; number];  ] input { 
        //      method [number;number] inner-input { 
        //          (inner-input > input) + inner-input return;
        //      } return;
        //}
        // 2 > ( method [number;number] input { input return } > m) 
        [Fact]
        public void MethodsOnMethods()
        {


            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), MethodType.CreateAndBuild(new NumberType(), new NumberType()), Access.ReadWrite);
            var innerInput = MemberDefinition.CreateAndBuild(new NameKey("inner-input"), new NumberType(), Access.ReadWrite);

            var methodMethod = Model.Instantiated.MethodDefinition.CreateAndBuild(
                                    MethodType.CreateAndBuild(new NumberType(), new NumberType()),
                                    input,
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic( input,false)
                                    }),
                                    new List<ICodeElement>{
                                        ReturnOperation.CreateAndBuild(
                                            Model.Instantiated.MethodDefinition.CreateAndBuild(
                                                new NumberType(),
                                                innerInput,
                                                Scope.CreateAndBuild(new List<IsStatic>{
                                                    new IsStatic( innerInput,false)
                                                }),
                                                new List<ICodeElement>{
                                                    ReturnOperation.CreateAndBuild(
                                                        AddOperation.CreateAndBuild(
                                                            NextCallOperation.CreateAndBuild(
                                                                Model.Instantiated.MemberReference.CreateAndBuild(innerInput),
                                                                Model.Instantiated.MemberReference.CreateAndBuild(input)),
                                                            Model.Instantiated.MemberReference.CreateAndBuild(innerInput)))
                                                },
                                                Array.Empty<ICodeElement>())
                                            )
                                    },
                                    Array.Empty<ICodeElement>());


            var m = MemberDefinition.CreateAndBuild(new NameKey("m"), methodMethod.Returns(), Access.ReadWrite);


            var input2 = MemberDefinition.CreateAndBuild(new NameKey("input"), new NumberType(), Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                      Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(m, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(methodMethod,Model.Instantiated.MemberReference.CreateAndBuild(m)),
                            NextCallOperation.CreateAndBuild(
                                ConstantNumber.CreateAndBuild(2),
                                NextCallOperation.CreateAndBuild(
                                    Model.Instantiated.MethodDefinition.CreateAndBuild(
                                            new NumberType(),
                                            input2,
                                            Scope.CreateAndBuild(new List<IsStatic>{
                                                new IsStatic( input2,false)
                                            }),
                                            new List<ICodeElement>{
                                                ReturnOperation.CreateAndBuild(
                                                            Model.Instantiated.MemberReference.CreateAndBuild(input2))
                                            },
                                            Array.Empty<ICodeElement>()),
                                    Model.Instantiated.MemberReference.CreateAndBuild(m))),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
            , 0.0);
        }

        //   1 =: number | bool n-b =: any a is number n {}
        [Fact]
        public void NumberOrAnyNumber()
        {

            var nb = MemberDefinition.CreateAndBuild(new NameKey("n-b"), TypeOr.CreateAndBuild(new NumberType(), new BooleanType()), Access.ReadWrite);
            var a = MemberDefinition.CreateAndBuild(new NameKey("a"), new AnyType(), Access.ReadWrite);
            var n = MemberDefinition.CreateAndBuild(new NameKey("n"), new NumberType(), Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                    Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(nb, false),
                            new IsStatic(a, false)
                        }),
                        new List<ICodeElement> {
                            TryAssignOperation.CreateAndBuild(
                                AssignOperation.CreateAndBuild(
                                    AssignOperation.CreateAndBuild(
                                        ConstantNumber.CreateAndBuild(1),
                                        Model.Instantiated.MemberReference.CreateAndBuild(nb)),
                                    Model.Instantiated.MemberReference.CreateAndBuild(a)),
                                Model.Instantiated.MemberReference.CreateAndBuild(n),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                    Array.Empty<ICodeElement>(),
                                    Array.Empty<ICodeElement>()
                                ),
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(n, false)
                                })
                            ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
          , 0.0);
        }


        //   1 =: number | some-type n-t =: any a is some-type t {}
        [Fact]
        public void NumberOrAnyType()
        {

            var (node, nodeBuilder) = InterfaceType.Create();

            var nodeOrNull = TypeOr.CreateAndBuild(node, new EmptyType());

            var next = MemberDefinition.CreateAndBuild(new NameKey("next"), nodeOrNull, Model.Elements.Access.ReadWrite);

            nodeBuilder.Build(new List<IMemberDefinition>
            {
                next
            });

            var nb = MemberDefinition.CreateAndBuild(new NameKey("n-t"), TypeOr.CreateAndBuild(new NumberType(), node), Access.ReadWrite);
            var a = MemberDefinition.CreateAndBuild(new NameKey("a"), new AnyType(), Access.ReadWrite);
            var n = MemberDefinition.CreateAndBuild(new NameKey("t"), node, Access.ReadWrite);

            Compiler.BuildAndRun<double, object>(
                    Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(nb, false),
                            new IsStatic(a, false)
                        }),
                        new List<ICodeElement> {
                            TryAssignOperation.CreateAndBuild(
                                AssignOperation.CreateAndBuild(
                                    AssignOperation.CreateAndBuild(
                                        ConstantNumber.CreateAndBuild(1),
                                        Model.Instantiated.MemberReference.CreateAndBuild(nb)),
                                    Model.Instantiated.MemberReference.CreateAndBuild(a)),
                                Model.Instantiated.MemberReference.CreateAndBuild(n),
                                BlockDefinition.CreateAndBuild(
                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                    Array.Empty<ICodeElement>(),
                                    Array.Empty<ICodeElement>()
                                ),
                                Scope.CreateAndBuild(new List<IsStatic>{
                                    new IsStatic(n, false)
                                })
                            ),
                            ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                        },
                        Array.Empty<ICodeElement>()))
          , 0.0);
        }

        // maybe a member on an or type
        //  object {number a := 1; number b := 2; number c := 3} is type { number a } | type {number a; number b} a-or-b { a-or-b.a =: a-or-b.a }
        [Fact]
        public void MemberOnOrType()
        {
            var abType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite),
                MemberDefinition.CreateAndBuild(new NameKey("b"),new NumberType(),Access.ReadWrite)
            });

            var aType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                MemberDefinition.CreateAndBuild(new NameKey("a"),new NumberType(),Access.ReadWrite)
            });

            var orType = TypeOr.CreateAndBuild(aType, abType);

            var orType_a = orType.Members.Where(x => x.Key.SafeIs(out NameKey nameKey) && nameKey.Name == "a").Single();

            var a_or_ab = MemberDefinition.CreateAndBuild(new NameKey("a-or-ab"), orType, Access.ReadWrite);

            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var memB = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);
            var memC = MemberDefinition.CreateAndBuild(new NameKey("c"), new NumberType(), Access.ReadWrite);

            var object1 = ObjectDefiniton.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(memA, false),
                                        new IsStatic(memB, false),
                                        new IsStatic(memC, false)
                        }),
                        new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(1),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memA)),
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(2),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memB)),
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(3),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memC))
                        });

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(new List<IsStatic>
                            {
                            }),
                            new List<ICodeElement> {
                                TryAssignOperation.CreateAndBuild(
                                    object1,
                                    Model.Instantiated.MemberReference.CreateAndBuild(a_or_ab),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                        new List<ICodeElement>{
                                            AssignOperation.CreateAndBuild(
                                                PathOperation.CreateAndBuild(
                                                    Model.Instantiated.MemberReference.CreateAndBuild(a_or_ab),
                                                    Model.Instantiated.MemberReference.CreateAndBuild(orType_a)),
                                                PathOperation.CreateAndBuild(
                                                    Model.Instantiated.MemberReference.CreateAndBuild(a_or_ab),
                                                    Model.Instantiated.MemberReference.CreateAndBuild(orType_a)))
                                        },
                                        Array.Empty<ICodeElement>()
                                    ),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(a_or_ab, false),
                                    })
                                ),
                                ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                            },
                            Array.Empty<ICodeElement>()))
             , 0.0);
        }

        // an or with methods 
        // method[num,num|bool] x = method[bool|num,bool] { true return; }
        // 5 > x
        [Fact]
        public void MethodOfOr()
        {
            var numOrBool = TypeOr.CreateAndBuild(new BooleanType(), new NumberType());
            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), numOrBool, Access.ReadWrite);

            var methodDef = Model.Instantiated.MethodDefinition.CreateAndBuild(
                new BooleanType(),
                input,
                Scope.CreateAndBuild(new List<IsStatic>
                {
                      new IsStatic(input, false),
                }),
                new List<ICodeElement>
                {
                    ReturnOperation.CreateAndBuild(ConstantBool.CreateAndBuild(true))
                },
                Array.Empty<ICodeElement>());

            var x = MemberDefinition.CreateAndBuild(new NameKey("x"), MethodType.CreateAndBuild(new NumberType(), numOrBool), Access.ReadWrite);


            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(new List<IsStatic>{
                                new IsStatic(x, false),
                            }),
                            new List<ICodeElement> {
                                AssignOperation.CreateAndBuild(
                                    methodDef,
                                    Model.Instantiated.MemberReference.CreateAndBuild(x)),
                                NextCallOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(4), Model.Instantiated.MemberReference.CreateAndBuild(x)),
                                ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                            },
                            Array.Empty<ICodeElement>()))
                , 0.0);

        }


        // object { number a := 1; number b := 2} is  bool | type {number a} input {
        //      input is type {number a} has-a {
        //          has-a.a return;
        //      }
        //}
        [Fact]
        public void IsOr()
        {
            var aTypeAMember = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);

            var aType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                aTypeAMember
            });

            var aOrBool = TypeOr.CreateAndBuild(aType, new BooleanType());
            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), aOrBool, Access.ReadWrite);
            var hasA = MemberDefinition.CreateAndBuild(new NameKey("has-a"), aType, Access.ReadWrite);


            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var memB = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);

            var object1 = ObjectDefiniton.CreateAndBuild(
                      Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(memA, false),
                                        new IsStatic(memB, false),
                      }),
                      new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(1),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memA)),
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(2),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memB))
                      });

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                            new List<ICodeElement> {
                                TryAssignOperation.CreateAndBuild(
                                    object1,
                                    Model.Instantiated.MemberReference.CreateAndBuild(input),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                        new List<ICodeElement>{
                                            TryAssignOperation.CreateAndBuild(
                                                Model.Instantiated.MemberReference.CreateAndBuild(input),
                                                Model.Instantiated.MemberReference.CreateAndBuild(hasA),
                                                BlockDefinition.CreateAndBuild(
                                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                                    new List<ICodeElement>{
                                                        ReturnOperation.CreateAndBuild(
                                                            PathOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(hasA),
                                                            Model.Instantiated.MemberReference.CreateAndBuild(aTypeAMember)))
                                                    },
                                                    Array.Empty<ICodeElement>()),
                                                    Scope.CreateAndBuild(new List<IsStatic>{
                                                        new IsStatic(hasA, false),
                                                    }))
                                        },
                                        Array.Empty<ICodeElement>()
                                    ),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(input, false),
                                    })),
                                ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                            },
                            Array.Empty<ICodeElement>()))
                , 0.0);

        }

        // an or type with an any in it 
        // object { number a := 1; number b := 2} is  bool | any input {
        //      input is type {number a} has-a {
        //          has-a.a return;
        //      }
        //}
        [Fact]
        public void IsBoolOrAny()
        {
            var aTypeAMember = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);

            var aType = InterfaceType.CreateAndBuild(new List<IMemberDefinition>
            {
                aTypeAMember
            });

            var anyOrBool = TypeOr.CreateAndBuild(new AnyType(), new BooleanType());
            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), anyOrBool, Access.ReadWrite);
            var hasA = MemberDefinition.CreateAndBuild(new NameKey("has-a"), aType, Access.ReadWrite);


            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var memB = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);

            var object1 = ObjectDefiniton.CreateAndBuild(
                      Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(memA, false),
                                        new IsStatic(memB, false),
                      }),
                      new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(1),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memA)),
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(2),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memB))
                      });

            Compiler.BuildAndRun<double, object>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new AnyType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                            new List<ICodeElement> {
                                TryAssignOperation.CreateAndBuild(
                                    object1,
                                    Model.Instantiated.MemberReference.CreateAndBuild(input),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                        new List<ICodeElement>{
                                            TryAssignOperation.CreateAndBuild(
                                                Model.Instantiated.MemberReference.CreateAndBuild(input),
                                                Model.Instantiated.MemberReference.CreateAndBuild(hasA),
                                                BlockDefinition.CreateAndBuild(
                                                    Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                                    new List<ICodeElement>{
                                                        ReturnOperation.CreateAndBuild(
                                                            PathOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(hasA),
                                                            Model.Instantiated.MemberReference.CreateAndBuild(aTypeAMember)))
                                                    },
                                                    Array.Empty<ICodeElement>()),
                                                    Scope.CreateAndBuild(new List<IsStatic>{
                                                        new IsStatic(hasA, false),
                                                    }))
                                        },
                                        Array.Empty<ICodeElement>()
                                    ),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(input, false),
                                    })),
                                ReturnOperation.CreateAndBuild(EmptyInstance.CreateAndBuild())
                            },
                            Array.Empty<ICodeElement>()))
                , 0.0);

        }

        // something that does not go in to an IS

        // an or type with an any in it 
        // object { number a := 1; number b := 2} is  bool | number input {  
        //      0 return;
        //  }
        //  1 return;
        [Fact]
        public void DoesNotGoInTheOr()
        {
            var numberOrBool = TypeOr.CreateAndBuild(new NumberType(), new BooleanType());
            var input = MemberDefinition.CreateAndBuild(new NameKey("input"), numberOrBool, Access.ReadWrite);


            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);
            var memB = MemberDefinition.CreateAndBuild(new NameKey("b"), new NumberType(), Access.ReadWrite);

            var object1 = ObjectDefiniton.CreateAndBuild(
                      Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(memA, false),
                                        new IsStatic(memB, false),
                      }),
                      new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(1),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memA)),
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(2),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memB))
                      });

            var res = Compiler.BuildAndRun<double, double>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                        EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                            new List<ICodeElement> {
                                TryAssignOperation.CreateAndBuild(
                                    object1,
                                    Model.Instantiated.MemberReference.CreateAndBuild(input),
                                    BlockDefinition.CreateAndBuild(
                                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                                        new List<ICodeElement>{
                                            ReturnOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(0))
                                        },
                                        Array.Empty<ICodeElement>()
                                    ),
                                    Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(input, false),
                                    })),
                                ReturnOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(1))
                            },
                            Array.Empty<ICodeElement>()))
                , 0.0);


            Assert.Equal(1.0, res);
        }


        // test something outside of the entry point
        // x := 2
        // entrypoint {
        //  x return;
        //}
        [Fact]
        public void SomethingOutsideEntryPoint()
        {
            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);


            var res = Compiler.BuildAndRun<double,double>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memA, false),
                        }),
                        new List<IAssignOperation>{
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(2) , Model.Instantiated.MemberReference.CreateAndBuild(memA))
                        },
                        EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                            Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                            new List<ICodeElement> {
                                ReturnOperation.CreateAndBuild(Model.Instantiated.MemberReference.CreateAndBuild(memA))
                            },
                            Array.Empty<ICodeElement>())),
                0.0);


            Assert.Equal(2.0, res);
        }


        // 3 =: x
        // method [ double, double ] test { test + x return } =: method-member
        // 4 =: x
        // 2 > method-member return
        [Fact]
        public void Closure2()
        {

            var memberDefinition = MemberDefinition.CreateAndBuild(new NameKey("x"), new NumberType(), Model.Elements.Access.ReadWrite);
            var methodMember = MemberDefinition.CreateAndBuild(new NameKey("method-member"), MethodType.CreateAndBuild(new NumberType(), new NumberType()), Model.Elements.Access.ReadWrite);

            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(), Model.Elements.Access.ReadWrite);

            var res = Compiler.BuildAndRun<double,double>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(memberDefinition, false),
                            new IsStatic(methodMember, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(3) , Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            AssignOperation.CreateAndBuild(
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
                                        new NumberType(),
                                        input,
                                        Scope.CreateAndBuild(new List<IsStatic>{
                                            new IsStatic(input,false)
                                        }),
                                        new List<ICodeElement>{
                                            ReturnOperation.CreateAndBuild(
                                                AddOperation.CreateAndBuild(
                                                    Tac.Model.Instantiated.MemberReference.CreateAndBuild(input),
                                                    Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)
                                                    ))
                                        },
                                        Array.Empty<ICodeElement>()),
                                Tac.Model.Instantiated.MemberReference.CreateAndBuild(methodMember)),
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(4), Tac.Model.Instantiated.MemberReference.CreateAndBuild(memberDefinition)),
                            ReturnOperation.CreateAndBuild(
                                NextCallOperation.CreateAndBuild(
                                    ConstantNumber.CreateAndBuild(2),
                                    Tac.Model.Instantiated.MemberReference.CreateAndBuild(methodMember)))
                        },
                    Array.Empty<ICodeElement>()))
                , 0.0);

            Assert.Equal(6.0, res);
        }

        // object { 3 =: x} =: o
        // method [ double, double ] test { test + o.x return } =: method-member
        // 4 =: o.x
        // 2 > method-member return
        [Fact]
        public void Closure3()
        {

            var memA = MemberDefinition.CreateAndBuild(new NameKey("a"), new NumberType(), Access.ReadWrite);

            var object1 = ObjectDefiniton.CreateAndBuild(
                      Scope.CreateAndBuild(new List<IsStatic>{
                                        new IsStatic(memA, false),
                      }),
                      new List<IAssignOperation>{
                                        AssignOperation.CreateAndBuild(
                                            ConstantNumber.CreateAndBuild(3),
                                            Model.Instantiated.MemberReference.CreateAndBuild(memA))
                      });

            var o_member = MemberDefinition.CreateAndBuild(new NameKey("o"), object1.Returns(), Model.Elements.Access.ReadWrite);
            var method_member = MemberDefinition.CreateAndBuild(new NameKey("method-member"), MethodType.CreateAndBuild(new NumberType(), new NumberType()), Model.Elements.Access.ReadWrite);

            var input = MemberDefinition.CreateAndBuild(new NameKey("test"), new NumberType(), Model.Elements.Access.ReadWrite);

            var res = Compiler.BuildAndRun<double,double>(
                       Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(),
                    EntryPointDefinition.CreateAndBuild(new NumberType(), MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite),
                        Scope.CreateAndBuild(new List<IsStatic>{
                            new IsStatic(o_member, false),
                            new IsStatic(method_member, false)
                        }),
                        new List<ICodeElement> {
                            AssignOperation.CreateAndBuild(object1 , Tac.Model.Instantiated.MemberReference.CreateAndBuild(o_member)),
                            AssignOperation.CreateAndBuild(
                                Model.Instantiated.MethodDefinition.CreateAndBuild(
                                        new NumberType(),
                                        input,
                                        Scope.CreateAndBuild(new List<IsStatic>{
                                            new IsStatic(input,false)
                                        }),
                                        new List<ICodeElement>{
                                            ReturnOperation.CreateAndBuild(
                                                AddOperation.CreateAndBuild(
                                                    Tac.Model.Instantiated.MemberReference.CreateAndBuild(input),
                                                    PathOperation.CreateAndBuild( Tac.Model.Instantiated.MemberReference.CreateAndBuild(o_member), Tac.Model.Instantiated.MemberReference.CreateAndBuild(memA))
                                                    ))
                                        },
                                        Array.Empty<ICodeElement>()),
                                Tac.Model.Instantiated.MemberReference.CreateAndBuild(method_member)),
                            AssignOperation.CreateAndBuild(ConstantNumber.CreateAndBuild(4), PathOperation.CreateAndBuild( Tac.Model.Instantiated.MemberReference.CreateAndBuild(o_member), Tac.Model.Instantiated.MemberReference.CreateAndBuild(memA))),
                            ReturnOperation.CreateAndBuild(
                                NextCallOperation.CreateAndBuild(
                                    ConstantNumber.CreateAndBuild(2),
                                    Tac.Model.Instantiated.MemberReference.CreateAndBuild(method_member)))
                        },
                    Array.Empty<ICodeElement>()))
                , 0.0);

            Assert.Equal(6.0, res);
        }

        [Fact]
        public void Passthrough()
        {
            var input = MemberDefinition.CreateAndBuild(new NameKey("entry-input"), new NumberType(), Access.ReadWrite);

            var entryPoint = EntryPointDefinition.CreateAndBuild(new NumberType(), input,
                        Scope.CreateAndBuild(new List<IsStatic> { new IsStatic(input, false) }),
                        new List<ICodeElement> { ReturnOperation.CreateAndBuild(Tac.Model.Instantiated.MemberReference.CreateAndBuild(input)) },
                        Array.Empty<ICodeElement>());

            var res = Compiler.BuildAndRun<double,double>(
                    Model.Instantiated.RootScope.CreateAndBuild(
                        Scope.CreateAndBuild(Array.Empty<IsStatic>()),
                        Array.Empty<IAssignOperation>(), entryPoint
                        )
                , 2.0);

            Assert.Equal(2.0, res);
        }


        // this will be useful to keep around 
        //[Fact]
        public void WTF() {

            var assemblyName = new AssemblyName();
            assemblyName.Name = "WTF_Assembly";
            var dynamicAssembly = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = dynamicAssembly.DefineDynamicModule("WTF_Module");

            var typebuilder = moduleBuilder.DefineType("WTF_Compilation", TypeAttributes.Public & TypeAttributes.Class, typeof(TacCompilation));
            var selfField = typebuilder.DefineField("WTF_Compilation_self", typebuilder, FieldAttributes.Static | FieldAttributes.Public);

            var initMethod = typebuilder.DefineMethod(nameof(TacCompilation.Init), MethodAttributes.Public | MethodAttributes.Virtual);
            typebuilder.DefineMethodOverride(initMethod, typeof(TacCompilation).GetMethod(nameof(TacCompilation.Init)));

            var intiGen = initMethod.GetILGenerator();

            // set the self field
            intiGen.Emit(OpCodes.Ldarg_0);
            intiGen.Emit(OpCodes.Stsfld, selfField);

            var outerTypeBuilder = moduleBuilder.DefineType("Outer");
            var outerTypeConstructor = outerTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new System.Type[] { });
            {
                var myConstructorIL = outerTypeConstructor.GetILGenerator();
                myConstructorIL.Emit(OpCodes.Ldarg_0);
                myConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
                myConstructorIL.Emit(OpCodes.Ret);
            }

            var methodTypeBuilder = moduleBuilder.DefineType("myfunc");
            var methodTypeConstructor = methodTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new System.Type[] { });
            {
                var myConstructorIL = methodTypeConstructor.GetILGenerator();
                myConstructorIL.Emit(OpCodes.Ldarg_0);
                myConstructorIL.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
                myConstructorIL.Emit(OpCodes.Ret);
            }

            var mainMethod = outerTypeBuilder.DefineMethod(
                "mainMethod",
                MethodAttributes.Public,
                CallingConventions.HasThis,
                typeof(object), new[] { typeof(object) });
            var mainGen = mainMethod.GetILGenerator();

            mainGen.Emit(OpCodes.Ldc_R8, 3.0);
            mainGen.DeclareLocal(typeof(double));
            mainGen.Emit(OpCodes.Stloc_0);


            var innerMethod = methodTypeBuilder.DefineMethod(
                "methodMethod",
                MethodAttributes.Public,
                CallingConventions.HasThis,
                typeof(double), new[] { typeof(double) });

            var innerMethodIlGenerator = innerMethod.GetILGenerator();
            innerMethodIlGenerator.Emit(OpCodes.Ldarg_1);
            innerMethodIlGenerator.Emit(OpCodes.Ldc_R8, 2.0);
            innerMethodIlGenerator.Emit(OpCodes.Add_Ovf);
            innerMethodIlGenerator.Emit(OpCodes.Ret);



            mainGen.Emit(OpCodes.Newobj, methodTypeConstructor);

            mainGen.Emit(OpCodes.Ldftn, innerMethod);
            mainGen.Emit(OpCodes.Newobj, typeof(Func<double, double>).GetConstructors().First());

            mainGen.Emit(OpCodes.Ldsfld, selfField);
            mainGen.Emit(OpCodes.Ldfld, typesField.Value);
            mainGen.Emit(OpCodes.Ldc_I4_0);
            mainGen.Emit(OpCodes.Ldelem_Ref);


            mainGen.Emit(OpCodes.Newobj, tacMethod_Simple_SimpleConstructor(typeof(double), typeof(double)));



            mainGen.Emit(OpCodes.Ldloc_0);


            var callSimpleSimple = typeof(ITacObject).GetMethod(nameof(ITacObject.Call_Simple_Simple)) ?? throw new NullReferenceException("should not be null!");
            var generic = callSimpleSimple.MakeGenericMethod(typeof(double), typeof(double));

            mainGen.Emit(OpCodes.Callvirt, generic);

            mainGen.Emit(OpCodes.Box, typeof(double));

            mainGen.Emit(OpCodes.Ret);

            intiGen.Emit(OpCodes.Ldarg_0);
            intiGen.Emit(OpCodes.Newobj, outerTypeConstructor);
            intiGen.Emit(OpCodes.Ldftn, mainMethod);
            intiGen.Emit(OpCodes.Newobj, typeof(Func<object, object>).GetConstructors().First());
            intiGen.Emit(OpCodes.Stfld, typeof(TacCompilation).GetField(nameof(TacCompilation<int,int>.main)));

            intiGen.Emit(OpCodes.Ret);


            methodTypeBuilder.CreateType();
            typebuilder.CreateType();
            outerTypeBuilder.CreateType();


            var complitation = (TacCompilation)dynamicAssembly.CreateInstance("WTF_Compilation");

            complitation.indexerArray = new Indexer[] { };
            complitation.verifyableTypesArray = new IVerifiableType[] { MethodType.CreateAndBuild(new NumberType(), new NumberType()) };
            complitation.Init();
            
            var res = ((TacCompilation<object, object>)complitation).main(null);

            Assert.Equal(5.0, res);

        }

        private ConstructorInfo tacMethod_Simple_SimpleConstructor(System.Type input, System.Type output)
        {
            return typeof(TacMethod_Simple_Simple<,>).MakeGenericType(input, output).GetConstructor(new[] { typeof(Func<,>).MakeGenericType(input, output), typeof(IVerifiableType) }) ?? throw new NullReferenceException("should not be null!");
        }

        private readonly Lazy<FieldInfo> typesField = new Lazy<FieldInfo>(() =>
        {
            return typeof(TacCompilation).GetField(nameof(TacCompilation.verifyableTypesArray)) ?? throw new NullReferenceException("should not be null!");
        });
    }
}
