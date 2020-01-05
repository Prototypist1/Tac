using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Model;
using Tac.Semantic_Model;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    // having this be it's own project was a bad idea!
    // these are very expensive abstractions


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
        }

        internal interface ISetUpTypeProblem
        {
            // a =: x

            void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
            TypeProblem2.Value CreateValue(IScope scope, IKey typeKey, IConvertTo<TypeProblem2.Value, PlaceholderValue> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IKey typeKey, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeProblem2.TypeReference, IFrontendType> converter);
            TypeProblem2.Scope CreateScope(IScope parent, IConvertTo<TypeProblem2.Scope, OrType<WeakBlockDefinition, WeakScope>> converter);
            TypeProblem2.Type CreateType(IScope parent, IKey key, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Type CreateGenericType(IScope parent, IKey key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<TypeProblem2.Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter);
            TypeProblem2.Object CreateObject(IScope parent, IKey key, IConvertTo<TypeProblem2.Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter);
            TypeProblem2.Method CreateMethod(IScope parent, string inputName, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.Method CreateMethod(IScope parent, TypeProblem2.TypeReference inputType, TypeProblem2.TypeReference outputType, string inputName, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
            TypeProblem2.TransientMember GetReturns(IValue s);
            TypeProblem2.TransientMember GetReturns(IScope s);
            TypeProblem2.Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> converter);
            TypeProblem2.OrType CreateOrType(IScope s, IKey key, TypeProblem2.TypeReference setUpSideNode1, TypeProblem2.TypeReference setUpSideNode2, IConvertTo<TypeProblem2.OrType, WeakTypeOrOperation> converter);
            IKey GetKey(TypeProblem2.TypeReference type);
            TypeProblem2.Member GetInput(IValue method);

            void IsNumber(IScope parent, ICanAssignFromMe target);
            void IsString(IScope parent, ICanAssignFromMe target);
            void IsEmpty(IScope parent, ICanAssignFromMe target);
            void IsBool(IScope parent, ICanAssignFromMe target);
            TypeProblem2.Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter);
        }

        internal interface ITypeSolution
        {
            IBox<PlaceholderValue> GetValue(TypeProblem2.Value value);
            IBox<WeakMemberDefinition> GetMember(TypeProblem2.Member member);
            IBox<IFrontendType> GetTypeReference(TypeProblem2.TypeReference typeReference);
            IBox<OrType<WeakBlockDefinition, WeakScope>> GetScope(TypeProblem2.Scope scope);
            // when I ungeneric this it should probably have the box inside the or..
            IBox<OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> GetExplicitType(TypeProblem2.Type explicitType);
            IBox<OrType<WeakObjectDefinition, WeakModuleDefinition>> GetObject(TypeProblem2.Object @object);
            IBox<MethodType> GetMethodType(TypeProblem2.MethodType methodType);
            IBox<WeakTypeOrOperation> GetOrType(TypeProblem2.OrType orType);
            IBox<OrType<WeakMethodDefinition, WeakImplementationDefinition>> GetMethod(TypeProblem2.Method method);
            IReadOnlyList<TypeProblem2.Member> GetMembers(IHaveMembers from);
            OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType> GetType(ILookUpType from);
            (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from);
            TypeProblem2.TransientMember GetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType> from);
            TypeProblem2.Member GetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType> from);
        }

        internal class ConcreteSolutionType : IReadOnlyDictionary<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)>
        {
            private readonly IReadOnlyDictionary<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)> members;

            public ConcreteSolutionType(IReadOnlyDictionary<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)> members)
            {
                this.members = members ?? throw new ArgumentNullException(nameof(members));
            }

            public (bool, OrType<OrSolutionType, ConcreteSolutionType>) this[IKey key]
            {
                get
                {
                    return members[key];
                }
            }

            public IEnumerable<IKey> Keys
            {
                get
                {
                    return members.Keys;
                }
            }

            public IEnumerable<(bool, OrType<OrSolutionType, ConcreteSolutionType>)> Values
            {
                get
                {
                    return members.Values;
                }
            }

            public int Count
            {
                get
                {
                    return members.Count;
                }
            }

            public bool ContainsKey(IKey key)
            {
                return members.ContainsKey(key);
            }

            public IEnumerator<KeyValuePair<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)>> GetEnumerator()
            {
                return members.GetEnumerator();
            }

            public bool TryGetValue(IKey key, out (bool, OrType<OrSolutionType, ConcreteSolutionType>) value)
            {
                return members.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return members.GetEnumerator();
            }
        }

        internal class OrSolutionType
        {
            private readonly OrType<OrSolutionType, ConcreteSolutionType> left;
            private readonly OrType<OrSolutionType, ConcreteSolutionType> right;

            public OrSolutionType(OrType<OrSolutionType, ConcreteSolutionType> left, OrType<OrSolutionType, ConcreteSolutionType> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
            }
        }

        internal interface IConvertTo<in TConvertFrom, out TConvertsTo>
        {
            TConvertsTo Convert(ITypeSolution typeSolution, TConvertFrom from);
        }

        // 🤫 the power was in you all along
        internal class TypeSolution : ITypeSolution
        {
            private readonly IReadOnlyDictionary<IHaveMembers, IReadOnlyList<TypeProblem2.Member>> members;
            private readonly IReadOnlyDictionary<ILookUpType, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType>> map;
            private readonly IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts;
            private readonly IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType>, TypeProblem2.Member> methodIn;
            private readonly IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType>, TypeProblem2.TransientMember> methodOut;

            public TypeSolution(
                IReadOnlyDictionary<ILookUpType, OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType>> map,
                IReadOnlyDictionary<IHaveMembers, IReadOnlyList<TypeProblem2.Member>> members,
                IReadOnlyDictionary<TypeProblem2.OrType, (TypeProblem2.TypeReference, TypeProblem2.TypeReference)> orTypeElememts,
                IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType>, TypeProblem2.Member> methodIn,
                IReadOnlyDictionary<OrType<TypeProblem2.Method, TypeProblem2.MethodType>, TypeProblem2.TransientMember> methodOut)
            {
                this.map = map ?? throw new ArgumentNullException(nameof(map));
                this.members = members ?? throw new ArgumentNullException(nameof(members));
                this.orTypeElememts = orTypeElememts ?? throw new ArgumentNullException(nameof(orTypeElememts));
                this.methodIn = methodIn ?? throw new ArgumentNullException(nameof(methodIn));
                this.methodOut = methodOut ?? throw new ArgumentNullException(nameof(methodOut));
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

            private readonly Dictionary<TypeProblem2.Scope, IBox<OrType<WeakBlockDefinition, WeakScope>>> cacheScope = new Dictionary<TypeProblem2.Scope, IBox<OrType<WeakBlockDefinition, WeakScope>>>();
            public IBox<OrType<WeakBlockDefinition, WeakScope>> GetScope(TypeProblem2.Scope scope)
            {
                if (!cacheScope.ContainsKey(scope))
                {
                    var box = new Box<OrType<WeakBlockDefinition, WeakScope>>();
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


            public IReadOnlyList<TypeProblem2.Member> GetMembers(IHaveMembers from)
            {
                if (!members.ContainsKey(from))
                {
                    return new List<TypeProblem2.Member>();
                }
                return members[from];
            }

            public OrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType> GetType(ILookUpType from)
            {
                return map[from];
            }

            public (TypeProblem2.TypeReference, TypeProblem2.TypeReference) GetOrTypeElements(TypeProblem2.OrType from)
            {
                return orTypeElememts[from];
            }

            public TypeProblem2.TransientMember GetResultMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType> from)
            {
                return methodOut[from];
            }

            public TypeProblem2.Member GetInputMember(OrType<TypeProblem2.Method, TypeProblem2.MethodType> from)
            {
                return methodIn[from];
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

        internal interface IHaveMembers : ITypeProblemNode { }
        internal interface ILookUpType : ITypeProblemNode { }

        internal interface ICanAssignFromMe : ITypeProblemNode, ILookUpType { }
        internal interface ICanBeAssignedTo : ITypeProblemNode, ILookUpType { }
        internal interface IValue : ITypeProblemNode, ILookUpType, ICanAssignFromMe { }
        //public interface Member :  IValue, ILookUpType, ICanBeAssignedTo {bool IsReadonly { get; }}
        internal interface IExplicitType : IHaveMembers, IScope { }
        internal interface IScope : IHaveMembers { }
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
            }
            public class Value : TypeProblemNode<Value, PlaceholderValue>, IValue
            {
                public Value(TypeProblem2 problem, string debugName, IConvertTo<Value, PlaceholderValue> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Member : TypeProblemNode<Member, WeakMemberDefinition>, IMember
            {
                public Member(TypeProblem2 problem, string debugName, IConvertTo<Member, WeakMemberDefinition> converter) : base(problem, debugName, converter)
                {
                }
            }

            public class TransientMember : TypeProblemNode, IMember
            {
                public TransientMember(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }
            }

            public class Type : TypeProblemNode<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>, IExplicitType
            {
                public Type(TypeProblem2 problem, string debugName, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter) : base(problem, debugName, converter)
                {
                }
            }

            public class DummyType : TypeProblemNode
            {
                public DummyType(TypeProblem2 problem, string debugName) : base(problem, debugName)
                {
                }
            }

            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class MethodType : TypeProblemNode<MethodType, SyntaxModel.Elements.AtomicTypes.MethodType>, IHaveInputAndOutput, IHaveMembers, IScope
            {
                public MethodType(TypeProblem2 problem, string debugName, IConvertTo<MethodType, SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }
            }

            public class InferredMethodType : MethodType
            {
                public InferredMethodType(TypeProblem2 problem, string debugName, IConvertTo<MethodType, SyntaxModel.Elements.AtomicTypes.MethodType> converter) : base(problem, debugName, converter)
                {
                }
            }


            public class OrType : TypeProblemNode<OrType, WeakTypeOrOperation>, IHaveMembers
            {
                public OrType(TypeProblem2 problem, string debugName, IConvertTo<OrType, WeakTypeOrOperation> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class InferredType : Type
            {
                public InferredType(TypeProblem2 problem, string debugName, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Scope : TypeProblemNode<Scope, OrType<WeakBlockDefinition, WeakScope>>, IScope
            {
                public Scope(TypeProblem2 problem, string debugName, IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope>> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Object : TypeProblemNode<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>>, IExplicitType
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter) : base(problem, debugName, converter)
                {
                }
            }
            // methods don't really have members in the way other things do
            // they have members while they are executing
            // but you can't really access their members
            public class Method : TypeProblemNode<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>>, IHaveMembers, IScope, IHaveInputAndOutput
            {
                public Method(TypeProblem2 problem, string debugName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter) : base(problem, debugName, converter)
                {
                }
            }


            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            public IScope Base { get; }
            public IScope Root { get; }

            // relationships
            private readonly Dictionary<IScope, IScope> kidParent = new Dictionary<IScope, IScope>();

            //hopeful methods 
            private readonly Dictionary<IValue, InferredMethodType> hopefulMethods = new Dictionary<IValue, InferredMethodType>();

            // legit methods
            private readonly Dictionary<OrType<Method, MethodType>, TransientMember> methodReturns = new Dictionary<OrType<Method, MethodType>, TransientMember>();
            private readonly Dictionary<OrType<Method, MethodType>, Member> methodInputs = new Dictionary<OrType<Method, MethodType>, Member>();

            private readonly Dictionary<IScope, List<Value>> values = new Dictionary<IScope, List<Value>>();
            private readonly Dictionary<IHaveMembers, Dictionary<IKey, Member>> members = new Dictionary<IHaveMembers, Dictionary<IKey, Member>>();
            private readonly Dictionary<IScope, List<TransientMember>> transientMembers = new Dictionary<IScope, List<TransientMember>>();
            private readonly Dictionary<IScope, Dictionary<IKey, Method>> methods = new Dictionary<IScope, Dictionary<IKey, Method>>();
            private readonly Dictionary<IScope, List<TypeReference>> refs = new Dictionary<IScope, List<TypeReference>>();
            private readonly Dictionary<IScope, Dictionary<IKey, OrType>> orTypes = new Dictionary<IScope, Dictionary<IKey, OrType>>();
            private readonly Dictionary<IScope, Dictionary<IKey, Type>> types = new Dictionary<IScope, Dictionary<IKey, Type>>();
            private readonly Dictionary<IScope, Dictionary<IKey, MethodType>> methodTypes = new Dictionary<IScope, Dictionary<IKey, MethodType>>();
            private readonly Dictionary<IScope, Dictionary<IKey, Object>> objects = new Dictionary<IScope, Dictionary<IKey, Object>>();

            // this holds real overlays 
            // the left hand side is the generic
            // the right hand side is the generic type parameter
            // it hold the placeholder and the realized type
            private readonly Dictionary<OrType<MethodType, Type>, Dictionary<IKey, OrType<MethodType, Type, Object, OrType, DummyType>>> genericOverlays = new Dictionary<OrType<MethodType, Type>, Dictionary<IKey, OrType<MethodType, Type, Object, OrType, DummyType>>>();

            private readonly Dictionary<OrType, (TypeReference, TypeReference)> orTypeComponents = new Dictionary<OrType, (TypeReference, TypeReference)>();

            private readonly Dictionary<IScope, Dictionary<IKey, Member>> possibleMembers = new Dictionary<IScope, Dictionary<IKey, Member>>();
            private readonly Dictionary<IValue, Dictionary<IKey, Member>> hopefulMembers = new Dictionary<IValue, Dictionary<IKey, Member>>();
            private List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();

            //private readonly List<(ICanAssignFromMe, ILookUpType)> calls = new List<(ICanAssignFromMe, ILookUpType)>();

            // members
            private readonly Dictionary<ILookUpType, IKey> lookUpTypeKey = new Dictionary<ILookUpType, IKey>();
            private readonly Dictionary<ILookUpType, IScope> lookUpTypeContext = new Dictionary<ILookUpType, IScope>();


            #region Building APIs

            public void IsChildOf(IScope parent, IScope kid)
            {
                kidParent.Add(kid, parent);
            }
            public void HasValue(IScope parent, Value value)
            {
                if (!values.ContainsKey(parent))
                {
                    values.Add(parent, new List<Value>());
                }
                values[parent].Add(value);
            }
            public void HasReference(IScope parent, TypeReference reference)
            {
                if (!refs.ContainsKey(parent))
                {
                    refs.Add(parent, new List<TypeReference>());
                }
                refs[parent].Add(reference);
            }
            public void HasType(IScope parent, IKey key, Type type)
            {
                if (!types.ContainsKey(parent))
                {
                    types.Add(parent, new Dictionary<IKey, Type>());
                }
                types[parent].Add(key, type);
            }
            public void HasMethodType(IScope parent, IKey key, MethodType type)
            {
                if (!methodTypes.ContainsKey(parent))
                {
                    methodTypes.Add(parent, new Dictionary<IKey, MethodType>());
                }
                methodTypes[parent].Add(key, type);
            }
            public void HasObject(IScope parent, IKey key, Object @object)
            {
                if (!objects.ContainsKey(parent))
                {
                    objects.Add(parent, new Dictionary<IKey, Object>());
                }
                objects[parent].Add(key, @object);
            }

            public void HasPlaceholderType(OrType<MethodType, Type> parent, IKey key, OrType<MethodType, Type, Object, OrType, DummyType> type)
            {
                if (!genericOverlays.ContainsKey(parent))
                {
                    genericOverlays.Add(parent, new Dictionary<IKey, OrType<MethodType, Type, Object, OrType, DummyType>>());
                }
                genericOverlays[parent].Add(key, type);
            }
            public void HasMember(IHaveMembers parent, IKey key, Member member)
            {
                if (!members.ContainsKey(parent))
                {
                    members.Add(parent, new Dictionary<IKey, Member>());
                }
                members[parent].Add(key, member);
            }
            public void HasMethod(IScope parent, IKey key, Method method)
            {
                if (!methods.ContainsKey(parent))
                {
                    methods.Add(parent, new Dictionary<IKey, Method>());
                }
                methods[parent].Add(key, method);
            }


            public void HasTransientMember(IScope parent, TransientMember member)
            {
                if (!transientMembers.ContainsKey(parent))
                {
                    transientMembers.Add(parent, new List<TransientMember>());
                }
                transientMembers[parent].Add(member);
            }
            public void HasMembersPossiblyOnParent(IScope parent, IKey key, Member member)
            {
                if (!possibleMembers.ContainsKey(parent))
                {
                    possibleMembers.Add(parent, new Dictionary<IKey, Member>());
                }
                possibleMembers[parent].TryAdd(key, member);
            }
            public void HasHopefulMember(IValue parent, IKey key, Member member)
            {

                if (!hopefulMembers.ContainsKey(parent))
                {
                    hopefulMembers.Add(parent, new Dictionary<IKey, Member>());
                }
                hopefulMembers[parent].TryAdd(key, member);
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
                var res = new Value(this, typeKey.ToString(), converter);
                HasValue(scope, res);
                lookUpTypeContext[res] = scope;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IKey typeKey, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString(), converter);
                HasMember(scope, key, res);
                lookUpTypeContext[res] = scope;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString(), converter);
                HasMember(scope, key, res);
                lookUpTypeContext[res] = scope;
                return res;
            }

            public Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                if (possibleMembers.TryGetValue(scope, out var members) && members.TryGetValue(key, out var res1))
                {
                    return res1;
                }
                var res = new Member(this, key.ToString(), converter);
                HasMembersPossiblyOnParent(scope, key, res);
                lookUpTypeContext[res] = scope;
                return res;
            }

            public TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TypeReference, IFrontendType> converter)
            {
                var res = new TypeReference(this, typeKey.ToString(), converter);
                HasReference(context, res);
                lookUpTypeContext[res] = context;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Scope CreateScope(IScope parent, IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope>> converter)
            {
                var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).debugName}", converter);
                IsChildOf(parent, res);
                return res;
            }

            public Type CreateType(IScope parent, IKey key, IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> converter)
            {
                var res = new Type(this, key.ToString(), converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
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
                    HasPlaceholderType(new OrType<MethodType, Type>(res), placeholder.key, new OrType<MethodType, Type, Object, OrType, DummyType>(placeholderType));
                }
                return res;
            }

            public Object CreateObject(IScope parent, IKey key, IConvertTo<Object, OrType<WeakObjectDefinition, WeakModuleDefinition>> converter)
            {
                var res = new Object(this, key.ToString(), converter);
                IsChildOf(parent, res);
                HasObject(parent, key, res);
                return res;
            }

            public Method CreateMethod(IScope parent, string inputName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                var returns = CreateTransientMember(res, new ImplicitKey(Guid.NewGuid()));
                methodReturns[new OrType<Method, MethodType>(res)] = returns;
                var input = CreateMember(res, new NameKey(inputName), inputConverter);
                methodInputs[new OrType<Method, MethodType>(res)] = input;
                return res;
            }


            public Method CreateMethod(IScope parent, TypeReference inputType, TypeReference outputType, string inputName, IConvertTo<Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<Member, WeakMemberDefinition> inputConverter)
            {

                var res = new Method(this, $"method{{inputName:{inputName},inputType:{inputType.debugName},outputType:{outputType.debugName}}}", converter);
                IsChildOf(parent, res);
                HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                var returns = lookUpTypeKey.TryGetValue(inputType, out var outkey) ? CreateTransientMember(res, outkey) : CreateTransientMember(res, new ImplicitKey(Guid.NewGuid()));
                methodReturns[new OrType<Method, MethodType>(res)] = returns;
                if (lookUpTypeKey.TryGetValue(inputType, out var inkey))
                {
                    methodInputs[new OrType<Method, MethodType>(res)] = CreateMember(res, new NameKey(inputName), inkey, inputConverter);
                }
                else
                {
                    methodInputs[new OrType<Method, MethodType>(res)] = CreateMember(res, new NameKey(inputName), inputConverter);
                }
                return res;
            }


            public Member CreateHopefulMember(IValue scope, IKey key, IConvertTo<Member, WeakMemberDefinition> converter)
            {
                var res = new Member(this, key.ToString(), converter);
                HasHopefulMember(scope, key, res);
                return res;
            }


            public OrType CreateOrType(IScope s, IKey key, TypeReference setUpSideNode1, TypeReference setUpSideNode2, IConvertTo<OrType, WeakTypeOrOperation> converter)
            {
                var res = new OrType(this, $"{((TypeProblemNode)setUpSideNode1).debugName} || {((TypeProblemNode)setUpSideNode2).debugName}", converter);
                Ors(res, setUpSideNode1, setUpSideNode2);
                HasOrType(s, key, res);

                return res;

            }


            private void Ors(OrType orType, TypeReference a, TypeReference b)
            {
                orTypeComponents[orType] = (a, b);
            }

            private void HasOrType(IScope scope, IKey kay, OrType orType1)
            {
                if (!orTypes.ContainsKey(scope))
                {
                    orTypes[scope] = new Dictionary<IKey, OrType>();
                }
                orTypes[scope][kay] = orType1;
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
                lookUpTypeContext[res] = parent;
                return res;
            }


            private TransientMember CreateTransientMember(IScope parent, IKey typeKey)
            {
                var res = new TransientMember(this, "");
                HasTransientMember(parent, res);
                lookUpTypeContext[res] = parent;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            // ok
            // so... this can not be converted
            // it is not a real method
            // it is just something of type method
            // it is really just a type
            //
            public Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<TypeProblem2.Method, OrType<WeakMethodDefinition, WeakImplementationDefinition>> converter, IConvertTo<TypeProblem2.Member, WeakMemberDefinition> inputConverter)
            {
                var key = new ImplicitKey(Guid.NewGuid());
                var thing = CreateTransientMember(parent, key);
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
                else
                {
                    return GetReturns(kidParent[s]);
                }
            }

            internal TransientMember GetReturns(Method method)
            {
                return methodReturns[new OrType<Method, MethodType>(method)];
            }



            public TransientMember GetReturns(IValue value)
            {
                if (hopefulMethods.TryGetValue(value, out var res))
                {
                    return methodReturns[new OrType<Method, MethodType>(res)];
                }
                else
                {
                    var inferredMethodType = new InferredMethodType(this, "zzzz", new MethodTypeConverter());
                    hopefulMethods[value] = inferredMethodType;

                    var methodInputKey = new ImplicitKey(Guid.NewGuid());
                    methodInputs[new OrType<Method, MethodType>(inferredMethodType)] = CreateMember(inferredMethodType, methodInputKey, new WeakMemberDefinitionConverter(false, methodInputKey)); ;
                    var returns = CreateTransientMember(inferredMethodType); ;
                    methodReturns[new OrType<Method, MethodType>(inferredMethodType)] = returns;

                    return returns;
                }
            }

            public Member GetInput(IValue value)
            {
                if (hopefulMethods.TryGetValue(value, out var res))
                {
                    return methodInputs[new OrType<Method, MethodType>(res)];
                }
                else {
                    var inferredMethodType = new InferredMethodType(this, "zzzz", new MethodTypeConverter());
                    hopefulMethods[value] = inferredMethodType;

                    var methodInputKey = new ImplicitKey(Guid.NewGuid());
                    var input = CreateMember(inferredMethodType, methodInputKey, new WeakMemberDefinitionConverter(false, methodInputKey));
                    methodInputs[new OrType<Method, MethodType>(inferredMethodType)] = input;
                    methodReturns[new OrType<Method, MethodType>(inferredMethodType)] = CreateTransientMember(inferredMethodType);

                    return input;
                }
            }

            public IKey GetKey(TypeProblem2.TypeReference type)
            {
                return lookUpTypeKey[type];
            }

            // pretty sure it is not safe to solve more than once 
            public ITypeSolution Solve(IConvertTo<Type, OrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>> inferedTypeConvert)
            {
                // create types for everything 
                var lookUps = new Dictionary<ILookUpType, OrType<MethodType, Type, Object, OrType, DummyType>>();

                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => !lookUpTypeKey.ContainsKey(x)))
                {
                    var type = new DummyType(this, $"for {((TypeProblemNode)node).debugName}");
                    lookUps[node] = new OrType<MethodType, Type, Object, OrType, DummyType>(type);
                }


                // generics register themsleves 
                var realizedGeneric = new Dictionary<GenericTypeKey, OrType<MethodType, Type, Object, OrType, DummyType>>();

                var localScopes = typeProblemNodes.OfType<MethodType>().Select(x => new OrType<MethodType, Type>(x)).Where(x=>genericOverlays.ContainsKey(x));
                var localMethodTypes = typeProblemNodes.OfType<Type>().Select(x => new OrType<MethodType, Type>(x)).Where(x => genericOverlays.ContainsKey(x));

                foreach (var node in new[] { localScopes, localMethodTypes }.SelectMany(x => x))
                {
                    var key = new GenericTypeKey(node, genericOverlays[node].Values.ToArray());
                    if (node.Is1(out var v1))
                    {
                        realizedGeneric[key] = new OrType<MethodType, Type, Object, OrType, DummyType>(v1);
                    }
                    else if (node.Is2(out var v2))
                    {
                        realizedGeneric[key] = new OrType<MethodType, Type, Object, OrType, DummyType>(v2);
                    }
                    else
                    {
                        throw new Exception("you are outside of the or type");
                    }
                }

                toLookUp = typeProblemNodes.OfType<ILookUpType>().Except(lookUps.Keys).ToArray();

                // overlay generics
                while (toLookUp.Any())
                {
                    foreach (var node in toLookUp)
                    {
                        LookUpOrOverlayOrThrow(node);
                    }
                    toLookUp = typeProblemNodes.OfType<ILookUpType>().Except(lookUps.Keys).ToArray();
                }

                // members that might be on parents 

                //var defersTo = new Dictionary<IHaveMembers, IHaveMembers>();

                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();

                foreach (var item in possibleMembers)
                {
                    foreach (var pair in item.Value)
                    {
                        if (TryGetMember(item.Key, pair.Key, out var member))
                        {
                            TryMerge(pair.Value, member);
                        }
                        else
                        {
                            HasMember(item.Key, pair.Key, pair.Value);
                        }
                    }
                }

                // hopeful members and methods are a little rough around the edges
                // they are very similar yet implemented differently 

                // hopeful members 
                foreach (var hopeful in hopefulMembers)
                {
                    foreach (var pair in hopeful.Value)
                    {
                        if (GetMembers2(GetType(hopeful.Key)).TryGetValue(pair.Key, out var member))
                        {
                            TryMerge(pair.Value, member);
                        }
                        else if (GetType(hopeful.Key).Is2(out var haveMembers) && haveMembers is InferredType inferred)
                        {
                            HasMember(inferred, pair.Key, pair.Value);
                        }
                        else if (GetType(hopeful.Key).Is5(out var dummy))
                        {
                            inferred = new InferredType(this, $"for {((TypeProblemNode)hopeful.Key).debugName}",inferedTypeConvert);
                            lookUps[hopeful.Key] = new OrType<MethodType, Type, Object, OrType, DummyType>(inferred);
                            HasMember(inferred, pair.Key, pair.Value);
                        }
                        else
                        {
                            throw new Exception("member could not be handled ");
                        }
                    }
                }

                // hopeful methods 
                foreach (var hopefulMethod in hopefulMethods)
                {
                    var type = GetType(hopefulMethod.Key);
                    if (type.Is1(out var methodType))
                    {
                        if (!methodType.Equals(hopefulMethod.Value))
                        {
                            lookUps[hopefulMethod.Key] = new OrType<MethodType, Type, Object, OrType, DummyType>(methodType);

                            var defererReturns = methodReturns[new OrType<Method, MethodType>(hopefulMethod.Value)];
                            var deferredToReturns = methodReturns[new OrType<Method, MethodType>(methodType)];
                            TryMerge(defererReturns, deferredToReturns);

                            var defererInput = methodInputs[new OrType<Method, MethodType>(hopefulMethod.Value)];
                            var deferredToInput = methodInputs[new OrType<Method, MethodType>(methodType)];
                            TryMerge(defererInput, deferredToInput);
                        }
                    }
                    else if (type.Is5(out var dummy))
                    {
                        lookUps[hopefulMethod.Key] = new OrType<MethodType, Type, Object, OrType, DummyType>(hopefulMethod.Value);
                    }
                    else {
                        throw new Exception("no good!");
                    }
                }


                // we don't want any dummies in our assignments 

                foreach (var group in assignments.GroupBy(x => x.Item1))
                {
                    if (group.All(x => lookUps[x.Item2].Is1(out var _)))
                    {
                        lookUps[group.Key] = new OrType<MethodType, Type, Object, OrType, DummyType>( new InferredMethodType(this, "last minute method", new MethodTypeConverter()));
                    }
                    else if (group.All(x => !lookUps[x.Item2].Is1(out var _)))
                    {
                        lookUps[group.Key] = new OrType<MethodType, Type, Object, OrType, DummyType>(new InferredType(this, "last minute type", inferedTypeConvert));
                    }
                    else {
                        throw new Exception("uhhh so it is a method and not a method 🤔");
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
                        var toType = lookUps[to];
                        var fromType = lookUps[from];
                        go |= Flow(fromType,toType);
                        
                    }
                } while (go);

                // we dont flow downstream

                return new TypeSolution(lookUps.ToDictionary(x=>x.Key, x=> {
                    if (x.Value.Is1(out var v1))
                    {
                        return new OrType<MethodType, Type, Object, OrType>(v1);
                    }
                    else if (x.Value.Is2(out var v2))
                    {
                        return new OrType<MethodType, Type, Object, OrType>(v2);
                    }
                    else if (x.Value.Is3(out var v3))
                    {
                        return new OrType<MethodType, Type, Object, OrType>(v3);
                    }
                    else if (x.Value.Is4(out var v4))
                    {
                        return new OrType<MethodType, Type, Object, OrType>(v4);
                    }
                    else if (x.Value.Is5(out var v5))
                    {
                        // or maybe I should throw??
                        // not really sure
                        return new OrType<MethodType, Type, Object, OrType>(new InferredType(this,"shrug",inferedTypeConvert));
                    }
                    else {
                        throw new Exception("grumble");
                    }
                }), members.ToDictionary(x => x.Key, x => (IReadOnlyList<Member>)x.Value.Select(y => y.Value).ToArray()), orTypeComponents, methodInputs, methodReturns);

                #region Helpers

                void TryMerge(IValue deferer, IValue deferredTo)
                {

                    var defererType = GetType(deferer);
                    if (defererType.Is2(out var temp) && (temp is InferredType).Not())
                    {
                        throw new Exception("we can't merge that!");
                    }
                    var deferredToType = GetType(deferredTo);

                    lookUps[deferer] = deferredToType;

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

                    if (defererType.Is2(out var defererHaveMembers) && deferredToType.Is2(out var deferredToHaveType))
                    {
                        foreach (var memberPair in GetMembers(defererHaveMembers))
                        {
                            if (!members.ContainsKey(deferredToHaveType))
                            {
                                members[deferredToHaveType] = new Dictionary<IKey, Member>();
                            }
                            var dict = members[deferredToHaveType];
                            if (dict.TryGetValue(memberPair.Key, out var deferedToMember))
                            {
                                TryMerge(memberPair.Value, deferedToMember);
                            }
                            else
                            {
                                var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                HasMember(deferredToHaveType, memberPair.Key, newValue);
                                lookUps[newValue] = lookUps[memberPair.Value];
                            }
                        }
                    }
                    else if (defererType.Is1(out var defererMethod) && deferredToType.Is1(out var deferredToMethod))
                    {
                        var defererReturns = methodReturns[new OrType<Method, MethodType>(defererMethod)];
                        var deferredToReturns = methodReturns[new OrType<Method, MethodType>(deferredToMethod)];
                        TryMerge(defererReturns, deferredToReturns);


                        var defererInput = methodInputs[new OrType<Method, MethodType>(defererMethod)];
                        var deferredToInput = methodInputs[new OrType<Method, MethodType>(deferredToMethod)];
                        TryMerge(defererInput, deferredToInput);
                    }
                    else if (defererType.Is5(out var _)) { 
                        // do nothing 
                    }
                    else
                    {
                        throw new Exception("or type has an unexpect type");
                    }
                }

                OrType<MethodType, Type, Object, OrType, DummyType> LookUpOrOverlayOrThrow(ILookUpType node)
                {
                    {
                        if (lookUps.TryGetValue(node, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var from = lookUpTypeContext[node];
                        var key = lookUpTypeKey[node];
                        if (!TryLookUpOrOverlay(from, key, out var res))
                        {
                            throw new Exception("could not find type");
                        }
                        lookUps[node] = res;
                        return res;
                    }
                }



                OrType<MethodType, Type, Object, OrType, DummyType> LookUpOrOverlayOrThrow2(IScope from, IKey key)
                {
                    if (!TryLookUpOrOverlay(from, key, out var res))
                    {
                        throw new Exception("could not find type");
                    }
                    return res;
                }

                bool TryLookUpOrOverlay(IScope from, IKey key, out OrType<MethodType, Type, Object, OrType, DummyType> res)
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
                            var map = new Dictionary<OrType<MethodType, Type, Object, OrType, DummyType>, OrType<MethodType, Type, Object, OrType, DummyType>>();
                            foreach (var (oldType, newType) in types.Zip(genericOverlays[new OrType<MethodType, Type>(method)], (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(new OrType<MethodType, Type>(method), new OrType<MethodType, Type>(new MethodType(this, $"generated-generic-{method.debugName}", method.Converter)), map);
                            if (@explicit.Is1(out var v1))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, DummyType>(v1));
                                res = new OrType<MethodType, Type, Object, OrType, DummyType>(v1);
                            }
                            else if (@explicit.Is2(out var v2))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, DummyType>(v2));
                                res = new OrType<MethodType, Type, Object, OrType, DummyType>(v2);
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
                            var map = new Dictionary<OrType<MethodType, Type, Object, OrType, DummyType>, OrType<MethodType, Type, Object, OrType, DummyType>>();
                            foreach (var (oldType, newType) in types.Zip(genericOverlays[new OrType<MethodType, Type>(type)], (x, y) => (y.Value, x)))
                            {
                                map[oldType] = newType;
                            }

                            var @explicit = CopyTree(new OrType<MethodType, Type>(type), new OrType<MethodType, Type>(new Type(this, $"generated-generic-{type.debugName}", type.Converter)), map);
                            if (@explicit.Is1(out var v1))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, DummyType>(v1));
                                res = new OrType<MethodType, Type, Object, OrType, DummyType>(v1);
                            }
                            else if (@explicit.Is2(out var v2))
                            {
                                realizedGeneric.Add(genericTypeKey, new OrType<MethodType, Type, Object, OrType, DummyType>(v2));
                                res = new OrType<MethodType, Type, Object, OrType, DummyType>(v2);
                            }
                            else
                            {
                                throw new Exception();
                            }
                            return true;
                        }
                        else if (outerLookedUp.Is3(out var @object))
                        {
                            res = new OrType<MethodType, Type, Object, OrType, DummyType>(@object);
                            return true;
                        }
                        else if (outerLookedUp.Is4(out var orType))
                        {
                            res = new OrType<MethodType, Type, Object, OrType, DummyType>(orType);
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

                bool TryLookUp(IScope haveTypes, IKey key, out OrType<MethodType, Type, Object, OrType, DummyType> result)
                {
                    while (true)
                    {
                        {
                            if (types.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, DummyType>(res);
                                return true;
                            }
                        }
                        {
                            if (objects.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, DummyType>(res);
                                return true;
                            }
                        }
                        {
                            if (orTypes.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, DummyType>(res);
                                return true;
                            }
                        }
                        {
                            if (methodTypes.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = new OrType<MethodType, Type, Object, OrType, DummyType>(res);
                                return true;
                            }
                        }
                        if (haveTypes is Type || haveTypes is MethodType)
                        {
                            OrType<MethodType, Type> orType;
                            if (haveTypes is Type type) {
                                orType = new OrType<MethodType, Type>(type);
                            } else if (haveTypes is MethodType methodType1) {
                                orType = new OrType<MethodType, Type>(methodType1);
                            } else {
                                throw new Exception("💩💩💩💩💩");
                            }

                            if (genericOverlays.TryGetValue(orType, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                if (res.Is1(out var methodType))
                                {
                                    result = new OrType<MethodType, Type, Object, OrType, DummyType>(methodType);
                                    return true;
                                }
                                else if (res.Is2(out var innerType))
                                {
                                    result = new OrType<MethodType, Type, Object, OrType, DummyType>(innerType);
                                    return true;
                                }
                                else
                                {
                                    throw new Exception("uh oh! we hit a type we did not want");
                                }
                            }
                        }

                        if (!kidParent.TryGetValue(haveTypes, out haveTypes))
                        {
                            result = null;
                            return false;
                        }
                    }
                }

                OrType<MethodType, Type> CopyTree(OrType<MethodType, Type> from, OrType<MethodType, Type> to, IReadOnlyDictionary<OrType<MethodType, Type, Object, OrType, DummyType>, OrType<MethodType, Type, Object, OrType, DummyType>> overlayed)
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
                        if (pair.Key is IScope fromScope && to is IScope toScope)
                        {
                            kidParent[toScope] = CopiedToOrSelf(kidParent[fromScope]);
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

                            if (lookUpTypeKey.TryGetValue(lookUpFrom, out var key))
                            {
                                lookUpTypeKey.Add(lookUpTo, key);
                            }

                            if (lookUpTypeContext.TryGetValue(lookUpFrom, out var context))
                            {
                                lookUpTypeContext.Add(lookUpTo, CopiedToOrSelf(context));
                            }
                        }

                        if (pair.Key is OrType orFrom && pair.Value is OrType orTo)
                        {
                            Ors(orTo, CopiedToOrSelf(orTypeComponents[orFrom].Item1), CopiedToOrSelf(orTypeComponents[orFrom].Item2));
                        }


                        if (pair.Key is Method methodFrom && pair.Value is Method methodTo)
                        {
                            methodInputs[new OrType<Method, MethodType>(methodTo)] = CopiedToOrSelf(methodInputs[new OrType<Method, MethodType>(methodFrom)]);
                            methodReturns[new OrType<Method, MethodType>(methodTo)] = CopiedToOrSelf(methodReturns[new OrType<Method, MethodType>(methodFrom)]);
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
                                if (values.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var item in dict)
                                    {
                                        var newValue = Copy(item, new Value(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                        HasValue(innerScopeTo, newValue);
                                    }
                                }
                            }

                            {
                                if (refs.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var item in dict)
                                    {
                                        var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}", item.Converter));
                                        HasReference(innerScopeTo, newValue);
                                    }
                                }
                            }

                            {
                                if (members.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var member in dict)
                                    {
                                        var newValue = Copy(member.Value, new Member(this, $"copied from {((TypeProblemNode)member.Value).debugName}", member.Value.Converter));
                                        HasMember(innerScopeTo, member.Key, newValue);
                                    }
                                }
                            }

                            {
                                if (transientMembers.TryGetValue(innerFromScope, out var list))
                                {
                                    foreach (var member in list)
                                    {
                                        var newValue = Copy(member, new TransientMember(this, $"copied from {((TypeProblemNode)member).debugName}"));
                                        HasTransientMember(innerScopeTo, newValue);
                                    }
                                }
                            }

                            {
                                if (objects.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var @object in dict)
                                    {
                                        var newValue = Copy(@object.Value, new Object(this, $"copied from {((TypeProblemNode)@object.Value).debugName}", @object.Value.Converter));
                                        HasObject(innerScopeTo, @object.Key, newValue);
                                    }
                                }
                            }

                            {
                                if (types.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                        HasType(innerScopeTo, type.Key, newValue);
                                    }
                                }
                            }
                            {
                                if (methods.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var method in dict)
                                    {
                                        var newValue = Copy(method.Value, new Method(this, $"copied from {((TypeProblemNode)method.Value).debugName}", method.Value.Converter));
                                        HasMethod(innerScopeTo, method.Key, newValue);
                                    }
                                }
                            }

                            {
                                if (orTypes.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}", type.Value.Converter));
                                        HasOrType(innerScopeTo, type.Key, newValue);
                                    }
                                }
                            }


                            {
                                if (possibleMembers.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var possible in dict)
                                    {
                                        var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                        HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                                    }
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
                            if (hopefulMembers.TryGetValue(innerFromHopeful, out var dict))
                            {
                                foreach (var possible in dict)
                                {
                                    var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}", possible.Value.Converter));
                                    HasHopefulMember(innerToHopeful, possible.Key, newValue);
                                }
                            }
                        
                            if (hopefulMethods.TryGetValue(innerFromHopeful, out var method))
                            {
                                var newValue = Copy(method, new InferredMethodType(this, $"copied from {((TypeProblemNode)method).debugName}", method.Converter));
                                hopefulMethods[innerToHopeful] = newValue;
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



                OrType<MethodType, Type, Object, OrType, DummyType> GetType(ITypeProblemNode value)
                {
                    if (value is ILookUpType lookup)
                    {
                        return lookUps[lookup];
                    }
                    if (value is MethodType methodType)
                    {
                        return new OrType<MethodType, Type, Object, OrType, DummyType>(methodType);
                    }

                    if (value is Type type)
                    {
                        return new OrType<MethodType, Type, Object, OrType, DummyType>(type);

                    }
                    if (value is Object @object)
                    {
                        return new OrType<MethodType, Type, Object, OrType, DummyType>(@object);
                    }
                    if (value is OrType orType)
                    {
                        return new OrType<MethodType, Type, Object, OrType, DummyType>(orType);
                    }

                    throw new Exception("flaming pile of piss");
                    // well, I guess I now know that we have a duality
                    // you either are a type, or you have a type
                    // 
                }

                // returns true if the target was modified 
                bool Flow(OrType<MethodType, Type, Object, OrType, DummyType> flowFrom, OrType<MethodType, Type, Object, OrType, DummyType> flowTo)
                {
                    var res = false;


                    if (flowFrom.Is1(out var fromMethod) && flowTo.Is1(out var toMethod))
                    {
                        var inFlowFrom = GetType(methodInputs[new OrType<Method, MethodType>(fromMethod)]);
                        var inFlowTo = GetType(methodInputs[new OrType<Method, MethodType>(toMethod)]);

                        if (IsInferred(inFlowTo))
                        {
                            res |= Flow(inFlowFrom, inFlowTo);
                        }
                        
                        var returnFlowFrom = GetType(methodReturns[new OrType<Method, MethodType>(fromMethod)]);
                        var retrunFlowTo = GetType(methodReturns[new OrType<Method, MethodType>(toMethod)]);

                        if (IsInferred(retrunFlowTo))
                        {
                            res |= Flow(returnFlowFrom, retrunFlowTo);
                        }
                    }

                    
                    IHaveMembers from;

                    if (flowFrom.Is2(out var fromType))
                    {
                        from = fromType;
                    }
                    else if (flowFrom.Is3(out var fromObject))
                    {
                        from = fromObject;
                    }
                    else if (flowFrom.Is4(out var fromOr))
                    {
                        from = fromOr;
                    }
                    else
                    {
                        throw new Exception("yuck");
                    }

                    // I think the only thing that "flow" are members
                    // but not all types will accept new members
                    if (flowTo.Is2(out var toType) && toType is InferredType inferredFlowTo)
                    {
                        foreach (var memberPair in GetMembers(from))
                        {
                            if (!members.ContainsKey(inferredFlowTo))
                            {
                                members[inferredFlowTo] = new Dictionary<IKey, Member>();
                            }
                            var dict = members[inferredFlowTo];
                            if (dict.TryGetValue(memberPair.Key, out var memberFlowTo))
                            {
                                var typeFlowTo = GetType(memberFlowTo);
                                var typeFlowFrom = GetType(memberPair.Value);
                                
                                if (IsInferred(typeFlowTo))
                                {
                                    res |= Flow(typeFlowFrom, typeFlowTo);
                                }
                            }
                            else
                            {
                                var newValue = new Member(this, $"copied from {memberPair.Value.debugName}", memberPair.Value.Converter);
                                HasMember(inferredFlowTo, memberPair.Key, newValue);
                                lookUps[newValue] = lookUps[memberPair.Value];
                                res = true;
                            }
                        }
                    }
                    return res;

                    // ooo! static local!
                    static bool IsInferred(OrType<MethodType, Type, Object, OrType, DummyType> toCheck)
                    {
                        return toCheck.Is2(out var v2) && v2 is InferredType || toCheck.Is1(out var v1) && v1 is InferredMethodType;
                    }
                }

                IReadOnlyDictionary<IKey, Member> GetMembers2(OrType<MethodType, Type, Object, OrType, DummyType> or)
                {
                    if (or.Is1(out var v1))
                    {
                        return new Dictionary<IKey, Member>();
                    }
                    else if (or.Is2(out var v2))
                    {
                        return GetMembers(v2);
                    }
                    else
                    {
                        throw new Exception("unexpected");
                    }
                }

                IReadOnlyDictionary<IKey, Member> GetMembers(IHaveMembers type)
                {
                    if (type is IExplicitType explictType)
                    {
                        if (members.TryGetValue(explictType, out var res))
                        {
                            return res;
                        }
                        return new Dictionary<IKey, Member>();
                    }

                    if (type is OrType orType)
                    {
                        if (orTypeMembers.TryGetValue(orType, out var res))
                        {
                            return res;
                        }

                        res = new Dictionary<IKey, Member>();
                        var (left, right) = orTypeComponents[orType];

                        var rightMembers = GetMembers2(GetType(right));
                        foreach (var leftMember in GetMembers2(GetType(left)))
                        {
                            if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                            {
                                // if they are the same type
                                if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
                                {
                                    var member = new Member(this, $"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}", leftMember.Value.Converter);
                                    lookUps[member] = GetType(rightMember);
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

            private bool TryGetMember(IScope context, IKey key, out Member member)
            {
                while (true)
                {
                    if (members.TryGetValue(context, out var contextMembers) && contextMembers.TryGetValue(key, out member))
                    {
                        return true;
                    }
                    if (!kidParent.TryGetValue(context, out context))
                    {
                        member = default;
                        return false;
                    }
                }
            }



            public TypeProblem2(IConvertTo<Scope, OrType<WeakBlockDefinition, WeakScope>> rootConverter)
            {

                Base = new Scope(this, "base", rootConverter);
                Root = CreateScope(Base, rootConverter);

                //CreateGenericType(Root, new NameKey("method"), new IKey[] {
                //    new NameKey("input"),
                //    new NameKey("output")
                //});

                //CreateGenericType(Root, new NameKey("implementation"), new IKey[] {
                //    new NameKey("context"),
                //    new NameKey("input"),
                //    new NameKey("output")
                //});
                CreateType(Base, new NameKey("number"), new PrimitiveTypeConverter(new NumberType()));
                CreateType(Base, new NameKey("string"), new PrimitiveTypeConverter(new StringType()));
                CreateType(Base, new NameKey("bool"), new PrimitiveTypeConverter(new BooleanType()));
                CreateType(Base, new NameKey("empty"), new PrimitiveTypeConverter(new EmptyType()));

                // shocked this works...
                IGenericTypeParameterPlacholder[] genericParameters = new IGenericTypeParameterPlacholder[] { new GenericTypeParameterPlacholder(new NameKey("T1")), new GenericTypeParameterPlacholder(new NameKey("T2")) };
                var key = new NameKey("method");
                var placeholders = new TypeAndConverter[] { new TypeAndConverter(new NameKey("T1"), new WeakTypeDefinitionConverter()), new TypeAndConverter(new NameKey("T2"), new WeakTypeDefinitionConverter()) };
                
                var res = new MethodType(
                    this,
                    $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.ToString())}",
                    new MethodTypeConverter());

                HasMethodType(Base, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.key.ToString()}", placeholder.converter);
                    HasPlaceholderType(new OrType<MethodType, Type>(res), placeholder.key, new OrType<MethodType, Type, Object, OrType, DummyType>(placeholderType));
                }

                var methodInputKey = new ImplicitKey(Guid.NewGuid());
                methodInputs[new OrType<Method, MethodType>(res)] = CreateMember(res, methodInputKey, new NameKey("T1"),new WeakMemberDefinitionConverter(false, methodInputKey));
                methodReturns[new OrType<Method, MethodType>(res)] = CreateTransientMember(res, new NameKey("T2"));
            }

            private class GenericTypeKey
            {
                private readonly OrType<MethodType, Type> primary;
                private readonly OrType<MethodType, Type, Object, OrType, DummyType>[] parameters;

                public GenericTypeKey(OrType<MethodType, Type> primary, OrType<MethodType, Type, Object, OrType, DummyType>[] parameters)
                {
                    this.primary = primary ?? throw new ArgumentNullException(nameof(primary));
                    this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
                }

                public override bool Equals(object obj)
                {
                    return Equals(obj as GenericTypeKey);
                }

                public bool Equals(GenericTypeKey other)
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
            return type.Problem.GetKey(type);
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IValue method)
        {
            return method.Problem.GetReturns(method);
        }

        public static Tpn.TypeProblem2.TransientMember Returns(this Tpn.IScope method)
        {
            return method.Problem.GetReturns(method);
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
