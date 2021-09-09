using Prototypist.TaskChain;
using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
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

            private readonly ConcurrentIndexed<EqualibleHashSet<Tpn.CombinedTypesAnd>, Yolo> cache = new ConcurrentIndexed<EqualibleHashSet<CombinedTypesAnd>, Yolo>();
            private Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes;

            private class Yolo
            {
                internal IOrType<IReadOnlyList<(IKey, IOrType<Yolo, IError>)>, IError> members;

                internal IIsPossibly<IOrType<Yolo, IError>> output;
                internal IIsPossibly<IOrType<Yolo, IError>> input;

                // for or types
                internal IIsPossibly<IOrType<Yolo, IError>> left;
                internal IIsPossibly<IOrType<Yolo, IError>> right;

                internal readonly Box<IOrType<IFrontendType<IVerifiableType>, IError>> type = new Box<IOrType<IFrontendType<IVerifiableType>, IError>>();
                
                // sometimes thing have no member but really we know they are an object or a type
                // if this is not set the yolo becomes an AnyType with it set the Yolo becomes a HasMembers with no members
                internal bool hasMemebers = false;

                internal IIsPossibly<IInterfaceType> external = Possibly.IsNot<IInterfaceType>();

            }

            public TypeSolution(
                IReadOnlyList<IOrType<Tpn.TypeProblem2.MethodType, Tpn.TypeProblem2.Type, Tpn.TypeProblem2.Object, Tpn.TypeProblem2.OrType, Tpn.TypeProblem2.InferredType, IError>> things,
                Dictionary<IOrType<ITypeProblemNode, IError>, IOrType<ConcreteFlowNode, InferredFlowNode, PrimitiveFlowNode, OrFlowNode>> flowNodes) {
                if (things is null)
                {
                    throw new ArgumentNullException(nameof(things));
                }

                this.flowNodes = flowNodes ?? throw new ArgumentNullException(nameof(flowNodes));


                foreach (var flowNode in flowNodes)
                {
                    IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError> rep = flowNode.Value.GetValueAs(out IVirtualFlowNode _).ToRep();
                    var yolo = GetOrAdd(rep);

                    // this feels a bit weird because it doesn't flow through the type problem
                    if (yolo.Is1(out var realYolo) && flowNode.Key.Is1(out var typeProblemNode)) {
                        realYolo.hasMemebers |= typeProblemNode.SafeIs<ITypeProblemNode, TypeProblem2.Object>();
                        if (typeProblemNode.SafeIs(out TypeProblem2.Type x))
                        {
                            realYolo.hasMemebers = true;
                            if (x.External.Is(out var _))
                            {
                                realYolo.external = x.External;
                            }
                        }
                    }
                }

                IOrType < Yolo, IError> GetOrAdd(IOrType<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>  rep){

                    return rep.TransformInner(equalableHashSet => {
                        
                        var myBox = new Yolo();
                        var current = cache.GetOrAdd(equalableHashSet, myBox);

                        // if we added it, fill it
                        if (current == myBox) {


                            if (equalableHashSet.Count() > 1)
                            {
                                myBox.left = Possibly.Is(GetOrAdd(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualibleHashSet<Tpn.CombinedTypesAnd>(equalableHashSet.Take(equalableHashSet.Count() - 1).ToHashSet()))));
                                myBox.right = Possibly.Is( GetOrAdd(OrType.Make<EqualibleHashSet<Tpn.CombinedTypesAnd>, IError>(new EqualibleHashSet<Tpn.CombinedTypesAnd>(new HashSet<Tpn.CombinedTypesAnd>() { equalableHashSet.Last() }))));
                            }
                            else {
                                myBox.left = Possibly.IsNot<IOrType<Yolo, IError>>();
                                myBox.right = Possibly.IsNot<IOrType<Yolo, IError>>();
                            }

                            myBox.members = equalableHashSet.VirtualMembers().TransformInner(members => members.Select(virtualMember => (virtualMember.Key, GetOrAdd(virtualMember.Value.Value))).ToArray());
                            myBox.input = equalableHashSet.VirtualInput().TransformInner(input => GetOrAdd(input));
                            myBox.output = equalableHashSet.VirtualOutput().TransformInner(output => GetOrAdd(output));
                        }
                        return current;
                    });
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


                IOrType<IFrontendType<IVerifiableType>, IError> Convert2(Tpn.CombinedTypesAnd flowNode, Yolo yolo)
                {

                    if (flowNode.And.Count == 0)
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

                    if (flowNode.VirtualInput().Is(out var inputOr))
                    {
                        if (inputOr.Is2(out var e2))
                        {
                            return OrType.Make<IFrontendType<IVerifiableType>, IError>(e2);
                        }
                    }
                    var input = inputOr?.Is1OrThrow();


                    if (flowNode.VirtualOutput().Is(out var outputOr))
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

                    if (input != default && output != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                cache[input].type,
                                cache[output].type));
                    }


                    if (input != default)
                    {
                        // I don't think this is safe see:
                        //  {D27D98BA-96CF-402C-824C-744DACC63FEE}
                        return
                             OrType.Make<IFrontendType<IVerifiableType>, IError>(
                            new MethodType(
                                cache[input].type,
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
                                cache[output].type));
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


            public static IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Method type)
            {
                var res = type.Generics.Select(x => {
                    if (x.Key.SafeIs(out NameKey nameKey)) {
                        return OrType.Make<NameKey, ImplicitKey>(nameKey);
                    }
                    if (x.Key.SafeIs(out ImplicitKey implicitKey))
                    {
                        return OrType.Make<NameKey, ImplicitKey>(nameKey);
                    }

                    // it's weird that I have x.Key and x.Value.Key
                    // and they have different types...
                    throw new Exception("this might or might not happen, let's work it work when it does");

                }).ToArray();

                if (res.Length != 0 )
                {
                    return Possibly.Is(res);
                }

                return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            }


            public static IIsPossibly<IOrType<NameKey, ImplicitKey>[]> HasPlacholders(TypeProblem2.Type type)
            {
                var res = type.Generics.Select(x => {
                    if (x.Key.SafeIs(out NameKey nameKey))
                    {
                        return OrType.Make<NameKey, ImplicitKey>(nameKey);
                    }
                    if (x.Key.SafeIs(out ImplicitKey implicitKey))
                    {
                        return OrType.Make<NameKey, ImplicitKey>(nameKey);
                    }

                    // it's weird that I have x.Key and x.Value.Key
                    // and they have different types...
                    throw new Exception("this might or might not happen, let's work it work when it does");

                }).ToArray();

                if (res.Length != 0)
                {
                    return Possibly.Is(res);
                }

                return Possibly.IsNot<IOrType<NameKey, ImplicitKey>[]>();
            }

            internal IOrType<IFrontendType<IVerifiableType>, IError> GetType(Tpn.ILookUpType from)
            {
                // this little block makes undefined type undefined
                // at time of writing if you uncommented it
                // undefined types are just infered types
                // a tempting notion
                if (from.LooksUp.Is(out var value))
                {
                    if (value.Is6(out var error)) { 
                        return OrType.Make<IFrontendType<IVerifiableType>, IError>(error);
                    }
                }
                else {
                    throw new Exception("it should be set by this point? right");
                }

                return flowNodes[from.LooksUp.GetOrThrow().SwitchReturns(
                    x => OrType.Make<ITypeProblemNode, IError>(x),
                    x => OrType.Make<ITypeProblemNode, IError>(x),
                    x => OrType.Make<ITypeProblemNode, IError>(x),
                    x => OrType.Make<ITypeProblemNode, IError>(x),
                    x => OrType.Make<ITypeProblemNode, IError>(x),
                    x => OrType.Make<ITypeProblemNode, IError>(x))].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                     x => cache[x].type.GetValue(),
                     x => OrType.Make<IFrontendType<IVerifiableType>, IError>(x));
            }
            internal IOrType<FrontEndOrType, IError> GetOrType(TypeProblem2.OrType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y => y.CastTo<FrontEndOrType>()),
                    x => OrType.Make<FrontEndOrType, IError>(x));

            internal IOrType<MethodType, IError> GetMethodType(TypeProblem2.MethodType from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y => y.CastTo<MethodType>()),
                    x => OrType.Make<MethodType, IError>(x));

            internal IOrType<HasMembersType, IError> GetHasMemberType(TypeProblem2.Type from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make< HasMembersType, IError > (x));

            internal IOrType<HasMembersType, IError> GetObjectType(TypeProblem2.Object from) =>
               flowNodes[OrType.Make<ITypeProblemNode, IError>(from)].GetValueAs(out IVirtualFlowNode _).ToRep().SwitchReturns(
                    x => cache[x].type.GetValue().TransformInner(y=>y.CastTo<HasMembersType>()),
                    x => OrType.Make<HasMembersType, IError>(x));


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
                        var type = cache[combinedTypesAnds].type.GetValue();
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
}
