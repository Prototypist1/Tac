//using Prototypist.Toolbox;
//using Prototypist.Toolbox.Bool;
//using Prototypist.Toolbox.Object;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using Tac.Frontend._3_Syntax_Model.Elements;
//using Tac.Frontend.SyntaxModel.Operations;
//using Tac.Model;
//using Tac.SemanticModel;
//using Tac.SyntaxModel.Elements.AtomicTypes;

//namespace Tac.Frontend.New.CrzayNamespace
//{

//    internal partial class Tpn
//    {

//        // this is a stupid solution
//        // for {A2333086-1634-4C8D-9FB1-453BE0BC2F03}
//        // it is wierd to make IFlowNode at this point in the process
//        // are they inferred??? who know it has no meaning here
//        // it is even wierder create a source at this point in the process 
//        // so I just don't
//        // I create an Uhh
//        // this idicates that they don't really have a source
//        //private class Uhh { }

//        // 🤫 the power was in you all along
//        internal class TypeSolution 
//        {
//            private readonly IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map;
//            private readonly IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts;
//            //private readonly IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint;

//            private readonly IReadOnlyDictionary<TypeProblem2.MethodType, IFlowNode<TypeProblem2.MethodType>> methodFlowNodes;
//            private readonly IReadOnlyDictionary<TypeProblem2.Type, IFlowNode<TypeProblem2.Type>> typeFlowNodes;
//            private readonly IReadOnlyDictionary<TypeProblem2.Object, IFlowNode<TypeProblem2.Object>> objectFlowNodes;
//            private readonly IReadOnlyDictionary<TypeProblem2.OrType, IFlowNode<TypeProblem2.OrType>> orFlowNodes;
//            private readonly IReadOnlyDictionary<TypeProblem2.InferredType, IFlowNode<TypeProblem2.InferredType>> inferredFlowNodes;
//            private readonly IReadOnlyDictionary<TypeProblem2.Member, (IKey Key, IOrType<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope> Owner)> memberLookup;

//            public TypeSolution(
//                IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map, 
//                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts, 
//                //IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint, 
//                IReadOnlyDictionary<TypeProblem2.MethodType, IFlowNode<TypeProblem2.MethodType>> methodFlowNodes, 
//                IReadOnlyDictionary<TypeProblem2.Type, IFlowNode<TypeProblem2.Type>> typeFlowNodes, 
//                IReadOnlyDictionary<TypeProblem2.Object, IFlowNode<TypeProblem2.Object>> objectFlowNodes, 
//                IReadOnlyDictionary<TypeProblem2.OrType, IFlowNode<TypeProblem2.OrType>> orFlowNodes, 
//                IReadOnlyDictionary<TypeProblem2.InferredType, IFlowNode<TypeProblem2.InferredType>> inferredFlowNodes,
//                IReadOnlyDictionary<TypeProblem2.Member, (IKey Key, IOrType<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope> Owner)> memberLookup)
//            {
//                this.map = map ?? throw new ArgumentNullException(nameof(map));
//                this.orTypeElememts = orTypeElememts ?? throw new ArgumentNullException(nameof(orTypeElememts));
//                //this.moduleEntryPoint = moduleEntryPoint ?? throw new ArgumentNullException(nameof(moduleEntryPoint));
//                this.methodFlowNodes = methodFlowNodes ?? throw new ArgumentNullException(nameof(methodFlowNodes));
//                this.typeFlowNodes = typeFlowNodes ?? throw new ArgumentNullException(nameof(typeFlowNodes));
//                this.objectFlowNodes = objectFlowNodes ?? throw new ArgumentNullException(nameof(objectFlowNodes));
//                this.orFlowNodes = orFlowNodes ?? throw new ArgumentNullException(nameof(orFlowNodes));
//                this.inferredFlowNodes = inferredFlowNodes ?? throw new ArgumentNullException(nameof(inferredFlowNodes));
//                this.memberLookup = memberLookup ?? throw new ArgumentNullException(nameof(memberLookup));
//            }

//            //public TypeSolution(
//            //    IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map,
//            //    IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
//            //   IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint)
//            //{
//            //    this.map = map ?? throw new ArgumentNullException(nameof(map));
//            //    this.orTypeElememts = orTypeElememts ?? throw new ArgumentNullException(nameof(orTypeElememts));
//            //    this.methodIn = methodIn ?? throw new ArgumentNullException(nameof(methodIn));
//            //    this.methodOut = methodOut ?? throw new ArgumentNullException(nameof(methodOut));
//            //    this.moduleEntryPoint = moduleEntryPoint ?? throw new ArgumentNullException(nameof(moduleEntryPoint));
//            //}


//            private readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> cacheType = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();
//            public IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType)
//            {
//                if (!cacheType.ContainsKey(explicitType))
//                {
//                    var box = new Box<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>();
//                    cacheType[explicitType] = box;
//                    box.Fill(explicitType.Converter.Convert(this, explicitType));
//                }
//                return cacheType[explicitType];
//            }

//            // just takes the type of the member
//            //public IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member)
//            //{
//            //    var (key,owner)= memberLookup[member];

//            //    return GetMember(owner, key, x => member.Converter.Convert(x, GetFlowNode2(member)));
//            //}


//            //private readonly Dictionary<(IOrType<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope> Owner,IKey Key), IBox<WeakMemberDefinition>> cacheMember = new Dictionary<(IOrType<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope> Owner, IKey Key), IBox<WeakMemberDefinition>>();

//            //public IBox<WeakMemberDefinition> GetMember(Tpn.IVirtualFlowNode owner , IKey key,  Func<TypeSolution, WeakMemberDefinition> convert)
//            //{
//            //    return GetMember(OrType.Make<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope>(owner), key, convert);
//            //}
//            private readonly Dictionary<(Tpn.ITypeProblemNode Owner, IKey Key), IBox<WeakMemberDefinition>> cacheMember2 = new Dictionary<(Tpn.ITypeProblemNode Owner, IKey Key), IBox<WeakMemberDefinition>>();
            
//            // GetMember is more painful to use than the other methods in this class
//            // the problem is members can be created in the type probelm
//            // and hopeful members need to be tracked back to the real member they long for 
//            // the safe way to do is this just to provide a context and key and look up the member 
//            public IBox<WeakMemberDefinition> GetMember((Tpn.ITypeProblemNode Owner, IKey Key) key, Func<WeakMemberDefinition> convert) {

//                if (!cacheMember2.ContainsKey(key))
//                {
//                    var box = new Box<WeakMemberDefinition>();
//                    cacheMember2[key] = box;
//                    box.Fill(convert());
//                }
//                return cacheMember2[key];
//            }

//            public IOrType<IIsPossibly<IBox<WeakMemberDefinition>>,IError> StaticScope(Tpn.IStaticScope staticScope, IKey key, WeakMemberDefinitionConverter converter) 
//            {
//                { 
//                    if (staticScope.SafeIs(out TypeProblem2.Type type)) {
//                        return GetFlowNode(type).VirtualMembers().TransformInner(items =>
//                        {
//                            foreach (var item in items)
//                            {
//                                if (item.Key.Equals(key)) {
//                                    return Possibly.Is(GetMember((Owner: type, Key: key), () => converter.Convert(this, item.Value)));
//                                }
//                            }
//                            return Possibly.IsNot<IBox<WeakMemberDefinition>>();
//                        });
//                    }
//                }
//                {
//                    if (staticScope.SafeIs(out TypeProblem2.MethodType methodType))
//                    {
//                        return GetFlowNode(methodType).VirtualMembers().TransformInner(items =>
//                        {
//                            foreach (var item in items)
//                            {
//                                if (item.Key.Equals(key))
//                                {
//                                    return Possibly.Is(GetMember((Owner: methodType, Key: key), () => converter.Convert(this, item.Value)));
//                                }
//                            }
//                            return Possibly.IsNot<IBox<WeakMemberDefinition>>();
//                        });
//                    }
//                }
//                {
//                    if (staticScope.SafeIs(out TypeProblem2.Scope scope))
//                    {
//                        if (scope.PrivateMembers.TryGetValue(key, out var member)) {
//                            return OrType.Make<IIsPossibly<IBox<WeakMemberDefinition>>, IError>(Possibly.Is(GetMember((Owner: scope, Key: key), () => converter.Convert(this, GetFlowNode2(member)))));
//                        }
//                        return OrType.Make<IIsPossibly<IBox<WeakMemberDefinition>>, IError>(Possibly.IsNot<IBox<WeakMemberDefinition>>());
//                    }
//                }
//                {
//                    if (staticScope.SafeIs(out TypeProblem2.Object @object))
//                    {
//                        return GetFlowNode(@object).VirtualMembers().TransformInner(items =>
//                        {
//                            foreach (var item in items)
//                            {
//                                if (item.Key.Equals(key))
//                                {
//                                    return Possibly.Is(GetMember((Owner: @object, Key: key), () => converter.Convert(this, item.Value)));
//                                }
//                            }
//                            return Possibly.IsNot<IBox<WeakMemberDefinition>>();
//                        });
//                    }
//                }
//                {
//                    if (staticScope.SafeIs(out TypeProblem2.Method method))
//                    {
//                        if (method.PrivateMembers.TryGetValue(key, out var member))
//                        {
//                            return OrType.Make<IIsPossibly<IBox<WeakMemberDefinition>>, IError>(Possibly.Is(GetMember((Owner: method, Key: key), () => converter.Convert(this, GetFlowNode2(member)))));
//                        }
//                        return OrType.Make<IIsPossibly<IBox<WeakMemberDefinition>>, IError>(Possibly.IsNot<IBox<WeakMemberDefinition>>());
//                    }
//                }
//                throw new Exception("should have been one of those");
//            }

//            //public IBox<WeakMemberDefinition> GetMember(IOrType<IVirtualFlowNode, TypeProblem2.Method, TypeProblem2.Scope> owner, IKey key, Func<TypeSolution, WeakMemberDefinition> convert)
//            //{
//            //    var keyTuple = (owner, key);
//            //    if (!cacheMember.ContainsKey(keyTuple))
//            //    {
//            //        var box = new Box<WeakMemberDefinition>();
//            //        cacheMember[keyTuple] = box;
//            //        box.Fill(convert(this));
//            //    }
//            //    return cacheMember[keyTuple];
//            //}

//            private readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>> cacheMethod = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>>();
//            public IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition,WeakEntryPointDefinition>> GetMethod(TypeProblem2.Method method)
//            {
//                if (!cacheMethod.ContainsKey(method))
//                {
//                    var box = new Box<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>();
//                    cacheMethod[method] = box;
//                    box.Fill(method.Converter.Convert(this, method));
//                }
//                return cacheMethod[method];
//            }

//            private readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>> cacheObject = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>>();
//            public IBox<IOrType<WeakObjectDefinition, WeakRootScope>> GetObject(TypeProblem2.Object @object)
//            {
//                if (!cacheObject.ContainsKey(@object))
//                {
//                    var box = new Box<IOrType<WeakObjectDefinition, WeakRootScope>>();
//                    cacheObject[@object] = box;
//                    box.Fill(@object.Converter.Convert(this, @object));
//                }
//                return cacheObject[@object];
//            }

//            private readonly Dictionary<TypeProblem2.OrType, IBox<WeakTypeOrOperation>> cacheOrType = new Dictionary<TypeProblem2.OrType, IBox<WeakTypeOrOperation>>();
//            public IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType)
//            {
//                if (!cacheOrType.ContainsKey(orType))
//                {
//                    var box = new Box<WeakTypeOrOperation>();
//                    cacheOrType[orType] = box;
//                    box.Fill(orType.Converter.Convert(this, orType));
//                }
//                return cacheOrType[orType];
//            }

//            private readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> cacheScope = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
//            public IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope)
//            {
//                if (!cacheScope.ContainsKey(scope))
//                {
//                    var box = new Box<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>();
//                    cacheScope[scope] = box;
//                    box.Fill(scope.Converter.Convert(this, scope));
//                }
//                return cacheScope[scope];
//            }

//            private readonly Dictionary<TypeProblem2.TypeReference, IBox<IFrontendType>> cacheTypeReference = new Dictionary<TypeProblem2.TypeReference, IBox<IFrontendType>>();
//            public IBox<IFrontendType> GetTypeReference(TypeProblem2.TypeReference typeReference)
//            {
//                if (!cacheTypeReference.ContainsKey(typeReference))
//                {
//                    var box = new Box<IFrontendType>();
//                    cacheTypeReference[typeReference] = box;
//                    box.Fill(typeReference.Converter.Convert(this, typeReference));
//                }
//                return cacheTypeReference[typeReference];
//            }

//            private readonly Dictionary<TypeProblem2.Value, IBox<PlaceholderValue>> cacheValue = new Dictionary<TypeProblem2.Value, IBox<PlaceholderValue>>();

//            public IBox<PlaceholderValue> GetValue(TypeProblem2.Value value)
//            {
//                if (!cacheValue.ContainsKey(value))
//                {
//                    var box = new Box<PlaceholderValue>();
//                    cacheValue[value] = box;
//                    box.Fill(value.Converter.Convert(this, value));
//                }
//                return cacheValue[value];
//            }

//            private readonly Dictionary<TypeProblem2.MethodType, IBox<MethodType>> cacheMethodType = new Dictionary<TypeProblem2.MethodType, IBox<MethodType>>();

//            public IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType)
//            {
//                if (!cacheMethodType.ContainsKey(methodType))
//                {
//                    var box = new Box<MethodType>();
//                    cacheMethodType[methodType] = box;
//                    box.Fill(methodType.Converter.Convert(this, methodType));
//                }
//                return cacheMethodType[methodType];
//            }

//            private readonly Dictionary<Tpn.VirtualNode, IBox<IOrType<IFrontendType, IError>>> cacheInferredType = new Dictionary<Tpn.VirtualNode, IBox<IOrType<IFrontendType, IError>>>();


//            public IBox<IOrType<IFrontendType, IError>> GetInferredType(Tpn.VirtualNode inferredType)
//            {
//                if (!cacheInferredType.ContainsKey(inferredType))
//                {
//                    var box = new Box<IOrType<IFrontendType, IError>>();
//                    cacheInferredType[inferredType] = box;
//                    box.Fill(new InferredTypeConverter2().Convert(this, inferredType));
//                }
//                return cacheInferredType[inferredType];
//            }

//            // naming!
//            private readonly Dictionary<Tpn.CombinedTypesAnd, IBox<IOrType<IFrontendType,IError>>> cacheInferredType2 = new Dictionary<Tpn.CombinedTypesAnd, IBox<IOrType<IFrontendType, IError>>>();


//            public IBox<IOrType<IFrontendType, IError>> GetInferredType(Tpn.CombinedTypesAnd inferredType)
//            {
//                if (!cacheInferredType2.ContainsKey(inferredType))
//                {
//                    var box = new Box<IOrType<IFrontendType, IError>>();
//                    cacheInferredType2[inferredType] = box;
//                    box.Fill(new InferredTypeConverter().Convert(this, inferredType));
//                }
//                return cacheInferredType2[inferredType];
//            }

//            public readonly struct FlowNodeMember
//            {
//                public FlowNodeMember(IKey key, IOrType< IVirtualFlowNode,IError> flowNode, IVirtualFlowNode of)
//                {
//                    Key = key ?? throw new ArgumentNullException(nameof(key));
//                    FlowNode = flowNode ?? throw new ArgumentNullException(nameof(flowNode));
//                    Of = of ?? throw new ArgumentNullException(nameof(of));
//                }

//                public IKey Key { get; }
//                public IOrType<IVirtualFlowNode, IError> FlowNode { get; }
//                // this is used for equality
//                // a member is by what it is on + it's key
//                public IVirtualFlowNode Of { get; }
//            }

//            public IEnumerable<KeyValuePair<IKey ,TypeProblem2.Member>> GetPrivateMembers(IHavePrivateMembers privateMembers)
//            {
//                return privateMembers.PrivateMembers;
//            }

//            public IOrType< IReadOnlyList<FlowNodeMember>,IError> GetPublicMembers(IVirtualFlowNode from)
//            {
//                return from.VirtualMembers().SwitchReturns(
//                    y=> OrType.Make<IReadOnlyList<FlowNodeMember>, IError> (y.Select(x=>  new FlowNodeMember(x.Key, x.Value, from)).ToList()),
//                    y=> OrType.Make<IReadOnlyList<FlowNodeMember>, IError>(y));
//            }

//            public IFlowNode<TypeProblem2.MethodType> GetFlowNode(TypeProblem2.MethodType type)
//            {
//                return methodFlowNodes[type];
//            }
//            public IFlowNode<TypeProblem2.Type> GetFlowNode(TypeProblem2.Type type)
//            {
//                return typeFlowNodes[type];
//            }

//            public IFlowNode<TypeProblem2.Object> GetFlowNode(TypeProblem2.Object type)
//            {
//                return objectFlowNodes[type];
//            }

//            public IFlowNode<TypeProblem2.OrType> GetFlowNode(TypeProblem2.OrType type)
//            {
//                return orFlowNodes[type];
//            }

//            public IFlowNode<TypeProblem2.InferredType> GetFlowNode(TypeProblem2.InferredType type)
//            {
//                return inferredFlowNodes[type];
//            }

//            public IOrType<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError> GetFlowNode(ILookUpType from)
//            {
//                return map[from].SwitchReturns(
//                    x=> OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError> (GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError>(x)
//                    );
//            }

//            public IOrType<IFlowNode,IError> GetFlowNode2(ILookUpType from)
//            {
//                return map[from].SwitchReturns(
//                    x => OrType.Make< IFlowNode, IError > (GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode, IError>(GetFlowNode(x)),
//                    x => OrType.Make<IFlowNode, IError>(x));
//            }

//            //public IOrType<IFlowNode<TypeProblem2.MethodType>, IFlowNode<TypeProblem2.Type>, IFlowNode<TypeProblem2.Object>, IFlowNode<TypeProblem2.OrType>, IFlowNode<TypeProblem2.InferredType>, IError> GetType(ILookUpType from)
//            //{
//            //    return map[from];
//            //}

//            public (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from)
//            {
//                return orTypeElememts[from];
//            }

//            //public bool TryGetResultMember(IVirtualFlowNode from, [MaybeNullWhen(false)] out IVirtualFlowNode? transientMember)
//            //{
//            //    if (from.VirtualOutput().Is(out var value)) {
//            //        transientMember = value;
//            //        return true;
//            //    }
//            //    transientMember = default;
//            //    return false;
//            //}

//            public bool TryGetResultMember(CombinedTypesAnd from, [MaybeNullWhen(false)] out IOrType<IVirtualFlowNode,IError>? transientMember)
//            {
//                if (from.VirtualOutput().Is(out var value))
//                {
//                    transientMember = value;
//                    return true;
//                }
//                transientMember = default;
//                return false;
//            }

//            //public bool TryGetInputMember(IVirtualFlowNode from, [MaybeNullWhen(false)] out IVirtualFlowNode? member)
//            //{
//            //    if (from.VirtualInput().Is(out var value))
//            //    {
//            //        member = value;
//            //        return true;
//            //    }
//            //    member = default;
//            //    return false;
//            //}

//            public bool TryGetInputMember(CombinedTypesAnd from, [MaybeNullWhen(false)] out IOrType<IVirtualFlowNode, IError>? member)
//            {
//                if (from.VirtualInput().Is(out var value))
//                {
//                    member = value;
//                    return true;
//                }
//                member = default;
//                return false;
//            }





//            public IBox<IOrType<IFrontendType,IError>> GetType(IOrType<IVirtualFlowNode, IError> or)
//            {
//                return or.SwitchReturns<IBox<IOrType<IFrontendType, IError>>>(node =>
//                {
//                    // the list of types here comes from the big Or in typeSolution 
//                    // + Uhh see {A2333086-1634-4C8D-9FB1-453BE0BC2F03}
//                    if (node is IFlowNode<TypeProblem2.MethodType> typeFlowMethodType)
//                    {
//                        return new UnWrappingMethodBox(GetMethodType(typeFlowMethodType.Source.GetOrThrow()));
//                    }
//                    if (node is IFlowNode<TypeProblem2.Type> typeFlowNode)
//                    {
//                        return new UnWrappingTypeBox(GetExplicitType(typeFlowNode.Source.GetOrThrow()));
//                    }
//                    if (node is IFlowNode<TypeProblem2.Object> typeFlowObject)
//                    {
//                        return 
//                            new UnWrappingObjectBox(
//                                GetObject(typeFlowObject.Source.GetOrThrow()));
//                    }
//                    if (node is IFlowNode<TypeProblem2.OrType> typeFlowOr)
//                    {
//                        return 
//                            new UnWrappingOrBox(GetOrType(typeFlowOr.Source.GetOrThrow()));
//                    }

//                    // at this point we are Concrete<Inferred>
//                    // or VirtualNode

//                    return node.ToRep().SwitchReturns(
//                        x => GetInferredType(new VirtualNode(x, node.SourcePath())), 
//                        x => new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(x)));

//                    //if (node is IFlowNode<Uhh> typeFlowUhh)
//                    //{
//                    //    return OrType.Make<IBox<IFrontendType>, IError>(GetInferredType(typeFlowUhh, new InferredTypeConverter()));
//                    //}
//                    throw new NotImplementedException("I thought i had to be one of those");
//                }, x => new Box<IOrType<IFrontendType, IError>>(OrType.Make<IFrontendType, IError>(x)));
//            }


//            //public IVirtualFlowNode GetResultMember(IFlowNode<TypeProblem2.MethodType> from)
//            //{
//            //    if (TryGetResultMember(from, out var res)) {
//            //        return res!;
//            //    }
//            //    throw new Exception("that should not happen for a method");
//            //}

//            //public IVirtualFlowNode GetInputMember(IFlowNode<TypeProblem2.MethodType> from)
//            //{
//            //    if (TryGetInputMember(from, out var res))
//            //    {
//            //        return  res!;
//            //    }
//            //    throw new Exception("that should not happen for a method");
//            //}

//            //public IIsPossibly<TypeProblem2.Scope> GetEntryPoint(IStaticScope from)
//            //{
//            //    if (moduleEntryPoint.TryGetValue(from, out var res))
//            //    {
//            //        return Possibly.Is(res);
//            //    }
//            //    return Possibly.IsNot<TypeProblem2.Scope>();
//            //}


//            public IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
//            {
//                var res = new List<IOrType<NameKey, ImplicitKey>>();
//                foreach (var value in type.GenericOverlays.Values.Select(x=>x.Possibly2()))
//                {
//                    value.If(definite => {
//                        if (definite.IsPlaceHolder) {
//                            definite.Key.If(x =>
//                            {
//                                res.Add(x);
//                                return 1; // todo I need a version of this api that takes an action
//                            });
//                        }
//                        return 1; // todo I need a version of this api that takes an action
//                    });
//                }

//                if (res.Any()) {
//                    return Possibly.Is(res.ToArray());
//                }

//                return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
//            }
//        }
//    }

    
//}
