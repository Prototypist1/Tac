using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {
        // 🤫 the power was in you all along
        internal class TypeSolution 
        {
            private readonly IReadOnlyDictionary<IHavePublicMembers, IReadOnlyList<TypeProblem2.Member>> publicMembers;
            private readonly IReadOnlyDictionary<IHavePrivateMembers, IReadOnlyList<TypeProblem2.Member>> privateMembers;
            private readonly IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map;
            private readonly IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts;
            private readonly IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn;
            private readonly IReadOnlyDictionary<IOrType<OuterFlowNode2<TypeProblem2.Method>, OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.InferredType>>, TypeProblem2.TransientMember> methodOut;
            private readonly IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint;

            private readonly IReadOnlyDictionary<TypeProblem2.MethodType, OuterFlowNode2<TypeProblem2.MethodType>> methodFlowNodes;
            private readonly IReadOnlyDictionary<TypeProblem2.Type, OuterFlowNode2<TypeProblem2.Type>> typeFlowNodes;
            private readonly IReadOnlyDictionary<TypeProblem2.Object, OuterFlowNode2<TypeProblem2.Object>> objectFlowNodes;
            private readonly IReadOnlyDictionary<TypeProblem2.OrType, OuterFlowNode2<TypeProblem2.OrType>> orFlowNodes;
            private readonly IReadOnlyDictionary<TypeProblem2.InferredType, OuterFlowNode2<TypeProblem2.InferredType>> inferredFlowNodes;


            private readonly IReadOnlyDictionary<Inflow2, OuterFlowNode2> inflowLookup;

            public TypeSolution(
                IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map,
                IReadOnlyDictionary<IHavePublicMembers, IReadOnlyList<TypeProblem2.Member>> publicMembers,
                IReadOnlyDictionary<IHavePrivateMembers, IReadOnlyList<TypeProblem2.Member>> privateMembers,
                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
                IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn,
                IReadOnlyDictionary<IOrType<OuterFlowNode2<TypeProblem2.Method>, OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.InferredType>>, TypeProblem2.TransientMember> methodOut,
                IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint)
            {
                this.map = map ?? throw new ArgumentNullException(nameof(map));
                this.publicMembers = publicMembers ?? throw new ArgumentNullException(nameof(publicMembers));
                this.privateMembers = privateMembers ?? throw new ArgumentNullException(nameof(privateMembers));
                this.orTypeElememts = orTypeElememts ?? throw new ArgumentNullException(nameof(orTypeElememts));
                this.methodIn = methodIn ?? throw new ArgumentNullException(nameof(methodIn));
                this.methodOut = methodOut ?? throw new ArgumentNullException(nameof(methodOut));
                this.moduleEntryPoint = moduleEntryPoint ?? throw new ArgumentNullException(nameof(moduleEntryPoint));
            }


            private readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> cacheType = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();
            public IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType)
            {
                if (!cacheType.ContainsKey(explicitType))
                {
                    var box = new Box<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>();
                    cacheType[explicitType] = box;
                    box.Fill(explicitType.Converter.Convert(this, explicitType));
                }
                return cacheType[explicitType];
            }

            private readonly Dictionary<TypeProblem2.Member, IBox<WeakMemberDefinition>> cacheMember = new Dictionary<TypeProblem2.Member, IBox<WeakMemberDefinition>>();
            
            // just takes the type of the member
            public IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member)
            {
                if (!cacheMember.ContainsKey(member))
                {
                    var box = new Box<WeakMemberDefinition>();
                    cacheMember[member] = box;
                    box.Fill(member.Converter.Convert(this, member));
                }
                return cacheMember[member];
            }

            // this is probably going to require  converter
            public IBox<WeakMemberDefinition> GetMember(FlowNodeMember member)
            {
                if (!cacheMember.ContainsKey(member))
                {
                    var box = new Box<WeakMemberDefinition>();
                    cacheMember[member] = box;
                    box.Fill(member.Converter.Convert(this, member));
                }
                return cacheMember[member];
            }

            private readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition>>> cacheMethod = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition>>>();
            public IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method)
            {
                if (!cacheMethod.ContainsKey(method))
                {
                    var box = new Box<IOrType<WeakMethodDefinition, WeakImplementationDefinition>>();
                    cacheMethod[method] = box;
                    box.Fill(method.Converter.Convert(this, method));
                }
                return cacheMethod[method];
            }

            private readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>>> cacheObject = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>>>();
            public IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object)
            {
                if (!cacheObject.ContainsKey(@object))
                {
                    var box = new Box<IOrType<WeakObjectDefinition, WeakModuleDefinition>>();
                    cacheObject[@object] = box;
                    box.Fill(@object.Converter.Convert(this, @object));
                }
                return cacheObject[@object];
            }

            private readonly Dictionary<TypeProblem2.OrType, IBox<WeakTypeOrOperation>> cacheOrType = new Dictionary<TypeProblem2.OrType, IBox<WeakTypeOrOperation>>();
            public IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType)
            {
                if (!cacheOrType.ContainsKey(orType))
                {
                    var box = new Box<WeakTypeOrOperation>();
                    cacheOrType[orType] = box;
                    box.Fill(orType.Converter.Convert(this, orType));
                }
                return cacheOrType[orType];
            }

            private readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> cacheScope = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            public IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope)
            {
                if (!cacheScope.ContainsKey(scope))
                {
                    var box = new Box<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>();
                    cacheScope[scope] = box;
                    box.Fill(scope.Converter.Convert(this, scope));
                }
                return cacheScope[scope];
            }

            private readonly Dictionary<TypeProblem2.TypeReference, IBox<IOrType<IFrontendType, IError>>> cacheTypeReference = new Dictionary<TypeProblem2.TypeReference, IBox<IOrType<IFrontendType, IError>>>();
            public IBox<IOrType<IFrontendType, IError>> GetTypeReference(TypeProblem2.TypeReference typeReference)
            {
                if (!cacheTypeReference.ContainsKey(typeReference))
                {
                    var box = new Box<IOrType<IFrontendType, IError>>();
                    cacheTypeReference[typeReference] = box;
                    box.Fill(typeReference.Converter.Convert(this, typeReference));
                }
                return cacheTypeReference[typeReference];
            }

            private readonly Dictionary<TypeProblem2.Value, IBox<PlaceholderValue>> cacheValue = new Dictionary<TypeProblem2.Value, IBox<PlaceholderValue>>();

            public IBox<PlaceholderValue> GetValue(TypeProblem2.Value value)
            {
                if (!cacheValue.ContainsKey(value))
                {
                    var box = new Box<PlaceholderValue>();
                    cacheValue[value] = box;
                    box.Fill(value.Converter.Convert(this, value));
                }
                return cacheValue[value];
            }

            private readonly Dictionary<TypeProblem2.MethodType, IBox<MethodType>> cacheMethodType = new Dictionary<TypeProblem2.MethodType, IBox<MethodType>>();

            public IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType)
            {
                if (!cacheMethodType.ContainsKey(methodType))
                {
                    var box = new Box<MethodType>();
                    cacheMethodType[methodType] = box;
                    box.Fill(methodType.Converter.Convert(this, methodType));
                }
                return cacheMethodType[methodType];
            }

            private readonly Dictionary<Tpn.OuterFlowNode2, IBox<IFrontendType>> cacheInferredType = new Dictionary<Tpn.OuterFlowNode2, IBox<IFrontendType>>();

            public IBox<IFrontendType> GetInferredType(Tpn.OuterFlowNode2 inferredType, Tpn.IConvertTo<Tpn.OuterFlowNode2, IFrontendType> converter)
            {
                if (!cacheInferredType.ContainsKey(inferredType))
                {
                    var box = new Box<IFrontendType>();
                    cacheInferredType[inferredType] = box;
                    box.Fill(converter.Convert(this, inferredType));
                }
                return cacheInferredType[inferredType];
            }

            public readonly struct FlowNodeMember
            {
                public FlowNodeMember(IKey key, OuterFlowNode2 flowNode)
                {
                    Key = key ?? throw new ArgumentNullException(nameof(key));
                    FlowNode = flowNode ?? throw new ArgumentNullException(nameof(flowNode));
                }

                public IKey Key { get; }
                public OuterFlowNode2 FlowNode { get; }
            }

            public IReadOnlyList<TypeProblem2.Member> GetPrivateMembers(IHavePrivateMembers privateMembers)
            {
                return privateMembers.PrivateMembers.Values.ToList();
            }

            public IReadOnlyList<FlowNodeMember> GetPublicMembers(OuterFlowNode2 from)
            {
                // this is a poopy cast.
                var realFrom = ((OuterFlowNode2<IHavePublicMembers>)from);



            }

            public OuterFlowNode2<TypeProblem2.MethodType> GetFlowNode(TypeProblem2.MethodType type)
            {
                return methodFlowNodes[type];
            }
            public OuterFlowNode2<TypeProblem2.Type> GetFlowNode(TypeProblem2.Type type)
            {
                return typeFlowNodes[type];
            }

            public OuterFlowNode2<TypeProblem2.Object> GetFlowNode(TypeProblem2.Object type)
            {
                return objectFlowNodes[type];
            }

            public OuterFlowNode2<TypeProblem2.OrType> GetFlowNode(TypeProblem2.OrType type)
            {
                return orFlowNodes[type];
            }

            public OuterFlowNode2<TypeProblem2.InferredType> GetFlowNode(TypeProblem2.InferredType type)
            {
                return inferredFlowNodes[type];
            }

            public IOrType<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError> GetFlowNode(ILookUpType from)
            {
                return map[from].SwitchReturns(
                    x=> OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError> (GetFlowNode(x)),
                    x => OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
                    x => OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
                    x => OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
                    x => OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
                    x => OrType.Make<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError>(x)
                    );
            }

            public OuterFlowNode2 GetFlowNode2(ILookUpType from)
            {
                return map[from].SwitchReturns<OuterFlowNode2>(
                    x => GetFlowNode(x),
                    x => GetFlowNode(x),
                    x => GetFlowNode(x),
                    x => GetFlowNode(x),
                    x => GetFlowNode(x),
                    x => throw new NotImplementedException()
                    );
            }

            //public IOrType<OuterFlowNode2<TypeProblem2.MethodType>, OuterFlowNode2<TypeProblem2.Type>, OuterFlowNode2<TypeProblem2.Object>, OuterFlowNode2<TypeProblem2.OrType>, OuterFlowNode2<TypeProblem2.InferredType>, IError> GetType(ILookUpType from)
            //{
            //    return map[from];
            //}

            public (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from)
            {
                return orTypeElememts[from];
            }

            public bool TryGetResultMember(OuterFlowNode2 from, [MaybeNullWhen(false)] out OuterFlowNode2? transientMember)
            {
                if (!from.Possible.Any()) {
                    transientMember = default;
                    return false;
                }

                if (!from.Possible.Any(x =>x.Output != null))
                {
                    transientMember = default;
                    return false;
                }

                if (from.Possible.Count == 1)
                {
                    transientMember = ToFlowNode(from.Possible.Single().Output!);
                    return true;
                }
                //{A2333086-1634-4C8D-9FB1-453BE0BC2F03}
                transientMember = new OuterFlowNode2<Uhh>(false, from.Possible.SelectMany(x => ToFlowNode(x.Input!).Possible).ToList(), new Uhh());
                return true;
            }

            public bool TryGetInputMember(OuterFlowNode2 from, [MaybeNullWhen(false)] out OuterFlowNode2? member)
            {
                if (!from.Possible.Any())
                {
                    member = default;
                    return false;
                }

                if (!from.Possible.Any(x => x.Input != null))
                {
                    member = default;
                    return false;
                }

                if (from.Possible.Count == 1)
                {
                    member = ToFlowNode(from.Possible.Single().Input!);
                    return true;
                }
                //{A2333086-1634-4C8D-9FB1-453BE0BC2F03}
                member = new OuterFlowNode2<Uhh>(false,from.Possible.SelectMany(x => ToFlowNode(x.Input!).Possible).ToList(), new Uhh());
                return true;
            }

            // this is a stupid solution
            // for {A2333086-1634-4C8D-9FB1-453BE0BC2F03}
            // it is wierd to make OuterFlowNode2 at this point in the process
            // are they inferred??? who know it has no meaning here
            // it is even wierder create a source at this point in the process 
            // so I just don't
            // I create an Uhh
            // this idicates that they don't really have a source
            private class Uhh { }

            private OuterFlowNode2 ToFlowNode(IOrType<Inflow2, OuterFlowNode2> orType) =>orType.SwitchReturns(x => inflowLookup[x], x => x);
            

            // this might not need to be IError right now I don't know of anythign driving it
            public IOrType<IBox<IFrontendType>,IError> GetType(OuterFlowNode2 node)
            {
                // the list of types here comes from the big Or in typeSolution 
                // + Uhh see {A2333086-1634-4C8D-9FB1-453BE0BC2F03}
                if (node is OuterFlowNode2<TypeProblem2.MethodType> typeFlowMethodType)
                {
                    return OrType.Make<IBox<IFrontendType>, IError>(
                        new Box<IFrontendType>(
                            GetMethodType(typeFlowMethodType.Source).GetValue()));
                }
                if (node is OuterFlowNode2<TypeProblem2.Type> typeFlowNode) {
                    return OrType.Make<IBox<IFrontendType>, IError> (
                        new Box<IFrontendType>(
                            GetExplicitType(typeFlowNode.Source).GetValue().SwitchReturns<IFrontendType>(x=>x.FrontendType(),x=>x.FrontendType(), x=>x)));
                }
                //if (node is OuterFlowNode2<TypeProblem2.Method> flowTypeMethod) {

                //    return OrType.Make<IBox<IFrontendType>, IError>(
                //        new Box<IFrontendType>(
                //            GetMethod(flowTypeMethod.Source).GetValue().SwitchReturns<IFrontendType>(x => x.FrontendType(), x => x.FrontendType())));
                //}
                if (node is OuterFlowNode2<TypeProblem2.Object> typeFlowObject) {
                    return OrType.Make<IBox<IFrontendType>, IError>(
                        new Box<IFrontendType>(
                            GetObject(typeFlowObject.Source).GetValue().SwitchReturns<IFrontendType>(x => x.AssuredReturns(), x => x.AssuredReturns())));
                }
                if (node is OuterFlowNode2<TypeProblem2.OrType> typeFlowOr) {
                    return OrType.Make<IBox<IFrontendType>, IError>(
                        new Box<IFrontendType>(GetOrType(typeFlowOr.Source).GetValue().FrontendType()));
                }
                if (node is OuterFlowNode2<TypeProblem2.InferredType> typeFlowInferred)
                {
                    return OrType.Make<IBox<IFrontendType>, IError>(
                        new Box<IFrontendType>(GetInferredType(typeFlowInferred, new InferredTypeConverter()).GetValue()));
                }

                if (node is OuterFlowNode2<Uhh> typeFlowUhh)
                {
                    return OrType.Make<IBox<IFrontendType>, IError>(
                        new Box<IFrontendType>(GetInferredType(typeFlowUhh, new InferredTypeConverter()).GetValue()));
                }
                throw new NotImplementedException("I thought i had to be one of those");
            }


            public OuterFlowNode2 GetResultMember(OuterFlowNode2<TypeProblem2.MethodType> from)
            {
                if (TryGetResultMember(from, out var res)) {
                    return res!;
                }
                throw new Exception("that should not happen for a method");
            }

            public OuterFlowNode2 GetInputMember(OuterFlowNode2<TypeProblem2.MethodType> from)
            {
                if (TryGetInputMember(from, out var res))
                {
                    return  res!;
                }
                throw new Exception("that should not happen for a method");
            }

            public IIsPossibly<TypeProblem2.Scope> GetEntryPoint(IStaticScope from)
            {
                if (moduleEntryPoint.TryGetValue(from, out var res))
                {
                    return Possibly.Is(res);
                }
                return Possibly.IsNot<TypeProblem2.Scope>();
            }

            public IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
            {
                var res = new List<IOrType<NameKey, ImplicitKey>>();
                foreach (var value in type.GenericOverlays.Values.Select(x=>x.Possibly2()))
                {
                    value.If(definite => {
                        if (definite.IsPlaceHolder) {
                            definite.Key.If(x =>
                            {
                                res.Add(x);
                                return 1; // todo I need a version of this api that takes an action
                            });
                        }
                        return 1; // todo I need a version of this api that takes an action
                    });
                }

                if (res.Any()) {
                    return Possibly.Is(res.ToArray());
                }

                return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            }
        }
    }
}
