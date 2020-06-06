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
            private readonly IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut;
            private readonly IReadOnlyDictionary<IStaticScope, TypeProblem2.Scope> moduleEntryPoint;

            public TypeSolution(
                IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map,
                IReadOnlyDictionary<IHavePublicMembers, IReadOnlyList<TypeProblem2.Member>> publicMembers,
                IReadOnlyDictionary<IHavePrivateMembers, IReadOnlyList<TypeProblem2.Member>> privateMembers,
                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
                IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn,
                IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut,
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

            private readonly Dictionary<TypeProblem2.InferredType, IBox<IFrontendType>> cacheInferredType = new Dictionary<TypeProblem2.InferredType, IBox<IFrontendType>>();

            public IBox<IFrontendType> GetInferredType(TypeProblem2.InferredType inferredType, IConvertTo<TypeProblem2.InferredType, IFrontendType> converter)
            {
                if (!cacheInferredType.ContainsKey(inferredType))
                {
                    var box = new Box<IFrontendType>();
                    cacheInferredType[inferredType] = box;
                    box.Fill(converter.Convert(this, inferredType));
                }
                return cacheInferredType[inferredType];
            }

            public IReadOnlyList<TypeProblem2.Member> GetMembers(IOrType<IHavePrivateMembers, IHavePublicMembers> from)
            {
                return from.SwitchReturns(x =>
                {
                    if (!privateMembers.ContainsKey(x))
                    {
                        return new List<TypeProblem2.Member>();
                    }
                    return privateMembers[x];
                },
                x =>
                {
                    if (!publicMembers.ContainsKey(x))
                    {
                        return new List<TypeProblem2.Member>();
                    }
                    return publicMembers[x];
                });

            }

            public IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> GetType(ILookUpType from)
            {
                return map[from];
            }

            public (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from)
            {
                return orTypeElememts[from];
            }

            public bool TryGetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, [MaybeNullWhen(false)] out TypeProblem2.TransientMember? transientMember)
            {
                return methodOut.TryGetValue(from, out transientMember);
            }

            public bool TryGetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, [MaybeNullWhen(false)] out TypeProblem2.Member? member)
            {
                return methodIn.TryGetValue(from, out member);
            }


            public TypeProblem2.TransientMember GetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from)
            {
                return methodOut[from];
            }

            public TypeProblem2.Member GetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from)
            {
                return methodIn[from];
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
