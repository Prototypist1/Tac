﻿using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        internal class TypeSolution
        {

            //readonly Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> generalLookUp = new Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<IVerifiableType>, IError>>>();
            //readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>> objectCache = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>>();
            //readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> scopeOrBlockCache = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>> methodCache = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> typeCache = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();

            //readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>> scopeCache = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>>();

            //readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodeLookUp;

            //readonly Dictionary<TypeProblem2.Method, Scope> methodScopeCache = new Dictionary<TypeProblem2.Method, Scope>();
            //readonly Dictionary<TypeProblem2.Scope, Scope> scopeScopeCache = new Dictionary<TypeProblem2.Scope, Scope>();

            private readonly ConcurrentIndexed<EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>, Yolo> cache = new();
            //private readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes;
            private readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> flowNodes2;

            private readonly Dictionary<(Yolo, IOrType<Member, Input, Output, Left, Right, PrivateMember>), List<Yolo>> positions;

            private class Yolo
            {
                public readonly EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key;

                internal IOrType<IReadOnlyList<(IKey, Yolo)>, IError>? members;
                internal IOrType<IReadOnlyList<(IKey, Yolo)>, IError>? privateMembers;

                internal IIsPossibly<IOrType<Yolo, IError>>? output;
                internal IIsPossibly<IOrType<Yolo, IError>>? input;

                // for or types
                internal IIsPossibly<Yolo>? left;
                internal IIsPossibly<Yolo>? right;
                internal IOrType<GenericTypeParameterPlacholder[], IError>? generics;
                internal IIsDefinately<IOrType<Yolo, IError>> looksUp;
                internal GenericTypeParameterPlacholder? genericTypeParameterPlaceholder;
                internal readonly Box<IOrType<IFrontendType<IVerifiableType>, IError>> type = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();

                public Yolo(EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                }

                // sometimes thing have no member but really we know they are an object or a type
                // if this is not set the yolo becomes an AnyType with it set the Yolo becomes a HasMembers with no members
                //internal bool hasMemebers = false;
                //internal IIsPossibly<TypeProblem2.GenericTypeParameter> isGeneric = Possibly.IsNot<TypeProblem2.GenericTypeParameter>();
                //internal IIsPossibly<TypeProblem2.GenericTypeParameter> isGenericConstraintFor = Possibly.IsNot<TypeProblem2.GenericTypeParameter>();
                //internal IIsPossibly<Box<IOrType<IFrontendType<IVerifiableType>, IError>>> isGenericConstraintFroRealized = Possibly.IsNot<Box<IOrType<IFrontendType<IVerifiableType>, IError>>>();

                //internal IIsPossibly<IInterfaceType> external = Possibly.IsNot<IInterfaceType>();
            }

            public TypeSolution(
                IReadOnlyList<IOrType<TypeProblem2.MethodType, TypeProblem2.Type, TypeProblem2.Object, TypeProblem2.OrType, TypeProblem2.InferredType, TypeProblem2.GenericTypeParameter, TypeProblem2.Method, IError>> things,
                //Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> flowNodes2)
            {

                if (things is null)
                {
                    throw new ArgumentNullException(nameof(things));
                }

                //this.flowNodes = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));
                this.flowNodes2 = flowNodes2 ?? throw new ArgumentNullException(nameof(flowNodes2));

                var constrainsToGenerics = things.SelectMany(x => { if (x.Is6(out var genericTypeParameter)) { return new[] { genericTypeParameter }; } return Array.Empty<TypeProblem2.GenericTypeParameter>(); }).ToDictionary(x => x.constraint, x => x);

                foreach (var flowNode2 in flowNodes2)
                {
                    var rep = flowNode2.Value.GetValueAs(out IConstraintSoruce _).GetExtendedConstraints().Flatten();
                    var yolo = GetOrAdd(rep);

                    // this feels a bit weird because it doesn't flow through the type problem
                    //if (flowNode2.Key.Is1(out var typeProblemNode))
                    //{
                    //    yolo.hasMemebers |= typeProblemNode.SafeIs<ITypeProblemNode, TypeProblem2.Object>();
                    //    if (typeProblemNode.SafeIs(out TypeProblem2.Type type))
                    //    {
                    //        yolo.hasMemebers = true;
                    //        if (type.External.Is(out var _))
                    //        {
                    //            yolo.external = type.External;
                    //        }
                    //    }
                    //    {
                    //        if (typeProblemNode.SafeIs(out TypeProblem2.GenericTypeParameter genericTypeParameter))
                    //        {
                    //            throw new Exception("we pass the constraint into the type problem, not the actual parameter");
                    //            //realYolo.isGeneric = Possibly.Is(genericTypeParameter);
                    //        }
                    //    }
                    //    {
                    //        if (typeProblemNode.SafeIs(out TypeProblem2.InferredType inferredType) && constrainsToGenerics.TryGetValue(inferredType, out var genericTypeParameter))
                    //        {
                    //            yolo.isGenericConstraintFor = Possibly.Is(genericTypeParameter);
                    //        }
                    //    }
                    //}
                }

                Yolo GetOrAdd(EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> equalableHashSet)
                {
                    var myBox = new Yolo(equalableHashSet);
                    var current = cache.GetOrAdd(equalableHashSet, myBox);

                    // if we added it, fill it
                    if (current == myBox)
                    {
                        if (equalableHashSet.Count() > 1)
                        {
                            myBox.left = Possibly.Is(GetOrAdd(new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>(equalableHashSet.Take(equalableHashSet.Count() - 1).ToHashSet())));
                            myBox.right = Possibly.Is(GetOrAdd(new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>(new HashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() { equalableHashSet.Last() })));

                            myBox.generics = OrType.Make<GenericTypeParameterPlacholder[], IError>(Array.Empty<GenericTypeParameterPlacholder>());
                            
                            myBox.members = OrType.Make<IReadOnlyList<(IKey, Yolo)>, IError>(Array.Empty<(IKey, Yolo)>());
                            myBox.privateMembers = OrType.Make<IReadOnlyList<(IKey, Yolo)>, IError>(Array.Empty<(IKey, Yolo)>());
                            myBox.input = Possibly.IsNot<IOrType<Yolo, IError>>();
                            myBox.output = Possibly.IsNot<IOrType<Yolo, IError>>();
                        }
                        else
                        {
                            myBox.left = Possibly.IsNot<Yolo>();
                            myBox.right = Possibly.IsNot<Yolo>();

                            var i = 0;
                            myBox.generics = equalableHashSet.First().Generics().TransformInner(generics => generics.Select(generic => new GenericTypeParameterPlacholder(i++,  GetOrAdd(generic.Flatten()).type)).ToArray());

                            myBox.members = equalableHashSet.First().Members().TransformInner(members => members.Select(memberPair => (memberPair.Key, GetOrAdd(memberPair.Value.Flatten()))).ToArray());
                            myBox.privateMembers = equalableHashSet.First().PrivateMembers().TransformInner(members => members.Select(memberPair => (memberPair.Key, GetOrAdd(memberPair.Value.Flatten()))).ToArray());
                            myBox.input = equalableHashSet.First().Input().TransformInner(inputOr => inputOr.TransformInner(input => GetOrAdd(input.Flatten())));
                            myBox.output = equalableHashSet.First().Output().TransformInner(outputOr => outputOr.TransformInner(output => GetOrAdd(output.Flatten())));
                        }

                    }
                    return current;
                }

                // child, path -> parent 
                positions = new ();
                // we build a hierarchy
                foreach (var item in cache)
                {
                    if (item.Value.input.Is(out var inputOrError) && inputOrError.Is1(out var input)) {
                        var key = (input, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new Input()));
                        if (positions.TryGetValue(key, out var list)) {
                            list.Add(item.Value);
                        }
                        else {
                            positions[key] = new List<Yolo> { item.Value };
                        }
                    }
                    if (item.Value.output.Is(out var outputOrError) && outputOrError.Is1(out var output))
                    {
                        var key = (output, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new Output()));
                        if (positions.TryGetValue(key, out var list))
                        {
                            list.Add(item.Value);
                        }
                        else
                        {
                            positions[key] = new List<Yolo> { item.Value };
                        }
                    }
                    if (item.Value.left.Is(out var left))
                    {
                        var key = (left, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new Left()));
                        if (positions.TryGetValue(key, out var list))
                        {
                            list.Add(item.Value);
                        }
                        else
                        {
                            positions[key] = new List<Yolo> { item.Value };
                        }
                    }
                    if (item.Value.right.Is(out var right))
                    {
                        var key = (right, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new Right()));
                        if (positions.TryGetValue(key, out var list))
                        {
                            list.Add(item.Value);
                        }
                        else
                        {
                            positions[key] = new List<Yolo> { item.Value };
                        }
                    }
                    if (item.Value.members.Is1(out var members))
                    {
                        foreach (var memberPair in members)
                        {
                            var key = (memberPair.Item2, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new Member(memberPair.Item1)));
                            if (positions.TryGetValue(key, out var list))
                            {
                                list.Add(item.Value);
                            }
                            else
                            {
                                positions[key] = new List<Yolo> { item.Value };
                            }
                        }
                    }
                    if (item.Value.privateMembers.Is1(out var privateMembers))
                    {
                        foreach (var memberPair in privateMembers)
                        {
                            var key = (memberPair.Item2, OrType.Make<Member, Input, Output, Left, Right, PrivateMember>(new PrivateMember(memberPair.Item1)));
                            if (positions.TryGetValue(key, out var list))
                            {
                                list.Add(item.Value);
                            }
                            else
                            {
                                positions[key] = new List<Yolo> { item.Value };
                            }
                        }
                    }
                }

                //foreach (var (key, value) in cache)
                //{
                //    if (value.generics.Is1(out var hasGenerics))
                //    {
                //        var i = 0;
                //        foreach (var generic in hasGenerics)
                //        {
                //            // you are here
                //            // this happen like
                //            // method [T1] [T1,T1] y =: x
                //            // y's input and x's input have the same constraints so they show up as one node here
                //            // 
                //            if (generic.genericTypeParameterPlaceholder != null)
                //            {
                //                throw new Exception("is that ok? can something be the generic on more than one thing?");
                //            }

                //            generic.genericTypeParameterPlaceholder = new GenericTypeParameterPlacholder(i, generic.type);
                //            i++;
                //        }
                //    }
                //}

                // match generics with what they look up to
                //foreach (var (key, value) in cache)
                //{
                //    if (value.generics.Is1(out var yolos) && yolos.Any())
                //    {
                //        Walk(value, new Yolo[] { value, }, Array.Empty<IOrType<Member, Input, Output, Left, Right>>());
                //    }
                //}

                // this is probably slow a dumb
                //IEnumerable<T> Add<T>(IEnumerable<T> current, T next)
                //{
                //    foreach (var item in current)
                //    {
                //        yield return item;
                //    }
                //    yield return next;
                //}

                //void Walk(Yolo toWalk, IEnumerable<Yolo> stack, IEnumerable<IOrType<Member, Input, Output, Left, Right>> path)
                //{
                //    if (stack.SkipLast(1).Contains(toWalk))
                //    {
                //        return;
                //    }
                    

                //    var justGenericConstraints = toWalk.key.Select(x => x
                //        .Where(y => y.Is5(out IsGeneric _))
                //        .Select(y => y.Is5OrThrow())
                //        .ToArray())
                //    .ToArray();

                //    var intersect = justGenericConstraints.First();

                //    foreach (var item in justGenericConstraints.Skip(1))
                //    {
                //        intersect = item.Intersect(intersect).ToArray();
                //    }

                //    var lookups = intersect.SelectMany(genericConstraint =>
                //    {
                //        var ourPathArray = path.ToArray();

                //        if (genericConstraint.pathFromOwner.Length > ourPathArray.Length)
                //        {
                //            return Array.Empty<IOrType<Yolo, IError>>();
                //            //goto notMatched;
                //        }

                //        for (int i = genericConstraint.pathFromOwner.Length - 1; i >= 0; i--)
                //        {
                //            if (!genericConstraint.pathFromOwner[i].Equals(ourPathArray[(ourPathArray.Length - genericConstraint.pathFromOwner.Length) + i]))
                //            {
                //                return Array.Empty<IOrType<Yolo, IError>>();
                //                //goto notMatched;
                //            }
                //        }

                //        var target = stack.SkipLast(genericConstraint.pathFromOwner.Length).Last();

                //        return new[] { target.generics.TransformInner(x => x[genericConstraint.index]) };

                //    }).Distinct().ToArray();

                //    if (lookups.Length == 1)
                //    {
                //        toWalk.looksUp = Possibly.Is(lookups.First());
                //    }
                //    else if (lookups.Length > 1)
                //    {
                //        // method [T] [T,T] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // pretty sure we have no idea what "c" is..
                //        // well probabaly method [T1,T2] [T1,T2] where T1: T,t1 and T2: T,t2 
                //        //
                //        // but...
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2 
                //        //
                //        // but I still don't know what index...
                //        // TODO, it's an error for now 
                //        //
                //        // any other pain point:
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        // o > c =: int x
                //        //
                //        // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2, int
                //        // but I have Ta, t2 and, int constraint on the output
                //        // while just Ta, t2 constring s on T2
                //        // how do I know that those collapse??
                //        //
                //        // 
                //        // I think it only works if the constraints are the same length
                //        // you can't do the assignment if you have different numbers of type parameters 
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // c is actually method [T1,T2] [??] where T1: Ta, t1  and T2 : Tb and t2 
                //        // c has an input of Tb, t1 
                //        // c has an output of Ta, t2
                //        //
                //        // c is actually method [T1,T2] [T1&T2,T1&T2]
                //        // once we assume c is method [T1,T2] [??]
                //        // from it's prospective 
                //        // "a" becomes: method [T1,T2] [T2,T1]
                //        // "b" becomes: method [T1,T2] [T1,T2]
                //        // now "c" has an input of T1, T2 
                //        // now "c" has an output of T1, T2

                //        //... anyway
                //        //... I don't even have AND types 

                //        // I think probably a flow from a generic is consider to be from your own generic
                //        // 
                //        // so what about this one?
                //        // 
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        // o > c =: int x
                //        //
                //        // is "c" method [T1:int,T2:int] [T1&T2,T1&T2] ?
                //        // they both don't need the "int" but how would I know which one?
                //        // or maybe "c" is method [T1,T2] [T1 & T2,T1 & T2 & int]
                //        throw new NotImplementedException("I think this should be an AND type, I don't really have those yet");
                //    }



                //    if (toWalk.left.Is(out var leftYolo))
                //    {
                //        Walk(leftYolo, Add(stack, leftYolo), Add(path, OrType.Make<Member, Input, Output, Left, Right>(new Left())));
                //    }
                //    if (toWalk.right.Is(out var rightYolo))
                //    {
                //        Walk(leftYolo, Add(stack, leftYolo), Add(path, OrType.Make<Member, Input, Output, Left, Right>(new Right())));
                //    }
                //    if (toWalk.input.Is(out var inputOrError) && inputOrError.Is1(out var inputYolo))
                //    {
                //        Walk(inputYolo, Add(stack, inputYolo), Add(path, OrType.Make<Member, Input, Output, Left, Right>(new Input())));
                //    }
                //    if (toWalk.output.Is(out var outputOrError) && inputOrError.Is1(out var outputYolo))
                //    {
                //        Walk(outputYolo, Add(stack, outputYolo), Add(path, OrType.Make<Member, Input, Output, Left, Right>(new Output())));
                //    }
                //    if (toWalk.members.Is1(out var members))
                //    {
                //        foreach (var member in members)
                //        {
                //            Walk(member.Item2, Add(stack, member.Item2), Add(path, OrType.Make<Member, Input, Output, Left, Right>(new Member(member.Item1))));
                //        }
                //    }
                //}

                foreach (var (key, value) in cache)
                {
                    if (key.Count() == 1)
                    {
                        value.type.Fill(Convert2(key.First(), value));
                    }
                    else
                    {
                        value.type.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(new FrontEndOrType(
                            value.left.IfElseReturn(x => x, () => throw new Exception("better have a left")).type,
                            value.right.IfElseReturn(x => x, () => throw new Exception("better have a right")).type,
                            value.members.TransformInner(actually => actually.Select(x => new WeakMemberDefinition(
                                Model.Elements.Access.ReadWrite,
                                x.Item1,
                                x.Item2.type)).ToList()),
                            value.input.TransformInner(x => x.SwitchReturns(
                                    y => y.type,
                                    error => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)))),
                            value.output.TransformInner(x => x.SwitchReturns(
                                    y => y.type,
                                    error => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error))))
                            )));
                    }
                }


                IOrType<IFrontendType<IVerifiableType>, IError> Convert2(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> constrains, Yolo yolo)
                {
                    //if (yolo.isGeneric.Is(out var genericTypeParameter)) {
                    //    var res = new GenericTypeParameterPlacholder(genericTypeParameter.index,
                    //       flowNodes[OrType.Make<ITypeProblemNode, IError>(genericTypeParameter.constraint)]
                    //       .GetValueAs(out IVirtualFlowNode _).ToRep()
                    //       .SwitchReturns(
                    //           x=> cache[x].type,
                    //           error=> { IBox<IOrType<IFrontendType<IVerifiableType>, IError>> x = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)); return x; }
                    //       ));
                    //    return OrType.Make<IFrontendType<IVerifiableType>, IError>(res);
                    //}

                    if (constrains.Count == 0)
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType());
                    }

                    var prim = constrains.Primitive();

                    if (prim.Is2(out var error))
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(error);
                    }

                    if (prim.Is1OrThrow().Is(out var _))
                    {
                        // I'd like to not pass "this" here
                        // the primitive convert willn't use it
                        // but... this isn't really ready to use
                        // it's method are not defined at this point in time
                        var source = constrains.Single().Is2OrThrow().primitiveFlowNode2.type;
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(source.Converter.Convert(this, source).Is3OrThrow());
                    }

                    if (yolo.members.Is2(out var e4))
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(e4);
                    }
                    var members = yolo.members.Is1OrThrow();

                    if (constrains.Input().Is(out var inputOr))
                    {
                        if (inputOr.Is2(out var e2))
                        {
                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(e2);
                        }
                    }
                    var input = inputOr?.Is1OrThrow();


                    if (constrains.Output().Is(out var outputOr))
                    {
                        if (outputOr.Is2(out var e3))
                        {
                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(e3);
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

                    if (yolo.generics.Is1(out var generics) && generics.Any())
                    {
                        //var i = 0;
                        //var convertedGenerics = generics.Select(x => new GenericTypeParameterPlacholder(i++, cache[x.Flatten()].type)).ToArray();


                        if (input != default && output != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                 OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new GenericMethodType(
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(input.Flatten(), positions),
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(output.Flatten(), positions),
                                        generics.Select(x => OrType.Make<IGenericTypeParameterPlacholder, IError>(x)).ToArray()));
                        }


                        if (input != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new GenericMethodType(
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(input.Flatten(), positions),
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                        generics.Select(x => OrType.Make<IGenericTypeParameterPlacholder, IError>(x)).ToArray()));
                        }

                        if (output != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new GenericMethodType(
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(output.Flatten(), positions),
                                        generics.Select(x => OrType.Make<IGenericTypeParameterPlacholder, IError>(x)).ToArray()));
                        }

                        return
                            OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                new GenericMethodType(
                                    new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                    new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                    generics.Select(x => OrType.Make<IGenericTypeParameterPlacholder, IError>(x)).ToArray()));
                    }

                    if (input != default && output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(input.Flatten(), positions),
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(output.Flatten(), positions)));
                    }


                    if (input != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(input.Flatten(), positions),
                                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType()))));
                    }

                    if (output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(output.Flatten(), positions)));
                    }

                    // if it has members it must be a scope
                    if (members.Any() || constrains.Any(x => x.Is4(out HasMembers _)))
                    {
                        var external = constrains.Where(x => x.Is6(out IsExternal _)).Select(x => x.Is6OrThrow()).ToArray();

                        if (external.Length > 1)
                        {
                            throw new Exception("what does that mean?!");
                        }

                        if (external.Length == 1)
                        {
                            var interfaceType = external.Single().interfaceType;

                            if (members.Count != interfaceType.Members.Count)
                            {
                                throw new Exception("these should have the same number of members!");
                            }

                            var dict = members.ToDictionary(x => x.Item1, x => x);

                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(
                               new ExternalHasMembersType(interfaceType, interfaceType.Members.Select(x => new WeakExternslMemberDefinition(
                                   x,
                                   dict[x.Key].Item2.type)).ToList()));
                        }

                        //if (yolo.external.Is(out var interfaceType))
                        //{

                        //    // we have one member list from the type problem
                        //    // and one member list from the external deffinition
                        //    // we need to join them together

                        //    if (members.Count != interfaceType.Members.Count)
                        //    {
                        //        throw new Exception("these should have the same number of members!");
                        //    }

                        //    var dict = members.ToDictionary(x => x.Item1, x => x);

                        //    return OrType.Make<IFrontendType<IVerifiableType>, IError>(
                        //       new ExternalHasMembersType(interfaceType, interfaceType.Members.Select(x => new WeakExternslMemberDefinition(
                        //           x,
                        //           dict[x.Key].Item2.type)).ToList()));

                        //}

                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new HasMembersType(new WeakScope(members.Select(x => new WeakMemberDefinition(
                                Access.ReadWrite,
                                x.Item1,
                                x.Item2.type)).ToList())));
                    }

                    return OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType());
                }



            }

            //public IIsPossibly<EqualableHashSet<Tpn.TypeProblem2.GenericTypeParameter>> GetGenericTypeParameter(EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key) {


            //    var justGenericConstraints = key.Select(x => x
            //            .Where(y => y.Is5(out IsGeneric _))
            //            .Select(y => y.Is5OrThrow())
            //            .ToArray())
            //        .ToArray();

            //    var intersect = justGenericConstraints.First();

            //    foreach (var item in justGenericConstraints.Skip(1))
            //    {
            //        intersect = item.Intersect(intersect).ToArray();
            //    }

            //    if (intersect.Length == 0)
            //    {
            //        return Possibly.IsNot<EqualableHashSet<Tpn.TypeProblem2.GenericTypeParameter>>();
            //    }

            //    //if (intersect.Length > 1)
            //    //{
            //    //    throw new Exception("uhh, we are move than one generic? 😬");
            //    //}

            //    return Possibly.Is(new EqualableHashSet<Tpn.TypeProblem2.GenericTypeParameter>(intersect.Select(x=>x.genericTypeParameter).ToHashSet()));
            //}

            //private ConcurrentIndexed<EqualableHashSet<Tpn.TypeProblem2.GenericTypeParameter>, GenericTypeParameterPlacholder> genericCache = new ConcurrentIndexed<EqualableHashSet<TypeProblem2.GenericTypeParameter>, GenericTypeParameterPlacholder>();

            Box<IOrType<IFrontendType<IVerifiableType>, IError>> GetFromCacheReplaceGenericConstrainsWithTheGeneric(
                EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key,
                Dictionary<(Yolo, IOrType<Member, Input, Output, Left, Right, PrivateMember>), List<Yolo>> positions)
            {

                var justGenericConstraints = key.Select(x => x
                    .Where(y => y.Is5(out IsGeneric _))
                    .Select(y => y.Is5OrThrow())
                    .ToArray())
                .ToArray();

                var intersect = justGenericConstraints.First();

                foreach (var item in justGenericConstraints.Skip(1))
                {
                    intersect = item.Intersect(intersect).ToArray();
                }

                var yolo = cache[key];

                var lookups = intersect.SelectMany(genericConstraint =>
                {

                    var currents = new List<Yolo> { yolo };

                    foreach (var pathPart in genericConstraint.pathFromOwner.Reverse())
                    {
                        var next = new List<Yolo>();
                        foreach (var item in currents)
                        {
                            next.AddRange(positions[(item, pathPart)]);
                        }
                        currents = next;
                    }

                    // I am curious to see if there is a case wehre Is1OrThrow will hit
                    // seems like it shouldn't happen we walked up the generic look up path, there should a type parameter defined there 
                    return currents.Select(current => current.generics.Is1OrThrow()[genericConstraint.index]);

                    //var ourPathArray = path.ToArray();

                    //if (genericConstraint.pathFromOwner.Length > ourPathArray.Length)
                    //{
                    //    return Array.Empty<IOrType<GenericTypeParameterPlacholder, IError>>();
                    //    //goto notMatched;
                    //}

                    //for (int i = genericConstraint.pathFromOwner.Length - 1; i >= 0; i--)
                    //{
                    //    if (!genericConstraint.pathFromOwner[i].Equals(ourPathArray[(ourPathArray.Length - genericConstraint.pathFromOwner.Length) + i]))
                    //    {
                    //        return Array.Empty<IOrType<GenericTypeParameterPlacholder, IError>>();
                    //        //goto notMatched;
                    //    }
                    //}

                    //var target = stack.SkipLast(genericConstraint.pathFromOwner.Length).Last();

                    //return new[] { target.generics.TransformInner(x => x[genericConstraint.index]) };

                }).Distinct().ToArray();

                if (lookups.Length == 0) {
                    return cache[key].type;
                }
                if (lookups.Length == 1)
                {
                    return new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<GenericTypeParameterPlacholder, IError > (lookups.First()));
                }
                else 
                {
                    // method [T] [T,T] a;
                    // method [t1,t2] [t1, t2] b;
                    // c =: a;
                    // c =: b;
                    //
                    // pretty sure we have no idea what "c" is..
                    // well probabaly method [T1,T2] [T1,T2] where T1: T,t1 and T2: T,t2 
                    //
                    // but...
                    //
                    // method [Ta, Tb] [Tb,Ta] a;
                    // method [t1,t2] [t1, t2] b;
                    // c =: a;
                    // c =: b;
                    //
                    // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2 
                    //
                    // but I still don't know what index...
                    // TODO, it's an error for now 
                    //
                    // any other pain point:
                    //
                    // method [Ta, Tb] [Tb,Ta] a;
                    // method [t1,t2] [t1, t2] b;
                    // c =: a;
                    // c =: b;
                    // o > c =: int x
                    //
                    // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2, int
                    // but I have Ta, t2 and, int constraint on the output
                    // while just Ta, t2 constring s on T2
                    // how do I know that those collapse??
                    //
                    // 
                    // I think it only works if the constraints are the same length
                    // you can't do the assignment if you have different numbers of type parameters 
                    //
                    // method [Ta, Tb] [Tb,Ta] a;
                    // method [t1,t2] [t1, t2] b;
                    // c =: a;
                    // c =: b;
                    //
                    // c is actually method [T1,T2] [??] where T1: Ta, t1  and T2 : Tb and t2 
                    // c has an input of Tb, t1 
                    // c has an output of Ta, t2
                    //
                    // c is actually method [T1,T2] [T1&T2,T1&T2]
                    // once we assume c is method [T1,T2] [??]
                    // from it's prospective 
                    // "a" becomes: method [T1,T2] [T2,T1]
                    // "b" becomes: method [T1,T2] [T1,T2]
                    // now "c" has an input of T1, T2 
                    // now "c" has an output of T1, T2

                    //... anyway
                    //... I don't even have AND types 

                    // I think probably a flow from a generic is consider to be from your own generic
                    // 
                    // so what about this one?
                    // 
                    // method [Ta, Tb] [Tb,Ta] a;
                    // method [t1,t2] [t1, t2] b;
                    // c =: a;
                    // c =: b;
                    // o > c =: int x
                    //
                    // is "c" method [T1:int,T2:int] [T1&T2,T1&T2] ?
                    // they both don't need the "int" but how would I know which one?
                    // or maybe "c" is method [T1,T2] [T1 & T2,T1 & T2 & int]
                    throw new NotImplementedException("I think this should be an AND type, I don't really have those yet");
                }

                //var res = cache[key];
                //if (res.looksUp.Is(out var orType))
                //{
                //    return orType.SwitchReturns(x => {
                //            if (res.genericTypeParameterPlaceholder != null)
                //            {
                //                return new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<GenericTypeParameterPlacholder, IError>(x.genericTypeParameterPlaceholder));
                //            }
                //            throw new Exception("is that ok? if it looks up to something that something should be a generic, right?");
                //        },
                //        error => new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)));
                //    //orType.TransformInner(x => );
                //}
                //if (res.genericTypeParameterPlaceholder!= null) {
                //    return new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<GenericTypeParameterPlacholder, IError>(res.genericTypeParameterPlaceholder));
                //}

                //return res.type;
                //var justGenericConstraints = key.Select(x => x
                //        .Where(y => y.Is5(out IsGeneric _))
                //        .Select(y => y.Is5OrThrow())
                //        .ToArray())
                //    .ToArray();



                ////if (res.isGenericConstraintFroRealized.Is(out var alreadyGotIt))
                ////{
                ////    return alreadyGotIt;
                ////}

                //var possiblyGenericTypeParameter = GetGenericTypeParameter(key);

                //if (possiblyGenericTypeParameter.Is(out var genericTypeParameter)) {
                //    var groupedParms = genericTypeParameter.GroupBy(x => x.index).ToArray();
                //    if (groupedParms.Count() > 1) {
                //        // method [T] [T,T] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // pretty sure we have no idea what "c" is..
                //        // well probabaly method [T1,T2] [T1,T2] where T1: T,t1 and T2: T,t2 
                //        //
                //        // but...
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2 
                //        //
                //        // but I still don't know what index...
                //        // TODO, it's an error for now 
                //        //
                //        // any other pain point:
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        // o > c =: int x
                //        //
                //        // "c" is method [T1,T2] [T1,T2] where T1: Tb, t1 and T2: Ta,t2, int
                //        // but I have Ta, t2 and, int constraint on the output
                //        // while just Ta, t2 constring s on T2
                //        // how do I know that those collapse??
                //        //
                //        // 
                //        // I think it only works if the constraints are the same length
                //        // you can't do the assignment if you have different numbers of type parameters 
                //        //
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        //
                //        // c is actually method [T1,T2] [??] where T1: Ta, t1  and T2 : Tb and t2 
                //        // c has an input of Tb, t1 
                //        // c has an output of Ta, t2
                //        //
                //        // c is actually method [T1,T2] [T1&T2,T1&T2]
                //        // once we assume c is method [T1,T2] [??]
                //        // from it's prospective 
                //        // "a" becomes: method [T1,T2] [T2,T1]
                //        // "b" becomes: method [T1,T2] [T1,T2]
                //        // now "c" has an input of T1, T2 
                //        // now "c" has an output of T1, T2

                //        //... anyway
                //        //... I don't even have AND types 

                //        // I think probably a flow from a generic is consider to be from your own generic
                //        // 
                //        // so what about this one?
                //        // 
                //        // method [Ta, Tb] [Tb,Ta] a;
                //        // method [t1,t2] [t1, t2] b;
                //        // c =: a;
                //        // c =: b;
                //        // o > c =: int x
                //        //
                //        // is "c" method [T1:int,T2:int] [T1&T2,T1&T2] ?
                //        // they both don't need the "int" but how would I know which one?
                //        // or maybe "c" is method [T1,T2] [T1 & T2,T1 & T2 & int]

                //        return new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(Error.Other("")));
                //    }

                //    genericCache.GetOrAdd(genericTypeParameter, )

                //    var innerRes = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new GenericTypeParameterPlacholder(groupedParms.First().Key, res.type)));
                //    //res.isGenericConstraintFroRealized = Possibly.Is(innerRes);
                //    return innerRes;
                //}

                //return res.type;
            }
            //public static IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Method type)
            //{
            //    var res = type.Generics.Select(x => {
            //        if (x.Key.SafeIs(out NameKey nameKey)) {
            //            return OrType.Make<NameKey, ImplicitKey>(nameKey);
            //        }
            //        if (x.Key.SafeIs(out ImplicitKey implicitKey))
            //        {
            //            return OrType.Make<NameKey, ImplicitKey>(nameKey);
            //        }

            //        // it's weird that I have x.Key and x.Value.Key
            //        // and they have different types...
            //        throw new Exception("this might or might not happen, let's work it work when it does");

            //    }).ToArray();

            //    if (res.Length != 0 )
            //    {
            //        return Possibly.Is(res);
            //    }

            //    return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            //}


            //public static IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
            //{
            //    var res = type.Generics.Select(x => {
            //        if (x.Key.SafeIs(out NameKey nameKey))
            //        {
            //            return OrType.Make<NameKey, ImplicitKey>(nameKey);
            //        }
            //        if (x.Key.SafeIs(out ImplicitKey implicitKey))
            //        {
            //            return OrType.Make<NameKey, ImplicitKey>(nameKey);
            //        }

            //        // it's weird that I have x.Key and x.Value.Key
            //        // and they have different types...
            //        throw new Exception("this might or might not happen, let's work it work when it does");

            //    }).ToArray();

            //    if (res.Length != 0)
            //    {
            //        return Possibly.Is(res);
            //    }

            //    return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            //}

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetType(Tpn.ILookUpType from)
            {
                // this little block makes undefined type undefined
                // at time of writing if you uncommented it
                // undefined types are just infered types
                // a tempting notion
                if (from.LooksUp.Is(out var value))
                {
                    if (value.Is7(out var error))
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(error);
                    }
                }
                else
                {
                    throw new Exception("it should be set by this point? right");
                }

                return from.LooksUp.GetOrThrow()
                    .SwitchReturns<IOrType<IFrontendType<IVerifiableType>, IError>>(
                        x => {
                            if (x.Generics.Any()) {
                                return GetGenericMethodType(x);
                            }
                            return GetMethodType(x);
                        },
                        x => GetHasMemberType(x),
                        x => GetObjectType(x),
                        x => GetOrType(x),
                        x => GetInferredType(x),
                        x => GetGenericPlaceholder(x),
                        x => OrType.Make<IFrontendType<IVerifiableType>, IError>(x));
            }

            internal IOrType<FrontEndOrType, IError> GetOrType(TypeProblem2.OrType from) =>
               cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue()
                .TransformInner(y => y.CastTo<FrontEndOrType>());

            internal IOrType<MethodType, IError> GetMethodType(TypeProblem2.MethodType from) =>
               cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue()
                .TransformInner(y => y.CastTo<MethodType>());

            //GenericMethodType
            internal IOrType<GenericMethodType, IError> GetGenericMethodType(TypeProblem2.MethodType from) =>
               cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue()
                .TransformInner(y => y.CastTo<GenericMethodType>());

            internal IOrType<HasMembersType, IError> GetHasMemberType(TypeProblem2.Type from) =>
                cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue()
                .TransformInner(y => y.CastTo<HasMembersType>());

            internal IOrType<HasMembersType, IError> GetObjectType(TypeProblem2.Object from) =>
               cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue()
                .TransformInner(y => y.CastTo<HasMembersType>());

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetInferredType(TypeProblem2.InferredType from) {
                if (from.constraintFor.Is(out var genericTypeParameter)) {
                    return GetGenericPlaceholder(genericTypeParameter);
                }
                return cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()].type
                .GetValue();
            }

            internal IOrType<GenericTypeParameterPlacholder, IError> GetGenericPlaceholder(TypeProblem2.GenericTypeParameter from) {
                var yolo = cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from.owner.GetValueAs(out ITypeProblemNode _))]
                   .GetValueAs(out IConstraintSoruce _)
                   .GetExtendedConstraints()
                   .Flatten()];
                return yolo.generics.TransformInner(array=> array[from.index]);
            }

            // this also ends up managing weak scopes that aren't types
            private readonly ConcurrentIndexed<Tpn.IHavePrivateMembers, WeakScope> nonTypeScopes = new ConcurrentIndexed<IHavePrivateMembers, WeakScope>();

            internal WeakScope GetWeakScope(Tpn.IHavePrivateMembers from) =>
                nonTypeScopes.GetOrAdd(from, () =>
                    new WeakScope(from.PrivateMembers.Select(x => new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(GetType(x.Value)))).ToList()));

            internal bool TryGetMember(IStaticScope scope, IKey key, [NotNullWhen(true)] out IOrType<WeakMemberDefinition, IError>? res)
            {
                if (flowNodes2.TryGetValue(OrType.Make<ITypeProblemNode, IError>(scope), out var flowNode))
                {
                    var rep = flowNode.GetValueAs(out IConstraintSoruce _).GetExtendedConstraints().Flatten();
                    var type = GetFromCacheReplaceGenericConstrainsWithTheGeneric(rep, positions).GetValue();
                    if (type.Is1(out var reallyType))
                    {
                        var maybeMember = reallyType.TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>());

                        if (maybeMember.Is1(out var member))
                        {
                            res = member;
                            return true;
                        }
                        else if (maybeMember.Is2(out var _))
                        {
                            res = default;
                            return false;
                        }
                        else
                        {
                            res = OrType.Make<WeakMemberDefinition, IError>(maybeMember.Is3OrThrow());
                            return true;
                        }

                    }
                    else
                    {
                        res = OrType.Make<WeakMemberDefinition, IError>(type.Is2OrThrow());
                        return true;
                    }
                }


                if (scope is Tpn.IHavePrivateMembers privateMembers)
                {
                    var matches = GetWeakScope(privateMembers).membersList.Where(x => x.Key.Equals(key)).ToArray();

                    if (matches.Length > 1)
                    {
                        throw new Exception("that's not right");
                    }

                    if (matches.Length == 0)
                    {
                        res = default;
                        return false;
                    }

                    res = OrType.Make<WeakMemberDefinition, IError>(matches.First());
                    return true;
                }


                // should pass in an more descritive type so I don't end up with this weird exception
                throw new Exception("I... don't think it should get here.");
            }

            // I am thinking maybe the conversion layer is where we should protect against something being converted twice
            // everything can set a box on the first pass
            // and return the box on the next passes
        }
    }

    // extension because that could make these context dependent
    // you shoul only use this after the problem is solved
    static class RepExtension
    {

        // this is basically
        // A and B and (C or D) to (A and B and C) or (A and B and D)
        // returns OR AND
        public static EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> Flatten(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>> self)
        {
            // AND OR AND 
            var andOrAnd = self
                .Where(x => x.Is4(out var _))
                .Select(x => x.Is4OrThrow())
                .Select(x => x.source.or
                    // OR AND
                    .SelectMany(y => y.GetValueAs(out IConstraintSoruce _).GetExtendedConstraints().Flatten())
                    .ToArray())
                .ToHashSet();

            var orAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() {
                new List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>()
            };


            foreach (var orAnd in andOrAnd)
            {

                var nextOrAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() { };

                foreach (var andRes in orAndRes)
                {
                    foreach (var and in orAnd)
                    {
                        if (andRes.All(andResItem => and.All(andItem => andResItem.GetValueAs(out IConstraint _).ExtendedIsCompatible(andItem))))
                        {
                            var list = andRes.ToList();
                            list.AddRange(and);
                            nextOrAndRes.Add(list);
                        }
                    }
                    orAndRes = nextOrAndRes;
                }
            }

            var shared = self.Where(x => !x.Is4(out var _))
                .Select(x => x.SwitchReturns(
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    _ => throw new Exception("I just said not that!"),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y),
                    y => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>(y)))
                .ToList();
            {
                var nextOrAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>() { };

                foreach (var andRes in orAndRes)
                {
                    if (andRes.All(andResItem => shared.All(sharedItem => andResItem.GetValueAs(out IConstraint _).ExtendedIsCompatible(sharedItem))))
                    {
                        andRes.AddRange(shared);
                        nextOrAndRes.Add(andRes);
                    }
                }
                orAndRes = nextOrAndRes;
            }

            return new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>>(
                orAndRes.Select(x => new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>(x.ToHashSet())).ToHashSet());
        }

        public static bool ExtendedIsCompatible(this IConstraint constraint, IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal> item)
        {
            return item.SwitchReturns(
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => constraint.IsCompatible(OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x), new List<UnorderedPair<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>>>()),
                x => true,
                x => true);
        }

        //public static IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric> Broaden(this IOrType<MustHave, MustBePrimitive, GivenPathThen,  HasMembers, IsGenericRestraintFor, IsExternal> self) {
        //    return self.SwitchReturns(
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x),
        //        x => OrType.Make<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric>(x));
        //}

        public static IOrType<IIsPossibly<Guid>, IError> Primitive(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            var primitives = self.SelectMany(x =>
            {
                if (x.Is2(out var v2))
                {
                    return new[] { v2 };
                }
                return Array.Empty<MustBePrimitive>();
            }).ToArray();

            if (primitives.Length == self.Count())
            {
                var groupedPrimitives = primitives.GroupBy(x => x.primitive).ToArray();
                if (groupedPrimitives.Length == 1)
                {
                    return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.Is(groupedPrimitives.First().Key));
                }
                return OrType.Make<IIsPossibly<Guid>, IError>(Error.Other("multiple primitives types..."));
            }

            if (primitives.Any())
            {
                return OrType.Make<IIsPossibly<Guid>, IError>(Error.Other("primitives and non primitives"));
            }
            return OrType.Make<IIsPossibly<Guid>, IError>(Possibly.IsNot<Guid>());
        }

        public static IOrType<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError> Members(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is1(out var _))
                .GroupBy(x => x.path.Is1OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is1(out var _))
                .GroupBy(x => x.path.Is1OrThrow());

            var list = new List<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                var extened = false;
                {
                    var sources = mustHaves.Select(x => x.dependent).ToHashSet();
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            sources.Add(givenPath.dependent);
                        }
                    }

                    if (sources.Count() == 1)
                    {
                        extened = true;
                    }
                }

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in extened ? mustHave.dependent.GetExtendedConstraints()
                        : mustHave.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in extened ? givenPath.dependent.GetExtendedConstraints()
                                    : givenPath.dependent.GetConstraints().Select(x => x.Broaden()))
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                list.Add(new KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>(mustHaves.Key.key, equalableSet));
            }

            return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(list);
        }


        public static IOrType<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError> PrivateMembers(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is5(out var _))
                .GroupBy(x => x.path.Is5OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is5(out var _))
                .GroupBy(x => x.path.Is5OrThrow());

            var list = new List<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                var extened = false;
                {
                    var sources = mustHaves.Select(x => x.dependent).ToHashSet();
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            sources.Add(givenPath.dependent);
                        }
                    }

                    if (sources.Count() == 1)
                    {
                        extened = true;
                    }
                }

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in extened ? mustHave.dependent.GetExtendedConstraints()
                        : mustHave.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in extened ? givenPath.dependent.GetExtendedConstraints()
                                    : givenPath.dependent.GetConstraints().Select(x => x.Broaden()))
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                list.Add(new KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>(mustHaves.Key.key, equalableSet));
            }

            return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>>, IError>(list);
        }

        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>> Input(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(error));
            }

            var mustHaves = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is2(out var _))
                .ToArray();

            if (mustHaves.Any())
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                var givenPaths = self
                    .SelectMany(x =>
                    {
                        if (x.Is3(out var v3))
                        {
                            return new[] { v3 };
                        }
                        return Array.Empty<GivenPathThen>();
                    })
                    .Where(x => x.path.Is2(out var _));

                // I don't really think I got this right 
                var extened = false;
                {
                    var sources = mustHaves.Select(x => x.dependent).ToHashSet();

                    foreach (var givenPath in givenPaths)
                    {
                        sources.Add(givenPath.dependent);
                    }

                    if (sources.Count() == 1)
                    {
                        extened = true;
                    }
                }


                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in extened ? mustHave.dependent.GetExtendedConstraints()
                                    : mustHave.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in extened ? givenPath.dependent.GetExtendedConstraints()
                                    : givenPath.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set)));

            }

            return Possibly.IsNot<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>>();
        }


        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>> Output(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(error));
            }

            var mustHaves = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is3(out var _))
                .ToArray();

            if (mustHaves.Any())
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();


                var givenPaths = self
                    .SelectMany(x =>
                    {
                        if (x.Is3(out var v3))
                        {
                            return new[] { v3 };
                        }
                        return Array.Empty<GivenPathThen>();
                    })
                    .Where(x => x.path.Is3(out var _));

                var extened = false;
                {
                    var sources = mustHaves.Select(x => x.dependent).ToHashSet();

                    foreach (var givenPath in givenPaths)
                    {
                        sources.Add(givenPath.dependent);
                    }

                    if (sources.Count() == 1)
                    {
                        extened = true;
                    }
                }

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in extened ? mustHave.dependent.GetExtendedConstraints()
                                    : mustHave.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in extened ? givenPath.dependent.GetExtendedConstraints()
                                    : givenPath.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set)));

            }
            return Possibly.IsNot<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>, IError>>();
        }

        public static IOrType<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError> Generics(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self)
        {

            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(error);
            }

            var givenPathDictionary = self
                .SelectMany(x =>
                {
                    if (x.Is3(out var v3))
                    {
                        return new[] { v3 };
                    }
                    return Array.Empty<GivenPathThen>();
                })
                .Where(x => x.path.Is4(out var _))
                .GroupBy(x => x.path.Is4OrThrow()).ToDictionary(x => x.Key, x => x);

            var mustHaveGroup = self
                .SelectMany(x =>
                {
                    if (x.Is1(out var v1))
                    {
                        return new[] { v1 };
                    }
                    return Array.Empty<MustHave>();
                })
                .Where(x => x.path.Is4(out var _))
                .GroupBy(x => x.path.Is4OrThrow());

            var pairs = new List<(int, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>)>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>();

                var extened = false;
                {
                    var sources = mustHaves.Select(x => x.dependent).ToHashSet();
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            sources.Add(givenPath.dependent);
                        }
                    }

                    if (sources.Count() == 1)
                    {
                        extened = true;
                    }
                }

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in extened ? mustHave.dependent.GetExtendedConstraints()
                                    : mustHave.dependent.GetConstraints().Select(x => x.Broaden()))
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in extened ? givenPath.dependent.GetExtendedConstraints()
                                    : givenPath.dependent.GetConstraints().Select(x => x.Broaden()))
                            {
                                set.Add(constraint);
                            }
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>(set);

                pairs.Add((mustHaves.Key.index, equalableSet));
            }

            if (!pairs.Any())
            {
                return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(Array.Empty<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>());
            }

            if (pairs.Select(x => x.Item1).Max() != pairs.Count() - 1)
            {
                // I think this is an exception and not an IError
                // you really shouldn't be able to have disconunious generic constraints
                throw new Exception("the generic constriants are discontinious...");
            }

            return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint, HasMembers, IsGeneric, IsExternal>>>, IError>(pairs.OrderBy(x => x.Item1).Select(x => x.Item2).ToArray());
        }

        private static bool ErrorCheck(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>> self, [NotNullWhen(true)] out IError? error)
        {
            if (self.Any(x => x.Is2(out var _)) && !self.All(x => x.Is2(out var _)))
            {
                error = Error.Other("primitives and non primitives");
                return true;
            }

            if (self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is1(out var _)) &&
                (
                    self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is2(out var _) ||
                    self.Any(x => x.Is1(out var mustHave) && mustHave.path.Is3(out var _)))
                ))
            {
                error = Error.Other("is it a method or is it an object");
                return true;
            }
            error = default;
            return false;
        }
    }
}


// this generic thing is pretty complex
// I think they do flow

// there is sort of two contexts
// 1 - 
//
// method [T] [T,T] a;
// c =: a;
//
// c is method [T] [T,T] but it has it's own T
// 
// 2 - 
// 
// method [T] [T,T] input {
//  a =: input
//  return input;
// }
//
// a is T
//
// in both cases the genericness flows

// are these two case different constraint?
//
// I need to think through a few more cases...
//
// method [T1] [T1, method [T] [T, T1]] input {
//      method[T][T, T1] a;
//      c =: a;
//      a return;
// }
//
// c is method [T] [T,T1] it has it's own T but T1 is shared
// so..
// if I am generic, I make other things the same generic
// if I have a generic I make other things have their own gernic
// but those are just different ways of saying the same thing...
// from the prospective of a's input, I'm generic
// from the prospective of a, I have a generic
//
// I need to name my cases "has" vs "is"
// 
// I think the key this is how they look up where they come from
// it is sort of a inverse path
// a's input looks up to [input] and then takes the first generic
// a's output looks up to [output, a] and then takes the first generic ... except it is actually just a different kind of constraint
//
// so really we have relationships
// like a's input is it's first generic
// maybe the constrait defining this relationship live on "a"?
// the thing about how it works now is: it can't flow very far
// a generic method has to be realized for befor you can call it
// so it can only flow by assigning to "a"
// 
// a's output is just T1




// ugh, idk
// method[T][int, method [T1][T,T1]] x
// method[T][string, method [T1][T,T1]] y
// 
// the inner method here combine
// but the outer methods don't 
// and so the inner methods shouldn't 
// 
// if we had
// method[T][int, method [T1][T,T1]] x
// method[T][int, method [T1][T,T1]] y
// then everythig combines but probably nothing would break if it didn't 
// it is going to be possible to assigne them to each other, even if they aren't the same object
// 
// only generics should be context dependent
// they can exist on many paths
// but all paths should look up to the same thing
// 
// so the source is one of the features that defines identity for generic constraints 












// what about external types?
// 