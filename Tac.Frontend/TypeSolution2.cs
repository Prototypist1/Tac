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


            private readonly ConcurrentIndexed<(Yolo,EqualableReadOnlyList<Yolo>, bool isConstraint), Box<IOrType<IFrontendType<IVerifiableType>, IError>>> typeByYoloAndContext = new();
            private readonly ConcurrentIndexed<(Yolo, EqualableReadOnlyList<Yolo>, int), IOrType<GenericTypeParameterPlacholder, IError>> genericByYoloContextAndIndex = new();

            private class Yolo
            {
                public readonly EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key;

                internal IOrType<IReadOnlyList<(IKey, Yolo)>, IError>? members;
                internal IOrType<IReadOnlyList<(IKey, Yolo)>, IError>? privateMembers;

                internal IIsPossibly<IOrType<Yolo, IError>>? output;
                internal IIsPossibly<IOrType<Yolo, IError>>? input;

                internal string? debugName;

                // for or types
                internal IIsPossibly<Yolo>? left;
                internal IIsPossibly<Yolo>? right;
                internal IOrType<Yolo[], IError>? genericsConstraints;
                internal IIsDefinately<IOrType<Yolo, IError>> looksUp;
                internal GenericTypeParameterPlacholder? genericTypeParameterPlaceholder;
                //internal readonly Box<IOrType<IFrontendType<IVerifiableType>, IError>> type = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();

                public Yolo(EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, HasMembers, IsGeneric, IsExternal>>> key)
                {
                    this.key = key ?? throw new ArgumentNullException(nameof(key));
                }

                public override bool Equals(object? obj)
                {
                    return obj is Yolo yolo &&
                           key.Equals(yolo.key);
                }

                public override int GetHashCode()
                {
                    return key.GetHashCode();
                }

                public override string ToString()
                {
                    return debugName;
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

                    if (flowNode2.Key.Is1(out var typeProblemNode)) {
                        yolo.debugName += " " + typeProblemNode.DebugName;
                    }

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

                            myBox.genericsConstraints = OrType.Make<Yolo[], IError>(Array.Empty<Yolo>());
                            
                            myBox.members = OrType.Make<IReadOnlyList<(IKey, Yolo)>, IError>(Array.Empty<(IKey, Yolo)>());
                            myBox.privateMembers = OrType.Make<IReadOnlyList<(IKey, Yolo)>, IError>(Array.Empty<(IKey, Yolo)>());
                            myBox.input = Possibly.IsNot<IOrType<Yolo, IError>>();
                            myBox.output = Possibly.IsNot<IOrType<Yolo, IError>>();
                        }
                        else
                        {
                            myBox.left = Possibly.IsNot<Yolo>();
                            myBox.right = Possibly.IsNot<Yolo>();

                            myBox.genericsConstraints = equalableHashSet.First().Generics().TransformInner(generics => generics.Select(generic => GetOrAdd(generic.Flatten())).ToArray());

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


                var simplifiedPositions = positions.GroupBy(x => x.Key.Item1).ToDictionary(x => x.Key, x => x.SelectMany(y => y.Value).ToList());

                foreach (var (key, value) in cache)
                {
                    var paths = Paths(value, simplifiedPositions);
                    foreach (var path in paths)
                    {
                        CachedConvert3(value, path, new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>());
                    }
                }

            }

            // I think I need a seprate generic cache
            // souce, index, context -> 

            private IOrType<GenericTypeParameterPlacholder, IError> LookUpGeneric(Yolo from, IEnumerable<Yolo> context, int index, Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>> alreadyConverting) {
                // myBox.generics = equalableHashSet.First().Generics().TransformInner(generics => generics.Select(generic => new GenericTypeParameterPlacholder(i++,  GetOrAdd(generic.Flatten()).type)).ToArray());
                //
                for (int i = 0; i < context.Count() +1; i++)
                {
                    var list = new EqualableReadOnlyList<Yolo>(context.Take(i).ToArray());
                    if (genericByYoloContextAndIndex.TryGetValue((from, list, index), out var res))
                    {
                        return res;
                    }
                }

                return from.genericsConstraints.SwitchReturns(
                    genericsConstraints => 
                    {
                        var res = OrType.Make<GenericTypeParameterPlacholder, IError>(new GenericTypeParameterPlacholder(index, CachedConvert3(genericsConstraints[index], context, alreadyConverting, true).Item1));

                        genericByYoloContextAndIndex.AddOrThrow((from, new EqualableReadOnlyList<Yolo>(context.ToArray()), index), res);
                        return res;
                    },
                    error => OrType.Make<GenericTypeParameterPlacholder, IError>(error));
            }

            private static bool DefinesGenerics(Yolo context, IsGeneric[] isGenerics, out int res) {
                var acceptedSource = isGenerics.Select(x => x.source).ToHashSet();

                var couldBe = context.key
                    .Select(x=>x
                            .Where(y => y.Is1(out MustHave mustHave) && mustHave.path.Is4(out Generic _) && acceptedSource.Contains(mustHave.dependent))
                            .Select(y => y.Is1OrThrow())
                            .ToArray())
                    .ToArray();

                var intersect = couldBe.First();

                foreach (var item in couldBe.Skip(1))
                {
                    intersect = intersect.Intersect(item).ToArray();
                }

                if (intersect.Length == acceptedSource.Count) {
                    var indexes = intersect.Select(x => x.path.Is4OrThrow().index).Distinct().ToArray();
                    if (indexes.Length == 1)
                    {
                        res = indexes.First();
                        return true;
                    }
                    else if (indexes.Length > 1)
                    {
                        // blah blah blah, it's an and type, we don't support those 

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
                        // {4C0E59B1-11EC-404B-9D57-760F2205E50C}
                        throw new NotImplementedException("I think this should be an AND type, I don't really have those yet");
                    }
                    else {
                        // ... it's zero.. ?
                        // what does that mean?
                        throw new Exception("huh?");
                    }

                }
                res = default;
                return false;
            }

            private static bool TryGetFromAndIndex(IEnumerable<Yolo> context, IsGeneric[] justGenericConstraints, out int index,[MaybeNullWhen(false)] out Yolo from) {

                foreach (var item in context.Reverse())
                {
                    if (DefinesGenerics(item, justGenericConstraints, out int innerIndex))
                    {
                        index = innerIndex;
                        from = item;
                        return true;
                    }
                }
                index = default;
                from = default;
                return false;
            }

            /// <summary>
            /// <param name="isConstraint">by default this will return GenericTypeParameterPlacholder when you look up a constrain. set this to true to true the constraint.</param>
            /// <returns></returns>
            private (Box<IOrType<IFrontendType<IVerifiableType>, IError>>, IReadOnlyList<Yolo>) CachedConvert3(
                Yolo yolo,
                IEnumerable<Yolo> context,
                Dictionary<(Yolo,bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>> alreadyConverting, // this is to stop stack overflows 
                bool isConstraint = false 
                ) {

                // we don't need to track the context of what we are already converting
                // they are bound to be a more specific context than what we are converting
                // but they should be defined at our context or able
                // a.a.a.a is just something reference itself, if I track context these would all have different contexts
                if (alreadyConverting.TryGetValue((yolo, !isConstraint), out var alreadyBox)) {
                    return (alreadyBox, Array.Empty<Yolo>()/*we return the empty this, not to say this is really exists at the root, but just what we don't offer an opion where it exists*/);
                }

                for (int i = 0; i < context.Count()+1; i++)
                {
                    var list = new EqualableReadOnlyList<Yolo>(context.Take(i).ToArray());
                    if (typeByYoloAndContext.TryGetValue((yolo, list, isConstraint), out var res))
                    {
                        return (res, list);
                    }
                }


                if (yolo.key.Count() == 1)
                {
                    if (!isConstraint)
                    {
                        var justGenericConstraints = yolo.key.First()
                            .Where(y => y.Is5(out IsGeneric _))
                            .Select(y => y.Is5OrThrow())
                            .ToArray();
                        if (justGenericConstraints.Any())
                        {
                            // walk up the stack till we find a context that defined what we are looking for 
                            if (TryGetFromAndIndex(context, justGenericConstraints, out var index, out var from))
                            {
                                var list = new EqualableReadOnlyList<Yolo>(Add(context.TakeWhile(x => !x.Equals(from)), from).ToArray());
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                                alreadyConverting.Add((yolo, !isConstraint), res);
                                res.Fill(LookUpGeneric(from, context, index, alreadyConverting));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }
                            else {
                                throw new Exception("we didn't find it 😖");
                            }
                        }
                    }

                    {

                        var constrains = yolo.key.Single();


                        if (constrains.Count == 0)
                        {
                            var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType()));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }

                        var prim = constrains.Primitive();

                        if (prim.Is2(out var error))
                        {
                            var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }

                        if (prim.Is1OrThrow().Is(out var _))
                        {
                            // I'd like to not pass "this" here
                            // the primitive convert willn't use it
                            // but... this isn't really ready to use
                            // it's method are not defined at this point in time
                            var source = constrains.Single().Is2OrThrow().primitiveFlowNode2.type;

                            var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(source.Converter.Convert(this, source, Array.Empty<Tpn.ITypeProblemNode>() /*this is a little sloppy, but a primitive converter better not depend on context*/ ).Is3OrThrow()));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);

                        }

                        if (yolo.members.Is2(out var e4))
                        {
                            var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(e4));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }
                        var members = yolo.members.Is1OrThrow();

                        if (constrains.Input().Is(out var inputOr))
                        {
                            if (inputOr.Is2(out var e2))
                            {
                                var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(e2));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }
                        }
                        var input = inputOr?.Is1OrThrow();


                        if (constrains.Output().Is(out var outputOr))
                        {
                            if (outputOr.Is2(out var e3))
                            {
                                var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(e3));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
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

                        if (yolo.genericsConstraints.Is1(out var generics) && generics.Any())
                        {
                            //var i = 0;
                            //var convertedGenerics = generics.Select(x => new GenericTypeParameterPlacholder(i++, cache[x.Flatten()].type)).ToArray();


                            var genericsList = new List<IOrType<IGenericTypeParameterPlacholder, IError>>();
                            for (int i = 0; i < generics.Length; i++)
                            {
                                genericsList.Add(LookUpGeneric(yolo, context, i++, alreadyConverting));
                            }
                            var genericsArray = genericsList.ToArray();


                            if (input != default && output != default)
                            {
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                                alreadyConverting.Add((yolo, !isConstraint), res);
                                var (inputBox, inputContext) = CachedConvert3(cache[input.Flatten()], Add(context, cache[input.Flatten()]),alreadyConverting);
                                var (outputBox, outputContext) = CachedConvert3(cache[output.Flatten()], Add(context, cache[output.Flatten()]), alreadyConverting);

                                IReadOnlyList<Yolo> resContext = new Yolo[] { };

                                if (inputContext.Count > resContext.Count)
                                {
                                    resContext = inputContext;
                                }
                                if (outputContext.Count > resContext.Count)
                                {
                                    resContext = outputContext;
                                }

                                // I don't think this is safe see:
                                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                                var list = new EqualableReadOnlyList<Yolo>(resContext.ToArray());
                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                        new GenericMethodType(
                                            inputBox,
                                            outputBox,
                                            genericsArray)));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }


                            if (input != default)
                            {
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                                alreadyConverting.Add((yolo, !isConstraint), res);

                                var (inputBox, inputContext) = CachedConvert3(cache[input.Flatten()], Add(context, cache[input.Flatten()]), alreadyConverting);

                                // I don't think this is safe see:
                                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                                var list = new EqualableReadOnlyList<Yolo>(inputContext.ToArray());
                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                        new GenericMethodType(
                                            inputBox,
                                            new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                            genericsArray)));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }

                            if (output != default)
                            {
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                                alreadyConverting.Add((yolo, !isConstraint), res);

                                var (outputBox, outputContext) = CachedConvert3(cache[output.Flatten()], Add(context, cache[output.Flatten()]), alreadyConverting);

                                // I don't think this is safe see:
                                //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                                var list = new EqualableReadOnlyList<Yolo>(outputContext.ToArray());
                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                        new GenericMethodType(
                                            new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                            outputBox,
                                            genericsArray)));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }

                            {
                                var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                                alreadyConverting.Add((yolo, !isConstraint), res);

                                var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                        new GenericMethodType(
                                            new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                            new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                            genericsArray)));
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }
                        }

                        if (input != default && output != default)
                        {
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                            alreadyConverting.Add((yolo, !isConstraint), res);

                            var (inputBox, inputContext) = CachedConvert3(cache[input.Flatten()], Add(context, cache[input.Flatten()]), alreadyConverting);
                            var (outputBox, outputContext) = CachedConvert3(cache[output.Flatten()], Add(context, cache[output.Flatten()]), alreadyConverting);

                            IReadOnlyList<Yolo> resContext = new Yolo[] { };

                            if (inputContext.Count > resContext.Count)
                            {
                                resContext = inputContext;
                            }
                            if (outputContext.Count > resContext.Count)
                            {
                                resContext = outputContext;
                            }

                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            var list = new EqualableReadOnlyList<Yolo>(resContext.ToArray());
                            res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                new MethodType(
                                    inputBox,
                                    outputBox)));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }


                        if (input != default)
                        {
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                            alreadyConverting.Add((yolo, !isConstraint), res);

                            var (inputBox, inputContext) = CachedConvert3(cache[input.Flatten()], Add(context, cache[input.Flatten()]), alreadyConverting);

                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            var list = new EqualableReadOnlyList<Yolo>(inputContext.ToArray());
                            res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new MethodType(
                                        inputBox,
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())))));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }

                        if (output != default)
                        {
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                            alreadyConverting.Add((yolo, !isConstraint), res);

                            var (outputBox, outputContext) = CachedConvert3(cache[output.Flatten()], Add(context, cache[output.Flatten()]), alreadyConverting);

                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            var list = new EqualableReadOnlyList<Yolo>(outputContext.ToArray());
                            res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new MethodType(
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                        outputBox)));
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }

                        // if it has members it must be a scope
                        if (members.Any() || constrains.Any(x => x.Is4(out HasMembers _)))
                        {
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                            alreadyConverting.Add((yolo, !isConstraint), res);

                            var external = constrains.Where(x => x.Is6(out IsExternal _)).Select(x => x.Is6OrThrow()).ToArray();

                            var membersAndContexts = members.Select(member => {
                                var (memberType, memberContext) = CachedConvert3(member.Item2, Add(context, member.Item2), alreadyConverting);
                                return (memberType, memberContext, member.Item1);
                            }).ToArray();

                            IReadOnlyList<Yolo> resContext = new Yolo[] { };

                            foreach (var (_, memberContext, _) in membersAndContexts)
                            {
                                if (memberContext.Count > resContext.Count)
                                {
                                    resContext = memberContext;
                                }
                            }

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

                                var dict = membersAndContexts.ToDictionary(x => x.Item3, x => x.Item1);

                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                   new ExternalHasMembersType(interfaceType, interfaceType.Members.Select(x => new WeakExternslMemberDefinition(
                                       x,
                                       dict[x.Key])).ToList())));
                                var list = new EqualableReadOnlyList<Yolo>(resContext.ToArray());
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);

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
                            {
                                res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new HasMembersType(new WeakScope(membersAndContexts.Select(memberAndContext => new WeakMemberDefinition(
                                         Model.Elements.Access.ReadWrite,
                                         memberAndContext.Item3,
                                         memberAndContext.Item1)).ToList()))));
                                var list = new EqualableReadOnlyList<Yolo>(resContext.ToArray());
                                typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                                return (res, list);
                            }
                        }

                        {
                            var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType()));
                            var list = new EqualableReadOnlyList<Yolo>(Array.Empty<Yolo>());
                            typeByYoloAndContext.AddOrThrow((yolo, list, isConstraint), res);
                            return (res, list);
                        }
                    }
                }
                else {
                    var res = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                    alreadyConverting.Add((yolo, !isConstraint), res);

                    // an or type..
                    var (left, leftContext) = CachedConvert3(yolo.left.IfElseReturn(x => x, () => throw new Exception("better have a left")), Add(context, yolo), alreadyConverting);
                    var (right, rightContext) = CachedConvert3(yolo.right.IfElseReturn(x => x, () => throw new Exception("better have a left")), Add(context, yolo), alreadyConverting);

                    var membersAndContextOrError = yolo.members.TransformInner(actually => actually.Select(member => {
                            var (memberType, memberContext) = CachedConvert3(member.Item2, Add(context, member.Item2), alreadyConverting);
                            return (memberType, memberContext, member.Item1);
                        }).ToArray());

                    var possiblyInput = yolo.input.TransformInner(x => x.SwitchReturns(
                                       y => CachedConvert3(y, Add(context, y), alreadyConverting),
                                       error => (new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)), new Yolo[] { }/*an error is an error in all contexts*/)));
                    var possiblyOutput = yolo.output.TransformInner(x => x.SwitchReturns(
                                       y => CachedConvert3(y, Add(context, y), alreadyConverting),
                                       error => (new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)), new Yolo[] { }/*an error is an error in all contexts*/)));

                    // find the longest context
                    IReadOnlyList<Yolo> resContext = new Yolo[] { };

                    if (leftContext.Count > resContext.Count) {
                        resContext = leftContext;
                    }
                    if (rightContext.Count > resContext.Count)
                    {
                        resContext = rightContext;
                    }

                    if (membersAndContextOrError.Is1(out var membersAndContexts))
                    {
                        foreach (var (_, memberContext, _) in membersAndContexts)
                        {
                            if (memberContext.Count > resContext.Count)
                            {
                                resContext = memberContext;
                            }
                        }
                    }

                    if (possiblyInput.Is(out var definatelyInput)) {
                        if (definatelyInput.Item2.Count > resContext.Count)
                        {
                            resContext = definatelyInput.Item2;
                        }
                    }
                    if (possiblyOutput.Is(out var definatelyOutput))
                    {
                        if (definatelyOutput.Item2.Count > resContext.Count)
                        {
                            resContext = definatelyOutput.Item2;
                        }
                    }

                    res.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError>(new FrontEndOrType(
                               left,
                               right,
                               membersAndContextOrError.TransformInner(membersAndContexts=>membersAndContexts.Select(memberAndContext => new WeakMemberDefinition(
                                   Model.Elements.Access.ReadWrite,
                                   memberAndContext.Item3,
                                   memberAndContext.Item1)).ToList()),
                               possiblyInput.TransformInner(x => x.Item1),
                               possiblyOutput.TransformInner(x=> x.Item1))));

                    typeByYoloAndContext.AddOrThrow((yolo, new EqualableReadOnlyList<Yolo>(resContext.ToArray()), isConstraint), res);
                    return (res, resContext);
                }
            }

            private IEnumerable<T> Add<T>(IEnumerable<T> context, T yolo)
            {
                foreach (var item in context)
                {
                    yield return item;
                }
                yield return yolo;
            }


            // list of paths
            // each path has the outer items towards the start
            // contains at
            List<List<Yolo>> Paths(Yolo at,
                Dictionary<Yolo,  List<Yolo>> positions) 
            {
                var currents = new List<List<Yolo>> { new List<Yolo> { at } };

                var go = true;
                while (go)
                {
                    go = false;

                    var next = new List<List<Yolo>>();
                    foreach (var item in currents)
                    {
                        if (positions.TryGetValue(item.First(), out var parents))
                        {
                            // we don't want to create circular contexts
                            // if the parent is already in the context don't add it again
                            parents = parents.Where(x => !item.Contains(x)).ToList();
                            if (parents.Any())
                            {
                                foreach (var parent in parents)
                                {
                                    var toAdd = new List<Yolo>();
                                    toAdd.Add(parent);
                                    toAdd.AddRange(item);
                                    next.Add(toAdd);
                                    go = true;
                                }
                            }
                            else
                            {
                                next.Add(item);
                            }
                        }
                        else
                        {
                            next.Add(item);
                        }
                    }
                    currents = next;
                }
                return currents;//.Select(x=>x.SkipLast(1).ToList()).ToList();
            }

            IEnumerable<Yolo> ConvertContext(IEnumerable<ITypeProblemNode> context) {
                return context
                        .SelectMany(x =>
                        {
                            if (flowNodes2.TryGetValue(OrType.Make<ITypeProblemNode, IError>(x), out var orType))
                            {
                                return new[]{ cache[orType
                                    .GetValueAs(out IConstraintSoruce _)
                                    .GetExtendedConstraints()
                                    .Flatten()]};
                            }
                            return Array.Empty<Yolo>();
                        });
            }

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetType(Tpn.ILookUpType from, IEnumerable<ITypeProblemNode> context)
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
                                return GetGenericMethodType(x, context);
                            }
                            return GetMethodType(x, context);
                        },
                        x => GetHasMemberType(x, context),
                        x => GetObjectType(x, context),
                        x => GetOrType(x, context),
                        x => GetInferredType(x, context),
                        x => GetGenericPlaceholder(x, context),
                        x => OrType.Make<IFrontendType<IVerifiableType>, IError>(x));
            }

            internal IOrType<FrontEndOrType, IError> GetOrType(TypeProblem2.OrType from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<FrontEndOrType>());
            }

            internal IOrType<MethodType, IError> GetMethodType(TypeProblem2.MethodType from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<MethodType>());
            }


            //GenericMethodType
            internal IOrType<GenericMethodType, IError> GetGenericMethodType(TypeProblem2.MethodType from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<GenericMethodType>());
            }

            internal IOrType<HasMembersType, IError> GetHasMemberType(TypeProblem2.Type from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<HasMembersType>());
            }

            internal IOrType<HasMembersType, IError> GetObjectType(TypeProblem2.Object from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<HasMembersType>());
            }

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetInferredType(TypeProblem2.InferredType from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue();
            }

            internal IOrType<GenericTypeParameterPlacholder, IError> GetGenericPlaceholder(TypeProblem2.GenericTypeParameter from, IEnumerable<ITypeProblemNode> context)
            {
                return CachedConvert3(
                    cache[flowNodes2[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IConstraintSoruce _)
                        .GetExtendedConstraints()
                        .Flatten()],
                    ConvertContext(context),
                    new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue().TransformInner(y => y.CastTo<GenericTypeParameterPlacholder>());
            }

            // this also ends up managing weak scopes that aren't types
            private readonly ConcurrentIndexed<Tpn.IHavePrivateMembers, WeakScope> nonTypeScopes = new ConcurrentIndexed<IHavePrivateMembers, WeakScope>();

            internal WeakScope GetWeakScope(Tpn.IHavePrivateMembers from, IEnumerable<ITypeProblemNode> context) =>
                nonTypeScopes.GetOrAdd(from, () =>
                    new WeakScope(from.PrivateMembers.Select(x => new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(GetType(x.Value, context)))).ToList()));

            internal bool TryGetMember(IStaticScope scope, IKey key, IEnumerable<ITypeProblemNode> context, [NotNullWhen(true)] out IOrType<WeakMemberDefinition, IError>? res)
            {
                if (flowNodes2.TryGetValue(OrType.Make<ITypeProblemNode, IError>(scope), out var flowNode))
                {
                    var rep = flowNode.GetValueAs(out IConstraintSoruce _).GetExtendedConstraints().Flatten();
                    var type = CachedConvert3(
                        cache[rep],
                        ConvertContext(context),
                        new Dictionary<(Yolo, bool couldBeGeneric), Box<IOrType<IFrontendType<IVerifiableType>, IError>>>()).Item1.GetValue();
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
                    var matches = GetWeakScope(privateMembers, context).membersList.Where(x => x.Key.Equals(key)).ToArray();

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

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in  mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
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

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
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

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in  mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
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

                //var extened = false;
                //{
                //    var sources = mustHaves.Select(x => x.dependent).ToHashSet();

                //    foreach (var givenPath in givenPaths)
                //    {
                //        sources.Add(givenPath.dependent);
                //    }

                //    if (sources.Count() == 1)
                //    {
                //        extened = true;
                //    }
                //}

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }


                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in  givenPath.dependent.GetExtendedConstraints())
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

                //var extened = false;
                //{
                //    var sources = mustHaves.Select(x => x.dependent).ToHashSet();
                //    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                //    {
                //        foreach (var givenPath in givenPaths)
                //        {
                //            sources.Add(givenPath.dependent);
                //        }
                //    }

                //    if (sources.Count() == 1)
                //    {
                //        extened = true;
                //    }
                //}

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetExtendedConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                {
                    if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                    {
                        foreach (var givenPath in givenPaths)
                        {
                            foreach (var constraint in givenPath.dependent.GetExtendedConstraints())
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









// Yolo's aren't 1-1 with types
// it's Yolo + context -> type
// context is a stack of Yolos
// 
// when you look something up you do it like: what is Yolo-X in Yolo-Y?
// which might turn around and do deeper looks like Yolo-Z in [Yolo-X, Yolo-Y]
// sometimes you can't figure it out maybe Yolo-Z in Yolo-X isn't well defined
// and that's ok
// at somepoint we'll look at Yolo-Y and that will give us the context we need 
//
// but.. that is going to break a lot of these apis... GetMethodType, GetObjectType
// I think I just have to pass it on and hope the call-e knowns the context




// oof generics again
// so two things can look up to the same generic type
// and they can have two different paths to owner
// often you know exactly who generic you are
//
// method [t] [t,t] a
// b =: a
//
// here flowing the way I do makes a lot of sense
//
// ...
// 
// ok so the plan now is paired constrains generic and generic source
// generic and generic source are paired 
// 
// but I really already have what I need 
// 