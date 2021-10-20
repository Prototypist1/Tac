using Prototypist.TaskChain;
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
    internal partial class Tpn {
        internal class TypeSolution {

            //readonly Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<IVerifiableType>, IError>>> generalLookUp = new Dictionary<EqualibleHashSet<CombinedTypesAnd>, IBox<IOrType<IFrontendType<IVerifiableType>, IError>>>();
            //readonly Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>> objectCache = new Dictionary<TypeProblem2.Object, IBox<IOrType<WeakObjectDefinition, WeakRootScope>>>();
            //readonly Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>> scopeOrBlockCache = new Dictionary<TypeProblem2.Scope, IBox<IOrType<WeakBlockDefinition, WeakScope, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>> methodCache = new Dictionary<TypeProblem2.Method, IBox<IOrType<WeakMethodDefinition, WeakImplementationDefinition, WeakEntryPointDefinition>>>();
            //readonly Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>> typeCache = new Dictionary<TypeProblem2.Type, IBox<IOrType<WeakTypeDefinition, WeakGenericTypeDefinition, IPrimitiveType>>>();

            //readonly Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>> scopeCache = new Dictionary<IOrType<EqualibleHashSet<CombinedTypesAnd>, IError>, IOrType<Scope, IError>>();

            //readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodeLookUp;

            //readonly Dictionary<TypeProblem2.Method, Scope> methodScopeCache = new Dictionary<TypeProblem2.Method, Scope>();
            //readonly Dictionary<TypeProblem2.Scope, Scope> scopeScopeCache = new Dictionary<TypeProblem2.Scope, Scope>();

            private readonly ConcurrentIndexed<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, Yolo> cache = new ConcurrentIndexed<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, Yolo>();
            private readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes;
            private readonly Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> flowNodes2;

            private class Yolo
            {
                internal IOrType<IReadOnlyList<(IKey, Yolo)>, IError>? members;

                internal IIsPossibly<IOrType<Yolo, IError>>? output;
                internal IIsPossibly<IOrType<Yolo, IError>>? input;

                // for or types
                internal IIsPossibly<IOrType<Yolo, IError>>? left;
                internal IIsPossibly<IOrType<Yolo, IError>>? right;

                internal readonly Box<IOrType<IFrontendType<IVerifiableType>, IError>> type = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                
                // sometimes thing have no member but really we know they are an object or a type
                // if this is not set the yolo becomes an AnyType with it set the Yolo becomes a HasMembers with no members
                internal bool hasMemebers = false; 
                //internal IIsPossibly<TypeProblem2.GenericTypeParameter> isGeneric = Possibly.IsNot<TypeProblem2.GenericTypeParameter>();
                internal IIsPossibly<TypeProblem2.GenericTypeParameter> isGenericConstraintFor = Possibly.IsNot<TypeProblem2.GenericTypeParameter>();
                internal IIsPossibly<Box<IOrType<IFrontendType<IVerifiableType>, IError>>> isGenericConstraintFroRealized = Possibly.IsNot<Box<IOrType<IFrontendType<IVerifiableType>, IError>>>();

                internal IIsPossibly<IInterfaceType> external = Possibly.IsNot<IInterfaceType>();
            }

            public TypeSolution(
                IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, TypeProblem2.GenericTypeParameter, IError>> things,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode2, InferredFlowNode2, PrimitiveFlowNode2, OrFlowNode2>> flowNodes2) {

                if (things is null)
                {
                    throw new ArgumentNullException(nameof(things));
                }

                //this.flowNodes = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));
                this.flowNodes2 = flowNodes2 ?? throw new ArgumentNullException(nameof(flowNodes2));

                var constrainsToGenerics = things.SelectMany(x => { if (x.Is6(out var genericTypeParameter)) { return new[] { genericTypeParameter }; } return Array.Empty<TypeProblem2.GenericTypeParameter>(); }).ToDictionary(x => x.constraint, x => x);

                foreach (var flowNode2 in flowNodes2)
                {
                    EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>> rep = flowNode2.Value.GetValueAs(out IConstraintSoruce _).GetConstraints();
                    var yolo = GetOrAdd(rep);

                    // this feels a bit weird because it doesn't flow through the type problem
                    if (flowNode2.Key.Is1(out var typeProblemNode)) {
                        yolo.hasMemebers |= typeProblemNode.SafeIs<ITypeProblemNode, TypeProblem2.Object>();
                        if (typeProblemNode.SafeIs(out TypeProblem2.Type type))
                        {
                            yolo.hasMemebers = true;
                            if (type.External.Is(out var _))
                            {
                                yolo.external = type.External;
                            }
                        }
                        {
                            if (typeProblemNode.SafeIs(out TypeProblem2.GenericTypeParameter genericTypeParameter))
                            {
                                throw new Exception("we pass the constraint into the type problem, not the actual parameter");
                                //realYolo.isGeneric = Possibly.Is(genericTypeParameter);
                            }
                        }
                        {
                            if (typeProblemNode.SafeIs(out TypeProblem2.InferredType inferredType) && constrainsToGenerics.TryGetValue(inferredType, out var genericTypeParameter))
                            {
                                yolo.isGenericConstraintFor = Possibly.Is(genericTypeParameter);
                            }
                        }
                    }
                }

                Yolo GetOrAdd(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>> equalableHashSet)
                {
                    var myBox = new Yolo();
                    var current = cache.GetOrAdd(equalableHashSet, myBox);

                    // if we added it, fill it
                    if (current == myBox) {


                        if (equalableHashSet.Count() > 1)
                        {
                            myBox.left = Possibly.Is(GetOrAdd(OrType.Make<EqualableHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualableHashSet<Tpn.CombinedTypesAnd>(equalableHashSet.Take(equalableHashSet.Count() - 1).ToHashSet()))));
                            myBox.right = Possibly.Is( GetOrAdd(OrType.Make<EqualableHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualableHashSet<Tpn.CombinedTypesAnd>(new HashSet<Tpn.CombinedTypesAnd>() { equalableHashSet.Last() }))));
                        }
                        else {
                            myBox.left = Possibly.IsNot<IOrType<Yolo, IError>>();
                            myBox.right = Possibly.IsNot<IOrType<Yolo, IError>>();
                        }

                        myBox.members = equalableHashSet.Members().TransformInner(members => members.Select(memberPair => (memberPair.Key, GetOrAdd(memberPair.Value))).ToArray());
                        myBox.input = equalableHashSet.Input().TransformInner(inputOr => inputOr.TransformInner(input => GetOrAdd(input)));
                        myBox.output = equalableHashSet.Output().TransformInner(outputOr => outputOr.TransformInner(output=> GetOrAdd(output)));
                    }
                    return current;
                }

                foreach (var (key, value) in cache)
                {
                    if (key.Count() == 1)
                    {
                        value.type.Fill(Convert2(key.First(), value));
                    }
                    else 
                    {
                        value.type.Fill(OrType.Make<IFrontendType<IVerifiableType>, IError> (new FrontEndOrType(
                            value.left.IfElseReturn(x => x, () => throw new Exception("better have a left")).SwitchReturns(
                                x=>x.type,
                                e=> new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(e))),
                            value.right.IfElseReturn(x => x, () => throw new Exception("better have a right")).SwitchReturns(
                                x => x.type,
                                e => new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(e))),
                            value.members.TransformInner(actually => actually.Select(x => new WeakMemberDefinition(
                                Model.Elements.Access.ReadWrite,
                                x.Item1,
                                x.Item2.SwitchReturns(
                                    y => y.type,
                                    y => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(y))))).ToList()),
                            value.input.TransformInner(x=>x.SwitchReturns(
                                    y=>y.type,
                                    error => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error)))),
                            value.output.TransformInner(x => x.SwitchReturns(
                                    y => y.type,
                                    error => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(error))))
                            )));
                    }
                }


                IOrType<IFrontendType<IVerifiableType>, IError> Convert2(EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>> flowNode, Yolo yolo)
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

                    if (flowNode.Count == 0)
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType());
                    }

                    var prim = flowNode.Primitive();

                    if (prim.Is2(out var error))
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(error);
                    }

                    if (prim.Is1OrThrow().Is(out var _) && flowNode.And.Single().Is2OrThrow().Source.Is(out var source))
                    {
                        // I'd like to not pass "this" here
                        // the primitive convert willn't use it
                        // but... this isn't really ready to use
                        // it's method are not defined at this point in time
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(source.Converter.Convert(this, source).Is3OrThrow());
                    }

                    if (yolo.members.Is2(out var e4))
                    {
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(e4);
                    }
                    var members = yolo.members.Is1OrThrow();

                    if (flowNode.Input().Is(out var inputOr))
                    {
                        if (inputOr.Is2(out var e2))
                        {
                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(e2);
                        }
                    }
                    var input = inputOr?.Is1OrThrow();


                    if (flowNode.Output().Is(out var outputOr))
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

                    if (flowNode.Generics().Is1(out var generics) && generics.Any()) {

                        if (input != default && output != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                 OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                new GenericMethodType(
                                    GetFromCacheReplaceGenericConstrainsWithTheGeneric(input),
                                    GetFromCacheReplaceGenericConstrainsWithTheGeneric(output),
                                    generics.Select(x=> x.Value.SwitchReturns(y=> GetFromCacheReplaceGenericConstrainsWithTheGeneric(y), error => { //*this is a poor mans safe cast, safe cast doesn't go this way (to a super class), but I need to communicate the type to C#
                                        IBox<IOrType<IFrontendType<IVerifiableType>, IError>> y = new Box<IOrType<IGenericTypeParameterPlacholder, IError>>(OrType.Make<IGenericTypeParameterPlacholder, IError>(error)); return y; })).ToArray()));
                        }


                        if (input != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new GenericMethodType(
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(input),
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                        generics.Select(x => x.Value.SwitchReturns(y => GetFromCacheReplaceGenericConstrainsWithTheGeneric(y), error => { //*this is a poor mans safe cast, safe cast doesn't go this way (to a super class), but I need to communicate the type to C#
                                            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> y = new Box<IOrType<IGenericTypeParameterPlacholder, IError>>(OrType.Make<IGenericTypeParameterPlacholder, IError>(error)); return y;
                                        })).ToArray()));
                        }

                        if (output != default)
                        {
                            // I don't think this is safe see:
                            //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                            return
                                OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                    new GenericMethodType(
                                        new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                        GetFromCacheReplaceGenericConstrainsWithTheGeneric(output),
                                        generics.Select(x => x.Value.SwitchReturns(y => GetFromCacheReplaceGenericConstrainsWithTheGeneric(y), error => { //*this is a poor mans safe cast, safe cast doesn't go this way (to a super class), but I need to communicate the type to C#
                                            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> y = new Box<IOrType<IGenericTypeParameterPlacholder, IError>>(OrType.Make<IGenericTypeParameterPlacholder, IError>(error)); return y;
                                        })).ToArray()));
                        }

                        return
                            OrType.Make<IFrontendType<IVerifiableType>, IError>(
                                new GenericMethodType(
                                    new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                    new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new EmptyType())),
                                    generics.Select(x => x.Value.SwitchReturns(y => GetFromCacheReplaceGenericConstrainsWithTheGeneric(y), error => { //*this is a poor mans safe cast, safe cast doesn't go this way (to a super class), but I need to communicate the type to C#
                                                                    IBox<IOrType<IFrontendType<IVerifiableType>, IError>> y = new Box<IOrType<IGenericTypeParameterPlacholder, IError>>(OrType.Make<IGenericTypeParameterPlacholder, IError>(error)); return y;
                                    })).ToArray()));
                    }

                    if (input != default && output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(input),
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(output)));
                    }


                    if (input != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(input),
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
                                GetFromCacheReplaceGenericConstrainsWithTheGeneric(output)));
                    }

                    // if it has members it must be a scope
                    if (members.Any() || yolo.hasMemebers)
                    {
                        if (yolo.external.Is(out var interfaceType)) {

                            // we have one member list from the type problem
                            // and one member list from the external deffinition
                            // we need to join them together

                            if (members.Count != interfaceType.Members.Count)
                            {
                                throw new Exception("these should have the same number of members!");
                            }

                            var dict = members.ToDictionary(x => x.Item1, x => x);

                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(
                               new ExternalHasMembersType(interfaceType, interfaceType.Members.Select(x => new WeakExternslMemberDefinition(
                                   x,
                                   dict[x.Key].Item2.SwitchReturns(
                                       y => y.type,
                                       y => (IBox<IOrType<IFrontendType<IVerifiableType>, IError>>)new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(y))))).ToList()));

                        }

                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new HasMembersType(new WeakScope(members.Select(x => new WeakMemberDefinition(
                                Model.Elements.Access.ReadWrite,
                                x.Item1,
                                x.Item2.SwitchReturns(
                                    y => y.type,
                                    y =>(IBox<IOrType<IFrontendType<IVerifiableType>, IError>>) new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(y))))).ToList())));
                    }


                    return OrType.Make<IFrontendType<IVerifiableType>, IError>(new AnyType());
                }



            }


            Box<IOrType<IFrontendType<IVerifiableType>, IError>> GetFromCacheReplaceGenericConstrainsWithTheGeneric(EqualableHashSet<Tpn.CombinedTypesAnd> key)
            {
                var res = cache[key];
                if (res.isGenericConstraintFor.Is(out var genericTypeParameter))
                {

                    if (res.isGenericConstraintFroRealized.Is(out var alreadyGotIt))
                    {
                        return alreadyGotIt;
                    }

                    var innerRes = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(new GenericTypeParameterPlacholder(genericTypeParameter.index, res.type)));
                    res.isGenericConstraintFroRealized = Possibly.Is(innerRes);
                    return innerRes;

                }
                return res.type;
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
                    if (value.Is7(out var error)) { 
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(error);
                    }
                }
                else {
                    throw new Exception("it should be set by this point? right");
                }

                return flowNodes[from.LooksUp.GetOrThrow()
                    .SwitchReturns(
                        x => OrType.Make<ITypeProblemNode, IError>(x),
                        x => OrType.Make<ITypeProblemNode, IError>(x),
                        x => OrType.Make<ITypeProblemNode, IError>(x),
                        x => OrType.Make<ITypeProblemNode, IError>(x),
                        x => OrType.Make<ITypeProblemNode, IError>(x),
                        x => OrType.Make<ITypeProblemNode, IError>(x.constraint),
                        x => OrType.Make<ITypeProblemNode, IError>(x))]
                    .GetValueAs(out IVirtualFlowNode _)
                    .ToRep()
                    .SwitchReturns(
                        x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue(),
                        x => OrType.Make<IFrontendType<IVerifiableType>, IError>(x));
            }
            internal IOrType<FrontEndOrType, IError> GetOrType(TypeProblem2.OrType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue().TransformInner(y => y.CastTo<FrontEndOrType>()),
                    x => OrType.Make<FrontEndOrType, IError>(x));

            internal IOrType<MethodType, IError> GetMethodType(TypeProblem2.MethodType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue().TransformInner(y => y.CastTo<MethodType>()),
                    x => OrType.Make<MethodType, IError>(x));

            internal IOrType<HasMembersType, IError> GetHasMemberType(TypeProblem2.Type from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make< HasMembersType, IError > (x));

            internal IOrType<HasMembersType, IError> GetObjectType(TypeProblem2.Object from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make<HasMembersType, IError>(x));

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetInferredType(TypeProblem2.InferredType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x).GetValue(),
                    x => OrType.Make<IFrontendType<IVerifiableType>, IError>(x));


            internal IOrType<GenericTypeParameterPlacholder, IError> GetGenericPlaceholder(TypeProblem2.GenericTypeParameter from) =>
                cache.Where(x => x.Value.isGenericConstraintFor.Is(out var y) && y == from)
                .Select(x => GetFromCacheReplaceGenericConstrainsWithTheGeneric(x.Key).GetValue().TransformInner(y => y.CastTo<GenericTypeParameterPlacholder>()))
                .Single();

            // this also ends up managing weak scopes that aren't types
            private readonly ConcurrentIndexed<Tpn.IHavePrivateMembers, WeakScope> nonTypeScopes = new ConcurrentIndexed<IHavePrivateMembers, WeakScope>();

            internal WeakScope GetWeakScope(Tpn.IHavePrivateMembers from)=>
                nonTypeScopes.GetOrAdd(from, () =>
                    new WeakScope(from.PrivateMembers.Select(x => new WeakMemberDefinition(Model.Elements.Access.ReadWrite, x.Key, new Box<IOrType< IFrontendType<IVerifiableType>, IError>>( GetType(x.Value)))).ToList()));

            internal bool TryGetMember(IStaticScope scope, IKey key, out IOrType<WeakMemberDefinition, IError> res)
            {
                if (flowNodes.TryGetValue(OrType.Make<ITypeProblemNode, IError>(scope), out var flowNode)) {
                    var rep = flowNode.GetValueAs(out IVirtualFlowNode _).ToRep();
                    if (rep.Is1(out var combinedTypesAnds)) {
                        var type = GetFromCacheReplaceGenericConstrainsWithTheGeneric(combinedTypesAnds).GetValue();
                        if (type.Is1(out var reallyType))
                        {
                            var maybeMember = reallyType.TryGetMember(key, new List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)>());

                            if (maybeMember.Is1(out var member)) {
                                res = member;
                                return true;
                            } else if (maybeMember.Is2(out var _)) {
                                res = default;
                                return false;
                            }
                            else
                            {
                                res = OrType.Make<WeakMemberDefinition, IError>(maybeMember.Is3OrThrow());
                                return true;
                            }

                        }
                        else {
                            res = OrType.Make<WeakMemberDefinition, IError>(type.Is2OrThrow());
                            return true;
                        }
                    } else {
                        res = OrType.Make<WeakMemberDefinition, IError>(rep.Is2OrThrow());
                        return true;
                    }
                }


                if (scope is Tpn.IHavePrivateMembers privateMembers) {
                    var matches = GetWeakScope(privateMembers).membersList.Where(x => x.Key.Equals(key)).ToArray();

                    if (matches.Length > 1) {
                        throw new Exception("that's not right");
                    }

                    if (matches.Length == 0) {
                        res = default;
                        return false;
                    }

                    res = OrType.Make < WeakMemberDefinition, IError > (matches.First());
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
    static class RepExtension {

        // this is basically
        // A and B and (C or D) to (A and B and C) or (A and B and D)
        // returns OR AND
        public static EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>> Flatten(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>> self) {
            // AND OR AND 
            var andOrAnd = self
                .Where(x => x.Is4(out var _))
                .Select(x => x.Is4OrThrow())
                .SelectMany(x => x.source.or
                    .Select(y => y.GetValueAs(out IConstraintSoruce _).GetConstraints().Flatten())
                    .ToArray())
                .ToHashSet();

            var orAndRes = new List<List<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>();

            
            foreach (var orAnd in andOrAnd)
            {
                var first = orAnd.First();
                foreach (var item in first)
                {
                    foreach (var andRes in orAndRes)
                    {
                        andRes.Add(item);
                    }
                }

                foreach (var and in orAnd.Skip(1))
                {
                    foreach (var andRes in orAndRes.ToArray())
                    {
                        var list = andRes.SkipLast(first.Count-1).ToList();
                        list.AddRange(and);
                        orAndRes.Add(list);
                    }
                }
            }

            var shared = self.Where(x => !x.Is4(out var _))
                .Select(x => x.SwitchReturns(
                    y => OrType.Make < MustHave, MustBePrimitive, GivenPathThen>(y),
                    y => OrType.Make < MustHave, MustBePrimitive, GivenPathThen>(y),
                    y => OrType.Make < MustHave, MustBePrimitive, GivenPathThen>(y),
                    _ => throw new Exception("I just said not that!")))
                .ToList();

            foreach (var list in orAndRes)
            {
                list.AddRange(shared);
            }

            return new EqualableHashSet<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>>(
                orAndRes.Select(x => new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>>(x.ToHashSet())).ToHashSet());
        }

        public static IOrType<IIsPossibly<Guid>, IError> Primitive(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self) {
            var primitives = self.SelectMany(x =>
            {
                if (x.Is2(out var v2))
                {
                    return new[] { v2 };
                }
                return Array.Empty<MustBePrimitive>();
            }).ToArray();

            if (primitives.Length == self.Count()) {
                var groupedPrimitives = primitives.GroupBy(x => x.primitive).ToArray();
                if (groupedPrimitives.Length == 1) {
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

        public static IOrType<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>>, IError> Members(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>>, IError>(error);
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
                .GroupBy(x => x.path.Is1OrThrow()).ToDictionary(x=>x.Key, x=>x);

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

            var list = new List<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>();
                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths)) {
                    foreach (var givenPath in givenPaths)
                    {
                        foreach (var constraint in givenPath.dependent.GetConstraints())
                        {
                            set.Add(constraint);
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>(set);

                list.Add(new KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>(mustHaves.Key.key, equalableSet));
            }

            return OrType.Make<ICollection<KeyValuePair<IKey, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>>, IError>(list);
        }


        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>> Input(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>(error));
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


            var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>();

            if (mustHaves.Any()) {

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }

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

                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in givenPath.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }

            }

            return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>( set)));
        }


        public static IIsPossibly<IOrType<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>> Output(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self)
        {
            if (self.ErrorCheck(out var error))
            {
                return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>(error));
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


            var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>();

            if (mustHaves.Any())
            {

                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }

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

                foreach (var givenPath in givenPaths)
                {
                    foreach (var constraint in givenPath.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }

            }

            return Possibly.Is(OrType.Make<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>, IError>(new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>(set)));
        }

        public static IOrType<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>, IError> Generics(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self) {

            if (self.ErrorCheck(out var error))
            {
                return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>, IError>(error);
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

            var pairs = new List<(int, EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>)>();
            foreach (var mustHaves in mustHaveGroup)
            {
                var set = new HashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>();
                foreach (var mustHave in mustHaves)
                {
                    foreach (var constraint in mustHave.dependent.GetConstraints())
                    {
                        set.Add(constraint);
                    }
                }
                if (givenPathDictionary.TryGetValue(mustHaves.Key, out var givenPaths))
                {
                    foreach (var givenPath in givenPaths)
                    {
                        foreach (var constraint in givenPath.dependent.GetConstraints())
                        {
                            set.Add(constraint);
                        }
                    }
                }
                var equalableSet = new EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>(set);

                pairs.Add((mustHaves.Key.index, equalableSet));
            }

            if (pairs.Select(x => x.Item1).Max() != pairs.Count() -1) {
                // I think this is an exception and not an IError
                // you really shouldn't be able to have disconunious generic constraints
                throw new Exception("the generic constriants are discontinious...");
            }

            return OrType.Make<IReadOnlyList<EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen, OrConstraint>>>, IError>(pairs.OrderBy(x=>x.Item1).Select(x=>x.Item2).ToArray());
        }
        
        private static bool ErrorCheck(this EqualableHashSet<IOrType<MustHave, MustBePrimitive, GivenPathThen>> self, [NotNullWhen(true)] out IError error) {
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            error = default;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }
    }
}
