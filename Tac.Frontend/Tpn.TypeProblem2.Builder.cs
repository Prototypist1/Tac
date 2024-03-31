using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._3_Syntax_Model.Elements;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Model;
using Tac.Model.Elements;
using Tac.SemanticModel;
using Tac.SyntaxModel.Elements.AtomicTypes;

namespace Tac.Frontend.New.CrzayNamespace
{

    internal partial class Tpn
    {


        internal partial class TypeProblem2 
        {

            public class Builder 
            {

                private readonly TypeProblem2 problem;

                public Builder(TypeProblem2 problem)
                {
                    this.problem = problem ?? throw new ArgumentNullException(nameof(problem));
                }

                public void IsChildOf(IStaticScope parent, IStaticScope kid)
                {
                    kid.Parent = Possibly.Is(parent);
                }
                public static void HasValue(IScope parent, Value value)
                {
                    parent.Values.Add(value);
                }
                public static void HasReference(IStaticScope parent, TypeReference reference)
                {
                    parent.Refs.Add(reference);
                }

                //public void HasEntryPoint(IStaticScope parent, Scope entry)
                //{
                //    parent.EntryPoints.Add(entry);
                //}

                public static void HasType(IStaticScope parent, IKey key, Type type)
                {
                    parent.Types.Add(key, type);
                }
                public void HasMethodType(IStaticScope parent, IKey key, MethodType type)
                {
                    parent.MethodTypes.Add(key, type);
                }
                // why do objects have keys?
                // that is wierd
                public static void HasObject(IStaticScope parent, IKey key, Object @object)
                {
                    parent.Objects.Add(key, @object);
                }

                public void HasGenericType(IOrType<MethodType, Type, Method> parent, NameKey key, GenericTypeParameter type)
                {
                    parent.Switch(x => x.Generics.Add(key, type), x => x.Generics.Add(key, type), x => x.Generics.Add(key, type));
                }
                public void HasOverlayedGeneric(IOrType<MethodType, Type, Method> parent, NameKey key, TypeLikeOrError type)
                {
                    parent.Switch(x => x.GenericOverlays.Add(key, type), x => x.GenericOverlays.Add(key, type), x => x.GenericOverlays.Add(key, type));
                }
                public static void HasPrivateMember(IHavePrivateMembers parent, IKey key, Member member)
                {
                    parent.PrivateMembers.Add(key, member);
                }
                public static void HasPublicMember(IHavePublicMembers parent, IKey key, Member member)
                {
                    parent.PublicMembers.Add(key, member);
                }
                public static void HasMethod(IStaticScope parent, IKey key, Method method)
                {
                    parent.Methods.Add(key, method);
                }


                public static void HasTransientMember(IScope parent, TransientMember member)
                {
                    parent.TransientMembers.Add(member);
                }
                public static Member HasMembersPossiblyOnParent(IHavePossibleMembers parent, IKey key, Func<Member> member)
                {
                    if (parent.PossibleMembers.TryGetValue(key, out var res))
                    {
                        return res;
                    }
                    res = member();
                    parent.PossibleMembers.Add(key, res);
                    return res;
                }
                public Member HasHopefulMember(IValue parent, IKey key, Func<Member> member)
                {
                    if (!parent.Hopeful.Is(out var inferredType))
                    {
                        inferredType = new InferredType(this, $"hopeful of {parent}");
                        parent.Hopeful = Possibly.Is(inferredType);
                    }

                    if (inferredType.PublicMembers.TryGetValue(key, out var res)) {
                        return res;
                    }

                    res = member();
                    inferredType.PublicMembers.Add(key, res);
                    return res;
                }


                public void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo)
                {
                    AssertIs(assignedFrom, assignedTo);
                }

                public void IsAssignedTo(Method assignedFrom, ICanBeAssignedTo assignedTo)
                {
                    problem.assignments.Add((
                        Prototypist.Toolbox.OrType.Make<ILookUpType, Tpn.TypeLikeWithMethodOrError>(Tpn.TypeLikeWithMethodOrError.Make(assignedFrom)),
                        Prototypist.Toolbox.OrType.Make<ILookUpType, TypeLikeOrError>(assignedTo)));
                }

                public void AssertIs(ILookUpType assignedFrom, ILookUpType assignedTo)
                {
                    problem.assignments.Add((
                        Prototypist.Toolbox.OrType.Make<ILookUpType, Tpn.TypeLikeWithMethodOrError>(assignedFrom),
                        Prototypist.Toolbox.OrType.Make<ILookUpType, TypeLikeOrError>(assignedTo)));
                }

                public void AssertIs(GenericTypeParameter assignedFrom, GenericTypeParameter assignedTo)
                {
                    problem.assignments.Add((
                        Prototypist.Toolbox.OrType.Make<ILookUpType, Tpn.TypeLikeWithMethodOrError>(Tpn.TypeLikeWithMethodOrError.Make(assignedFrom)),
                        Prototypist.Toolbox.OrType.Make<ILookUpType, TypeLikeOrError>(TypeLikeOrError.Make(assignedTo))));
                }

                public Value CreateValue(IScope scope, IKey typeKey)
                {
                    var res = new Value(this, typeKey.ToString()!);
                    HasValue(scope, res);
                    res.Context = Possibly.Is(scope);
                    res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                    return res;
                }

                public Member CreateMember(
                    IStaticScope scope,
                    IKey key,
                    IOrType<IKey, IError> typeKey)
                {
                    var res = new Member(this, key.ToString()!);
                    if (scope is IHavePublicMembers publicMembers)
                    {
                        HasPublicMember(publicMembers, key, res);
                    }
                    else
                    if (scope is IHavePrivateMembers privateMembers)
                    {
                        HasPrivateMember(privateMembers, key, res);
                    }
                    else
                    {
                        throw new Exception("this is probably really an IError - you tried to add a member somewhere one cannot go");
                    }
                    res.Context = Possibly.Is(scope);
                    res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                    return res;
                }

                public Member CreatePublicMember(
                    IStaticScope scope,
                    IHavePublicMembers havePublicMembers,
                    IKey key,
                    IOrType<IKey, IError> typeKey)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPublicMember(havePublicMembers, key, res);
                    res.Context = Possibly.Is(scope);
                    res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                    return res;
                }

                public Member CreatePublicMember(
                    IStaticScope scope,
                    IHavePublicMembers havePublicMembers,
                    IKey key)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPublicMember(havePublicMembers, key, res);
                    res.Context = Possibly.Is(scope);
                    return res;
                }

                public Member CreatePublicMember(
                    IHavePublicMembers scope,
                    IKey key,
                    TypeLikeOrError type)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPublicMember(scope, key, res);
                    res.LooksUp = Possibly.Is(type);
                    return res;
                }

                public Member CreatePrivateMember(
                    IStaticScope scope,
                    IHavePrivateMembers havePrivateMembers,
                    IKey key,
                    IOrType<IKey, IError> typeKey)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPrivateMember(havePrivateMembers, key, res);
                    res.Context = Possibly.Is(scope);
                    res.TypeKey = typeKey.SwitchReturns(x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x), x => Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(x));
                    return res;
                }

                public Member CreatePrivateMember(
                    IStaticScope scope,
                    IHavePrivateMembers havePrivateMembers,
                    IKey key)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPrivateMember(havePrivateMembers, key, res);
                    res.Context = Possibly.Is(scope);
                    return res;
                }

                public Member CreatePrivateMember(
                    IHavePrivateMembers scope,
                    IKey key,
                    TypeLikeOrError type)
                {
                    var res = new Member(this, key.ToString()!);
                    HasPrivateMember(scope, key, res);
                    res.LooksUp = Possibly.Is(type);
                    return res;
                }

                public Member CreateMemberPossiblyOnParent(IStaticScope scope, IHavePossibleMembers havePossibleMembers, IKey key)
                {
                    // this is weird, but since C# does not have and types...
                    // scope and havePossibleMembers are expected to be the same object
                    if (!ReferenceEquals(scope, havePossibleMembers))
                    {
                        throw new Exception($"{scope} and {havePossibleMembers} should be the same object");
                    }

                    if (havePossibleMembers.PossibleMembers.TryGetValue(key, out var res1))
                    {
                        return res1;
                    }

                    var res = HasMembersPossiblyOnParent(havePossibleMembers, key, ()=> new Member(this, "possibly on parent -" + key.ToString()));
                    res.Context = Possibly.Is(scope);
                    return res;
                }

                public TypeReference CreateTypeReference(IStaticScope context, IKey typeKey, IConvertTo<TypeReference, IFrontendType<IVerifiableType>> converter)
                {
                    var res = new TypeReference(this, typeKey.ToString()!, converter);
                    HasReference(context, res);
                    res.Context = Possibly.Is(context);
                    res.TypeKey = Prototypist.Toolbox.OrType.Make<IKey, IError, Unset>(typeKey);
                    return res;
                }

                public Scope CreateScope(IStaticScope parent, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> converter)
                {
                    var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).DebugName}", converter);
                    IsChildOf(parent, res);
                    return res;
                }

                public Type CreateType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter)
                {
                    return CreateType(parent, key, converter, Possibly.IsNot<Guid>());
                }

                public Type CreateType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter, IIsPossibly<Guid> primitive)
                {
                    var res = new Type(this, key.ToString()!, Possibly.Is(key), converter,  primitive, Possibly.IsNot<IInterfaceType>());
                    IsChildOf(parent, res);
                    HasType(parent, key.SwitchReturns<IKey>(x => x, x => x), res);
                    return res;
                }

                public Type CreateType(IStaticScope parent, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter)
                {
                    var key = new ImplicitKey(Guid.NewGuid());
                    var res = new Type(this, key.ToString()!, Possibly.IsNot<IOrType<NameKey, ImplicitKey>>(), converter,  Possibly.IsNot<Guid>(), Possibly.IsNot<IInterfaceType>());
                    IsChildOf(parent, res);
                    // migiht need this, let's try without first
                    //HasType(parent, key, res);
                    return res;
                }
                public Type CreateTypeExternalType(IStaticScope parent, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter, IInterfaceType interfaceType)
                {
                    var key = new ImplicitKey(Guid.NewGuid());
                    var res = new Type(this, key.ToString()!, Possibly.IsNot<IOrType<NameKey, ImplicitKey>>(), converter,  Possibly.IsNot<Guid>(), Possibly.Is(interfaceType));
                    IsChildOf(parent, res);
                    // migiht need this, let's try without first
                    //HasType(parent, key, res);
                    return res;
                }

                public Type CreateGenericType(IStaticScope parent, IOrType<NameKey, ImplicitKey> key, IReadOnlyList<TypeAndConverter> placeholders, IConvertTo<Type, IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, Tac.SyntaxModel.Elements.AtomicTypes.IPrimitiveType>> converter)
                {
                    var res = new Type(
                        this,
                        $"generic-{key}-{placeholders.Aggregate("", (x, y) => x + "-" + y)}",
                        Possibly.Is(key),
                        converter,
                        Possibly.IsNot<Guid>(),
                        Possibly.IsNot<IInterfaceType>());
                    IsChildOf(parent, res);
                    HasType(parent, key.SwitchReturns<IKey>(x => x, x => x), res);
                    var i = 0;
                    foreach (var placeholder in placeholders)
                    {
                        var placeholderType = new GenericTypeParameter(this, $"generic-parameter-{placeholder.key}", i++, Prototypist.Toolbox.OrType.Make<MethodType, Type, Method,InferredType>(res));
                        //var placeholderType = new Type(
                        //    this,
                        //    $"generic-parameter-{placeholder.key}",
                        //    Possibly.Is(placeholder.key),
                        //    placeholder.converter,
                        //    Possibly.IsNot<Guid>(),
                        //    Possibly.IsNot<IInterfaceType>());
                        HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(res), placeholder.key, placeholderType);
                    }
                    return res;
                }

                // why do objects have keys?
                // that is wierd
                public Object CreateObjectOrModule(IStaticScope parent, IKey key, IConvertTo<Object, IOrType<WeakObjectDefinition, WeakRootScope>> converter, IConvertTo<Scope, IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>> innerConverter)
                {
                    var res = new Object(this, key.ToString()!, converter, innerConverter);
                    IsChildOf(parent, res);
                    HasObject(parent, key, res);
                    return res;
                }

                public Method CreateMethod(IStaticScope parent, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition,WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter)
                {
                    var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                    IsChildOf(parent, res);
                    HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                    // here it is ok for these to be members because we are using a method
                    var returns = CreateTransientMember(res, $"return of {res.DebugName}");
                    res.Returns = Possibly.Is(returns);
                    var input = CreatePrivateMember(res, res, new NameKey(inputName));
                    res.Input = Possibly.Is(input);
                    return res;
                }

                public (Method, IOrType<TypeReference, IError>, IOrType<TypeReference, IError>) CreateGenericMethod(IStaticScope parent, Func<IStaticScope, IOrType<TypeReference, IError>> inputTypeBuilder, Func<IStaticScope, IOrType<TypeReference, IError>> outputTypeBuilder, string inputName, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter, IReadOnlyList<TypeAndConverter> placeholders)
                {
                    var (method, inputType, outputType) = CreateMethod(parent, inputTypeBuilder, outputTypeBuilder,  inputName, converter);
                    var i = 0;
                    foreach (var placeholder in placeholders)
                    {
                        var placeholderType = new GenericTypeParameter(this, $"generic-parameter-{placeholder.key}", i++, Prototypist.Toolbox.OrType.Make<MethodType, Type, Method,InferredType>(method));

                        HasGenericType(Prototypist.Toolbox.OrType.Make<MethodType, Type, Method>(method), placeholder.key, placeholderType);
                    }

                    // why doesn't this have genericOverlays ({4BFD0274-B70F-4BD8-B290-63B69FF74FE7})
                    // the overlays are t1 and t2 of the base method "method [t1,t2]"
                    // they are never references anywhere so we don't care to overlay them
                    // we build the right MethodType
                    // at {2E20DFFB-7BD2-4351-9CAF-10A63491ABCF}
                    // this isn't great
                    // it would be a little work to build the genericOverlays
                    // genericOverlays take a looked up type not a key

                    return (method, inputType, outputType);
                }


                public (Method, IOrType<TypeReference, IError>, IOrType<TypeReference, IError>) CreateMethod(
                    IStaticScope parent, 
                    Func<IStaticScope, IOrType<TypeReference, IError>> inputTypeBuilder, // these type references look up from inside the method they could look up to a generic type defined in the method
                    Func<IStaticScope, IOrType<TypeReference, IError>> outputTypeBuilder, 
                    string inputName, 
                    IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter)
                {
                    //if (!inputType.Is1(out var inputTypeValue))
                    //{
                    //    throw new NotImplementedException();
                    //}
                    //if (!outputType.Is1(out var outputTypeValue))
                    //{
                    //    throw new NotImplementedException();
                    //}

                    var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                    var inputType = inputTypeBuilder(res);
                    if (!inputType.Is1(out var inputTypeValue))
                    {
                        throw new NotImplementedException();
                    }
                    var outputType = outputTypeBuilder(res);
                    if (!outputType.Is1(out var outputTypeValue))
                    {
                        throw new NotImplementedException();
                    }
                    IsChildOf(parent, res);
                    HasMethod(parent, new ImplicitKey(Guid.NewGuid()), res);
                    {
                        var returns = outputTypeValue.TypeKey is IIsDefinately<IKey> typeKey ? CreateTransientMember(res, typeKey.Value, $"return of {res.DebugName}") : CreateTransientMember(res, $"return of {res.DebugName}");
                        res.Returns = Possibly.Is(returns);
                    }
                    {
                        if (inputTypeValue.TypeKey is IIsDefinately<IKey> typeKey)
                        {

                            // here it is ok for these to be members because we are using a method
                            res.Input = Possibly.Is(CreatePrivateMember(res, res, new NameKey(inputName), Prototypist.Toolbox.OrType.Make<IKey, IError>(typeKey.Value)));
                        }
                        else
                        {

                            // here it is ok for these to be members because we are using a method
                            res.Input = Possibly.Is(CreatePrivateMember(res, res, new NameKey(inputName)));
                        }
                    }
                    return (res, inputType, outputType);
                }


                public Member CreateHopefulMember(IValue scope, IKey key)
                {
                    var res = HasHopefulMember(scope, key, ()=> new Member(this, "hopeful - " + key.ToString()!));
                    return res;
                }


                public OrType CreateOrType(IStaticScope s, IKey key, IOrType<TypeReference, IError> setUpSideNode1, IOrType<TypeReference, IError> setUpSideNode2, IConvertTo<OrType, IFrontendType<IVerifiableType>> converter)
                {
                    if (!setUpSideNode1.Is1(out var node1))
                    {
                        throw new NotImplementedException();
                    }
                    if (!setUpSideNode2.Is1(out var node2))
                    {
                        throw new NotImplementedException();
                    }

                    var res = new OrType(this, $"{node1.DebugName} || {node2.DebugName}", converter);
                    Ors(res, node1, node2);
                    HasOrType(s, key, res);

                    return res;

                }
                public MethodType GetMethod(TypeLikeOrError input, TypeLikeOrError output)
                {
                    throw new NotImplementedException();
                }

                public static void Ors(OrType orType, TypeReference a, TypeReference b)
                {
                    orType.Left = Possibly.Is(a);
                    orType.Right = Possibly.Is(b);
                }

                public static void HasOrType(IStaticScope scope, IKey kay, OrType orType1)
                {
                    scope.OrTypes[kay] = orType1;
                }


                public void IsNumber(IScope parent, ILookUpType target)
                {
                    // super weird that this has to be a value
                    var thing = CreateValue(parent, new NameKey("number"));
                    AssertIs(target, thing);
                }

                public void IsBlock(IScope parent, ILookUpType target)
                {
                    // super weird that this has to be a value
                    var thing = CreateValue(parent, new NameKey("block"));
                    AssertIs(target, thing);
                }

                public void IsBool(IScope parent, ILookUpType target)
                {
                    // super weird that this has to be a value
                    var thing = CreateValue(parent, new NameKey("bool"));
                    AssertIs(target, thing);
                }

                public void IsEmpty(IScope parent, ILookUpType target)
                {
                    // super weird that this has to be a value
                    var thing = CreateValue(parent, new NameKey("empty"));
                    AssertIs(target, thing);
                }

                public void IsString(IScope parent, ILookUpType target)
                {
                    // super weird that this has to be a value
                    var thing = CreateValue(parent, new NameKey("string"));
                    AssertIs(target, thing);
                }

                public TransientMember CreateTransientMember(IScope parent, string debugName)
                {
                    var res = new TransientMember(this, debugName);
                    HasTransientMember(parent, res);
                    res.Context = Possibly.Is(parent);
                    return res;
                }

                public TransientMember CreateTransientMember(IScope parent, IKey typeKey, string debugName)
                {
                    var res = new TransientMember(this, debugName);
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
                //public Method IsMethod(IScope parent, ICanAssignFromMe target, IConvertTo<Method, IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition, WeakGenericMethodDefinition>> converter)
                //{
                //    var thing = CreateTransientMember(parent, $"is method for {target.DebugName}");
                //    var method = CreateMethod(parent, "input", converter);
                //    IsAssignedTo(target, thing);
                //    return method;
                //}


                public Member GetInput(IValue value)
                {
                    if (value.Hopeful.Is(out var inferredType))
                    {
                        return inferredType.Input.GetOrThrow();
                    }
                    else
                    {
                        var inferredMethodType = new InferredType(this, "generated infered method type");
                        value.Hopeful = Possibly.Is(inferredMethodType);

                        // shared code {A9E37392-760B-427D-852E-8829EEFCAE99}
                        // we don't use has member input/output doesn't go in the member list
                        // it is not a public member
                        // and infered to do not have private members
                        var methodInputKey = new NameKey("generated infered method input - " + Guid.NewGuid());
                        var inputMember = new Member(this, methodInputKey.ToString()!);
                        inputMember.LooksUp = Possibly.Is(TypeLikeOrError.Make(new InferredType(this, "implicit input")));
                        inferredMethodType.Input = Possibly.Is(inputMember);

                        var returnMember = new TransientMember(this, "generated infered method return -" + Guid.NewGuid());
                        returnMember.LooksUp = Possibly.Is(TypeLikeOrError.Make(new InferredType(this, "implicit return")));
                        inferredMethodType.Returns = Possibly.Is(returnMember);

                        return inputMember;
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

                public TransientMember GetReturns(IStaticScope s)
                {
                    if (s is Method method)
                    {
                        return GetReturns(method);
                    }
                    else if (s.Parent is IIsDefinately<IStaticScope> definatelyScope)
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

                // TODO
                // {A48C75B3-07B4-4D84-8803-250D6406695D}
                // this is a bit weird
                // it is get or create
                // but like if something as already made a hopeful member it will just throw
                // that is probably a bug
                // x.y := 5
                // 5 > x
                // boom, probably 
                public TransientMember GetReturns(IValue value)
                {
                    if (value.Hopeful is IIsDefinately<InferredType> inferredType)
                    {
                        return inferredType.Value.Returns.GetOrThrow();
                    }
                    else
                    {
                        var inferredMethodType = new InferredType(this, "generated infered method type");
                        value.Hopeful = Possibly.Is(inferredMethodType);

                        // shared code {A9E37392-760B-427D-852E-8829EEFCAE99}
                        // we don't use has member input/output doesn't go in the member list
                        // it is not a public member
                        // and infered to do not have private members
                        var methodInputKey = new NameKey("generated infered method input - " + Guid.NewGuid());
                        var inputMember = new Member(this, methodInputKey.ToString()!);
                        inputMember.LooksUp = Possibly.Is(TypeLikeOrError.Make(new InferredType(this, "implicit input")));
                        inferredMethodType.Input = Possibly.Is(inputMember);

                        var returnMember = new TransientMember(this, "generated infered method return -" + Guid.NewGuid());
                        returnMember.LooksUp = Possibly.Is(TypeLikeOrError.Make(new InferredType(this, "implicit return")));
                        inferredMethodType.Returns = Possibly.Is(returnMember);

                        return returnMember;
                    }
                }

                public IReadOnlyDictionary<NameKey, GenericTypeParameter> HasGenerics(IValue value, NameKey[] keys)
                {
                    InferredType? inferredType;
                    if (value.Hopeful.Is(out var innerInferredType))
                    {
                        inferredType = innerInferredType;
                    }
                    else
                    {
                        var inferredMethodType = new InferredType(this, "generated infered method type");
                        value.Hopeful = Possibly.Is(inferredMethodType);
                        inferredType = inferredMethodType;
                    }
                    if (inferredType.Generics.Count > 0)
                    {
                        throw new Exception("this already has generics!");
                    }
                    var i = 0;
                    foreach (var key in keys)
                    {
                        inferredType.Generics.Add(key, new GenericTypeParameter(this, $"generic-parameter-{key}", i++, Prototypist.Toolbox.OrType.Make<MethodType, Type, Method, InferredType>(inferredType)));
                    }
                    return inferredType.Generics;
                }

                public T Register<T>(T typeProblemNode)
                    where T : ITypeProblemNode
                {
                    problem.typeProblemNodes.Add(typeProblemNode);
                    return typeProblemNode;
                }


            }
        }
    }
}
