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


            readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IBox<IOrType<
                MethodType,WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType,WeakObjectDefinition, WeakRootScope,WeakTypeOrOperation,
                IError>>> generalLookUp = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IBox<IOrType<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>>>();


            Dictionary<
                IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>> scopeCache = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>>();


            public void Init(
                IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> things,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes) {

                var todo = new List<Action>();

                foreach (var thing in things)
                {
                    var box = new Box<IOrType<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>>();
                    thing.Switch(
                        methodType => {

                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(methodType)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => { 
                                box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation,IError > (
                                    methodType.Converter.Convert(this, methodType))); 
                            });
                        }, 
                        type => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(type)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => {
                                type.Converter.Convert(this, type).Switch(
                                    weakType=> box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakType)),
                                    weakGenericType=> box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakGenericType)),
                                    primitiveType => box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(primitiveType)))
                                ; 
                            });
                        }, 
                        obj => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(obj)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => {
                                obj.Converter.Convert(this, obj).Switch(
                                    weakObj=> box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakObj)), 
                                    weakRoot=> box.Fill(OrType.Make<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>(weakRoot))); 
                            });
                        }, 
                        orType => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(orType)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => {
                                box.Fill(OrType.Make < MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError > (
                                    orType.Converter.Convert(this, orType))); 
                            });
                        }, 
                        inferred => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(inferred)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            todo.Add(() => { 
                            
                            });
                        }, 
                        error => {
                            generalLookUp[flowNodes[OrType.Make<ITypeProblemNode, IError>(error)].GetValueAs(out IVirtualFlowNode _).ToRep()] = box;
                            box.Fill(OrType.Make< MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError> (
                                error));
                        });
                }

                foreach (var action in todo)
                {
                    action();
                }
                
            }

            public IOrType<IFrontendType, IError> Convert(Tpn.TypeSolution typeSolution, EqualibleHashSet<CombinedTypesAnd> flowNode)
            {
                if (flowNode.backing.Count == 0)
                {
                    return OrType.Make<IFrontendType, IError>(new AnyType());
                }

                if (flowNode.backing.Count == 1)
                {
                    return typeSolution.GetInferredType(flowNode.backing.First()).GetValue();
                }

                // make a big Or!
                var array = flowNode.backing.ToArray();
                var first = array[0];
                var second = array[1];
                var res = new FrontEndOrType(typeSolution.GetInferredType(first).GetValue(), typeSolution.GetInferredType(second).GetValue());
                foreach (var entry in array.Skip(2))
                {
                    res = new FrontEndOrType(OrType.Make<IFrontendType, IError>(res), typeSolution.GetInferredType(entry).GetValue());
                }

                return OrType.Make<IFrontendType, IError>(res);

            }

            public IOrType<IFrontendType, IError> Convert(Tpn.TypeSolution typeSolution, Tpn.CombinedTypesAnd flowNode) {

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
                    var single = flowNode.And.Single().Is2OrThrow();
                    return OrType.Make<IFrontendType, IError>(typeSolution.GetExplicitType(single.Source.GetOrThrow()).GetValue().Is3OrThrow());
                }

                var scopeOr = GetMyScope(flowNode);

                if (scopeOr.Is2(out var e4))
                {
                    return OrType.Make<IFrontendType, IError>(e4);
                }
                var scope = scopeOr.Is1OrThrow();

                if (typeSolution.TryGetInputMember(flowNode, out var inputOr))
                {
                    if (inputOr.Is2(out var e2))
                    {
                        return OrType.Make<IFrontendType, IError>(e2);
                    }
                }
                var input = inputOr?.Is1OrThrow();


                if (typeSolution.TryGetResultMember(flowNode, out var outputOr))
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
                            typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(input)).GetValue().TransformInner(x => x.CastTo<IFrontendType>()),
                            typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(output)).GetValue().TransformInner(x => x.CastTo<IFrontendType>())));
                }


                if (input != default)
                {
                    // I don't think this is safe see:
                    //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                    return
                         OrType.Make<IFrontendType, IError>(
                        new MethodType(
                            typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(input)).GetValue().TransformInner(x => x.SafeCastTo<IFrontendType, IFrontendType>()),
                            OrType.Make<IFrontendType, IError>(new EmptyType())));
                }

                if (output != default)
                {
                    // I don't think this is safe see:
                    //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                    return
                         OrType.Make<IFrontendType, IError>(
                        new MethodType(
                            OrType.Make<IFrontendType, IError>(new EmptyType()),
                            typeSolution.GetType(OrType.Make<Tpn.IVirtualFlowNode, IError>(output)).GetValue().TransformInner(x => x.SafeCastTo<IFrontendType, IFrontendType>())));
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


            private IOrType<WeakScope, IError> GetWeakScope(Tpn.IVirtualFlowNode node) {
                return GetMyScope(node).TransformInner(x => x.weakScope);
            }


            private IOrType<WeakMemberDefinition, IError> GetMember(Tpn.IVirtualFlowNode node, IKey key)
            {
                return GetMyScope(node).TransformInner(x => x.members[key]);
            }

            private IBox<IOrType<IFrontendType, IError>> GetReturns(Tpn.IVirtualFlowNode node)
            {
                return new FuncBox<IOrType<IFrontendType, IError>>(()=>
                    node.VirtualOutput().GetOrThrow().TransformInner(x => ToType(generalLookUp[x.ToRep()].GetValue())));
            }

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
                    weakGenericTypeDefinition => throw new Exception("I don't think this should happen. shouldn't generics be erased at this point?"), // weakGenericTypeDefinition.FrontendType()
                    primitiveType => OrType.Make<IFrontendType, IError>(primitiveType),
                    weakObjectDefinition => weakObjectDefinition.Returns(),
                    weakRootScopeDefinition => throw new Exception("that is not a type"), // is this really a type?? weakRootScopeDefinition.Returns()
                    weakOrTypeOperation => OrType.Make<IFrontendType, IError>(weakOrTypeOperation.FrontendType()),
                    error => OrType.Make<IFrontendType, IError>(error));
            }

            // I am thinking maybe the conversion layer is where we should protect against something being converted twice
            // everything can set a box on the first pass
            // and return the box on the next passes
        }

        private class Scope {
            public readonly Dictionary<IKey, WeakMemberDefinition> members;
            public readonly WeakScope weakScope;

            public Scope(Dictionary<IKey, IBox<IOrType<MethodType, WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType, WeakObjectDefinition, WeakRootScope, WeakTypeOrOperation, IError>>> members, TypeSolution typeSolution)
            {
                this.members = members?.ToDictionary(x=>x.Key, x=> new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new FuncBox<IOrType<IFrontendType, IError>>(() => typeSolution.ToType(x.Value.GetValue())
                 ))) ?? throw new ArgumentNullException(nameof(members));
                this.weakScope = ToWeakScope(this.members);
            }

            private WeakScope ToWeakScope(Dictionary<IKey, WeakMemberDefinition> betterView)
            {
                return new WeakScope(betterView.Select(x => (IBox<WeakMemberDefinition>)new Box<WeakMemberDefinition>(x.Value)).ToList());
            }


        }

    }
}
