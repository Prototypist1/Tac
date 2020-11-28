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

            readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IBox<IOrType<IFrontendType, IError>>> generalLookUp = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IBox<IOrType<IFrontendType, IError>>>();
            readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>> scopeCache = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>>();
            readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>> objectCache = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>>();
            readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> scopeOrBlockCache = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>> methodCache = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>>();
            readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> typeCache = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();



            readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodeLookUp;

            public TypeSolution(
                IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> things,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes) {

                flowNodeLookUp = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));

                var todo = new List<Action>();

                foreach (var thing in things)
                {
                    var box = new Box<IOrType<IFrontendType, IError>>();
                    thing.Switch(
                        methodType => {

                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(methodType)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => { 
                                box.Fill(OrType.Make<IFrontendType, IError>(
                                    methodType.Converter.Convert(this, methodType))); 
                            });
                        }, 
                        type => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(type)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            var typeBox = new Box<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>();
                            typeCache[type] = typeBox;
                            todo.Add(() => {
                                type.Converter.Convert(this, type).Switch(
                                    weakType =>
                                    {
                                        box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakType)));
                                        typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(weakType));
                                    },
                                    weakGenericType =>
                                    {
                                        box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakGenericType)));
                                        typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(weakGenericType));
                                    },
                                    primitiveType =>
                                    {
                                        box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(primitiveType)));
                                        typeBox.Fill(OrType.Make<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>(primitiveType));
                                    })
                                ; 
                            });
                        }, 
                        obj => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(obj)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            var objBox= new Box<IOrType<WeakObjectDefinition, WeakRootScope>>();
                            objectCache[obj] = objBox;
                            todo.Add(() => {

                            obj.Converter.Convert(this, obj).Switch(
                                weakObj => {
                                    box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakObj)));
                                    objBox.Fill(OrType.Make<WeakObjectDefinition, WeakRootScope>(weakObj));
                                },
                                weakRoot => { 
                                    box.Fill(ToType(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakRoot)));
                                    objBox.Fill(OrType.Make<WeakObjectDefinition, WeakRootScope>(weakRoot));
                                }); 
                            });
                        }, 
                        orType => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(orType)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => {
                                box.Fill(ToType(OrType.Make < MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError > (
                                    orType.Converter.Convert(this, orType)))); 
                            });
                        }, 
                        inferred => {
                            // inferred go letter
                            // they might end up with the same key as something else
                            // in that case they defer
                        }, 
                        error => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(error)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            box.Fill(OrType.Make<IFrontendType, IError>(
                                error));
                        });
                }

                foreach (var thing in things)
                {
                    
                    thing.Switch(
                        methodType => {},
                        type => {},
                        obj => {},
                        orType => {},
                        inferred => {
                            var key = flowNodes[OrType.Make<ITypeProblemNode, IError>(inferred)].GetValueAs(out IVirtualFlowNode _).ToRep();

                            // we defer if this rep is already claimed
                            if (!generalLookUp.TryGetValue(key, out var _))
                            {
                                key.Switch(equalibleHashSet =>
                                {



                                    if (equalibleHashSet.backing.Count == 0)
                                    {
                                        var box = new Box<IOrType<IFrontendType, IError>>();
                                        generalLookUp[key] = box;
                                        todo.Add(() => {
                                            box.Fill(OrType.Make<IFrontendType, IError>(new AnyType()));
                                        });
                                        return;
                                    }


                                    if (equalibleHashSet.backing.Count == 1)
                                    {
                                        var box = new Box<IOrType<IFrontendType, IError>>();
                                        generalLookUp[key] = box;
                                        todo.Add(() => {
                                            box.Fill(Convert(equalibleHashSet.backing.First()));
                                        });
                                        return;
                                    }

                                    foreach (var backer in equalibleHashSet.backing)
                                    {
                                        // we convert each component
                                        if (!generalLookUp.TryGetValue(backer.ToRep(), out var _))
                                        {
                                            var innerBox = new Box<IOrType<IFrontendType, IError>>();
                                            generalLookUp[key] = innerBox;
                                            todo.Add(() => {
                                                innerBox.Fill(Convert(backer));
                                            });
                                        }

                                        // build the ors by looking up the componets
                                        var array = equalibleHashSet.backing.ToArray();
                                        var first = array[0];
                                        var second = array[1];                                        
                                        var orKey = OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(new HashSet<CombinedTypesAnd> { first, second }));
                                        if (!generalLookUp.TryGetValue(orKey, out var _))
                                        {
                                            var firstOrBox = new Box<IOrType<IFrontendType, IError>>();
                                            generalLookUp[orKey] = firstOrBox;
                                            todo.Add(() => {
                                                firstOrBox.Fill(OrType.Make<IFrontendType, IError>(new FrontEndOrType(generalLookUp[first.ToRep()].GetValue(), generalLookUp[second.ToRep()].GetValue())));
                                            });
                                        }

                                        foreach (var entry in array.Skip(2))
                                        {
                                            var nextOrKeyBacking = orKey.Is1OrThrow().backing.ToHashSet();
                                            nextOrKeyBacking.Add(entry); ;
                                            var nextOrKey = OrType.Make<EqualibleHashSet<CombinedTypesAnd>, IError>(new EqualibleHashSet<CombinedTypesAnd>(nextOrKeyBacking));
                                            if (!generalLookUp.TryGetValue(nextOrKey, out var _))
                                            {
                                                var orBox = new Box<IOrType<IFrontendType, IError>>();
                                                generalLookUp[nextOrKey] = orBox;
                                                todo.Add(() => {
                                                    orBox.Fill(OrType.Make<IFrontendType, IError>(new FrontEndOrType(generalLookUp[orKey].GetValue(), generalLookUp[entry.ToRep()].GetValue())));
                                                });
                                            }
                                            orKey = nextOrKey;
                                        }
                                    }
                                },
                                error =>
                                {
                                    var box = new Box<IOrType<IFrontendType, IError>>();
                                    generalLookUp[key] = box;
                                    generalLookUp[key] = box;
                                    box.Fill(OrType.Make<IFrontendType, IError>(error));
                                });
                            }
                        },
                        error => {});
                }

                foreach (var action in todo)
                {
                    action();
                }
                
            }

            private IOrType<IFrontendType, IError> Convert(EqualibleHashSet<CombinedTypesAnd> flowNode)
            {

                if (flowNode.backing.Count == 0)
                {
                    return OrType.Make<IFrontendType, IError>(new AnyType());
                }

                if (flowNode.backing.Count == 1)
                {
                    return Convert(flowNode.backing.First());
                }

                // make a big Or!
                var array = flowNode.backing.ToArray();
                var first = array[0];
                var second = array[1];
                var res = new FrontEndOrType(Convert(first), Convert(second));
                foreach (var entry in array.Skip(2))
                {
                    res = new FrontEndOrType(OrType.Make<IFrontendType, IError>(res), Convert(entry));
                }

                return OrType.Make<IFrontendType, IError>(res);

            }

            private IOrType<IFrontendType, IError> Convert(Tpn.CombinedTypesAnd flowNode) {

                if (flowNode.And.Count == 0)
                {
                    return OrType.Make<IFrontendType, IError>(new AnyType());
                }

                var prim = flowNode.Primitive();

                if (prim.Is2(out var error))
                {
                    return OrType.Make<IFrontendType, IError>(error);
                }

                if (prim.Is1OrThrow().Is(out var _))
                {
                    throw new Exception("this should have been converted already");
                }

                var scopeOr = GetMyScope(flowNode);

                if (scopeOr.Is2(out var e4))
                {
                    return OrType.Make<IFrontendType, IError>(e4);
                }
                var scope = scopeOr.Is1OrThrow();

               

                if (flowNode.VirtualInput().Is(out var inputOr))
                {
                    if (inputOr.Is2(out var e2))
                    {
                        return OrType.Make<IFrontendType, IError>(e2);
                    }
                }
                var input = inputOr?.Is1OrThrow();


                if (flowNode.VirtualOutput().Is(out var outputOr))
                {
                    if (outputOr.Is2(out var e3))
                    {
                        return OrType.Make<IFrontendType, IError>(e3);
                    }

                }
                var output = outputOr?.Is1OrThrow();

                if ((input != default || output != default) && scope.members.Count > 1)
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
                         OrType.Make<IFrontendType, IError>(
                        new MethodType(
                            generalLookUp[input.ToRep()],
                            generalLookUp[output.ToRep()]));
                }


                if (input != default)
                {
                    // I don't think this is safe see:
                    //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                    return
                         OrType.Make<IFrontendType, IError>(
                        new MethodType(
                            generalLookUp[input.ToRep()],
                            new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new EmptyType()))));
                }

                if (output != default)
                {
                    // I don't think this is safe see:
                    //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                    return
                         OrType.Make<IFrontendType, IError>(
                        new MethodType(
                            new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(new EmptyType())),
                            generalLookUp[output.ToRep()]));
                }

                // if it has members it must be a scope
                if (scope.members.Any())
                {
                    return new WeakTypeDefinition(OrType.Make<IBox<WeakScope>, IError>(new Box<WeakScope>(scope.weakScope))).FrontendType();
                }

                return OrType.Make<IFrontendType, IError>(new AnyType());
            }

            private IOrType<Scope, IError> GetMyScope(Tpn.IVirtualFlowNode node)
            {
                var rep = node.ToRep();

                if (scopeCache.TryGetValue(rep, out var current)) {
                    return current;
                }
                var scope = node.VirtualMembers().TransformInner(x => 
                    new Scope(x.ToDictionary(
                            pair => pair.Key,
                            pair => generalLookUp[pair.Value.TransformInner(virtualNode => virtualNode.ToRep())]),
                        this));
                scopeCache[rep] = scope;
                return scope;
            }


            public IOrType<WeakScope, IError> GetWeakScope(Tpn.IVirtualFlowNode node) {
                return GetMyScope(node).TransformInner(x => x.weakScope);
            }

            public IOrType<WeakMemberDefinition, IError> GetMember(Tpn.IVirtualFlowNode node, IKey key)
            {
                return GetMyScope(node).TransformInner(x => x.members[key]);
            }

            public bool TryGetMember(Tpn.IVirtualFlowNode node, IKey key, out IOrType<WeakMemberDefinition, IError> res)
            {
                var foundIt = false;

                var outer = GetMyScope(node).TransformInner(x => {
                    foundIt = x.members.TryGetValue(key, out var member);
                    return member;
                    });
                if (foundIt) {
                    res = outer;
                    return true;
                }

                res = default;
                return false;
            }

            //public IBox<IOrType<IFrontendType, IError>> GetReturns(Tpn.IVirtualFlowNode node)
            //{
            //    return new FuncBox<IOrType<IFrontendType, IError>>(()=>
            //        node.VirtualOutput().GetOrThrow().TransformInner(x => generalLookUp[x.ToRep()].GetValue()));
            //}

            //public IBox<IOrType<IFrontendType, IError>> GetInput(Tpn.IVirtualFlowNode node)
            //{
            //    return new FuncBox<IOrType<IFrontendType, IError>>(() =>
            //        node.VirtualInput().GetOrThrow().TransformInner(x => generalLookUp[x.ToRep()].GetValue()));
            //}

            public IOrType<IFrontendType, IError> ToType(IOrType<
                    MethodType,
                    WeakTypeDefinition,
                    WeakGenericTypeDefinition,
                    IPrimitiveType,
                    WeakObjectDefinition,
                    WeakRootScope,
                    WeakTypeOrOperation,
                    IError> typeOr)
            {
                return typeOr.SwitchReturns(
                    methodType => OrType.Make<IFrontendType, IError>(methodType),
                    weakTypeDefinition => weakTypeDefinition.FrontendType(),
                    weakGenericTypeDefinition => weakGenericTypeDefinition.FrontendType(), //throw new Exception("I don't think this should happen. shouldn't generics be erased at this point?")
                    primitiveType => OrType.Make<IFrontendType, IError>(primitiveType),
                    weakObjectDefinition => weakObjectDefinition.Returns(),
                    weakRootScopeDefinition => weakRootScopeDefinition.Returns(), // is this really a type?? throw new Exception("that is not a type")
                    weakOrTypeOperation => OrType.Make<IFrontendType, IError>(weakOrTypeOperation.FrontendType()),
                    error => OrType.Make<IFrontendType, IError>(error));
            }

            internal IVirtualFlowNode GetFlowNode(TypeProblem2.Object from)
            {
                return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            }

            internal IVirtualFlowNode GetFlowNode(TypeProblem2.Type from)
            {
                return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            }
            //internal IVirtualFlowNode GetFlowNode(TypeProblem2.Method from)
            //{
            //    return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            //}
            internal IVirtualFlowNode GetFlowNode(TypeProblem2.MethodType from)
            {
                return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            }
            internal IVirtualFlowNode GetFlowNode(Tpn.IStaticScope from)
            {
                return flowNodeLookUp[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _);
            }

            internal IBox<IOrType<WeakObjectDefinition, WeakRootScope>> GetObject(TypeProblem2.Object from)
            {
                return objectCache[from];
            }

            internal IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type from)
            {
                return typeCache[from];
            }

            internal IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope from)
            {
                if (scopeOrBlockCache.TryGetValue(from, out var res)) {
                    return res;
                }
                res = new Box<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>( from.Converter.Convert(this, from));
                scopeOrBlockCache[from] = res;
                return res;
            }


            internal IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>> GetMethod(TypeProblem2.Method from)
            {
                if (methodCache.TryGetValue(from, out var res))
                {
                    return res;
                }
                res = new Box<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>(from.Converter.Convert(this, from));
                methodCache[from] = res;
                return res;
            }


            internal IBox<IOrType<IFrontendType, IError>> GetType(IOrType<IVirtualFlowNode, IError> from)
            {
                return from.SwitchReturns(x => generalLookUp[x.ToRep()],x=> new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(x)));
            }


            public IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
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

            internal IBox<IOrType<IFrontendType, IError>> GetType(ILookUpType from)
            {
                return generalLookUp[
                flowNodeLookUp[from.LooksUp.GetOrThrow().SwitchReturns(
                    methodType => OrType.Make<ITypeProblemNode,IError>(methodType),
                    type => OrType.Make<ITypeProblemNode, IError>(type),
                    obj => OrType.Make<ITypeProblemNode, IError>(obj),
                    orType=> OrType.Make<ITypeProblemNode, IError>(orType),
                    inferred => OrType.Make<ITypeProblemNode, IError>(inferred),
                    error=> OrType.Make<ITypeProblemNode, IError>(error))]
                   .GetValueAs(out IVirtualFlowNode _).ToRep()];
            }

            internal IVirtualFlowNode GetFlowNode(ILookUpType from)
            {
                return 
                flowNodeLookUp[from.LooksUp.GetOrThrow().SwitchReturns(
                    methodType => OrType.Make<ITypeProblemNode, IError>(methodType),
                    type => OrType.Make<ITypeProblemNode, IError>(type),
                    obj => OrType.Make<ITypeProblemNode, IError>(obj),
                    orType => OrType.Make<ITypeProblemNode, IError>(orType),
                    inferred => OrType.Make<ITypeProblemNode, IError>(inferred),
                    error => OrType.Make<ITypeProblemNode, IError>(error))]
                   .GetValueAs(out IVirtualFlowNode _);
            }


            readonly Dictionary<TypeProblem2.Scope, Scope> scopeScopeCache = new Dictionary<TypeProblem2.Scope, Scope>();
            internal WeakScope GetWeakScope(TypeProblem2.Scope from)
            {
                if (scopeScopeCache.TryGetValue(from, out var res)) {
                    return res.weakScope;
                }
                res = new Scope(from.PrivateMembers.ToDictionary(x => x.Key, x => GetType(x.Value)),this);
                scopeScopeCache[from] = res;
                return res.weakScope;
            }
            readonly Dictionary<TypeProblem2.Method, Scope> methodScopeCache = new Dictionary<TypeProblem2.Method, Scope>();

            internal WeakScope GetWeakScope(TypeProblem2.Method from)
            {
                if (methodScopeCache.TryGetValue(from, out var res))
                {
                    return res.weakScope;
                }
                res = new Scope(from.PrivateMembers.ToDictionary(x => x.Key, x => GetType(x.Value)), this);
                methodScopeCache[from] = res;
                return res.weakScope;
            }



            internal WeakMemberDefinition GetMethodMember(TypeProblem2.Method from, IKey key)
            {
                if (methodScopeCache.TryGetValue(from, out var res))
                {
                    return res.members[key];
                }
                res = new Scope(from.PrivateMembers.ToDictionary(x => x.Key, x => GetType(x.Value)), this);
                methodScopeCache[from] = res;
                return res.members[key];
            }

            // I am thinking maybe the conversion layer is where we should protect against something being converted twice
            // everything can set a box on the first pass
            // and return the box on the next passes
        }

        private class Scope {
            public readonly Dictionary<IKey, WeakMemberDefinition> members;
            public readonly WeakScope weakScope;

            public Scope(Dictionary<IKey, IBox<IOrType<IFrontendType, IError>>> members, TypeSolution typeSolution)
            {
                this.members = members?.ToDictionary(x=>x.Key, x=> new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new FuncBox<IOrType<IFrontendType, IError>>(() => x.Value.GetValue()))) ?? throw new ArgumentNullException(nameof(members));
                this.weakScope = ToWeakScope(this.members);
            }

            private WeakScope ToWeakScope(Dictionary<IKey, WeakMemberDefinition> betterView)
            {
                return new WeakScope(betterView.Select(x => (IBox<WeakMemberDefinition>)new Box<WeakMemberDefinition>(x.Value)).ToList());
            }


        }

    }
}
