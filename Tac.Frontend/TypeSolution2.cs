﻿using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{
    internal partial class Tpn {
        internal class TypeSolution {

            //readonly Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>> generalLookUp = new Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>>();
            //readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>> objectCache = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>>();
            //readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> scopeOrBlockCache = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>> methodCache = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> typeCache = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();

            //readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>> scopeCache = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>>();

            //readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodeLookUp;

            //readonly Dictionary<TypeProblem2.Method, Scope> methodScopeCache = new Dictionary<TypeProblem2.Method, Scope>();
            //readonly Dictionary<TypeProblem2.Scope, Scope> scopeScopeCache = new Dictionary<TypeProblem2.Scope, Scope>();

            private ConcurrentIndexed<EqualibleHashSet<Tpn.CombinedTypesAnd>, Yolo> cache = new ConcurrentIndexed<EqualibleHashSet<CombinedTypesAnd>, Yolo>();
            private Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes;

            private class Yolo
            {
                internal IOrType<IReadOnlyList<(IKey, IOrType<Yolo, IError>)>, IError> members;

                internal IIsPossibly<IOrType<Yolo, IError>> output;
                internal IIsPossibly<IOrType<Yolo, IError>> input;

                // for or types
                internal IIsPossibly<IOrType<Yolo, IError>> left;
                internal IIsPossibly<IOrType<Yolo, IError>> right;

                internal Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> type = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
            }

            public TypeSolution(
                IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> things,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes) {
                if (things is null)
                {
                    throw new ArgumentNullException(nameof(things));
                }

                this.flowNodes = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));


                foreach (var flowNode in flowNodes)
                {
                    IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError> rep = flowNode.Value.GetValueAs(out IVirtualFlowNode _).ToRep();
                    GetOrAdd(rep);
                }

                IOrType < Yolo, IError> GetOrAdd(IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>  rep){

                    return rep.TransformInner(equalableHashSet => {

                        var myBox = new Yolo();
                        var current = cache.GetOrAdd(equalableHashSet, myBox);

                        // if we added it, fill it
                        if (current == myBox) {


                            myBox.members = equalableHashSet.VirtualMembers().TransformInner(members => members.Select(virtualMember=>(virtualMember.Key, GetOrAdd(virtualMember.Value.Value))).ToArray());
                            myBox.input = equalableHashSet.VirtualInput().TransformInner(input => GetOrAdd(input));
                            myBox.output = equalableHashSet.VirtualOutput().TransformInner(output => GetOrAdd(output));

                            if (equalableHashSet.Count() > 1)
                            {
                                myBox.left = Possibly.Is(GetOrAdd(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualibleHashSet<Tpn.CombinedTypesAnd>(equalableHashSet.Take(equalableHashSet.Count() - 1).ToHashSet()))));
                                myBox.right = Possibly.Is( GetOrAdd(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualibleHashSet<Tpn.CombinedTypesAnd>(new HashSet<Tpn.CombinedTypesAnd>() { equalableHashSet.Last() }))));
                            }
                            else {
                                myBox.left = Possibly.IsNot<IOrType<Yolo, IError>>();
                                myBox.right = Possibly.IsNot<IOrType<Yolo, IError>>();
                            }
                        }
                        return current;
                    });
                }

                foreach (var (key, value) in cache)
                {
                    if (key.Count() == 1)
                    {
                        value.type.Fill(Convert2(key.First(), value));
                    }
                    else 
                    {
                        value.type.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError> (new FrontEndOrType(
                            value.left.IfElseReturn(x => x, () => throw new Exception("better have a left")).SwitchReturns(
                                x=>x.type,
                                e=> new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e))),
                            value.right.IfElseReturn(x => x, () => throw new Exception("better have a right")).SwitchReturns(
                                x => x.type,
                                e => new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e))),
                            value.members.TransformInner(actually => actually.Select(x => new WeakMemberDefinition(
                                Model.Elements.Access.ReadWrite,
                                x.Item1,
                                x.Item2.SwitchReturns(
                                    y => y.type,
                                    y => (IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(y))))).ToList()),
                            value.input.TransformInner(x=>x.SwitchReturns(
                                    y=>y.type,
                                    error => (IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error)))),
                            value.output.TransformInner(x => x.SwitchReturns(
                                    y => y.type,
                                    error => (IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error))))
                            )));
                    }
                }


                IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError> Convert2(Tpn.CombinedTypesAnd flowNode, Yolo yolo)
                {

                    if (flowNode.And.Count == 0)
                    {
                        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType());
                    }

                    var prim = flowNode.Primitive();

                    if (prim.Is2(out var error))
                    {
                        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error);
                    }

                    if (prim.Is1OrThrow().Is(out var _) && flowNode.And.Single().CastTo<PrimitiveFlowNode>().Source.Is(out var source))
                    {
                        // I'd like to not pass "this" here
                        // the primitive convert willn't use it
                        // but... this isn't really ready to use
                        // it's method are not defined at this point in time
                        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(source.Converter.Convert(this, source).Is3OrThrow());
                    }

                    if (yolo.members.Is2(out var e4))
                    {
                        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e4);
                    }
                    var members = yolo.members.Is1OrThrow();

                    if (flowNode.VirtualInput().Is(out var inputOr))
                    {
                        if (inputOr.Is2(out var e2))
                        {
                            return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e2);
                        }
                    }
                    var input = inputOr?.Is1OrThrow();


                    if (flowNode.VirtualOutput().Is(out var outputOr))
                    {
                        if (outputOr.Is2(out var e3))
                        {
                            return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e3);
                        }

                    }
                    var output = outputOr?.Is1OrThrow();

                    if ((input != default || output != default) && members.Count > 1)
                    {
                        // this might be wrong
                        // methods might end up with more than one member
                        // input counts as a member but it is really something different
                        // todo
                        throw new Exception("so... this is a type and a method?!");
                    }

                    if (input != default && output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                            new MethodType(
                                cache[input].type,
                                cache[output].type));
                    }


                    if (input != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                            new MethodType(
                                cache[input].type,
                                new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new EmptyType()))));
                    }

                    if (output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                            new MethodType(
                                new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new EmptyType())),
                                cache[output].type));
                    }

                    // if it has members it must be a scope
                    if (members.Any())
                    {
                        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                            new HasMembersType(new WeakScope(members.Select(x => new WeakMemberDefinition(
                                Model.Elements.Access.ReadWrite,
                                x.Item1,
                                x.Item2.SwitchReturns(
                                    y => y.type,
                                    y =>(IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>) new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(y))))).ToList())));
                    }

                    return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType());
                }

                // ok, all the boxes are filled!

                //foreach (var thing in things)
                //{
                //    thing.Switch(
                //        methodType => {
                //             methodType.Converter.Convert(this, methodType);
                //        }, 
                //        type => {
                //            type.Converter.Convert(this, type);
                //        }, 
                //        obj => {
                //            obj.Converter.Convert(this, obj);
                //        }, 
                //        orType => {
                //            orType.Converter.Convert(this, orType);
                //        }, 
                //        inferredType => {
                //        }, 
                //        error =>{
                //            // what happens here
                //        });

                //}


                //flowNodeLookUp = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));

                //Queue<Action> todo = new Queue<Action>();

                //foreach (var thing in things)
                //{
                //    var box = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //    thing.Switch(
                //        methodType => {

                //            flowNodes[OrType.Make<ITypeProblemNode, IError>(methodType)].GetValueAs(out IVirtualFlowNode _).ToRep().IfNotError(value =>
                //            {
                //                generalLookUp[value] = box;
                //                todo.Enqueue(() =>
                //                {
                //                    box.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                //                        methodType.Converter.Convert(this, methodType)));
                //                });
                //            });
                //        },
                //        type => {
                //            flowNodes[OrType.Make<ITypeProblemNode, IError>(type)].GetValueAs(out IVirtualFlowNode _).ToRep().IfNotError(value =>
                //            {
                //                generalLookUp[value] = box;
                //                var typeBox = new Box<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>();
                //                typeCache[type] = typeBox;
                //                todo.Enqueue(() =>
                //                {
                //                    type.Converter.Convert(this, type).Switch(
                //                        weakType =>
                //                        {
                //                            box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakType)));
                //                            typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(weakType));
                //                        },
                //                        weakGenericType =>
                //                        {
                //                            box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakGenericType)));
                //                            typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(weakGenericType));
                //                        },
                //                        primitiveType =>
                //                        {
                //                            box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(primitiveType)));
                //                            typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(primitiveType));
                //                        })
                //                    ;
                //                });
                //            });
                //        },
                //        obj => {
                //            flowNodes[OrType.Make<ITypeProblemNode, IError>(obj)].GetValueAs(out IVirtualFlowNode _).ToRep().IfNotError(value => {
                //                generalLookUp[value] = box;
                //                var objBox = new Box<IOrType<WeakObjectDefinition, WeakRootScope>>();
                //                objectCache[obj] = objBox;
                //                todo.Enqueue(() => {

                //                    obj.Converter.Convert(this, obj).Switch(
                //                        weakObj => {
                //                            box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakObj)));
                //                            objBox.Fill(OrType.Make<WeakObjectDefinition, WeakRootScope>(weakObj));
                //                        },
                //                        weakRoot => {
                //                            box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakRoot)));
                //                            objBox.Fill(OrType.Make<WeakObjectDefinition, WeakRootScope>(weakRoot));
                //                        });
                //                });
                //            });
                //        },
                //        orType => {
                //            // or types go letter
                //            // they need to go after anything that they could be looking up
                //        },
                //        inferred => {
                //            // inferred go letter
                //            // they might end up with the same key as something else
                //            // in that case they defer
                //        },
                //        error => {
                //            flowNodes[OrType.Make<ITypeProblemNode, IError>(error)].GetValueAs(out IVirtualFlowNode _).ToRep().IfNotError(value => {
                //                generalLookUp[value] = box;
                //                box.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                //                    error));
                //            });
                //        });
                //}

                //// for now ors go before inferred
                //// this might have to changed if I allow "Cat | inferred x"
                //// then I think they just need to go after their component parts
                //// that might get a little complex lacing them into the whole process
                //foreach (var thing in things)
                //{
                //    var box = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //    thing.Switch(
                //        methodType => { },
                //        type => { },
                //        obj => { },
                //        orType => {

                //            // in some cases this has already been added
                //            // bool | bool say
                //            var key = flowNodes[OrType.Make<ITypeProblemNode, IError>(orType)].GetValueAs(out IVirtualFlowNode _).ToRep();
                //            key.IfNotError(value =>
                //            {
                //                if (!generalLookUp.ContainsKey(value))
                //                {
                //                    generalLookUp[value] = box;
                //                    todo.Enqueue(() =>
                //                    {
                //                        box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(
                //                            orType.Converter.Convert(this, orType))));
                //                        var rep = GetFlowNode(orType).ToRep();
                //                        scopeCache[rep] = rep.Is1OrThrow().VirtualMembers().TransformInner(x => new Scope(x.ToDictionary(
                //                            pair => pair.Key,
                //                            pair => LookUpOrBuild(pair.Value.Value))));

                //                    });
                //                }
                //            });
                //        },
                //        inferred => { },
                //        error => { });
                //}

                //foreach (var thing in things)
                //{

                //    thing.Switch(
                //        methodType => {},
                //        type => {},
                //        obj => {},
                //        orType => {},
                //        inferred => {
                //            var key = flowNodes[OrType.Make<ITypeProblemNode, IError>(inferred)].GetValueAs<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode, IVirtualFlowNode>(out var _).ToRep();
                //            key.Switch(equalibleHashSet =>
                //            {
                //                EnqueConversionForSet(equalibleHashSet);
                //            },
                //            error =>
                //            {
                //            });
                //        },
                //        error => {});
                //}

                //while (todo.TryDequeue(out var action)) {
                //    action();
                //}

                //IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError> Convert(Tpn.CombinedTypesAnd flowNode)
                //{

                //    if (flowNode.And.Count == 0)
                //    {
                //        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType());
                //    }

                //    var prim = flowNode.Primitive();

                //    if (prim.Is2(out var error))
                //    {
                //        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error);
                //    }

                //    if (prim.Is1OrThrow().Is(out var _))
                //    {
                //        throw new Exception("this should have been converted already");
                //    }

                //    var scopeOr = GetScopeOrBuildScope(flowNode);

                //    if (scopeOr.Is2(out var e4))
                //    {
                //        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e4);
                //    }
                //    var scope = scopeOr.Is1OrThrow();

                //    if (flowNode.VirtualInput().Is(out var inputOr))
                //    {
                //        if (inputOr.Is2(out var e2))
                //        {
                //            return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e2);
                //        }
                //    }
                //    var input = inputOr?.Is1OrThrow();


                //    if (flowNode.VirtualOutput().Is(out var outputOr))
                //    {
                //        if (outputOr.Is2(out var e3))
                //        {
                //            return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(e3);
                //        }

                //    }
                //    var output = outputOr?.Is1OrThrow();

                //    if ((input != default || output != default) && scope.members.Count > 1)
                //    {
                //        // this might be wrong
                //        // methods might end up with more than one member
                //        // input counts as a member but it is really something different
                //        // todo
                //        throw new Exception("so... this is a type and a method?!");
                //    }

                //    if (input != default && output != default)
                //    {
                //        // I don't think this is safe see:
                //        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                //        return
                //             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                //            new MethodType(
                //                SafeLookUp(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>( input)),
                //                SafeLookUp(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(output))));
                //    }


                //    if (input != default)
                //    {
                //        // I don't think this is safe see:
                //        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                //        return
                //             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                //            new MethodType(
                //                SafeLookUp(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(input)),
                //                new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new EmptyType()))));
                //    }

                //    if (output != default)
                //    {
                //        // I don't think this is safe see:
                //        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                //        return
                //             OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(
                //            new MethodType(
                //                new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new EmptyType())),
                //                SafeLookUp(OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(output))));
                //    }

                //    // if it has members it must be a scope
                //    if (scope.members.Any())
                //    {
                //        return new WeakTypeDefinition(OrType.Make<IBox<WeakScope>, IError>(new Box<WeakScope>(scope.weakScope))).FrontendType();
                //    }

                //    return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType());
                //}

                //void EnqueConversionForSet(EqualibleHashSet<CombinedTypesAnd> equalibleHashSet)
                //{
                //    if (equalibleHashSet.backing.Count == 0)
                //    {
                //        var box = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //        generalLookUp[equalibleHashSet] = box;
                //        todo.Enqueue(() =>
                //        {
                //            box.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType()));
                //        });
                //        return;
                //    }

                //    if (equalibleHashSet.backing.Count == 1)
                //    {
                //        var box = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //        if (!generalLookUp.TryGetValue(equalibleHashSet, out var _))
                //        {
                //            generalLookUp[equalibleHashSet] = box;

                //            todo.Enqueue(() =>
                //            {
                //                box.Fill(Convert(equalibleHashSet.backing.First()));
                //            });
                //        }
                //        return;
                //    }

                //    foreach (var backer in equalibleHashSet.backing)
                //    {
                //        var backerRep = backer.ToRep().Is1OrThrow();
                //        // we convert each component
                //        if (!generalLookUp.TryGetValue(backerRep, out var _))
                //        {
                //            var innerBox = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //            generalLookUp[backerRep] = innerBox;
                //            todo.Enqueue(() =>
                //            {
                //                innerBox.Fill(Convert(backer));
                //            });
                //        }
                //    }

                //    // build the ors by looking up the componets
                //    var array = equalibleHashSet.backing.ToArray();
                //    var first = array[0];
                //    var second = array[1];
                //    var orKey = new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { first, second });
                //    if (!generalLookUp.TryGetValue(orKey, out var _))
                //    {
                //        var firstOrBox = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //        generalLookUp[orKey] = firstOrBox;
                //        todo.Enqueue(() =>
                //        {
                //            firstOrBox.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new FrontEndOrType(generalLookUp[first.ToRep().Is1OrThrow()].GetValue(), generalLookUp[second.ToRep().Is1OrThrow()].GetValue())));
                //            // TODO this need to get the scope from the FrontEndOrType
                //            scopeCache[OrType.Make <EqualibleHashSet<CombinedTypesAnd>, IError>(orKey)] = orKey.VirtualMembers().TransformInner(x =>new Scope(x.ToDictionary(
                //                pair => pair.Key,
                //                pair => LookUpOrBuild(pair.Value.Value))));
                //        });
                //    }

                //    foreach (var entry in array.Skip(2))
                //    {
                //        var nextOrKeyBacking = orKey.backing.ToHashSet();
                //        nextOrKeyBacking.Add(entry); ;
                //        var nextOrKey = new EqualibleHashSet<CombinedTypesAnd>(nextOrKeyBacking);
                //        if (!generalLookUp.TryGetValue(nextOrKey, out var _))
                //        {
                //            var orBox = new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>();
                //            generalLookUp[nextOrKey] = orBox;
                //            var myOrKey = orKey;
                //            var myEntry = entry;
                //            todo.Enqueue(() =>
                //            {
                //                orBox.Fill(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new FrontEndOrType(generalLookUp[myOrKey].GetValue(), generalLookUp[myEntry.ToRep().Is1OrThrow()].GetValue())));
                //                // TODO this need to get the scope from the FrontEndOrType
                //                scopeCache[OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(orKey)] = orKey.VirtualMembers().TransformInner(x => new Scope(x.ToDictionary(
                //                    pair => pair.Key,
                //                    pair => LookUpOrBuild(pair.Value.Value))));
                //            });
                //        }
                //        orKey = nextOrKey;
                //    }
                //}

                //// has a related method
                //// {164031F9-9DFA-45FB-9C54-B23902DF29DC}
                //IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> LookUpOrBuild(IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> key)
                //{
                //    return key.SwitchReturns(x => {
                //        if (generalLookUp.TryGetValue(x, out var res)) {
                //            return res;
                //        }
                //        EnqueConversionForSet(x);

                //        return generalLookUp[x];

                //    }, error => new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error)));
                //}

                //// has a related method
                //// {7737F6C2-0328-477B-900B-2E6C44AEF6D3}
                //IOrType<Scope, IError> GetScopeOrBuildScope(CombinedTypesAnd node)
                //{
                //    var rep = node.ToRep();

                //    if (scopeCache.TryGetValue(rep, out var current))
                //    {
                //        return current;
                //    }
                //    var scope = node.VirtualMembers().TransformInner(x =>
                //        new Scope(x.ToDictionary(
                //                pair => pair.Key,
                //                pair => LookUpOrBuild(pair.Value.Value))));
                //    scopeCache[rep] = scope;
                //    return scope;
                //}
            }


            // has a related method
            // {7737F6C2-0328-477B-900B-2E6C44AEF6D3}


            //private IOrType<Scope, IError> GetScope(IIsPossibly<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> looksUp) {
            //    return 
            //        looksUp.IfElseReturn(
            //            outer => outer.SwitchReturns(
            //                x => ScopeOrError(x),
            //                x => ScopeOrError(x),
            //                x => ScopeOrError(x),
            //                x => GetTypesScope(GetFlowNode( x)),
            //                x => GetTypesScope(GetFlowNode(x)),
            //                x => OrType.Make<Scope, IError>(x)),
            //            () => OrType.Make<Scope, IError>(Error.Other("this should exist right?")));
            //}


            //internal IOrType<WeakMemberDefinition, IError> GetMember(IIsPossibly<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> looksUp, IKey key)
            //{
            //    return GetScope(looksUp).TransformInner(x =>
            //    {
            //        if (x.members.TryGetValue(key, out var y))
            //        {
            //            return y;
            //        }
            //        else
            //        {
            //            throw new NotImplementedException("what does this mean?, probaly just return an IError");
            //        }
            //    });

            //    //return looksUp.IfElseReturn(outer => outer.SwitchReturns(
            //    //    x =>
            //    //    {
            //    //        if (TryGetMember(x, key, out var y))
            //    //        {
            //    //            return y;
            //    //        }
            //    //        else
            //    //        {
            //    //            throw new NotImplementedException("what does this mean?, probaly just return an IError");
            //    //        }
            //    //    },
            //    //    x =>
            //    //    {
            //    //        if (TryGetMember(x, key, out var y))
            //    //        {
            //    //            return y;
            //    //        }
            //    //        else
            //    //        {
            //    //            throw new NotImplementedException("what does this mean?, probaly just return an IError");
            //    //        }
            //    //    },
            //    //    x =>
            //    //    {
            //    //        if (TryGetMember(x, key, out var y))
            //    //        {
            //    //            return y;
            //    //        }
            //    //        else
            //    //        {
            //    //            throw new NotImplementedException("what does this mean?, probaly just return an IError");
            //    //        }
            //    //    },
            //    //    x => GetMemberFromType(GetFlowNode(x), key),
            //    //    x => GetMemberFromType(GetFlowNode(x), key),
            //    //    x => OrType.Make<WeakMemberDefinition, IError>(x)),
            //    //    () => OrType.Make<WeakMemberDefinition, IError>(Error.Other("this should exist right?")));
            //}

            //private IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError> Convert(EqualibleHashSet<CombinedTypesAnd> flowNode)
            //{

            //    if (flowNode.backing.Count == 0)
            //    {
            //        return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(new AnyType());
            //    }

            //    if (flowNode.backing.Count == 1)
            //    {
            //        return Convert(flowNode.backing.First());
            //    }

            //    // make a big Or!
            //    var array = flowNode.backing.ToArray();
            //    var first = array[0];
            //    var second = array[1];
            //    var res = new FrontEndOrType(Convert(first), Convert(second));
            //    foreach (var entry in array.Skip(2))
            //    {
            //        res = new FrontEndOrType(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(res), Convert(entry));
            //    }

            //    return OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(res);

            //}

            // has a related method
            // {164031F9-9DFA-45FB-9C54-B23902DF29DC}

            //public IOrType<WeakMemberDefinition, IError> GetMemberFromType(TypeProblem2.Object from, IKey key)
            //{
            //    return GetTypesScope(flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _)).TransformInner(x => x.members[key]);
            //}

            //public static IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError> ToType(IOrType<
            //        MethodType,
            //        WeakTypeDefinition,
            //        WeakGenericTypeDefinition,
            //        IPrimitiveType,
            //        WeakObjectDefinition,
            //        WeakRootScope,
            //        WeakTypeOrOperation,
            //        IError> typeOr)
            //{
            //    return typeOr.SwitchReturns(
            //        methodType => OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(methodType),
            //        weakTypeDefinition => weakTypeDefinition.FrontendType(),
            //        weakGenericTypeDefinition => weakGenericTypeDefinition.FrontendType(), //throw new Exception("I don't think this should happen. shouldn't generics be erased at this point?")
            //        primitiveType => OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(primitiveType),
            //        weakObjectDefinition => weakObjectDefinition.Returns(),
            //        weakRootScopeDefinition => weakRootScopeDefinition.Returns(), // is this really a type?? throw new Exception("that is not a type")
            //        weakOrTypeOperation => OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(weakOrTypeOperation.FrontendType()),
            //        error => OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error));
            //}

            //private IVirtualFlowNode GetFlowNode(TypeProblem2.OrType from)
            //{
            //    return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            //}


            //private IVirtualFlowNode GetFlowNode(TypeProblem2.InferredType from)
            //{
            //    return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            //}

            //private IOrType<Scope, IError> ScopeOrError(Tpn.IStaticScope from)
            //{
            //    if (from.SafeIs(out TypeProblem2.Method method))
            //    {
            //        return OrType.Make<Scope,IError>( GetWeakScopeInner(method));
            //    }

            //    if (from.SafeIs(out TypeProblem2.Scope typeProblemScope))
            //    {
            //        return OrType.Make<Scope, IError>(GetWeakScopeInner(typeProblemScope));
            //    }

            //    //if (!flowNodeLookUp.TryGetValue(OrType.Make<ITypeProblemNode, IError>(from), out var node))
            //    //{
            //    //    return OrType.Make<Scope, IError>(Error.Other("node not found"));
            //    //}
            //    //return GetTypesScope(node.GetValueAs(out IVirtualFlowNode _));
            //}

            //internal IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> GetOrType(TypeProblem2.OrType from)
            //{
            //    return SafeLookUp(flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep());
            //}

            //private Scope GetWeakScopeInner(TypeProblem2.Scope from)
            //{
            //    if (scopeScopeCache.TryGetValue(from, out var res))
            //    {
            //        return res;
            //    }
            //    res = new Scope(from.PrivateMembers.ToDictionary(x => x.Key, x => GetType(x.Value)));
            //    scopeScopeCache[from] = res;
            //    return res;
            //}
            //private IVirtualFlowNode GetFlowNode(TypeProblem2.Type from)
            //{
            //    return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            //}

            //private IVirtualFlowNode GetFlowNode(TypeProblem2.Object from)
            //{
            //    return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            //}

            //private IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> SafeLookUp(IOrType<EqualibleHashSet<CombinedTypesAnd>, IError> key)
            //{
            //    return key.SwitchReturns(x => generalLookUp[x], error => new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(error)));
            //}

            //private IOrType<Scope, IError> GetTypesScope(Tpn.IVirtualFlowNode node)
            //{
            //    var rep = node.ToRep();

            //    if (scopeCache.TryGetValue(rep, out var current))
            //    {
            //        return current;
            //    }
            //    var scope = node.VirtualMembers().TransformInner(x =>
            //        new Scope(x.ToDictionary(
            //                pair => pair.Key,
            //                pair => SafeLookUp(pair.Value.Value.TransformInner(virtualNode => virtualNode.ToRep())))));
            //    scopeCache[rep] = scope;
            //    return scope;
            //}

            //private Scope GetWeakScopeInner(TypeProblem2.Method from)
            //{
            //    if (methodScopeCache.TryGetValue(from, out var res))
            //    {
            //        return res;
            //    }
            //    res = new Scope(from.PrivateMembers.ToDictionary(x => x.Key, x => GetType(x.Value)));
            //    methodScopeCache[from] = res;
            //    return res;
            //}



            //internal IOrType<WeakScope, IError> GetWeakTypeScope(TypeProblem2.Type from)
            //{
            //    return GetTypesScope(GetFlowNode(from)).TransformInner(x => x.weakScope);
            //    //    return ScopeOrError(from).TransformInner(x => x.weakScope);
            //}
            //internal IOrType<WeakScope, IError> GetWeakTypeScope(TypeProblem2.Object from)
            //{
            //    return GetTypesScope(GetFlowNode(from)).TransformInner(x => x.weakScope);
            //    //    return ScopeOrError(from).TransformInner(x => x.weakScope);
            //}
            //internal bool TryGetMember(Tpn.IStaticScope from, IKey key, out IOrType<WeakMemberDefinition, IError> res)
            //{
            //    if (from.SafeIs(out TypeProblem2.Method method))
            //    {
            //        var scope = GetWeakScopeInner(method);

            //        if (scope.members.TryGetValue(key, out var memberDef))
            //        {
            //            res = OrType.Make<WeakMemberDefinition, IError>(memberDef);
            //            return true;
            //        }
            //        else
            //        {
            //            res = default;
            //            return false;
            //        }
            //    }
            //    else
            //    if (from.SafeIs(out TypeProblem2.Scope typeProblemScope))
            //    {
            //        var scope = GetWeakScopeInner(typeProblemScope);

            //        if (scope.members.TryGetValue(key, out var memberDef))
            //        {
            //            res = OrType.Make<WeakMemberDefinition, IError>(memberDef);
            //            return true;
            //        }
            //        else
            //        {
            //            res = default;
            //            return false;
            //        }
            //    }
            //    else if (from.SafeIs(out TypeProblem2.Object typeProblemObject))
            //    {
            //        // gross
            //        var value = objectCache[typeProblemObject].GetValue();
            //        if (value.Is1(out var v1))
            //        {
            //            if (v1.Scope.Is1(out var isScope))
            //            {
            //                WeakScope scopeValue = isScope.GetValue();
            //                if (!scopeValue.membersList.Any(z => z.Key.Equals(key)))
            //                {

            //                    res = default;
            //                    return false;
            //                }
            //                else
            //                {
            //                    res = OrType.Make<WeakMemberDefinition, IError>(scopeValue.membersList.Single(z => z.Key.Equals(key)));
            //                    return true;
            //                }

            //            }
            //            else if (v1.Scope.Is2(out var error))
            //            {
            //                res = OrType.Make<WeakMemberDefinition, IError>(error);
            //                return true;
            //            }
            //            else
            //            {
            //                throw new Exception("or overflown");
            //            }
            //        }
            //        else if (value.Is2(out var v2))
            //        {
            //            if (v2.Scope.Is1(out var isScope))
            //            {
            //                WeakScope scopeValue = isScope.GetValue();
            //                if (!scopeValue.membersList.Any(z => z.Key.Equals(key)))
            //                {

            //                    res = default;
            //                    return false;
            //                }
            //                else
            //                {
            //                    res = OrType.Make<WeakMemberDefinition, IError>(scopeValue.membersList.Single(z => z.Key.Equals(key)));
            //                    return true;
            //                }

            //            }
            //            else if (v2.Scope.Is2(out var error))
            //            {
            //                res = OrType.Make<WeakMemberDefinition, IError>(error);
            //                return true;
            //            }
            //            else
            //            {
            //                throw new Exception("or overflown");
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("or overflown");
            //        }
            //    }
            //    else if (from.SafeIs(out TypeProblem2.Type typeProblemType))
            //    {
            //        // gross
            //        var value = typeCache[typeProblemType].GetValue();
            //        if (value.Is1(out var v1))
            //        {
            //            if (v1.Scope.Is1(out var isScope))
            //            {
            //                WeakScope scopeValue = isScope.GetValue();
            //                if (!scopeValue.membersList.Any(z => z.Key.Equals(key)))
            //                {

            //                    res = default;
            //                    return false;
            //                }
            //                else
            //                {
            //                    res = OrType.Make<WeakMemberDefinition, IError>(scopeValue.membersList.Single(z => z.Key.Equals(key)));
            //                    return true;
            //                }

            //            }
            //            else if (v1.Scope.Is2(out var error))
            //            {
            //                res = OrType.Make<WeakMemberDefinition, IError>(error);
            //                return true;
            //            }
            //            else
            //            {
            //                throw new Exception("or overflown");
            //            }
            //        }
            //        else if (value.Is2(out var v2))
            //        {
            //            if (v2.Scope.Is1(out var isScope))
            //            {
            //                WeakScope scopeValue = isScope.GetValue();
            //                if (!scopeValue.membersList.Any(z => z.Key.Equals(key)))
            //                {

            //                    res = default;
            //                    return false;
            //                }
            //                else
            //                {
            //                    res = OrType.Make<WeakMemberDefinition, IError>(scopeValue.membersList.Single(z => z.Key.Equals(key)));
            //                    return true;
            //                }

            //            }
            //            else if (v2.Scope.Is2(out var error))
            //            {
            //                res = OrType.Make<WeakMemberDefinition, IError>(error);
            //                return true;
            //            }
            //            else
            //            {
            //                throw new Exception("or overflown");
            //            }
            //        } else if (value.Is3(out var v3)) {

            //            throw new Exception("how did we get in a primitive, what does it mean?");
            //        }
            //        else
            //        {
            //            throw new Exception("or overflown");
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("I think this is really just for Methods, Scopes, objects and types...");
            //    }

            //    //if (!flowNodeLookUp.TryGetValue(OrType.Make<ITypeProblemNode, IError>(from), out var node))
            //    //{
            //    //    res = default;
            //    //    return false;
            //    //}

            //    //var foundIt = false;

            //    //var outer = GetTypesScope(node.GetValueAs(out IVirtualFlowNode _)).TransformInner(x =>
            //    //{
            //    //    foundIt = x.members.TryGetValue(key, out var member);
            //    //    return member;
            //    //});
            //    //if (foundIt)
            //    //{
            //    //    res = outer;
            //    //    return true;
            //    //}

            //}
            //internal IBox<IOrType<WeakObjectDefinition, WeakRootScope>> GetObject(TypeProblem2.Object from)
            //{
            //    return objectCache[from];
            //}
            //internal IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type from)
            //{
            //    return typeCache[from];
            //}
            //internal IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope from)
            //{
            //    if (scopeOrBlockCache.TryGetValue(from, out var res)) {
            //        return res;
            //    }
            //    res = new Box<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>( from.Converter.Convert(this, from));
            //    scopeOrBlockCache[from] = res;
            //    return res;
            //}
            //internal IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>> GetMethod(TypeProblem2.Method from)
            //{
            //    if (methodCache.TryGetValue(from, out var res))
            //    {
            //        return res;
            //    }
            //    res = new Box<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>(from.Converter.Convert(this, from));
            //    methodCache[from] = res;
            //    return res;
            //}
            //internal IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> GetType(IOrType<IVirtualFlowNode, IError> from)
            //{
            //    return from.SwitchReturns(x => SafeLookUp(x.ToRep()),x=> new Box<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>(OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(x)));
            //}

            //internal IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>> GetType(ILookUpType from)
            //{
            //    return SafeLookUp(
            //    flowNodeLookUp[from.LooksUp.GetOrThrow().SwitchReturns(
            //        methodType => OrType.Make<ITypeProblemNode,IError>(methodType),
            //        type => OrType.Make<ITypeProblemNode, IError>(type),
            //        obj => OrType.Make<ITypeProblemNode, IError>(obj),
            //        orType=> OrType.Make<ITypeProblemNode, IError>(orType),
            //        inferred => OrType.Make<ITypeProblemNode, IError>(inferred),
            //        error=> OrType.Make<ITypeProblemNode, IError>(error))]
            //       .GetValueAs(out IVirtualFlowNode _).ToRep());
            //}
            //internal IVirtualFlowNode GetFlowNode(ILookUpType from)
            //{
            //    return 
            //    flowNodeLookUp[from.LooksUp.GetOrThrow().SwitchReturns(
            //        methodType => OrType.Make<ITypeProblemNode, IError>(methodType),
            //        type => OrType.Make<ITypeProblemNode, IError>(type),
            //        obj => OrType.Make<ITypeProblemNode, IError>(obj),
            //        orType => OrType.Make<ITypeProblemNode, IError>(orType),
            //        inferred => OrType.Make<ITypeProblemNode, IError>(inferred),
            //        error => OrType.Make<ITypeProblemNode, IError>(error))]
            //       .GetValueAs(out IVirtualFlowNode _);
            //}
            //internal WeakScope GetWeakScope(TypeProblem2.Scope from)
            //{
            //    return GetWeakScopeInner(from).weakScope;
            //}
            //internal WeakScope GetWeakScope(TypeProblem2.Method from) => GetWeakScopeInner(from).weakScope;
            //internal WeakMemberDefinition GetMethodMember(TypeProblem2.Method from, IKey key)
            //{
            //    return GetWeakScopeInner(from).members[key];
            //}


            public static IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
            {
                var res = new List<IOrType<NameKey, ImplicitKey>>();
                foreach (var value in type.GenericOverlays.Values.Select(x => x.Possibly2()))
                {
                    value.If(definite =>
                    {
                        if (definite.IsPlaceHolder)
                        {
                            definite.Key.If(x =>
                            {
                                res.Add(x);
                                return 1; // todo I need a version of this api that takes an action
                            });
                        }
                        return 1; // todo I need a version of this api that takes an action
                    });
                }

                if (res.Any())
                {
                    return Possibly.Is(res.ToArray());
                }

                return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            }

            internal IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError> GetType(Tpn.ILookUpType from) =>
               flowNodes[from.LooksUp.GetOrThrow().SwitchReturns(
                   x=> OrType.Make<ITypeProblemNode, IError>(x),
                   x => OrType.Make<ITypeProblemNode, IError>(x),
                   x => OrType.Make<ITypeProblemNode, IError>(x),
                   x => OrType.Make<ITypeProblemNode, IError>(x),
                   x => OrType.Make<ITypeProblemNode, IError>(x),
                   x => OrType.Make<ITypeProblemNode, IError>(x))].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue(),
                    x => OrType.Make<IFrontendType<Model.Elements.IVerifiableType>, IError>(x));

            internal IOrType<FrontEndOrType, IError> GetOrType(TypeProblem2.OrType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y => y.CastTo<FrontEndOrType>()),
                    x => OrType.Make<FrontEndOrType, IError>(x));

            internal IOrType<MethodType, IError> GetMethodType(TypeProblem2.MethodType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y => y.CastTo<MethodType>()),
                    x => OrType.Make<MethodType, IError>(x));

            internal IOrType<HasMembersType, IError> GetHasMemberType(TypeProblem2.Type from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make< HasMembersType, IError > (x));

            internal IOrType<HasMembersType, IError> GetObjectType(TypeProblem2.Object from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make<HasMembersType, IError>(x));


            // this also ends up managing weak scopes that aren't types
            private readonly ConcurrentIndexed<Tpn.IHavePrivateMembers, WeakScope> nonTypeScopes = new ConcurrentIndexed<IHavePrivateMembers, WeakScope>();

            internal WeakScope GetWeakScope(Tpn.IHavePrivateMembers from)=>
                nonTypeScopes.GetOrAdd(from, () =>
                    new WeakScope(from.PrivateMembers.Select(x => new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new Box<IOrType< IFrontendType<Model.Elements.IVerifiableType>, IError>>( GetType(x.Value)))).ToList()));

            internal bool TryGetMember(IStaticScope scope, IKey key, out IOrType<WeakMemberDefinition, IError> res)
            {
                if (flowNodes.TryGetValue(OrType.Make<ITypeProblemNode, IError>(scope), out var flowNode)) {
                    var rep = flowNode.GetValueAs(out IVirtualFlowNode _).ToRep();
                    if (rep.Is1(out var combinedTypesAnds)) {
                        var type = cache[combinedTypesAnds].type.GetValue();
                        if (type.Is1(out var reallyType))
                        {
                            var maybeMember = reallyType.TryGetMember(key, new List<(IFrontendType<Model.Elements.IVerifiableType>, IFrontendType<Model.Elements.IVerifiableType>)>());

                            if (maybeMember.Is1(out var member)) {
                                res = member;
                                return true;
                            } else if (maybeMember.Is2(out var _)) {
                                res = default;
                                return false;
                            }
                            else
                            {
                                res = OrType.Make<WeakMemberDefinition, IError>(maybeMember.Is3OrThrow());
                                return true;
                            }

                        }
                        else {
                            res = OrType.Make<WeakMemberDefinition, IError>(type.Is2OrThrow());
                            return true;
                        }
                    } else {
                        res = OrType.Make<WeakMemberDefinition, IError>(rep.Is2OrThrow());
                        return true;
                    }
                }


                if (scope is Tpn.IHavePrivateMembers privateMembers) {
                    var matches = GetWeakScope(privateMembers).membersList.Where(x => x.Key.Equals(key)).ToArray();

                    if (matches.Length > 1) {
                        throw new Exception("that's not right");
                    }

                    if (matches.Length == 0) {
                        res = default;
                        return false;
                    }

                    res = OrType.Make < WeakMemberDefinition, IError > (matches.First());
                    return true;
                }


                // should pass in an more descritive type so I don't end up with this weird exception
                throw new Exception("I... don't think it should get here.");
            }

            // I am thinking maybe the conversion layer is where we should protect against something being converted twice
            // everything can set a box on the first pass
            // and return the box on the next passes
        }

        //private class Scope {
        //    public readonly Dictionary<IKey, WeakMemberDefinition> members;
        //    public readonly WeakScope weakScope;

        //    public Scope(Dictionary<IKey, IBox<IOrType<IFrontendType<Model.Elements.IVerifiableType>, IError>>> members)
        //    {
        //        this.members = members?.ToDictionary(x=>x.Key, x=> new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, x.Value)) ?? throw new ArgumentNullException(nameof(members));
        //        this.weakScope = new WeakScope(this.members.Select(x => x.Value).ToList());
        //    }
        //}
    }
}
