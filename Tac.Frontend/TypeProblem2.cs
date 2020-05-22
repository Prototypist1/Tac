﻿using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    // todo there are some pretty stupid helpers here
    // I think I like isPossibly more than IKey? atm it is stronger
    // if there is anywhere I need comments it is here

    internal class Unset { }

    // this static class is here just to make us all think in terms of these bros
    internal class Tpn
    {

        internal class TypeAndConverter
        {
            public readonly IOrType<NameKey, ImplicitKey> key;
            public readonly IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter;

            public TypeAndConverter(IOrType<NameKey, ImplicitKey> key, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            }

            public override string? ToString()
            {
                return key.ToString();
            }
        }

        // is this interface doing me any good?
        internal interface ISetUpTypeProblem
        {
            // a =: x
            TypeProblem2.Type NumberType { get; }
            TypeProblem2.Type StringType { get; }
            TypeProblem2.Type BooleanType { get; }
            TypeProblem2.Type EmptyType { get; }
            void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
            TypeProblem2.Value CreateValue(IScope scope, IKey typeKey, IConvertTo<TypeProblem2.Value, PlaceholderValue> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IOrType<IKey, IError> typeKey, bool isExposed, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, bool isExposed, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> type,  IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, bool isExposed, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeProblem2.TypeReference, IOrType<IFrontendType, IError>> converter);
            TypeProblem2.Scope CreateScope(IScope parent, IConvertTo<TypeProblem2.Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter);
            TypeProblem2.Type CreateType(IScope parent, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateType(IScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateGenericType(IScope parent, IOrType<NameKey, ImplicitKey> key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<TypeProblem2.Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Object CreateObjectOrModule(IScope parent, IKey key, IConvertTo<TypeProblem2.Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter);
            TypeProblem2.Method CreateMethod(IScope parent, string inputName, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.Method CreateMethod(IScope parent, IOrType<TypeProblem2.TypeReference, IError> inputType, IOrType<TypeProblem2.TypeReference, IError> outputType, string inputName, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.TransientMember GetReturns(IValue s);
            TypeProblem2.TransientMember GetReturns(IScope s);
            TypeProblem2.Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.OrType CreateOrType(IScope s, IKey key, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode1, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode2, IConvertTo<TypeProblem2.OrType, WeakTypeOrOperation> converter);
            IIsPossibly<IKey> GetKey(TypeProblem2.TypeReference type);
            TypeProblem2.Member GetInput(IValue method);
            TypeProblem2.Member GetInput(TypeProblem2.Method method);

            TypeProblem2.MethodType GetMethod(IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> input, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> output);
            void IsNumber(IScope parent, ILookUpType target);
            void IsString(IScope parent, ILookUpType target);
            void IsEmpty(IScope parent, ILookUpType target);
            void IsBool(IScope parent, ILookUpType target);
            void IsBlock(IScope parent, ILookUpType target);

            void HasEntryPoint(IScope parent, TypeProblem2.Scope entry);
            TypeProblem2.Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<TypeProblem2.Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
        }

        internal interface ITypeSolution
        {

            IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type); 
            IBox<PlaceholderValue> GetValue(TypeProblem2.Value value);
            IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member);
            IBox<IOrType<IFrontendType, IError>> GetTypeReference(TypeProblem2.TypeReference typeReference);
            IBox<IFrontendType> GetInferredType(TypeProblem2.InferredType inferredType, IConvertTo<TypeProblem2.InferredType, IFrontendType> converter);
            IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope);
            // when I ungeneric this it should probably have the box inside the or..
            IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType);
            IBox<IOrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object);
            IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType);
            IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType);
            IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method);
            IReadOnlyList<TypeProblem2.Member> GetMembers(IHaveMembers from);
            IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError> GetType(ILookUpType from);
            (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from);
            bool TryGetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.TransientMember? transientMember);
            bool TryGetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.Member? member);

            TypeProblem2.TransientMember GetResultMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
            TypeProblem2.Member GetInputMember(IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
            IIsPossibly<TypeProblem2.Scope> GetEntryPoint(TypeProblem2.Object from);
        }

        internal interface IConvertTo<in TConvertFrom, out TConvertsTo>
        {
            TConvertsTo Convert(ITypeSolution typeSolution, TConvertFrom from);
        }

        // 🤫 the power was in you all along
        internal class TypeSolution : ITypeSolution
        {
            private readonly IReadOnlyDictionary<IHaveMembers, IReadOnlyList<TypeProblem2.Member>> members;
            private readonly IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map;
            private readonly IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts;
            private readonly IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn;
            private readonly IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut;
            private readonly IReadOnlyDictionary<IScope, TypeProblem2.Scope> moduleEntryPoint;

            public TypeSolution(
                IReadOnlyDictionary<ILookUpType, IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> map,
                IReadOnlyDictionary<IHaveMembers, IReadOnlyList<TypeProblem2.Member>> members,
                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
                IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn,
                IReadOnlyDictionary<IOrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut,
                IReadOnlyDictionary<IScope, TypeProblem2.Scope> moduleEntryPoint)
            {
                this.map = map ?? throw new ArgumentNullException(nameof(map));
                this.members = members ?? throw new ArgumentNullException(nameof(members));
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

            public IReadOnlyList<TypeProblem2.Member> GetMembers(IHaveMembers from)
            {
                if (!members.ContainsKey(from))
                {
                    return new List<TypeProblem2.Member>();
                }
                return members[from];
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

            public IIsPossibly<TypeProblem2.Scope> GetEntryPoint(TypeProblem2.Object from)
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

        // the simple model of or-types:
        // they don't have any members
        // they don't have any types

        // they might be able to flow there or-ness up stream 
        // but that is more complex than I am interested in right now

        // maybe they are a primitive generic - no 
        // they are a concept created by the type system

        // to the type system they almost just look like an empty user defined type
        // 


        internal interface ITypeProblemNode
        {
            ISetUpTypeProblem Problem { get; }
        }

        //internal class PossiblyExposedMember {
        //    public readonly bool isExposed;
        //    public readonly TypeProblem2.Member member;

        //    public PossiblyExposedMember(bool isExposed, TypeProblem2.Member member)
        //    {
        //        this.isExposed = isExposed;
        //        this.member = member ?? throw new ArgumentNullException(nameof(member));
        //    }
        //}

        internal interface IHaveMembers : ITypeProblemNode
        {
            public Dictionary<IKey, TypeProblem2.Member> Members { get; }
        }

        internal interface ILookUpType : ITypeProblemNode
        {
            public IOrType<IKey, IError, Unset> TypeKey { get; set; }
            public IIsPossibly<IScope> Context { get; set; }
            public IIsPossibly<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> LooksUp { get; set; }
        }

        internal interface ICanAssignFromMe : ITypeProblemNode, ILookUpType { }
        internal interface ICanBeAssignedTo : ITypeProblemNode, ILookUpType { }
        internal interface IValue : ITypeProblemNode, ILookUpType, ICanAssignFromMe
        {
            public Dictionary<IKey, TypeProblem2.Member> HopefulMembers { get; }
            public IIsPossibly<TypeProblem2.InferredType> HopefulMethod { get; set; }
        }
        //public interface Member :  IValue, ILookUpType, ICanBeAssignedTo {bool IsReadonly { get; }}
        internal interface IExplicitType : IHaveMembers, IScope { }
        internal interface IScope : IHaveMembers
        {
            IIsPossibly<IScope> Parent { get; set; }

            public List<TypeProblem2.Scope> EntryPoints { get; }
            public List<TypeProblem2.Value> Values { get; }
            public List<TypeProblem2.TransientMember> TransientMembers { get; }
            public Dictionary<IKey, TypeProblem2.Method> Methods { get; }
            public List<TypeProblem2.TypeReference> Refs { get; }
            public Dictionary<IKey, TypeProblem2.OrType> OrTypes { get; }
            public Dictionary<IKey, TypeProblem2.Type> Types { get; }
            public Dictionary<IKey, TypeProblem2.MethodType> MethodTypes { get; }
            public Dictionary<IKey, TypeProblem2.Object> Objects { get; }
            public Dictionary<IKey, TypeProblem2.Member> PossibleMembers { get; }
        }
        //internal interface IMethod : IHaveMembers, IScope { }
        internal interface IHaveInputAndOutput : ITypeProblemNode { }
        //internal interface IHavePlaceholders: ITypeProblemNode { }

        // TODO is transient member really not a member?
        // can't it's transientness be captured but waht dict it is in??
        internal interface IMember : IValue, ILookUpType, ICanBeAssignedTo { }

        internal class TypeProblem2 : ISetUpTypeProblem
        {
            public abstract class TypeProblemNode : ITypeProblemNode
            {
                public readonly string debugName;

                public TypeProblemNode(TypeProblem2 problem, string debugName)
                {
                    Problem = problem ?? throw new ArgumentNullException(nameof(problem));
                    this.debugName = debugName;
                    problem.Register(this);
                }

                public ISetUpTypeProblem Problem { get; }
            }

            public abstract class TypeProblemNode<Tin, Tout> : TypeProblemNode//, IConvertable<T>
            {

                public TypeProblemNode(TypeProblem2 problem, string debugName, IConvertTo<Tin, Tout> converter) : base(problem, debugName)
                {
                    Converter = converter;
                }

                internal IConvertTo<Tin, Tout> Converter { get; }
            }
            public class TypeReference : TypeProblemNode<TypeReference, IOrType<IFrontendType, IError>>, ILookUpType
            {
                public TypeReference(TypeProblem2 problem, string debugName, IConvertTo<TypeReference, IOrType<IFrontendType, IError>> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IScope> Context { get; set; } = Possibly.IsNot<IScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
            }

            public class Value : TypeProblemNode<Value, PlaceholderValue>, IValue
            {
                public Value(TypeProblem2 problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IScope> Context { get; set; } = Possibly.IsNot<IScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }
            public class Member : TypeProblemNode<Member, WeakMemberDefinition>, IMember
            {
                public Member(TypeProblem2 problem, bool isExposed, string debugName, IConvertTo<Member, WeakMemberDefinition> converter) : base(problem, debugName, converter)
                {
                    this.IsExposed = isExposed;
                }

                public bool IsExposed { get; set; }
                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IScope> Context { get; set; } = Possibly.IsNot<IScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }

            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }

                public IOrType<IKey, IError, Unset> TypeKey { get; set; } = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(new Unset());
                public IIsPossibly<IScope> Context { get; set; } = Possibly.IsNot<IScope>();
                public IIsPossibly<IOrType<MethodType, Type, Object, OrType, InferredType, IError>> LooksUp { get; set; } = Possibly.IsNot<IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }

            public class Type : TypeProblemNode<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType
            {
                public Type(
                    TypeProblem2 problem, 
                    string debugName, 
                    IIsPossibly<IOrType<NameKey, ImplicitKey>> key,
                    IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter,
                    bool isPlaceHolder
                    ) : base(problem, debugName, converter)
                {
                    Key = key;
                    IsPlaceHolder = isPlaceHolder;
                }

                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();


                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                public IIsPossibly<IOrType<NameKey, ImplicitKey>> Key { get; }

                public bool IsPlaceHolder { get; }
            }

            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class MethodType : TypeProblemNode<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IHaveMembers, IScope
            {
                public MethodType(TypeProblem2 problem, string debugName, IConvertTo<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();

                public Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> GenericOverlays { get; } = new Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
            }

            public class InferredType : TypeProblemNode, IHaveInputAndOutput, IHaveMembers, IScope
            {
                public InferredType(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }
                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();
            }

            public class OrType : TypeProblemNode<OrType, WeakTypeOrOperation>, IHaveMembers
            {
                public OrType(TypeProblem2 problem, string debugName, IConvertTo<OrType, WeakTypeOrOperation> converter) : base(problem, debugName, converter)
                {
                }
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<TypeReference> Left { get; set; } = Possibly.IsNot<TypeReference>();
                public IIsPossibly<TypeReference> Right { get; set; } = Possibly.IsNot<TypeReference>();
            }
            public class Scope : TypeProblemNode<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>, IScope
            {
                public Scope(TypeProblem2 problem, string debugName, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
            }
            public class Object : TypeProblemNode<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();
            }
            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class Method : TypeProblemNode<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>>, IHaveMembers, IScope, IHaveInputAndOutput
            {
                public Method(TypeProblem2 problem, string debugName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IIsPossibly<IScope> Parent { get; set; } = Possibly.IsNot<IScope>();
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();
                public List<Scope> EntryPoints { get; } = new List<Scope>();
                public List<Value> Values { get; } = new List<Value>();
                public List<TransientMember> TransientMembers { get; } = new List<TransientMember>();
                public Dictionary<IKey, Method> Methods { get; } = new Dictionary<IKey, Method>();
                public List<TypeReference> Refs { get; } = new List<TypeReference>();
                public Dictionary<IKey, OrType> OrTypes { get; } = new Dictionary<IKey, OrType>();
                public Dictionary<IKey, Type> Types { get; } = new Dictionary<IKey, Type>();
                public Dictionary<IKey, MethodType> MethodTypes { get; } = new Dictionary<IKey, MethodType>();
                public Dictionary<IKey, Object> Objects { get; } = new Dictionary<IKey, Object>();
                public Dictionary<IKey, Member> PossibleMembers { get; } = new Dictionary<IKey, Member>();

                public IIsPossibly<Member> Input { get; set; } = Possibly.IsNot<Member>();
                public IIsPossibly<TransientMember> Returns { get; set; } = Possibly.IsNot<TransientMember>();

            }


            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }
            public Type NumberType { get; }
            public Type StringType { get; }
            public Type BooleanType { get; }
            public Type EmptyType { get; }


            // these are pretty much the same
            private List<(ILookUpType, ILookUpType)> assignments = new List<(ILookUpType, ILookUpType)>();
            

            #region Building APIs

            public void IsChildOf(IScope parent, IScope kid)
            {
                kid.Parent = Possibly.Is(parent);
            }
            public static void HasValue(IScope parent, Value value)
            {
                parent.Values.Add(value);
            }
            public static void HasReference(IScope parent, TypeReference reference)
            {
                parent.Refs.Add(reference);
            }


            public void HasEntryPoint(IScope parent, Scope entry)
            {
                parent.EntryPoints.Add(entry);
            }

            public static void HasType(IScope parent, IKey key, Type type)
            {
                parent.Types.Add(key, type);
            }
            public void HasMethodType(IScope parent, IKey key, MethodType type)
            {
                parent.MethodTypes.Add(key, type);
            }
            // why do objects have keys?
            // that is wierd
            public static void HasObject(IScope parent, IKey key, Object @object)
            {
                parent.Objects.Add(key, @object);
            }

            public void HasPlaceholderType(IOrType<MethodType, Type> parent, IKey key, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type)
            {
                parent.Switch(x => x.GenericOverlays.Add(key, type), x => x.GenericOverlays.Add(key, type));
            }
            public static void HasMember(IHaveMembers parent, IKey key, Member member)
            {
                parent.Members.Add(key, member);
            }
            public static void HasMethod(IScope parent, IKey key, Method method)
            {
                parent.Methods.Add(key, method);
            }


            public static void HasTransientMember(IScope parent, TransientMember member)
            {
                parent.TransientMembers.Add(member);
            }
            public static Member HasMembersPossiblyOnParent(IScope parent, IKey key, Member member)
            {

                parent.PossibleMembers.TryAdd(key, member);
                return parent.PossibleMembers[key];
            }
            public static Member HasHopefulMember(IValue parent, IKey key, Member member)
            {
                parent.HopefulMembers.TryAdd(key, member);
                return parent.HopefulMembers[key];
            }

            private T Register<T>(T typeProblemNode)
                where T : ITypeProblemNode
            {
                typeProblemNodes.Add(typeProblemNode);
                return typeProblemNode;
            }

            public void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo)
            {
                AssertIs(assignedFrom, assignedTo);
            }

            public void AssertIs(ILookUpType assignedFrom, ILookUpType assignedTo)
            {
                assignments.Add((assignedFrom, assignedTo));
            }

            public Value CreateValue(IScope scope, IKey typeKey, IConvertTo<Value, PlaceholderValue> converter)
            {
                var res = new Value(this, typeKey.ToString()!, converter);
                HasValue(scope, res);
                res.Context = Possibly.Is(scope);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IOrType<IKey, IError> typeKey, bool exposed, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this,exposed, key.ToString()!,  converter);
                HasMember(scope, key, res);
                res.Context = Possibly.Is(scope);
                res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, bool exposed, IConvertTo<Member,  WeakMemberDefinition> converter)
            {
                var res = new Member(this,exposed, key.ToString()!,  converter);
                HasMember(scope, key, res);
                res.Context = Possibly.Is(scope);
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, bool exposed, IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, exposed, key.ToString()!, converter);
                HasMember(scope, key, res);
                res.LooksUp = Possibly.Is(type);
                return res;
            }

            public Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                if (scope.PossibleMembers.TryGetValue(key, out var res1))
                {
                    return res1;
                }
                var res = new Member(this, false, "possibly on parent -" + key.ToString(), converter);
                res = HasMembersPossiblyOnParent(scope, key, res);
                res.Context = Possibly.Is(scope);
                return res;
            }

            public TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeReference, IOrType<IFrontendType, IError>> converter)
            {
                var res = new TypeReference(this, typeKey.ToString()!, converter);
                HasReference(context, res);
                res.Context = Possibly.Is(context);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            public Scope CreateScope(IScope parent, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter)
            {
                var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).debugName}", converter);
                IsChildOf(parent, res);
                return res;
            }

            public Type CreateType(IScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(this, key.ToString()!, Possibly.Is(key), converter, false);
                IsChildOf(parent, res);
                HasType(parent, key.SwitchReturns<IKey>(x => x, x => x), res);
                return res;
            }

            public Type CreateType(IScope parent, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var key = new ImplicitKey(Guid.NewGuid());
                var res = new Type(this, key.ToString()!,Possibly.IsNot<IOrType<NameKey, ImplicitKey>>(),  converter, false);
                IsChildOf(parent, res);
                // migiht need this, let's try without first
                //HasType(parent, key, res);
                return res;
            }


            public Type CreateGenericType(IScope parent, IOrType<NameKey, ImplicitKey> key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(
                    this, 
                    $"generic-{key}-{placeholders.Aggregate("", (x, y) => x + "-" + y)}", 
                    Possibly.Is(key),
                    converter, 
                    false);
                IsChildOf(parent, res);
                HasType(parent, key.SwitchReturns<IKey>(x=>x, x=>x), res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(
                        this, 
                        $"generic-parameter-{placeholder.key}", 
                        Possibly.Is( placeholder.key), 
                        placeholder.converter,
                        true);
                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(res), placeholder.key.SwitchReturns<IKey>(x=>x,x=>x), Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(placeholderType));
                }
                return res;
            }

            // why do objects have keys?
            // that is wierd
            public Object CreateObjectOrModule(IScope parent, IKey key, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> converter)
            {
                var res = new Object(this, key.ToString()!, converter);
                IsChildOf(parent, res);
                HasObject(parent, key, res);
                return res;
            }

            public Method CreateMethod(IScope parent, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                var returns = CreateTransientMember(res);
                res.Returns = Possibly.Is(returns);
                var input = CreateMember(res, new NameKey(inputName),false, inputConverter);
                res.Input = Possibly.Is(input);
                return res;
            }


            public Method CreateMethod(IScope parent, IOrType<TypeProblem2.TypeReference, IError> inputType, IOrType<TypeProblem2.TypeReference, IError> outputType, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                if (!inputType.Is1(out var inputTypeValue))
                {
                    throw new NotImplementedException();
                }
                if (!outputType.Is1(out var outputTypeValue))
                {
                    throw new NotImplementedException();
                }

                var res = new Method(this, $"method{{inputName:{inputName},inputType:{inputTypeValue.debugName},outputType:{outputTypeValue.debugName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                {
                    var returns = outputTypeValue.TypeKey is IIsDefinately<IKey> typeKey ? CreateTransientMember(res, typeKey.Value) : CreateTransientMember(res);
                    res.Returns = Possibly.Is(returns);
                }
                {
                    if (inputTypeValue.TypeKey is IIsDefinately<IKey> typeKey)
                    {
                        res.Input = Possibly.Is(CreateMember(res, new NameKey(inputName), Prototypist.Toolbox.OrType.Make<IKey, IError>(typeKey.Value),false, inputConverter));
                    }
                    else
                    {
                        res.Input = Possibly.Is(CreateMember(res, new NameKey(inputName), false, inputConverter));
                    }
                }
                return res;
            }


            public Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, true, "hopeful - " + key.ToString()!, converter);
                res = HasHopefulMember(scope, key, res);
                return res;
            }


            public OrType CreateOrType(IScope s, IKey key, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode1, IOrType<TypeProblem2.TypeReference, IError> setUpSideNode2, IConvertTo<OrType, WeakTypeOrOperation> converter)
            {
                if (!setUpSideNode1.Is1(out var node1))
                {
                    throw new NotImplementedException();
                }
                if (!setUpSideNode2.Is1(out var node2))
                {
                    throw new NotImplementedException();
                }

                var res = new OrType(this, $"{node1.debugName} || {node2.debugName}", converter);
                Ors(res, node1, node2);
                HasOrType(s, key, res);

                return res;

            }
            public MethodType GetMethod(IOrType<MethodType, Type, Object, OrType, InferredType, IError> input, IOrType<MethodType, Type, Object, OrType, InferredType, IError> output)
            {
                throw new NotImplementedException();
            }

            private static void Ors(OrType orType, TypeReference a, TypeReference b)
            {
                orType.Left = Possibly.Is(a);
                orType.Right = Possibly.Is(b);
            }

            private static void HasOrType(IScope scope, IKey kay, OrType orType1)
            {
                scope.OrTypes[kay] = orType1;
            }


            public void IsNumber(IScope parent, ILookUpType target)
            {
                var thing = CreateTransientMember(parent, new NameKey("number"));
                AssertIs(target, thing);
            }

            public void IsBlock(IScope parent, ILookUpType target)
            {
                var thing = CreateTransientMember(parent, new NameKey("block"));
                AssertIs(target, thing);
            }

            public void IsBool(IScope parent, ILookUpType target)
            {
                var thing = CreateTransientMember(parent, new NameKey("bool"));
                AssertIs(target, thing);
            }

            public void IsEmpty(IScope parent, ILookUpType target)
            {
                var thing = CreateTransientMember(parent, new NameKey("empty"));
                AssertIs(target, thing);
            }

            public void IsString(IScope parent, ILookUpType target)
            {
                var thing = CreateTransientMember(parent, new NameKey("string"));
                AssertIs(target, thing);
            }

            private TransientMember CreateTransientMember(IScope parent)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = Possibly.Is(parent);
                return res;
            }


            private TransientMember CreateTransientMember(IScope parent, IKey typeKey)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = Possibly.Is(parent);
                res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                return res;
            }

            // ok
            // so... this can not be converted
            // it is not a real method
            // it is just something of type method
            // it is really just a type
            //
            public Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var thing = CreateTransientMember(parent);
                var method = CreateMethod(parent, "input", converter, inputConverter);
                IsAssignedTo(target, thing);
                return method;
            }

            #endregion


            public TransientMember GetReturns(IScope s)
            {
                if (s is Method method)
                {
                    return GetReturns(method);
                }
                else if (s.Parent is IIsDefinately<IScope> definatelyScope)
                {
                    return GetReturns(definatelyScope.Value);
                }
                else
                {
                    throw new Exception("s.Parent should not be null");
                }
            }

            internal TransientMember GetReturns(Method method)
            {
                return method.Returns.GetOrThrow();
            }



            public TransientMember GetReturns(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Returns.GetOrThrow();
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    var methodInputKey = new NameKey("implicit input -" + Guid.NewGuid());
                    inferredMethodType.Input = Possibly.Is(CreateMember(inferredMethodType, methodInputKey,false, new WeakMemberDefinitionConverter(false, methodInputKey))); ;
                    var returns = CreateTransientMember(inferredMethodType); ;
                    inferredMethodType.Returns = Possibly.Is(returns);
                    return returns;
                }
            }

            public Member GetInput(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Input.GetOrThrow();
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    var methodInputKey = new NameKey("implicit input -" + Guid.NewGuid());
                    var input = CreateMember(inferredMethodType, methodInputKey,false, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inferredMethodType.Input = Possibly.Is(input);
                    inferredMethodType.Returns = Possibly.Is(CreateTransientMember(inferredMethodType));

                    return input;
                }
            }

            public Member GetInput(Method method)
            {
                return method.Input.GetOrThrow();
            }


            public IIsPossibly<IKey> GetKey(TypeReference type)
            {
                return type.TypeKey.Possibly1();
            }

            // pretty sure it is not safe to solve more than once 
            public ITypeSolution Solve()
            {
                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>) && x.TypeKey.Is3(out var _)))
                {
                    var type = new InferredType(this, $"for {((TypeProblemNode)node).debugName}");
                    node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type));
                }

                // what happens here if x.TypeKey.Is2??
                if (toLookUp.Any(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>) && x.TypeKey.Is2(out var _)))
                {
                    throw new NotImplementedException();
                }

                // generics register themsleves 
                var realizedGeneric = new Dictionary<GenericTypeKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();

                foreach (var methodType in typeProblemNodes.OfType<MethodType>().Where(x => x.GenericOverlays.Any()))
                {
                    var key = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(methodType), methodType.GenericOverlays.Values.ToArray());
                }
                foreach (var type in typeProblemNodes.OfType<Type>().Where(x => x.GenericOverlays.Any()))
                {
                    var key = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), type.GenericOverlays.Values.ToArray());
                }

                toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>)).ToArray();

                // overlay generics
                while (toLookUp.Any())
                {
                    foreach (var node in toLookUp)
                    {
                        LookUpOrOverlayOrThrow(node);
                    }
                    toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => !(x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>)).ToArray();
                }

                // members that might be on parents 

                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();

                foreach (var (node, possibleMembers) in typeProblemNodes.OfType<IScope>().Select(x => (x, x.PossibleMembers)))
                {
                    foreach (var pair in possibleMembers)
                    {
                        TryGetMember(node, pair.Key).IfElse(member => TryMerge(pair.Value, member!), () => HasMember(node, pair.Key, pair.Value));
                    }
                }

                // hopeful members and methods are a little rough around the edges
                // they are very similar yet implemented differently 

                // hopeful members 
                foreach (var (node, hopeful) in typeProblemNodes.OfType<IValue>().Select(x => (x, x.HopefulMembers)))
                {
                    foreach (var pair in hopeful)
                    {
                        if (GetMembers2(GetType(node)).TryGetValue(pair.Key, out var member))
                        {
                            TryMerge(pair.Value, member);
                        }
                        else if (GetType(node).Is5(out var inferred))
                        {
                            HasMember(inferred, pair.Key, pair.Value);
                        }
                        else
                        {
                            throw new Exception("member could not be handled ");
                        }
                    }
                }

                // hopeful methods 
                foreach (var (node, hopefulMethod) in typeProblemNodes.OfType<IValue>().Select(x => (x, x.HopefulMethod)))
                {
                    if (hopefulMethod is IIsDefinately<InferredType> definately)
                    {
                        var type = GetType(node);
                        if (type.Is1(out var methodType))
                        {
                            if (!methodType.Equals(hopefulMethod))
                            {
                                node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType));

                                var defererReturns = definately.Value.Returns.GetOrThrow();
                                var deferredToReturns = methodType.Returns.GetOrThrow();
                                TryMerge(defererReturns, deferredToReturns);

                                var defererInput = definately.Value.Input.GetOrThrow();
                                var deferredToInput = methodType.Input.GetOrThrow();
                                TryMerge(defererInput, deferredToInput);
                            }
                        }
                        else if (type.Is5(out var dummy))
                        {
                            node.LooksUp = Possibly.Is(Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(definately.Value));
                        }
                        else
                        {
                            throw new Exception("no good!");
                        }
                    }
                }

                // flow up stream
                // very sloppy and slow
                // if I ever am worried about speed I am sure this will be a canidate
                bool go;
                do
                {
                    go = false;

                    foreach (var (from, to) in assignments)
                    {
                        // nothing should look up to null at this point
                        var toType = to.LooksUp.GetOrThrow();
                        var fromType = from.LooksUp.GetOrThrow();

                        go |= Flow(toType, fromType);

                    }

                    //foreach (var (from, to) in assertions)
                    //{
                    //    // nothing should look up to null at this point
                    //    var fromType = from.LooksUp.GetOrThrow();

                    //    go |= Flow(to, fromType);

                    //}

                } while (go);

                // we dont flow downstream

                return new TypeSolution(
                    typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>>).ToDictionary(x => x, x => x.LooksUp.GetOrThrow()),
                    typeProblemNodes.OfType<IHaveMembers>().ToDictionary(x => x, x => (IReadOnlyList<Member>)x.Members.Select(y => y.Value).ToArray()),
                    typeProblemNodes.OfType<OrType>().ToDictionary(x => x, x => (x.Left.GetOrThrow(), x.Right.GetOrThrow())),
                    typeProblemNodes.Select(x =>
                    {
                        if (x.SafeIs(out Method m))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(m));
                        }

                        if (x.SafeIs(out MethodType mt))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(mt));
                        }

                        if (x.SafeIs(out InferredType it) && it.Input is IIsDefinately<Member>)
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<IOrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<IOrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<IOrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Input.GetOrThrow(), v1 => v1.Input.GetOrThrow(), v1 => v1.Input.GetOrThrow())),
                    typeProblemNodes.Select(x =>
                    {
                        if (x.SafeIs(out Method m))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(m));
                        }
                        if (x.SafeIs(out MethodType mt))
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(mt));
                        }
                        if (x.SafeIs(out InferredType it) && it.Returns is IIsDefinately<TransientMember>)
                        {
                            return Possibly.Is(Prototypist.Toolbox.OrType.Make<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<IOrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<IOrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<IOrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Returns.GetOrThrow(), v1 => v1.Returns.GetOrThrow(), v1 => v1.Returns.GetOrThrow())),
                    typeProblemNodes.OfType<IScope>().Where(x => x.EntryPoints.Any()).ToDictionary(x => x, x => x.EntryPoints.Single()));

                #region Helpers

                void TryMerge(IValue deferer, IValue deferredTo)
                {

                    var defererType = GetType(deferer);
                    if (defererType.Is5(out var deferringInferred).Not())
                    {
                        throw new Exception("we can't merge that!");
                    }
                    var deferredToType = GetType(deferredTo);

                    var toReplace = new List<ILookUpType>();

                    foreach (var lookUper in typeProblemNodes.OfType<ILookUpType>())
                    {
                        if (lookUper.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> lookUperLooksUp && lookUperLooksUp.Value.Equals(defererType))
                        {
                            toReplace.Add(lookUper);
                        }
                    }

                    foreach (var key in toReplace)
                    {
                        key.LooksUp = Possibly.Is(deferredToType);
                    }

                    // why do I need this?
                    // 
                    {
                        if (deferredTo.SafeIs<ITypeProblemNode,ICanAssignFromMe>(out var deferredToLeft))
                        {
                            if (deferredTo.SafeIs<ITypeProblemNode, ICanBeAssignedTo>(out var deferredToRight))
                            {
                                var nextAssignments = new List<(ILookUpType, ILookUpType)>();
                                foreach (var assignment in assignments)
                                {
                                    var left = assignment.Item1 == deferer ? deferredToLeft : assignment.Item1;
                                    var right = assignment.Item2 == deferer ? deferredToRight : assignment.Item2;
                                    nextAssignments.Add((left, right));
                                }
                                assignments = nextAssignments;
                            }
                            else
                            {
                                var nextAssignments = new List<(ILookUpType, ILookUpType)>();
                                foreach (var assignment in assignments)
                                {
                                    var left = assignment.Item1 == deferer ? deferredToLeft : assignment.Item1;
                                    nextAssignments.Add((left, assignment.Item2));
                                }
                                assignments = nextAssignments;
                            }
                        }
                        else if (deferredTo.SafeIs<ITypeProblemNode, ICanBeAssignedTo>(out var deferredToRight))
                        {
                            var nextAssignments = new List<(ILookUpType, ILookUpType)>();

                            foreach (var assignment in assignments)
                            {
                                var right = assignment.Item2 == deferer ? deferredToRight : assignment.Item2;
                                nextAssignments.Add((assignment.Item1, right));
                            }
                            assignments = nextAssignments;
                        }
                    }

                    {
                        if (deferredToType.Is1(out var deferredToMethod))
                        {
                            if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringReturns && deferredToMethod.Returns is IIsDefinately<TransientMember> deferredToReturns)
                            {
                                TryMerge(deferringReturns.Value, deferredToReturns.Value);
                            }


                            if (deferringInferred.Input is IIsDefinately<Member> deferringInput && deferredToMethod.Input is IIsDefinately<Member> deferredToInput)
                            {
                                TryMerge(deferringInput.Value, deferredToInput.Value);
                            }
                        }
                    }

                    {
                        if (IsNotInferedHasMembers(deferredToType, out var hasMembers))
                        {
                            foreach (var memberPair in GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(deferringInferred)))
                            {
                                if (hasMembers!.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    TryMerge(memberPair.Value, deferedToMember);
                                }
                                else
                                {
                                    throw new Exception("the implicit type has members the real type does not");
                                    //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                    //HasMember(deferredToHaveType, memberPair.Key, newValue);
                                    //lookUps[newValue] = lookUps[memberPair.Value];
                                }
                            }
                        }
                    }

                    {
                        if (deferredToType.Is5(out var deferredToInferred))
                        {

                            foreach (var memberPair in GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(deferringInferred)))
                            {
                                if (deferredToInferred.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    TryMerge(memberPair.Value, deferedToMember);
                                }
                                else
                                {
                                    var newValue = new Member(this, memberPair.Value.IsExposed, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                    HasMember(deferredToInferred, memberPair.Key, newValue);
                                    newValue.LooksUp = memberPair.Value.LooksUp;
                                }
                            }

                            if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns)
                            {
                                if (deferredToInferred.Returns is IIsDefinately<TransientMember> deferredToInferredReturns)
                                {
                                    TryMerge(deferringInferredReturns.Value, deferredToInferredReturns.Value);
                                }
                                else
                                {
                                    deferredToInferred.Returns = deferringInferred.Returns;
                                }
                            }

                            if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput)
                            {
                                if (deferredToInferred.Input is IIsDefinately<Member> deferredToInferredInput)
                                {
                                    TryMerge(deferringInferredInput.Value, deferredToInferredInput.Value);
                                }
                                else
                                {
                                    deferredToInferred.Input = deferringInferred.Input;
                                }
                            }
                        }
                    }
                }

                IOrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrOverlayOrThrow(ILookUpType node)
                {

                    if (node.LooksUp is IIsDefinately<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, IError>> nodeLooksUp)
                    {
                        return nodeLooksUp.Value;
                    }

                    // if we don't have a lookup we damn well better have a context and a key
                    var res = TryLookUpOrOverlay(node.Context.GetOrThrow(), node.TypeKey.Is1OrThrow());
                    node.LooksUp = Possibly.Is(res);
                    return res;
                }

                IOrType<MethodType, Type, Object, OrType, InferredType, IError> LookUpOrOverlayOrThrow2(IScope from, IKey key)
                {
                    return TryLookUpOrOverlay(from, key);
                }

                IOrType<MethodType, Type, Object, OrType, InferredType, IError> TryLookUpOrOverlay(IScope from, IKey key)
                {

                    if (key is GenericNameKey genericNameKey)
                    {

                        var types = genericNameKey.Types.Select(typeKey => typeKey.SwitchReturns(
                            x => LookUpOrOverlayOrThrow2(from, x),
                            y => throw new NotImplementedException("Type could not be resolved")) // I want to use NIEs to keep my code near compilablity. once I have a lot in type problem node, I can think about how to handle them.
                        ).ToArray();


                        var outerLookedUp = LookUpOrOverlayOrThrow2(from, genericNameKey.Name);

                        return outerLookedUp.SwitchReturns(method =>
                        {
                            var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(method), types);

                            if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                            {
                                return res2;
                            }

                            // this is duplicate code - 94875369485
                            var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                            foreach (var (oldType, newType) in types.Zip(method.GenericOverlays, (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(method), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new MethodType(this, $"generated-generic-{method.debugName}", method.Converter)), map);

                            return @explicit.SwitchReturns(v1 =>
                            {
                                realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
                            }, v2 =>
                            {
                                realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
                            });

                        },
                        type =>
                        {
                            var genericTypeKey = new GenericTypeKey(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), types);

                            if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                            {
                                return res2;
                            }

                            // this is duplicate code - 94875369485
                            var map = new Dictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>>();
                            foreach (var (oldType, newType) in types.Zip(type.GenericOverlays, (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(Prototypist.Toolbox.OrType.Make<MethodType, Type>(type), Prototypist.Toolbox.OrType.Make<MethodType, Type>(new Type(this, $"generated-generic-{type.debugName}", type.Key, type.Converter, type.IsPlaceHolder)), map);

                            return @explicit.SwitchReturns(v1 =>
                            {
                                realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1));
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v1);
                            },
                            v2 =>
                            {
                                realizedGeneric.Add(genericTypeKey, Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2));
                                return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(v2);
                            });

                        },
                        @object => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object),
                        orType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType),
                        inferredType => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(inferredType),
                        error => Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error));
                    }
                    else if (TryLookUp(from, key, out var res2))
                    {
                        //:'(
                        // I am sad about the !
                        return res2!;
                    }
                    else
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(Error.TypeNotFound($"could not find type for {key.ToString()} in {from.ToString()}"));
                    }
                }

                // [MaybeNullWhen(false)] grumble
                static bool TryLookUp(IScope haveTypes, IKey key, out OrType<MethodType, Type, Object, OrType, InferredType, IError>? result)
                {
                    while (true)
                    {
                        {
                            if (haveTypes.Types.TryGetValue(key, out var res))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.Objects.TryGetValue(key, out var res))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.OrTypes.TryGetValue(key, out var res))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.MethodTypes.TryGetValue(key, out var res))
                            {
                                result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(res);
                                return true;
                            }
                        }
                        if (haveTypes.SafeIs(out Type _) || haveTypes.SafeIs(out MethodType _))
                        {
                            Dictionary<IKey, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> genericOverlays;
                            if (haveTypes.SafeIs(out Type type))
                            {
                                genericOverlays = type.GenericOverlays;
                            }
                            else if (haveTypes.SafeIs(out MethodType methodType1))
                            {
                                genericOverlays = methodType1.GenericOverlays;
                            }
                            else
                            {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (genericOverlays.TryGetValue(key, out var res))
                            {
                                if (res.Is1(out var methodType))
                                {
                                    result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType);
                                    return true;
                                }
                                else if (res.Is2(out var innerType))
                                {
                                    result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(innerType);
                                    return true;
                                }
                                else if (res.Is6(out var error))
                                {
                                    result = Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(error);
                                    return true;
                                }
                                else
                                {
                                    throw new Exception("uh oh! we hit a type we did not want");
                                }
                            }
                        }

                        if (haveTypes.Parent.SafeIs(out IIsDefinately<IScope> defScope))
                        {
                            haveTypes = defScope.Value;
                        }
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
                }

                IOrType<MethodType, Type> CopyTree(IOrType<MethodType, Type> from, IOrType<MethodType, Type> to, IReadOnlyDictionary<IOrType<MethodType, Type, Object, OrType, InferredType, IError>, IOrType<MethodType, Type, Object, OrType, InferredType, IError>> overlayed)
                {

                    var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();

                    from.Switch(from1 =>
                    {
                        if (to.Is1(out var to1))
                        {
                            Copy(from1, to1);
                        }
                        else
                        {
                            throw new Exception("or exception");
                        }
                    },
                    from2 =>
                    {
                        if (to.Is2(out var to2))
                        {
                            Copy(from2, to2);
                        }
                        else
                        {
                            throw new Exception("or exception");
                        }
                    });

                    foreach (var pair in map)
                    {
                        if (pair.Key.SafeIs(out IScope fromScope) && fromScope.Parent.SafeIs(out IIsDefinately<IScope> defScope))
                        {
                            to.Switch(method =>
                            {
                                method.Parent = Possibly.Is(CopiedToOrSelf(defScope.Value));
                            },
                            type =>
                            {
                                type.Parent = Possibly.Is(CopiedToOrSelf(defScope.Value));
                            });
                        }
                    }

                    var oldAssignments = assignments.ToArray();
                    foreach (var pair in map)
                    {
                        if (pair.Key.SafeIs(out ICanBeAssignedTo assignedToFrom) && pair.Value.SafeIs(out ICanBeAssignedTo assignedToTo))
                        {
                            foreach (var item in oldAssignments)
                            {
                                if (item.Item2 == assignedToFrom)
                                {
                                    assignments.Add((CopiedToOrSelf(item.Item1), assignedToTo));
                                }
                            }
                        }

                        if (pair.Value.SafeIs(out ICanAssignFromMe assignFromFrom) && pair.Value.SafeIs(out ICanAssignFromMe assignFromTo))
                        {
                            foreach (var item in oldAssignments)
                            {
                                if (item.Item1 == assignFromFrom)
                                {
                                    assignments.Add((assignFromTo, CopiedToOrSelf(item.Item2)));
                                }
                            }
                        }
                    }

                    foreach (var pair in map)
                    {
                        if (pair.Key.SafeIs(out ILookUpType lookUpFrom) && pair.Value.SafeIs(out ILookUpType lookUpTo))
                        {

                            lookUpTo.TypeKey = lookUpFrom.TypeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));

                            if (lookUpFrom.Context.SafeIs(out IIsDefinately<IScope> definateContext))
                            {
                                lookUpTo.Context = Possibly.Is(CopiedToOrSelf(definateContext.Value));
                            }
                        }

                        if (pair.Key.SafeIs(out OrType orFrom) && pair.Value.SafeIs(out OrType orTo))
                        {
                            // from should have been populated
                            Ors(orTo, CopiedToOrSelf(orFrom.Left.GetOrThrow()), CopiedToOrSelf(orFrom.Right.GetOrThrow()));
                        }


                        if (pair.Key.SafeIs(out Method methodFrom) && pair.Value.SafeIs(out Method methodTo))
                        {
                            methodTo.Input = Possibly.Is(CopiedToOrSelf(methodFrom.Input.GetOrThrow()));
                            methodTo.Returns = Possibly.Is(CopiedToOrSelf(methodFrom.Returns.GetOrThrow()));
                        }

                        if (pair.Key.SafeIs(out MethodType methodFromType) && pair.Value.SafeIs(out MethodType methodToType))
                        {
                            methodToType.Input = Possibly.Is(CopiedToOrSelf(methodFromType.Input.GetOrThrow()));
                            methodToType.Returns = Possibly.Is(CopiedToOrSelf(methodFromType.Returns.GetOrThrow()));
                        }

                        if (pair.Key.SafeIs(out InferredType inferedFrom) && pair.Value.SafeIs(out InferredType inferedTo))
                        {
                            inferedTo.Input = Possibly.Is(CopiedToOrSelf(inferedFrom.Input.GetOrThrow()));
                            inferedTo.Returns = Possibly.Is(CopiedToOrSelf(inferedFrom.Returns.GetOrThrow()));
                        }
                    }

                    return to;

                    T CopiedToOrSelf<T>(T item)
                        where T : ITypeProblemNode
                    {
                        if (map.TryGetValue(item, out var res))
                        {
                            return (T)res;
                        }
                        return item;
                    }

                    // hasGenerics -- the root of the root will have had its generics replaced
                    // for the rest of the tree the generics will need to be copied
                    T Copy<T>(T innerFrom, T innerTo)
                        where T : ITypeProblemNode
                    {
                        map.Add(innerFrom, innerTo);

                        if (innerFrom.SafeIs<ITypeProblemNode, IScope>(out var innerFromScope) && innerTo.SafeIs<ITypeProblemNode, IScope>(out var innerScopeTo))
                        {

                            {
                                foreach (var item in innerFromScope.Values)
                                {
                                    var newValue = Copy(item, new Value(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                    HasValue(innerScopeTo, newValue);
                                }
                            }

                            {
                                foreach (var item in innerFromScope.Refs)
                                {
                                    var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                    HasReference(innerScopeTo, newValue);
                                }
                            }

                            {
                                foreach (var member in innerFromScope.Members)
                                {
                                    var newValue = Copy(member.Value, new Member(this, member.Value.IsExposed, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                    HasMember(innerScopeTo, member.Key, newValue);
                                }
                            }

                            {
                                foreach (var member in innerFromScope.TransientMembers)
                                {
                                    var newValue = Copy(member, new TransientMember(this, $"copied from {((TypeProblemNode)member).debugName}"));
                                    HasTransientMember(innerScopeTo, newValue);
                                }
                            }

                            {
                                foreach (var @object in innerFromScope.Objects)
                                {
                                    var newValue = Copy(@object.Value, new Object(this, $"copied from {((TypeProblemNode)@object.Value).debugName}", @object.Value.Converter));
                                    HasObject(innerScopeTo, @object.Key, newValue);
                                }

                            }

                            {
                                foreach (var type in innerFromScope.Types)
                                {
                                    var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Key, type.Value.Converter, type.Value.IsPlaceHolder));
                                    HasType(innerScopeTo, type.Key, newValue);
                                }
                            }
                            {
                                foreach (var method in innerFromScope.Methods)
                                {
                                    var newValue = Copy(method.Value, new Method(this, $"copied from {((TypeProblemNode)method.Value).debugName}", method.Value.Converter));
                                    HasMethod(innerScopeTo, method.Key, newValue);
                                }
                            }

                            {
                                foreach (var type in innerFromScope.OrTypes)
                                {
                                    var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                    HasOrType(innerScopeTo, type.Key, newValue);
                                }
                            }

                            {
                                foreach (var possible in innerFromScope.PossibleMembers)
                                {
                                    var newValue = Copy(possible.Value, new Member(this, possible.Value.IsExposed, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                    HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                                }
                            }
                        }

                        if (innerFrom.SafeIs<ITypeProblemNode, Type>(out var innerFromType) && innerTo.SafeIs<ITypeProblemNode, Type>(out var innerTypeTo))
                        {

                            foreach (var type in innerFromType.GenericOverlays)
                            {
                                if (overlayed.TryGetValue(type.Value, out var toType))
                                {
                                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, toType);
                                }
                                else
                                {
                                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerTypeTo), type.Key, type.Value);
                                }
                            }
                        }

                        if (innerFrom.SafeIs<ITypeProblemNode, MethodType>(out var innerFromMethodType) && innerTo.SafeIs<ITypeProblemNode, MethodType>(out var innerMethodTypeTo))
                        {

                            foreach (var type in innerFromMethodType.GenericOverlays)
                            {
                                if (overlayed.TryGetValue(type.Value, out var toType))
                                {
                                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, toType);
                                }
                                else
                                {
                                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(innerMethodTypeTo), type.Key, type.Value);
                                }
                            }
                        }

                        if (innerFrom.SafeIs<ITypeProblemNode, IValue>(out var innerFromHopeful) && innerTo.SafeIs<ITypeProblemNode, IValue>(out var innerToHopeful))
                        {

                            foreach (var possible in innerFromHopeful.HopefulMembers)
                            {
                                var newValue = Copy(possible.Value, new Member(this, possible.Value.IsExposed, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                HasHopefulMember(innerToHopeful, possible.Key, newValue);
                            }


                            if (innerFromHopeful.HopefulMethod  is IIsDefinately<InferredType> infered)
                            {
                                var newValue = Copy(infered.Value, new InferredType(this, $"copied from {((TypeProblemNode)infered.Value).debugName}"));
                                innerToHopeful.HopefulMethod = Possibly.Is(newValue);
                            }
                        }

                        return innerTo;
                    }
                }

                static bool IsHasMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, out IHaveMembers? haveMembers)
                {
                    var res = false;

                    (haveMembers, res) = type.SwitchReturns<(IHaveMembers?, bool)>(
                        v1 => (default, false),
                        v2 => (v2, true),
                        v3 => (v3, true),
                        v4 => (v4, true),
                        v5 => (v5, true),
                        v1 => (default, false));

                    return res;
                }

                static bool IsNotInferedHasMembers(IOrType<MethodType, Type, Object, OrType, InferredType, IError> type, out IHaveMembers? haveMembers)
                {
                    var res = false;
                    (haveMembers, res) = type.SwitchReturns<(IHaveMembers?, bool)>(
                        v1 => (default, false),
                        v2 => (v2, true),
                        v3 => (v3, true),
                        v4 => (v4, true),
                        v5 => (default, false),
                        v1 => (default, false));

                    return res;
                }

                static IOrType<MethodType, Type, Object, OrType, InferredType, IError> GetType(ITypeProblemNode value)
                {
                    if (value.SafeIs(out ILookUpType lookup))
                    {
                        // look up needs to be populated at this point
                        return lookup.LooksUp.GetOrThrow();
                    }
                    if (value.SafeIs(out MethodType methodType))
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(methodType);
                    }

                    if (value.SafeIs(out Type type))
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(type);

                    }
                    if (value.SafeIs(out Object @object))
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(@object);
                    }
                    if (value.SafeIs(out OrType orType))
                    {
                        return Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(orType);
                    }

                    throw new Exception("flaming pile of piss");
                    // well, I guess I now know that we have a duality
                    // you either are a type, or you have a type
                    // 
                }

                // returns true if the target was modified 
                bool Flow(IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowFrom, IOrType<MethodType, Type, Object, OrType, InferredType, IError> flowTo)
                {
                    var res = false;

                    if (flowFrom.Is1(out var fromMethod) && flowTo.Is1(out var toMethod))
                    {
                        var inFlowFrom = GetType(fromMethod.Input.GetOrThrow());
                        var inFlowTo = GetType(toMethod.Input.GetOrThrow());

                        res |= Flow(inFlowFrom, inFlowTo);


                        var returnFlowFrom = GetType(fromMethod.Returns.GetOrThrow());
                        var retrunFlowTo = GetType(toMethod.Returns.GetOrThrow());

                        res |= Flow(returnFlowFrom, retrunFlowTo);

                    }

                    if (IsHasMembers(flowFrom, out var fromType))
                    {

                        {
                            if (flowTo.Is2(out var deferredToType))
                            {
                                foreach (var memberPair in GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (memberPair.Value.IsExposed && deferredToType.Members.TryGetValue(memberPair.Key, out var deferedToMember) && deferedToMember.IsExposed)
                                    {
                                        res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
                                    }
                                    else
                                    {
                                        throw new Exception("the implicit type has members the real type does not, or one is exposed and the other is not");
                                        //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                        //HasMember(deferredToType, memberPair.Key, newValue);
                                        //lookUps[newValue] = lookUps[memberPair.Value];
                                    }
                                }
                            }
                        }

                        {
                            if (flowTo.Is3(out var deferredToObject))
                            {
                                foreach (var memberPair in GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (memberPair.Value.IsExposed && deferredToObject.Members.TryGetValue(memberPair.Key, out var deferedToMember) && deferedToMember.IsExposed)
                                    {
                                        res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
                                    }
                                    else
                                    {
                                        throw new Exception("the implicit type has members the real type does not, or one is exposed and the other is not");
                                        //var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                        //HasMember(deferredToHaveType, memberPair.Key, newValue);
                                        //lookUps[newValue] = lookUps[memberPair.Value];
                                    }
                                }
                            }
                        }

                        {
                            if (flowTo.Is5(out var deferredToInferred))
                            {

                                foreach (var memberPair in GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (memberPair.Value.IsExposed && deferredToInferred.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                    {
                                        if (deferedToMember.IsExposed)
                                        {
                                            res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
                                        }
                                        else {
                                            throw new Exception("one is exposed and the other is not!");
                                        }
                                    }
                                    else
                                    {
                                        var newValue = new Member(this, memberPair.Value.IsExposed, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                        HasMember(deferredToInferred, memberPair.Key, newValue);
                                        newValue.LooksUp = memberPair.Value.LooksUp;
                                        res = true;
                                    }
                                }
                            }
                        }
                    }

                    if (flowFrom.Is5(out var deferringInferred))
                    {

                        {
                            if (flowTo.Is1(out var deferredToMethod))
                            {
                                if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns && deferredToMethod.Returns is IIsDefinately<TransientMember> deferredToMethodReturns)
                                {
                                    res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToMethodReturns.Value));
                                }


                                if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput && deferredToMethod.Input is IIsDefinately<Member> deferredToMethodInput)
                                {
                                    res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToMethodInput.Value));
                                }
                            }
                        }

                        {
                            if (flowTo.Is5(out var deferredToInferred))
                            {
                                if (deferringInferred.Returns is IIsDefinately<TransientMember> deferringInferredReturns)
                                {
                                    if (deferredToInferred.Returns is IIsDefinately<TransientMember> deferredToInferredReturns)
                                    {
                                        res |= Flow(GetType(deferringInferredReturns.Value), GetType(deferredToInferredReturns.Value));
                                    }
                                    else
                                    {
                                        deferredToInferred.Returns = deferringInferred.Returns;
                                        res = true;
                                    }
                                }

                                if (deferringInferred.Input is IIsDefinately<Member> deferringInferredInput)
                                {
                                    if (deferredToInferred.Input is IIsDefinately<Member> deferredToInferredInput)
                                    {
                                        res |= Flow(GetType(deferringInferredInput.Value), GetType(deferredToInferredInput.Value));
                                    }
                                    else
                                    {
                                        deferredToInferred.Input = deferringInferred.Input;
                                        res = true;
                                    }
                                }
                            }
                        }
                    }

                    return res;

                }

                IReadOnlyDictionary<IKey, Member> GetMembers2(IOrType<MethodType, Type, Object, OrType, InferredType, IError> or)
                {

                    return or.SwitchReturns(
                        _ => new Dictionary<IKey, Member>(),
                        v2 => GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v2)),
                        v3 => GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v3)),
                        v4 => GetMembers(Prototypist.Toolbox.OrType.Make<IHaveMembers, OrType>(v4)),
                        _ => new Dictionary<IKey, Member>(),
                        _ => new Dictionary<IKey, Member>()
                        );
                }

                IReadOnlyDictionary<IKey, Member> GetMembers(IOrType<IHaveMembers, OrType> type)
                {
                    if (type.Is1(out var explictType))
                    {
                        return explictType.Members;
                    }

                    if (type.Is2(out var orType))
                    {
                        if (orTypeMembers.TryGetValue(orType, out var res))
                        {
                            return res;
                        }

                        res = new Dictionary<IKey, Member>();
                        var left = orType.Left.GetOrThrow();
                        var right = orType.Right.GetOrThrow();

                        var rightMembers = GetMembers2(GetType(right));
                        foreach (var leftMember in GetMembers2(GetType(left)))
                        {
                            if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                            {
                                // if they are the same type
                                if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
                                {
                                    var member = new Member(this, leftMember.Value.IsExposed, $"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}", leftMember.Value.Converter)
                                    {
                                        LooksUp = Possibly.Is(GetType(rightMember))
                                    };
                                    res[leftMember.Key] = member;
                                }
                            }
                        }

                        orTypeMembers[orType] = res;

                        return res;
                    }

                    throw new Exception($"{type.GetType()} unexpected");

                }

                #endregion

            }

            private static IIsPossibly<Member> TryGetMember(IScope context, IKey key)
            {
                while (true)
                {
                    if (context.Members.TryGetValue(key, out var member))
                    {
                        return Possibly.Is(member);
                    }
                    if (context.Parent is IIsDefinately<IScope> defParent)
                    {
                        context = defParent.Value;
                    }
                    else
                    {
                        return Possibly.IsNot<Member>();
                    }
                }
            }


            public TypeProblem2(IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakModuleDefinition>> moduleConverter)
            {

                Primitive = new Scope(this, "base", rootConverter);
                Dependency = CreateScope(Primitive, rootConverter);
                ModuleRoot = CreateObjectOrModule(CreateScope(Dependency, rootConverter), new ImplicitKey(Guid.NewGuid()), moduleConverter);

                CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("block")), new PrimitiveTypeConverter(new BlockType()));
                NumberType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make <NameKey,ImplicitKey>( new NameKey("number")), new PrimitiveTypeConverter(new NumberType()));
                StringType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("string")), new PrimitiveTypeConverter(new StringType()));
                BooleanType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("bool")), new PrimitiveTypeConverter(new BooleanType()));
                EmptyType = CreateType(Primitive, Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("empty")), new PrimitiveTypeConverter(new EmptyType()));

                // shocked this works...
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1"))), new GenericTypeParameterPlacholder(Prototypist.Toolbox. OrType.Make<NameKey, ImplicitKey>(new NameKey("T2"))) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] { new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T1")), new WeakTypeDefinitionConverter()), new TypeAndConverter(Prototypist.Toolbox.OrType.Make<NameKey, ImplicitKey>(new NameKey("T2")), new WeakTypeDefinitionConverter()) };

                var res = new MethodType(
                    this,
                    $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                HasMethodType(Primitive, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.key}", Possibly.Is(placeholder.key), placeholder.converter,true);
                    HasPlaceholderType(Prototypist.Toolbox.OrType.Make<MethodType, Type>(res), placeholder.key.SwitchReturns<IKey>(x=>x,x=>x), Prototypist.Toolbox.OrType.Make<MethodType, Type, Object, OrType, InferredType, IError>(placeholderType));
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                res.Input = Possibly.Is(CreateMember(res, methodInputKey, Prototypist.Toolbox.OrType.Make<IKey, IError>(new NameKey("T1")),false, new WeakMemberDefinitionConverter(false, methodInputKey)));
                res.Returns = Possibly.Is(CreateTransientMember(res, new NameKey("T2")));
                IsChildOf(Primitive, res);
            }

            private class GenericTypeKey
            {
                private readonly IOrType<MethodType, Type> primary;
                private readonly IOrType<MethodType, Type, Object, OrType, InferredType, IError>[] parameters;

                public GenericTypeKey(IOrType<MethodType, Type> primary, IOrType<MethodType, Type, Object, OrType, InferredType, IError>[] parameters)
                {
                    this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
                    this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
                }

                public override bool Equals(object? obj)
                {
                    return Equals(obj as GenericTypeKey);
                }

                public bool Equals(GenericTypeKey? other)
                {
                    return other != null &&
                        primary.Equals(other.primary) &&
                        parameters.Length == other.parameters.Length &&
                        parameters.Zip(other.parameters, (x, y) => x.Equals(y)).All(x => x);
                }

                public override int GetHashCode()
                {
                    return unchecked(primary.GetHashCode() + parameters.Aggregate(0, (y, x) => unchecked(y + x.GetHashCode())));
                }
            }
        }
    }

    internal static class TpnExtensions
    {
        //extensions
        public static IKey Key(this Tpn.TypeProblem2.TypeReference type)
        {
            // I THINK typeReference always have a Key
            return type.Problem.GetKey(type).GetOrThrow();
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IValue method)
        {
            return method.Problem.GetReturns(method);
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IScope method)
        {
            return method.Problem.GetReturns(method);
        }


        public static Tpn.TypeProblem2.Member Input(this Tpn.TypeProblem2.Method method)
        {
            return method.Problem.GetInput(method);
        }

        public static Tpn.TypeProblem2.Member Input(this Tpn.IValue method)
        {
            return method.Problem.GetInput(method);
        }

        public static void AssignTo(this Tpn.ICanAssignFromMe from, Tpn.ICanBeAssignedTo to)
        {
            from.Problem.IsAssignedTo(from, to);
        }
    }
}
