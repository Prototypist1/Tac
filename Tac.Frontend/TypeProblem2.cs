using Prototypist.Toolbox;
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


    // this static class is here just to make us all think in terms of these bros
    internal class Tpn
    {

        internal class TypeAndConverter
        {
            public readonly IKey key;
            public readonly IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter;

            public TypeAndConverter(IKey key, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
                this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            }

            public override string? ToString()
            {
                return key.ToString();
            }
        }

        internal interface ISetUpTypeProblem
        {
            // a =: x

            void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
            TypeProblem2.Value CreateValue(IScope scope, IKey typeKey, IConvertTo<TypeProblem2.Value, PlaceholderValue> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IKey typeKey, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType> type, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeProblem2.TypeReference, IFrontendType> converter);
            TypeProblem2.Scope CreateScope(IScope parent, IConvertTo<TypeProblem2.Scope, OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter);
            TypeProblem2.Type CreateType(IScope parent, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateType(IScope parent, IKey key, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateGenericType(IScope parent, IKey key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Object CreateObjectOrModule(IScope parent, IKey key, IConvertTo<TypeProblem2.Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter);
            TypeProblem2.Method CreateMethod(IScope parent, string inputName, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.Method CreateMethod(IScope parent, TypeProblem2.TypeReference inputType, TypeProblem2.TypeReference outputType, string inputName, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.TransientMember GetReturns(IValue s);
            TypeProblem2.TransientMember GetReturns(IScope s);
            TypeProblem2.Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.OrType CreateOrType(IScope s, IKey key, TypeProblem2.TypeReference setUpSideNode1, TypeProblem2.TypeReference setUpSideNode2, IConvertTo<TypeProblem2.OrType, WeakTypeOrOperation> converter);
            IIsPossibly<IKey> GetKey(TypeProblem2.TypeReference type);
            TypeProblem2.Member GetInput(IValue method);
            TypeProblem2.Member GetInput(TypeProblem2.Method method);

            TypeProblem2.MethodType GetMethod(OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType> input, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType> output);
            void IsNumber(IScope parent, ICanAssignFromMe target);
            void IsString(IScope parent, ICanAssignFromMe target);
            void IsEmpty(IScope parent, ICanAssignFromMe target);
            void IsBool(IScope parent, ICanAssignFromMe target);

            void HasEntryPoint(IScope parent, TypeProblem2.Scope entry);
            TypeProblem2.Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
        }

        internal interface ITypeSolution
        {
            IBox<PlaceholderValue> GetValue(TypeProblem2.Value value);
            IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member);
            IBox<IFrontendType> GetTypeReference(TypeProblem2.TypeReference typeReference);
            IBox<IFrontendType> GetInferredType(TypeProblem2.InferredType inferredType, IConvertTo<TypeProblem2.InferredType, IFrontendType> converter);
            IBox<OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope);
            // when I ungeneric this it should probably have the box inside the or..
            IBox<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType);
            IBox<OrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object);
            IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType);
            IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType);
            IBox<OrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method);
            IReadOnlyList<TypeProblem2.Member> GetMembers(IHaveMembers from);
            OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType> GetType(ILookUpType from);
            (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from);
            bool TryGetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.TransientMember? transientMember);
            bool TryGetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, out TypeProblem2.Member? member);

            TypeProblem2.TransientMember GetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
            TypeProblem2.Member GetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from);
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
            private readonly IReadOnlyDictionary<ILookUpType, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType>> map;
            private readonly IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts;
            private readonly IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn;
            private readonly IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut;
            private readonly IReadOnlyDictionary<IScope, TypeProblem2.Scope> moduleEntryPoint;

            public TypeSolution(
                IReadOnlyDictionary<ILookUpType, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType>> map,
                IReadOnlyDictionary<IHaveMembers, IReadOnlyList<TypeProblem2.Member>> members,
                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
                IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.Member> methodIn,
                IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType>, TypeProblem2.TransientMember> methodOut,
                IReadOnlyDictionary<IScope, TypeProblem2.Scope> moduleEntryPoint)
            {
                this.map = map ?? throw new ArgumentNullException(nameof(map));
                this.members = members ?? throw new ArgumentNullException(nameof(members));
                this.orTypeElememts = orTypeElememts ?? throw new ArgumentNullException(nameof(orTypeElememts));
                this.methodIn = methodIn ?? throw new ArgumentNullException(nameof(methodIn));
                this.methodOut = methodOut ?? throw new ArgumentNullException(nameof(methodOut));
                this.moduleEntryPoint = moduleEntryPoint ?? throw new ArgumentNullException(nameof(moduleEntryPoint));
            }


            private readonly Dictionary<TypeProblem2.Type, IBox<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> cacheType = new Dictionary<TypeProblem2.Type, IBox<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();
            public IBox<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType)
            {
                if (!cacheType.ContainsKey(explicitType))
                {
                    var box = new Box<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>();
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

            private readonly Dictionary<TypeProblem2.Method, IBox<OrType<WeakMethodDefinition, WeakImplementationDefinition>>> cacheMethod = new Dictionary<TypeProblem2.Method, IBox<OrType<WeakMethodDefinition, WeakImplementationDefinition>>>();
            public IBox<OrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method)
            {
                if (!cacheMethod.ContainsKey(method))
                {
                    var box = new Box<OrType<WeakMethodDefinition, WeakImplementationDefinition>>();
                    cacheMethod[method] = box;
                    box.Fill(method.Converter.Convert(this, method));
                }
                return cacheMethod[method];
            }

            private readonly Dictionary<TypeProblem2.Object, IBox<OrType<WeakObjectDefinition, WeakModuleDefinition>>> cacheObject = new Dictionary<TypeProblem2.Object, IBox<OrType<WeakObjectDefinition, WeakModuleDefinition>>>();
            public IBox<OrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object)
            {
                if (!cacheObject.ContainsKey(@object))
                {
                    var box = new Box<OrType<WeakObjectDefinition, WeakModuleDefinition>>();
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

            private readonly Dictionary<TypeProblem2.Scope, IBox<OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> cacheScope = new Dictionary<TypeProblem2.Scope, IBox<OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            public IBox<OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> GetScope(TypeProblem2.Scope scope)
            {
                if (!cacheScope.ContainsKey(scope))
                {
                    var box = new Box<OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>();
                    cacheScope[scope] = box;
                    box.Fill(scope.Converter.Convert(this, scope));
                }
                return cacheScope[scope];
            }

            private readonly Dictionary<TypeProblem2.TypeReference, IBox<IFrontendType>> cacheTypeReference = new Dictionary<TypeProblem2.TypeReference, IBox<IFrontendType>>();
            public IBox<IFrontendType> GetTypeReference(TypeProblem2.TypeReference typeReference)
            {
                if (!cacheTypeReference.ContainsKey(typeReference))
                {
                    var box = new Box<IFrontendType>();
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

            public OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType> GetType(ILookUpType from)
            {
                return map[from];
            }

            public (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from)
            {
                return orTypeElememts[from];
            }

            public bool TryGetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, [MaybeNullWhen(false)] out TypeProblem2.TransientMember? transientMember)
            {
                return methodOut.TryGetValue(from, out transientMember);
            }

            public bool TryGetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from, [MaybeNullWhen(false)] out TypeProblem2.Member? member)
            {
                return methodIn.TryGetValue(from, out member);
            }


            public TypeProblem2.TransientMember GetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from)
            {
                return methodOut[from];
            }

            public TypeProblem2.Member GetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType, TypeProblem2.InferredType> from)
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

        internal interface IHaveMembers : ITypeProblemNode
        {
            public Dictionary<IKey, TypeProblem2.Member> Members { get; }
        }
        internal interface ILookUpType : ITypeProblemNode
        {
            public IIsPossibly<IKey> TypeKey { get; set; }
            public IScope? Context { get; set; }
            public OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType>? LooksUp { get; set; }

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
            IScope? Parent { get; set; }

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
            public class TypeReference : TypeProblemNode<TypeReference, IFrontendType>, ILookUpType
            {
                public TypeReference(TypeProblem2 problem, string debugName, IConvertTo<TypeReference, IFrontendType> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IKey> TypeKey { get; set; } = Possibly.IsNot<IKey>();
                public IScope? Context { get; set; }
                public OrType<MethodType, Type, Object, OrType, InferredType>? LooksUp { get; set; }
            }

            public class Value : TypeProblemNode<Value, PlaceholderValue>, IValue
            {
                public Value(TypeProblem2 problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IKey> TypeKey { get; set; } = Possibly.IsNot<IKey>();
                public IScope? Context { get; set; }
                public OrType<MethodType, Type, Object, OrType, InferredType>? LooksUp { get; set; }

                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }
            public class Member : TypeProblemNode<Member, WeakMemberDefinition>, IMember
            {
                public Member(TypeProblem2 problem, string debugName, IConvertTo<Member, WeakMemberDefinition> converter) : base(problem, debugName, converter)
                {
                }

                public IIsPossibly<IKey> TypeKey { get; set; } = Possibly.IsNot<IKey>();
                public IScope? Context { get; set; }
                public OrType<MethodType, Type, Object, OrType, InferredType>? LooksUp { get; set; }
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }

            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }

                public IIsPossibly<IKey> TypeKey { get; set; } = Possibly.IsNot<IKey>();
                public IScope? Context { get; set; }
                public OrType<MethodType, Type, Object, OrType, InferredType>? LooksUp { get; set; }
                public Dictionary<IKey, Member> HopefulMembers { get; } = new Dictionary<IKey, Member>();
                public IIsPossibly<InferredType> HopefulMethod { get; set; } = Possibly.IsNot<InferredType>();
            }

            public class Type : TypeProblemNode<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType
            {
                public Type(TypeProblem2 problem, string debugName, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter) : base(problem, debugName, converter)
                {
                }

                public IScope? Parent { get; set; }
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
            public class MethodType : TypeProblemNode<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IHaveMembers, IScope
            {
                public MethodType(TypeProblem2 problem, string debugName, IConvertTo<MethodType, Tac.SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }

                public IScope? Parent { get; set; }
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

                public Member? Input { get; set; }
                public TransientMember? Returns { get; set; }
            }

            public class InferredType : TypeProblemNode, IHaveInputAndOutput, IHaveMembers, IScope
            {
                public InferredType(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }
                public IScope? Parent { get; set; }
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

                public Member? Input { get; set; }
                public TransientMember? Returns { get; set; }
            }

            public class OrType : TypeProblemNode<OrType, WeakTypeOrOperation>, IHaveMembers
            {
                public OrType(TypeProblem2 problem, string debugName, IConvertTo<OrType, WeakTypeOrOperation> converter) : base(problem, debugName, converter)
                {
                }
                public Dictionary<IKey, Member> Members { get; } = new Dictionary<IKey, Member>();

                public TypeReference? Left { get; set; }
                public TypeReference? Right { get; set; }
            }
            public class Scope : TypeProblemNode<Scope, OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>, IScope
            {
                public Scope(TypeProblem2 problem, string debugName, IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IScope? Parent { get; set; }
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
            public class Object : TypeProblemNode<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IScope? Parent { get; set; }
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
            public class Method : TypeProblemNode<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>>, IHaveMembers, IScope, IHaveInputAndOutput
            {
                public Method(TypeProblem2 problem, string debugName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter) : base(problem, debugName, converter)
                {
                }
                public IScope? Parent { get; set; }
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

                public Member? Input { get; set; }
                public TransientMember? Returns { get; set; }

            }


            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            private Scope Primitive { get; }
            public Scope Dependency { get; }
            public Object ModuleRoot { get; }


            // this holds real overlays 
            // the left hand side is the generic
            // the right hand side is the generic type parameter
            // it hold the placeholder and the realized type
            private readonly Dictionary<OrType<MethodType, Type>, Dictionary<IKey, OrType<MethodType, Type, Object, OrType, InferredType>>> genericOverlays = new Dictionary<OrType<MethodType, Type>, Dictionary<IKey, OrType<MethodType, Type, Object, OrType, InferredType>>>();



            //private readonly Dictionary<IValue, Dictionary<IKey, Member>> hopefulMembers = new Dictionary<IValue, Dictionary<IKey, Member>>();
            //private readonly Dictionary<IValue, InferredType> hopefulMethods = new Dictionary<IValue, InferredType>();

            private List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();


            #region Building APIs

            public void IsChildOf(IScope parent, IScope kid)
            {
                kid.Parent = parent;
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

            public void HasPlaceholderType(OrType<MethodType, Type> parent, IKey key, OrType<MethodType, Type, Object, OrType, InferredType> type)
            {
                if (!genericOverlays.ContainsKey(parent))
                {
                    genericOverlays.Add(parent, new Dictionary<IKey, OrType<MethodType, Type, Object, OrType, InferredType>>());
                }
                genericOverlays[parent].Add(key, type);
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
            public static  Member HasHopefulMember(IValue parent, IKey key, Member member)
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
                assignments.Add((assignedFrom, assignedTo));
            }

            public Value CreateValue(IScope scope, IKey typeKey, IConvertTo<Value, PlaceholderValue> converter)
            {
                var res = new Value(this, typeKey.ToString()!, converter);
                HasValue(scope, res);
                res.Context = scope;
                res.TypeKey = Possibly.Is(typeKey);
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IKey typeKey, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasMember(scope, key, res);
                res.Context = scope;
                res.TypeKey = Possibly.Is(typeKey);
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasMember(scope, key, res);
                res.Context = scope;
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, OrType<MethodType, Type, Object, OrType, InferredType> type, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString()!, converter);
                HasMember(scope, key, res);
                res.LooksUp = type;
                return res;
            }

            public Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                if (scope.PossibleMembers.TryGetValue(key, out var res1))
                {
                    return res1;
                }
                var res = new Member(this, "possibly on parent -" + key.ToString()!, converter);
                res = HasMembersPossiblyOnParent(scope, key, res);
                res.Context = scope;
                return res;
            }

            public TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeReference, IFrontendType> converter)
            {
                var res = new TypeReference(this, typeKey.ToString()!, converter);
                HasReference(context, res);
                res.Context = context;
                res.TypeKey = Possibly.Is(typeKey);
                return res;
            }

            public Scope CreateScope(IScope parent, IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter)
            {
                var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).debugName}", converter);
                IsChildOf(parent, res);
                return res;
            }

            public Type CreateType(IScope parent, IKey key, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(this, key.ToString()!, converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
                return res;
            }

            public Type CreateType(IScope parent, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var key = new ImplicitKey(Guid.NewGuid());
                var res = new Type(this, key.ToString()!, converter);
                IsChildOf(parent, res);
                // migiht need this, let's try without first
                //HasType(parent, key, res);
                return res;
            }


            public Type CreateGenericType(IScope parent, IKey key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(this, $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.ToString())}", converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.key.ToString()}", placeholder.converter);
                    HasPlaceholderType(new OrType<MethodType, Type>(res), placeholder.key, new OrType<MethodType, Type, Object, OrType, InferredType>(placeholderType));
                }
                return res;
            }

            // why do objects have keys?
            // that is wierd
            public Object CreateObjectOrModule(IScope parent, IKey key, IConvertTo<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter)
            {
                var res = new Object(this, key.ToString()!, converter);
                IsChildOf(parent, res);
                HasObject(parent, key, res);
                return res;
            }

            public Method CreateMethod(IScope parent, string inputName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                var returns = CreateTransientMember(res);
                res.Returns = returns;
                var input = CreateMember(res, new NameKey(inputName), inputConverter);
                res.Input = input;
                return res;
            }


            public Method CreateMethod(IScope parent, TypeReference inputType, TypeReference outputType, string inputName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {

                var res = new Method(this, $"method{{inputName:{inputName},inputType:{inputType.debugName},outputType:{outputType.debugName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                {
                    var returns = inputType.TypeKey is IIsDefinately<IKey> typeKey ? CreateTransientMember(res, typeKey.Value) : CreateTransientMember(res);
                    res.Returns = returns;
                }
                {
                    if (inputType.TypeKey is IIsDefinately<IKey> typeKey)
                    {
                        res.Input = CreateMember(res, new NameKey(inputName), typeKey.Value, inputConverter);
                    }
                    else
                    {
                        res.Input = CreateMember(res, new NameKey(inputName), inputConverter);
                    }
                }
                return res;
            }


            public Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, "hopeful - " + key.ToString()!, converter);
                res = HasHopefulMember(scope, key, res);
                return res;
            }


            public OrType CreateOrType(IScope s, IKey key, TypeReference setUpSideNode1, TypeReference setUpSideNode2, IConvertTo<OrType, WeakTypeOrOperation> converter)
            {
                var res = new OrType(this, $"{((TypeProblemNode)setUpSideNode1).debugName} || {((TypeProblemNode)setUpSideNode2).debugName}", converter);
                Ors(res, setUpSideNode1, setUpSideNode2);
                HasOrType(s, key, res);

                return res;

            }
            public MethodType GetMethod(OrType<MethodType, Type, Object, OrType, InferredType> input, OrType<MethodType, Type, Object, OrType, InferredType> output)
            {
                throw new NotImplementedException();
            }

            private static void Ors(OrType orType, TypeReference a, TypeReference b)
            {
                orType.Left = a;
                orType.Right = b;
            }

            private static void HasOrType(IScope scope, IKey kay, OrType orType1)
            {
                scope.OrTypes[kay] = orType1;
            }


            public void IsNumber(IScope parent, ICanAssignFromMe target)
            {
                var thing = CreateTransientMember(parent, new NameKey("number"));
                IsAssignedTo(target, thing);
            }

            public void IsBool(IScope parent, ICanAssignFromMe target)
            {
                var thing = CreateTransientMember(parent, new NameKey("bool"));
                IsAssignedTo(target, thing);
            }

            public void IsEmpty(IScope parent, ICanAssignFromMe target)
            {
                var thing = CreateTransientMember(parent, new NameKey("empty"));
                IsAssignedTo(target, thing);
            }

            public void IsString(IScope parent, ICanAssignFromMe target)
            {
                var thing = CreateTransientMember(parent, new NameKey("string"));
                IsAssignedTo(target, thing);
            }

            private TransientMember CreateTransientMember(IScope parent)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = parent;
                return res;
            }


            private TransientMember CreateTransientMember(IScope parent, IKey typeKey)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                res.Context = parent;
                res.TypeKey = Possibly.Is(typeKey);
                return res;
            }

            // ok
            // so... this can not be converted
            // it is not a real method
            // it is just something of type method
            // it is really just a type
            //
            public Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
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
                else if (s.Parent != null)
                {
                    return GetReturns(s.Parent);
                }
                else
                {
                    throw new Exception("s.Parent should not be null");
                }
            }

            internal TransientMember GetReturns(Method method)
            {
                return method.Returns ?? throw new NullReferenceException(nameof(method.Returns));
            }



            public TransientMember GetReturns(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Returns ?? throw new NullReferenceException(nameof(inferredType.Value.Returns));
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    var methodInputKey = new NameKey("implicit input -" + Guid.NewGuid());
                    inferredMethodType.Input = CreateMember(inferredMethodType, methodInputKey, new WeakMemberDefinitionConverter(false, methodInputKey)); ;
                    var returns = CreateTransientMember(inferredMethodType); ;
                    inferredMethodType.Returns = returns;
                    return returns;
                }
            }

            public Member GetInput(IValue value)
            {
                if (value.HopefulMethod is IIsDefinately<InferredType> inferredType)
                {
                    return inferredType.Value.Input?? throw new NullReferenceException(nameof(inferredType.Value.Input));
                }
                else
                {
                    var inferredMethodType = new InferredType(this, "zzzz");
                    value.HopefulMethod = Possibly.Is(inferredMethodType);

                    var methodInputKey = new NameKey("implicit input -" + Guid.NewGuid());
                    var input = CreateMember(inferredMethodType, methodInputKey, new WeakMemberDefinitionConverter(false, methodInputKey));
                    inferredMethodType.Input = input;
                    inferredMethodType.Returns = CreateTransientMember(inferredMethodType);

                    return input;
                }
            }

            public Member GetInput(Method method)
            {
                return method.Input ?? throw new NullReferenceException(nameof(method.Input));
            }


            public IIsPossibly<IKey> GetKey(TypeReference type)
            {
                return type.TypeKey;
            }

            // pretty sure it is not safe to solve more than once 
            public ITypeSolution Solve()
            {
                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => x.LooksUp == null && x.TypeKey is IIsDefinatelyNot))
                {
                    var type = new InferredType(this, $"for {((TypeProblemNode)node).debugName}");
                    node.LooksUp = new OrType<MethodType, Type, Object, OrType, InferredType>(type);
                }


                // generics register themsleves 
                var realizedGeneric = new Dictionary<GenericTypeKey, OrType<MethodType, Type, Object, OrType, InferredType>>();

                var localScopes = typeProblemNodes.OfType<MethodType>().Select(x => new OrType<MethodType, Type>(x)).Where(x => genericOverlays.ContainsKey(x));
                var localMethodTypes = typeProblemNodes.OfType<Type>().Select(x => new OrType<MethodType, Type>(x)).Where(x => genericOverlays.ContainsKey(x));

                foreach (var node in new[] { localScopes, localMethodTypes }.SelectMany(x => x))
                {
                    var key = new GenericTypeKey(node, genericOverlays[node].Values.ToArray());
                    if (node.Is1(out var v1))
                    {
                        realizedGeneric[key] = new OrType<MethodType, Type, Object, OrType, InferredType>(v1);
                    }
                    else if (node.Is2(out var v2))
                    {
                        realizedGeneric[key] = new OrType<MethodType, Type, Object, OrType, InferredType>(v2);
                    }
                    else
                    {
                        throw new Exception("you are outside of the or type");
                    }
                }

                toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp == null).ToArray();

                // overlay generics
                while (toLookUp.Any())
                {
                    foreach (var node in toLookUp)
                    {
                        LookUpOrOverlayOrThrow(node);
                    }
                    toLookUp = typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp == null).ToArray();
                }

                // members that might be on parents 

                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();

                foreach (var (node, possibleMembers) in typeProblemNodes.OfType<IScope>().Select(x => (x, x.PossibleMembers)))
                {
                    foreach (var pair in possibleMembers)
                    {
                        if (TryGetMember(node, pair.Key, out var member))
                        {
                            TryMerge(pair.Value, member!);
                        }
                        else
                        {
                            HasMember(node, pair.Key, pair.Value);
                        }
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
                                node.LooksUp = new OrType<MethodType, Type, Object, OrType, InferredType>(methodType);

                                var defererReturns = definately.Value.Returns ?? throw new NullReferenceException(nameof(definately.Value.Returns));
                                var deferredToReturns = methodType.Returns ?? throw new NullReferenceException(nameof(methodType.Returns)); ;
                                TryMerge(defererReturns, deferredToReturns);

                                var defererInput = definately.Value.Input ?? throw new NullReferenceException(nameof(definately.Value.Input)); ;
                                var deferredToInput = methodType.Input ?? throw new NullReferenceException(nameof(methodType.Input)); ;
                                TryMerge(defererInput, deferredToInput);
                            }
                        }
                        else if (type.Is5(out var dummy))
                        {
                            node.LooksUp = new OrType<MethodType, Type, Object, OrType, InferredType>(definately.Value);
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
                        var toType = to.LooksUp;
                        var fromType = from.LooksUp;

                        // nothing should look up to null at this point
                        if (toType == null)
                        {
                            throw new NullReferenceException(nameof(toType));
                        }
                        if (fromType == null)
                        {
                            throw new NullReferenceException(nameof(fromType));
                        }

                        go |= Flow(toType, fromType);

                    }
                } while (go);

                // we dont flow downstream

                return new TypeSolution(
                    typeProblemNodes.OfType<ILookUpType>().Where(x => x.LooksUp != null).ToDictionary(x => x, x => x.LooksUp!),
                    typeProblemNodes.OfType<IHaveMembers>().ToDictionary(x => x, x => (IReadOnlyList<Member>)x.Members.Select(y => y.Value).ToArray()),
                    typeProblemNodes.OfType<OrType>().ToDictionary(x => x, x => (x.Left!, x.Right!)),
                    typeProblemNodes.Select<ITypeProblemNode, IIsPossibly<OrType<Method, MethodType, InferredType>>>(x =>
                    {
                        if (x is Method m)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(m));
                        }

                        if (x is MethodType mt)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(mt));
                        }

                        if (x is InferredType it)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<OrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<OrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<OrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Input!, v1 => v1.Input!, v1 => v1.Input!)),
                    typeProblemNodes.Select<ITypeProblemNode, IIsPossibly<OrType<Method, MethodType, InferredType>>>(x =>
                    {
                        if (x is Method m)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(m));
                        }
                        if (x is MethodType mt)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(mt));
                        }
                        if (x is InferredType it)
                        {
                            return Possibly.Is(new OrType<Method, MethodType, InferredType>(it));
                        }
                        return Possibly.IsNot<OrType<Method, MethodType, InferredType>>();
                    }).Where(x => x is IIsDefinately<OrType<Method, MethodType, InferredType>>)
                    .Select(x => x.CastTo<IIsDefinately<OrType<Method, MethodType, InferredType>>>().Value)
                    .ToDictionary(x => x, x => x.SwitchReturns(v1 => v1.Returns!, v1 => v1.Returns!, v1 => v1.Returns!)),
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
                        if (lookUper.LooksUp != null && lookUper.LooksUp.Equals(defererType))
                        {
                            toReplace.Add(lookUper);
                        }
                    }

                    foreach (var key in toReplace)
                    {
                        key.LooksUp = deferredToType;
                    }

                    // why do I need this?
                    // 
                    {
                        if (deferredTo is ICanAssignFromMe deferredToLeft)
                        {
                            if (deferredTo is ICanBeAssignedTo deferredToRight)
                            {
                                var nextAssignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
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
                                var nextAssignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
                                foreach (var assignment in assignments)
                                {
                                    var left = assignment.Item1 == deferer ? deferredToLeft : assignment.Item1;
                                    nextAssignments.Add((left, assignment.Item2));
                                }
                                assignments = nextAssignments;
                            }
                        }
                        else if (deferredTo is ICanBeAssignedTo deferredToRight)
                        {
                            var nextAssignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();

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
                            if (deferringInferred.Returns != null && deferredToMethod.Returns != null)
                            {
                                var deferredToReturns = deferredToMethod.Returns;
                                TryMerge(deferringInferred.Returns, deferredToReturns);
                            }


                            if (deferringInferred.Input != null && deferredToMethod.Input != null)
                            {
                                var deferredToInput = deferredToMethod.Input;
                                TryMerge(deferringInferred.Input, deferredToInput);
                            }
                        }
                    }

                    {
                        if (IsNotInferedHasMembers(deferredToType, out var hasMembers))
                        {
                            foreach (var memberPair in GetMembers(new OrType<IHaveMembers, OrType>(deferringInferred)))
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

                            foreach (var memberPair in GetMembers(new OrType<IHaveMembers, OrType>(deferringInferred)))
                            {
                                if (deferredToInferred.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                {
                                    TryMerge(memberPair.Value, deferedToMember);
                                }
                                else
                                {
                                    var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                    HasMember(deferredToInferred, memberPair.Key, newValue);
                                    newValue.LooksUp = memberPair.Value.LooksUp;
                                }
                            }

                            if (deferringInferred.Returns != null)
                            {
                                if (deferredToInferred.Returns != null)
                                {
                                    TryMerge(deferringInferred.Returns, deferredToInferred.Returns);
                                }
                                else
                                {
                                    deferredToInferred.Returns = deferringInferred.Returns;
                                }
                            }

                            if (deferringInferred.Input != null)
                            {
                                if (deferredToInferred.Input != null)
                                {
                                    TryMerge(deferringInferred.Input, deferredToInferred.Input);
                                }
                                else
                                {
                                    deferredToInferred.Input = deferringInferred.Input;
                                }
                            }
                        }
                    }
                }

                OrType<MethodType, Type, Object, OrType, InferredType> LookUpOrOverlayOrThrow(ILookUpType node)
                {

                    if (node.LooksUp != null)
                    {
                        return node.LooksUp;
                    }

                    // if we don't have a lookup we damn well better have a context and a key
                    var from = node.Context ?? throw new NullReferenceException(nameof(node.Context));

                    if (!TryLookUpOrOverlay(from, node.TypeKey.GetOrThrow(), out var res))
                    {
                        throw new Exception("could not find type");
                    }
                    node.LooksUp = res!;
                    return res!;

                }



                OrType<MethodType, Type, Object, OrType, InferredType> LookUpOrOverlayOrThrow2(IScope from, IKey key)
                {
                    if (!TryLookUpOrOverlay(from, key, out var res))
                    {
                        throw new Exception("could not find type");
                    }
                    return res!;
                }

                //[MaybeNullWhen(false)]
                bool TryLookUpOrOverlay(IScope from, IKey key, out OrType<MethodType, Type, Object, OrType, InferredType>? res)
                {

                    if (key is GenericNameKey genericNameKey)
                    {

                        var types = genericNameKey.Types.Select(typeKey => LookUpOrOverlayOrThrow2(from, typeKey)).ToArray();


                        var outerLookedUp = LookUpOrOverlayOrThrow2(from, genericNameKey.Name);

                        if (outerLookedUp.Is1(out var method))
                        {
                            var genericTypeKey = new GenericTypeKey(new OrType<MethodType, Type>(method), types);


                            if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                            {
                                res = res2;
                                return true;
                            }

                            // this is duplicate code - 94875369485
                            var map = new Dictionary<OrType<MethodType, Type, Object, OrType, InferredType>, OrType<MethodType, Type, Object, OrType, InferredType>>();
                            foreach (var (oldType, newType) in types.Zip(genericOverlays[new OrType<MethodType, Type>(method)], (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(new OrType<MethodType, Type>(method), new OrType<MethodType, Type>(new MethodType(this, $"generated-generic-{method.debugName}", method.Converter)), map);
                            if (@explicit.Is1(out var v1))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, InferredType>(v1));
                                res = new OrType<MethodType, Type, Object, OrType, InferredType>(v1);
                            }
                            else if (@explicit.Is2(out var v2))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, InferredType>(v2));
                                res = new OrType<MethodType, Type, Object, OrType, InferredType>(v2);
                            }
                            else
                            {
                                throw new Exception();
                            }
                            return true;
                        }
                        else if (outerLookedUp.Is2(out var type))
                        {
                            var genericTypeKey = new GenericTypeKey(new OrType<MethodType, Type>(type), types);

                            if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                            {
                                res = res2;
                                return true;
                            }

                            // this is duplicate code - 94875369485
                            var map = new Dictionary<OrType<MethodType, Type, Object, OrType, InferredType>, OrType<MethodType, Type, Object, OrType, InferredType>>();
                            foreach (var (oldType, newType) in types.Zip(genericOverlays[new OrType<MethodType, Type>(type)], (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(new OrType<MethodType, Type>(type), new OrType<MethodType, Type>(new Type(this, $"generated-generic-{type.debugName}", type.Converter)), map);
                            if (@explicit.Is1(out var v1))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, InferredType>(v1));
                                res = new OrType<MethodType, Type, Object, OrType, InferredType>(v1);
                            }
                            else if (@explicit.Is2(out var v2))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, InferredType>(v2));
                                res = new OrType<MethodType, Type, Object, OrType, InferredType>(v2);
                            }
                            else
                            {
                                throw new Exception();
                            }
                            return true;
                        }
                        else if (outerLookedUp.Is3(out var @object))
                        {
                            res = new OrType<MethodType, Type, Object, OrType, InferredType>(@object);
                            return true;
                        }
                        else if (outerLookedUp.Is4(out var orType))
                        {
                            res = new OrType<MethodType, Type, Object, OrType, InferredType>(orType);
                            return true;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    if (TryLookUp(from, key, out var res2))
                    {
                        //:'(
                        res = res2;
                        return true;
                    }
                    else
                    {
                        res = default;
                        return false;
                    }
                }

                // [MaybeNullWhen(false)] grumble
                bool TryLookUp(IScope haveTypes, IKey key, out OrType<MethodType, Type, Object, OrType, InferredType>? result)
                {
                    while (true)
                    {
                        {
                            if (haveTypes.Types.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, InferredType>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.Objects.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, InferredType>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.OrTypes.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, InferredType>(res);
                                return true;
                            }
                        }
                        {
                            if (haveTypes.MethodTypes.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, InferredType>(res);
                                return true;
                            }
                        }
                        if (haveTypes is Type || haveTypes is MethodType)
                        {
                            OrType<MethodType, Type> orType;
                            if (haveTypes is Type type)
                            {
                                orType = new OrType<MethodType, Type>(type);
                            }
                            else if (haveTypes is MethodType methodType1)
                            {
                                orType = new OrType<MethodType, Type>(methodType1);
                            }
                            else
                            {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (genericOverlays.TryGetValue(orType, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                if (res.Is1(out var methodType))
                                {
                                    result = new OrType<MethodType, Type, Object, OrType, InferredType>(methodType);
                                    return true;
                                }
                                else if (res.Is2(out var innerType))
                                {
                                    result = new OrType<MethodType, Type, Object, OrType, InferredType>(innerType);
                                    return true;
                                }
                                else
                                {
                                    throw new Exception("uh oh! we hit a type we did not want");
                                }
                            }
                        }

                        if (haveTypes.Parent == null)
                        {
                            result = null;
                            return false;
                        }
                        haveTypes = haveTypes.Parent;
                    }
                }

                OrType<MethodType, Type> CopyTree(OrType<MethodType, Type> from, OrType<MethodType, Type> to, IReadOnlyDictionary<OrType<MethodType, Type, Object, OrType, InferredType>, OrType<MethodType, Type, Object, OrType, InferredType>> overlayed)
                {

                    var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();
                    if (from.Is1(out var from1) && to.Is1(out var to1))
                    {
                        Copy(from1, to1);
                    }
                    else if (from.Is2(out var from2) && to.Is2(out var to2))
                    {
                        Copy(from2, to2);
                    }
                    else
                    {
                        throw new Exception("or exception");
                    }

                    foreach (var pair in map)
                    {
                        if (pair.Key is IScope fromScope && fromScope.Parent != null)
                        {
                            if (to.Is1(out var method))
                            {
                                method.Parent = CopiedToOrSelf(fromScope.Parent);
                            }
                            else if (to.Is2(out var type))
                            {
                                type.Parent = CopiedToOrSelf(fromScope.Parent);
                            }
                        }
                    }

                    var oldAssignments = assignments.ToArray();
                    foreach (var pair in map)
                    {
                        if (pair.Key is ICanBeAssignedTo assignedToFrom && pair.Value is ICanBeAssignedTo assignedToTo)
                        {
                            foreach (var item in oldAssignments)
                            {
                                if (item.Item2 == assignedToFrom)
                                {
                                    assignments.Add((CopiedToOrSelf(item.Item1), assignedToTo));
                                }
                            }
                        }

                        if (pair.Value is ICanAssignFromMe assignFromFrom && pair.Value is ICanAssignFromMe assignFromTo)
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
                        if (pair.Key is ILookUpType lookUpFrom && pair.Value is ILookUpType lookUpTo)
                        {

                            if (lookUpFrom.TypeKey != null)
                            {
                                lookUpTo.TypeKey = lookUpFrom.TypeKey;
                            }

                            if (lookUpFrom.Context != null)
                            {
                                lookUpTo.Context = CopiedToOrSelf(lookUpFrom.Context);
                            }
                        }

                        if (pair.Key is OrType orFrom && pair.Value is OrType orTo)
                        {
                            // from should have been populated
                            Ors(orTo, CopiedToOrSelf(orFrom.Left ?? throw new NullReferenceException(nameof(orFrom.Left))), CopiedToOrSelf(orFrom.Right ?? throw new NullReferenceException(nameof(orFrom.Right))));
                        }


                        if (pair.Key is Method methodFrom && pair.Value is Method methodTo)
                        {
                            methodTo.Input = CopiedToOrSelf(methodFrom.Input ?? throw new NullReferenceException(nameof(methodFrom.Input)));
                            methodTo.Returns = CopiedToOrSelf(methodFrom.Returns ?? throw new NullReferenceException(nameof(methodFrom.Returns)));
                        }

                        if (pair.Key is MethodType methodFromType && pair.Value is MethodType methodToType)
                        {
                            methodToType.Input = CopiedToOrSelf(methodFromType.Input ?? throw new NullReferenceException(nameof(methodFromType.Input)));
                            methodToType.Returns = CopiedToOrSelf(methodFromType.Returns ?? throw new NullReferenceException(nameof(methodFromType.Returns)));
                        }

                        if (pair.Key is InferredType inferedFrom && pair.Value is InferredType inferedTo)
                        {
                            inferedTo.Input = CopiedToOrSelf(inferedFrom.Input ?? throw new NullReferenceException(nameof(inferedFrom.Input)));
                            inferedTo.Returns = CopiedToOrSelf(inferedFrom.Returns ?? throw new NullReferenceException(nameof(inferedFrom.Returns)));
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

                        if (innerFrom is IScope innerFromScope && innerTo is IScope innerScopeTo)
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
                                    var newValue = Copy(member.Value, new Member(this, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
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
                                    var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
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
                                    var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                    HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                                }
                            }
                        }

                        if (innerFrom is Type innerFromType && innerTo is Type innerTypeTo)
                        {

                            {
                                if (genericOverlays.TryGetValue(new OrType<MethodType, Type>(innerFromType), out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        if (overlayed.TryGetValue(type.Value, out var toType))
                                        {
                                            HasPlaceholderType(new OrType<MethodType, Type>(innerTypeTo), type.Key, toType);
                                        }
                                        else
                                        {
                                            HasPlaceholderType(new OrType<MethodType, Type>(innerTypeTo), type.Key, type.Value);
                                        }
                                    }
                                }
                            }
                        }

                        if (innerFrom is MethodType innerFromMethodType && innerTo is MethodType innerMethodTypeTo)
                        {

                            {
                                if (genericOverlays.TryGetValue(new OrType<MethodType, Type>(innerFromMethodType), out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        if (overlayed.TryGetValue(type.Value, out var toType))
                                        {
                                            HasPlaceholderType(new OrType<MethodType, Type>(innerMethodTypeTo), type.Key, toType);
                                        }
                                        else
                                        {
                                            HasPlaceholderType(new OrType<MethodType, Type>(innerMethodTypeTo), type.Key, type.Value);
                                        }
                                    }
                                }
                            }
                        }

                        if (innerFrom is IValue innerFromHopeful && innerTo is IValue innerToHopeful)
                        {

                            foreach (var possible in innerFromHopeful.HopefulMembers)
                            {
                                var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                HasHopefulMember(innerToHopeful, possible.Key, newValue);
                            }


                            if (innerFromHopeful.HopefulMethod is IIsDefinately<InferredType> infered)
                            {
                                var newValue = Copy(infered.Value, new InferredType(this, $"copied from {((TypeProblemNode)infered.Value).debugName}"));
                                innerToHopeful.HopefulMethod = Possibly.Is(newValue);
                            }
                        }

                        return innerTo;
                    }
                }


                //IHaveMembers GetType2(ILookUpType value)
                //{
                //    var res = lookUps[value];
                //    while (true)
                //    {
                //        if (res is IExplicitType explicitType && defersTo.TryGetValue(explicitType, out var nextRes))
                //        {
                //            res = nextRes;
                //        }
                //        else
                //        {
                //            return res;
                //        }
                //    }
                //}

                //[MaybeNullWhen(false)]
                static bool IsHasMembers(OrType<MethodType, Type, Object, OrType, InferredType> type, out IHaveMembers? haveMembers)
                {
                    if (type.Is1(out var v1))
                    {
                        haveMembers = default;
                        return false;
                    }
                    else if (type.Is2(out var v2))
                    {
                        haveMembers = v2;
                        return true;
                    }
                    else if (type.Is3(out var v3))
                    {
                        haveMembers = v3;
                        return true;
                    }
                    else if (type.Is4(out var v4))
                    {
                        haveMembers = v4;
                        return true;
                    }
                    else if (type.Is5(out var v5))
                    {
                        haveMembers = v5;
                        return true;
                    }

                    throw new Exception("boom!");
                }

                static bool IsNotInferedHasMembers(OrType<MethodType, Type, Object, OrType, InferredType> type, out IHaveMembers? haveMembers)
                {
                    if (type.Is1(out var v1))
                    {
                        haveMembers = default;
                        return false;
                    }
                    else if (type.Is2(out var v2))
                    {
                        haveMembers = v2;
                        return true;
                    }
                    else if (type.Is3(out var v3))
                    {
                        haveMembers = v3;
                        return true;
                    }
                    else if (type.Is4(out var v4))
                    {
                        haveMembers = v4;
                        return true;
                    }
                    else if (type.Is5(out var v5))
                    {
                        haveMembers = default;
                        return false;
                    }

                    throw new Exception("boom!");
                }


                OrType<MethodType, Type, Object, OrType, InferredType> GetType(ITypeProblemNode value)
                {
                    if (value is ILookUpType lookup)
                    {
                        // look up needs to be populated at this point
                        return lookup.LooksUp!;
                    }
                    if (value is MethodType methodType)
                    {
                        return new OrType<MethodType, Type, Object, OrType, InferredType>(methodType);
                    }

                    if (value is Type type)
                    {
                        return new OrType<MethodType, Type, Object, OrType, InferredType>(type);

                    }
                    if (value is Object @object)
                    {
                        return new OrType<MethodType, Type, Object, OrType, InferredType>(@object);
                    }
                    if (value is OrType orType)
                    {
                        return new OrType<MethodType, Type, Object, OrType, InferredType>(orType);
                    }

                    throw new Exception("flaming pile of piss");
                    // well, I guess I now know that we have a duality
                    // you either are a type, or you have a type
                    // 
                }

                // returns true if the target was modified 
                bool Flow(OrType<MethodType, Type, Object, OrType, InferredType> flowFrom, OrType<MethodType, Type, Object, OrType, InferredType> flowTo)
                {
                    var res = false;

                    if (flowFrom.Is1(out var fromMethod) && flowTo.Is1(out var toMethod))
                    {
                        var inFlowFrom = GetType(fromMethod.Input ?? throw new NullReferenceException(nameof(fromMethod.Input)));
                        var inFlowTo = GetType(toMethod.Input ?? throw new NullReferenceException(nameof(toMethod.Input)));

                        res |= Flow(inFlowFrom, inFlowTo);


                        var returnFlowFrom = GetType(fromMethod.Returns ?? throw new NullReferenceException(nameof(fromMethod.Returns)));
                        var retrunFlowTo = GetType(toMethod.Returns ?? throw new NullReferenceException(nameof(toMethod.Returns)));

                        res |= Flow(returnFlowFrom, retrunFlowTo);

                    }

                    if (IsHasMembers(flowFrom, out var fromType))
                    {

                        {
                            if (flowTo.Is2(out var deferredToHaveType))
                            {
                                foreach (var memberPair in GetMembers(new OrType<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (deferredToHaveType.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                    {
                                        res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
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
                            if (flowTo.Is3(out var deferredToObject))
                            {
                                foreach (var memberPair in GetMembers(new OrType<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (deferredToObject.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                    {
                                        res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
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
                            if (flowTo.Is5(out var deferredToInferred))
                            {

                                foreach (var memberPair in GetMembers(new OrType<IHaveMembers, OrType>(fromType!)))
                                {
                                    if (deferredToInferred.Members.TryGetValue(memberPair.Key, out var deferedToMember))
                                    {
                                        res |= Flow(GetType(memberPair.Value), GetType(deferedToMember));
                                    }
                                    else
                                    {
                                        var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
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
                                if (deferringInferred.Returns != null && deferredToMethod.Returns != null)
                                {
                                    var deferredToReturns = deferredToMethod.Returns;
                                    res |= Flow(GetType(deferringInferred.Returns), GetType(deferredToReturns));
                                }


                                if (deferringInferred.Input != null && deferredToMethod.Input != null)
                                {
                                    var deferredToInput = deferredToMethod.Input;
                                    res |= Flow(GetType(deferringInferred.Input), GetType(deferredToInput));
                                }
                            }
                        }

                        {
                            if (flowTo.Is5(out var deferredToInferred))
                            {
                                if (deferringInferred.Returns != null)
                                {
                                    if (deferredToInferred.Returns != null)
                                    {
                                        res |= Flow(GetType(deferringInferred.Returns), GetType(deferredToInferred.Returns));
                                    }
                                    else
                                    {
                                        deferredToInferred.Returns = deferringInferred.Returns;
                                        res = true;
                                    }
                                }

                                if (deferringInferred.Input != null)
                                {
                                    if (deferredToInferred.Input != null)
                                    {
                                        res |= Flow(GetType(deferringInferred.Input), GetType(deferredToInferred.Input));
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

                IReadOnlyDictionary<IKey, Member> GetMembers2(OrType<MethodType, Type, Object, OrType, InferredType> or)
                {
                    if (or.Is1(out var _))
                    {
                        return new Dictionary<IKey, Member>();
                    }
                    else if (or.Is2(out var v2))
                    {
                        return GetMembers(new OrType<IHaveMembers, OrType>(v2));
                    }
                    else if (or.Is3(out var v3))
                    {
                        return GetMembers(new OrType<IHaveMembers, OrType>(v3));
                    }
                    else if (or.Is4(out var v4))
                    {
                        return GetMembers(new OrType<IHaveMembers, OrType>(v4));

                    }
                    else if (or.Is5(out var v5))
                    {
                        return new Dictionary<IKey, Member>();
                    }
                    else
                    {
                        throw new Exception("unexpected");
                    }
                }

                IReadOnlyDictionary<IKey, Member> GetMembers(OrType<IHaveMembers, OrType> type)
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
                        var left = orType.Left ?? throw new NullReferenceException(nameof(orType.Left));
                        var right = orType.Right ?? throw new NullReferenceException(nameof(orType.Right));

                        var rightMembers = GetMembers2(GetType(right));
                        foreach (var leftMember in GetMembers2(GetType(left)))
                        {
                            if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                            {
                                // if they are the same type
                                if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
                                {
                                    var member = new Member(this, $"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}", leftMember.Value.Converter);
                                    member.LooksUp = GetType(rightMember);
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

            private static bool TryGetMember(IScope context, IKey key, [MaybeNullWhen(false)] out Member? member)
            {
                while (true)
                {
                    if (context.Members.TryGetValue(key, out member))
                    {
                        return true;
                    }
                    if (context.Parent == null)
                    {
                        member = default;
                        return false;
                    }
                    context = context.Parent;
                }
            }

            public TypeProblem2(IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> rootConverter, IConvertTo<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> moduleConverter)
            {

                Primitive = new Scope(this, "base", rootConverter);
                Dependency = CreateScope(Primitive, rootConverter);
                ModuleRoot = CreateObjectOrModule(CreateScope(Dependency, rootConverter), new ImplicitKey(Guid.NewGuid()), moduleConverter);

                CreateType(Primitive, new NameKey("number"), new PrimitiveTypeConverter(new NumberType()));
                CreateType(Primitive, new NameKey("string"), new PrimitiveTypeConverter(new StringType()));
                CreateType(Primitive, new NameKey("bool"), new PrimitiveTypeConverter(new BooleanType()));
                CreateType(Primitive, new NameKey("empty"), new PrimitiveTypeConverter(new EmptyType()));

                // shocked this works...
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(new NameKey("T1")), new GenericTypeParameterPlacholder(new NameKey("T2")) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] { new TypeAndConverter(new NameKey("T1"), new WeakTypeDefinitionConverter()), new TypeAndConverter(new NameKey("T2"), new WeakTypeDefinitionConverter()) };

                var res = new MethodType(
                    this,
                    $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.key.ToString())}",
                    new MethodTypeConverter());

                HasMethodType(Primitive, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.key.ToString()}", placeholder.converter);
                    HasPlaceholderType(new OrType<MethodType, Type>(res), placeholder.key, new OrType<MethodType, Type, Object, OrType, InferredType>(placeholderType));
                }

                var methodInputKey = new NameKey("method type input" + Guid.NewGuid());
                res.Input = CreateMember(res, methodInputKey, new NameKey("T1"), new WeakMemberDefinitionConverter(false, methodInputKey));
                res.Returns = CreateTransientMember(res, new NameKey("T2"));
                IsChildOf(Primitive, res);
            }

            private class GenericTypeKey
            {
                private readonly OrType<MethodType, Type> primary;
                private readonly OrType<MethodType, Type, Object, OrType, InferredType>[] parameters;

                public GenericTypeKey(OrType<MethodType, Type> primary, OrType<MethodType, Type, Object, OrType, InferredType>[] parameters)
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
