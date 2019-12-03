using Prototypist.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{

    // this static class is here just to make us all think in terms of these bros
    public static class Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>
    {
        public interface ISetUpTypeProblem
        {
            // a =: x

            void IsAssignedTo(ICanAssignFromMe assignedFrom, ICanBeAssignedTo assignedTo);
            TypeProblem2.Value CreateValue(IScope scope, IKey typeKey,IConvertTo<TType> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IKey typeKey, bool isReadonly, IConvertTo<TType> converter);
            TypeProblem2.Member CreateMember(IScope scope, IKey key, IConvertTo<TType> converter);
            TypeProblem2.Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TType> converter);
            TypeProblem2.TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TType> converter);
            TypeProblem2.Scope CreateScope(IScope parent, IConvertTo<TScope> converter);
            TypeProblem2.Type CreateType(IScope parent, IKey key, IConvertTo<TType> converter);
            TypeProblem2.Type CreateGenericType(IScope parent, IKey key, IReadOnlyList<IKey> placeholders, IConvertTo<TType> converter);
            TypeProblem2.Object CreateObject(IScope parent, IKey key, IConvertTo<TObject> converter);
            TypeProblem2.Method CreateMethod(IScope parent, string inputName, IConvertTo<TMethod> converter);
            TypeProblem2.Method CreateMethod(IScope parent, ITypeReference inputType, ITypeReference outputType, string inputName, IConvertTo<TMethod> converter);
            TypeProblem2.Member GetReturns(IScope s);
            TypeProblem2.Member CreateHopefulMember(IHaveHopefulMembers scope, IKey key, IConvertTo<TType> converter);
            TypeProblem2.OrType CreateOrType(IScope s, IKey key, ITypeReference setUpSideNode1, ITypeReference setUpSideNode2, IConvertTo<TOrType> converter);
            IKey GetKey(ITypeReference type);
            TypeProblem2.Member GetInput(IMethod method);
        }

        public interface ITypeSolution
        {
            TType GetValueType(TypeProblem2.Value value);
            TType GetMemberType(TypeProblem2.Member member);
            TType GetTypeReferenceType(TypeProblem2.TypeReference typeReference);
            TScope GetScope(TypeProblem2.Scope scope);
            TExplictType GetExplicitTypeType(TypeProblem2.Type explicitType);
            TObject GetObjectType(TypeProblem2.Object @object);
            TOrType GetOrType(TypeProblem2.OrType orType);
            TMethod GetMethodScopeType(TypeProblem2.Method method);
        }

        public class ConcreteSolutionType : IReadOnlyDictionary<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)>
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

        public class OrSolutionType
        {
            private readonly OrType<OrSolutionType, ConcreteSolutionType> left;
            private readonly OrType<OrSolutionType, ConcreteSolutionType> right;

            public OrSolutionType(OrType<OrSolutionType, ConcreteSolutionType> left, OrType<OrSolutionType, ConcreteSolutionType> right)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
            }
        }

        internal class TypeSolution : ITypeSolution
        {
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


        public interface ITypeProblemNode
        {
            ISetUpTypeProblem Problem { get; }
        }


        public interface IConvertable<out T> { 
            IConvertTo<T> Converter { get; }
        }

        public interface IHaveMembers : ITypeProblemNode { }
        public interface IHaveHopefulMembers : ITypeProblemNode { }
        public interface ILookUpType : ITypeProblemNode { }

        public interface ICanAssignFromMe : ITypeProblemNode, ILookUpType { }
        public interface ICanBeAssignedTo : ITypeProblemNode, ILookUpType { }

        public interface ITypeReference : ITypeProblemNode, ILookUpType {}
        public interface IValue :  ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe {}
        //public interface Member :  IValue, ILookUpType, ICanBeAssignedTo {bool IsReadonly { get; }}
        public interface IExplicitType : IHaveMembers, IScope {}
        public interface IScope : IHaveMembers { }
        public interface IMethod : IHaveMembers, IScope { }


        public class TypeProblem2 : ISetUpTypeProblem
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

            public abstract class TypeProblemNode<T> : TypeProblemNode, IConvertable<T>
            {

                public TypeProblemNode(TypeProblem2 problem, string debugName, IConvertTo<T> converter): base(problem,debugName)
                {
                    Converter = converter;
                }

                public IConvertTo<T> Converter { get; }
            }
            public class TypeReference : TypeProblemNode<TType>, ITypeReference
            {
                public TypeReference(TypeProblem2 problem, string debugName, IConvertTo<TType> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Value : TypeProblemNode<TType>, IValue
            {
                public Value(TypeProblem2 problem, string debugName, IConvertTo<TType> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Member : TypeProblemNode<TType>, IValue, ILookUpType, ICanBeAssignedTo
            {
                public Member(TypeProblem2 problem, bool isReadonly, string debugName, IConvertTo<TType> converter) : base(problem, debugName, converter)
                {
                    IsReadonly = isReadonly;
                }

                public bool IsReadonly
                {
                    get;
                }
            }
            public class Type : TypeProblemNode<TExplictType>, IExplicitType
            {
                public Type(TypeProblem2 problem, string debugName, IConvertTo<TExplictType> converter) : base(problem, debugName, converter)
                {
                }
            }

            public class OrType : TypeProblemNode<TOrType>, IHaveMembers
            {
                public OrType(TypeProblem2 problem, string debugName, IConvertTo<TOrType> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class InferedType : Type
            {
                public InferedType(TypeProblem2 problem, string debugName, IConvertTo<TExplictType> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Scope : TypeProblemNode<TScope>, IScope
            {
                public Scope(TypeProblem2 problem, string debugName, IConvertTo<TScope> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Object : TypeProblemNode<TObject>,  IExplicitType
            {
                public Object(TypeProblem2 problem, string debugName, IConvertTo<TObject> converter) : base(problem, debugName, converter)
                {
                }
            }
            public class Method : TypeProblemNode<TMethod>, IMethod
            {
                public Method(TypeProblem2 problem, string debugName, IConvertTo<TMethod> converter) : base(problem, debugName, converter)
                {
                }
            }

            // basic stuff
            private readonly HashSet<ITypeProblemNode> typeProblemNodes = new HashSet<ITypeProblemNode>();

            public IScope Root { get; }
            // relationships
            private readonly Dictionary<IScope, IScope> kidParent = new Dictionary<IScope, IScope>();

            private readonly Dictionary<Method, Member> methodReturns = new Dictionary<Method, Member>();
            private readonly Dictionary<Method, Member> methodInputs = new Dictionary<Method, Member>();

            private readonly Dictionary<IScope, List<IValue>> values = new Dictionary<IScope, List<IValue>>();
            private readonly Dictionary<IHaveMembers, Dictionary<IKey, Member>> members = new Dictionary<IHaveMembers, Dictionary<IKey, Member>>();
            private readonly Dictionary<IScope, List<ITypeReference>> refs = new Dictionary<IScope, List<ITypeReference>>();
            private readonly Dictionary<IScope, Dictionary<IKey, OrType>> orTypes = new Dictionary<IScope, Dictionary<IKey, OrType>>();
            private readonly Dictionary<IScope, Dictionary<IKey, IExplicitType>> types = new Dictionary<IScope, Dictionary<IKey, IExplicitType>>();
            private readonly Dictionary<IScope, Dictionary<IKey, IHaveMembers>> genericOverlays = new Dictionary<IScope, Dictionary<IKey, IHaveMembers>>();

            private readonly Dictionary<OrType, (ITypeReference, ITypeReference)> orTypeComponets = new Dictionary<OrType, (ITypeReference, ITypeReference)>();

            private readonly Dictionary<IScope, Dictionary<IKey, Member>> possibleMembers = new Dictionary<IScope, Dictionary<IKey, Member>>();
            private readonly Dictionary<IHaveHopefulMembers, Dictionary<IKey, Member>> hopefulMembers = new Dictionary<IHaveHopefulMembers, Dictionary<IKey, Member>>();
            private readonly List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
            // members
            private readonly Dictionary<ILookUpType, IKey> lookUpTypeKey = new Dictionary<ILookUpType, IKey>();
            private readonly Dictionary<ILookUpType, IScope> lookUpTypeContext = new Dictionary<ILookUpType, IScope>();


            #region Building APIs

            public void IsChildOf(IScope parent, IScope kid)
            {
                kidParent.Add(kid, parent);
            }
            public void HasValue(IScope parent, IValue value)
            {
                if (!values.ContainsKey(parent))
                {
                    values.Add(parent, new List<IValue>());
                }
                values[parent].Add(value);
            }
            public void HasReference(IScope parent, ITypeReference reference)
            {
                if (!refs.ContainsKey(parent))
                {
                    refs.Add(parent, new List<ITypeReference>());
                }
                refs[parent].Add(reference);
            }
            public void HasType(IScope parent, IKey key, IExplicitType type)
            {
                if (!types.ContainsKey(parent))
                {
                    types.Add(parent, new Dictionary<IKey, IExplicitType>());
                }
                types[parent].Add(key, type);
            }

            public void HasPlaceholderType(IScope parent, IKey key, IHaveMembers type)
            {
                if (!genericOverlays.ContainsKey(parent))
                {
                    genericOverlays.Add(parent, new Dictionary<IKey, IHaveMembers>());
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
            public void HasMembersPossiblyOnParent(IScope parent, IKey key, Member member)
            {
                if (!possibleMembers.ContainsKey(parent))
                {
                    possibleMembers.Add(parent, new Dictionary<IKey, Member>());
                }
                possibleMembers[parent].Add(key, member);
            }
            public void HasHopefulMember(IHaveHopefulMembers parent, IKey key, Member member)
            {

                if (!hopefulMembers.ContainsKey(parent))
                {
                    hopefulMembers.Add(parent, new Dictionary<IKey, Member>());
                }
                hopefulMembers[parent].Add(key, member);
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

            public Value CreateValue(IScope scope, IKey typeKey, IConvertTo<TType> converter)
            {
                var res = new Value(this, typeKey.ToString(), converter);
                HasValue(scope, res);
                lookUpTypeContext[res] = scope;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IKey typeKey, bool isReadonly, IConvertTo<TType> converter)
            {
                var res = new Member(this, isReadonly, key.ToString(), converter);
                HasMember(scope, key, res);
                lookUpTypeContext[res] = scope;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Member CreateMember(IScope scope, IKey key, IConvertTo<TType> converter)
            {
                var res = new Member(this, false, key.ToString(), converter);
                HasMember(scope, key, res);
                lookUpTypeContext[res] = scope;
                return res;
            }

            public Member CreateMemberPossiblyOnParent(IScope scope, IKey key, IConvertTo<TType> converter)
            {
                var res = new Member(this, false, key.ToString(),converter);
                HasMembersPossiblyOnParent(scope, key, res);
                lookUpTypeContext[res] = scope;
                return res;
            }

            public TypeReference CreateTypeReference(IScope context, IKey typeKey, IConvertTo<TType> converter)
            {
                var res = new TypeReference(this, typeKey.ToString(),converter);
                HasReference(context, res);
                lookUpTypeContext[res] = context;
                lookUpTypeKey[res] = typeKey;
                return res;
            }

            public Scope CreateScope(IScope parent, IConvertTo<TScope> converter)
            {
                var res = new Scope(this, $"child-of-{((TypeProblemNode)parent).debugName}", converter);
                IsChildOf(parent, res);
                return res;
            }

            public Type CreateType(IScope parent, IKey key, IConvertTo<TExplictType> converter)
            {
                var res = new Type(this, key.ToString(), converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
                return res;
            }

            public Type CreateGenericType(IScope parent, IKey key, IReadOnlyList<IKey> placeholders, IConvertTo<TExplictType> converter)
            {
                var res = new Type(this, $"generic-{key.ToString()}-{placeholders.Aggregate("", (x, y) => x + "-" + y.ToString())}", converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
                foreach (var placeholder in placeholders)
                {
                    var placeholderType = new Type(this, $"generic-parameter-{placeholder.ToString()}");
                    HasPlaceholderType(res, placeholder, placeholderType);
                }
                return res;
            }

            public Object CreateObject(IScope parent, IKey key, IConvertTo<TObject> converter)
            {
                var res = new Object(this, key.ToString(), converter);
                IsChildOf(parent, res);
                HasType(parent, key, res);
                return res;
            }

            public Method CreateMethod(IScope parent, string inputName, IConvertTo<TMethod> converter)
            {
                var res = new Method(this, $"method{{inputName:{inputName}}}", converter);
                IsChildOf(parent, res);
                var returns = CreateMember(res, new ImplicitKey());
                methodReturns[res] = returns;
                var input = CreateMember(res, new NameKey(inputName));
                methodInputs[res] = input;
                return res;
            }


            public Method CreateMethod(IScope parent, ITypeReference inputType, ITypeReference outputType, string inputName, IConvertTo<TMethod> converter)
            {

                var res = new Method(this, $"method{{inputName:{inputName},inputType:{((TypeProblemNode)inputType).debugName},outputType:{((TypeProblemNode)outputType).debugName}}}", converter);
                IsChildOf(parent, res);
                var returns = lookUpTypeKey.TryGetValue(inputType, out var outkey) ? CreateMember(res, new ImplicitKey(), outkey, false) : CreateMember(res, new ImplicitKey());
                methodReturns[res] = returns;
                if (lookUpTypeKey.TryGetValue(inputType, out var inkey))
                {
                    methodInputs[res] = CreateMember(res, new NameKey(inputName), inkey, false);
                }
                else
                {
                    methodInputs[res] = CreateMember(res, new NameKey(inputName));
                }
                return res;
            }


            public Member CreateHopefulMember(IHaveHopefulMembers scope, IKey key, IConvertTo<TType> converter)
            {
                var res = new Member(this, false, key.ToString(), converter);
                HasHopefulMember(scope, key, res);
                return res;
            }


            public OrType CreateOrType(IScope s, IKey key, ITypeReference setUpSideNode1, ITypeReference setUpSideNode2, IConvertTo<TOrType> converter)
            {
                var res = new OrType(this, $"{((TypeProblemNode)setUpSideNode1).debugName} || {((TypeProblemNode)setUpSideNode2).debugName}", converter);
                Ors(res, setUpSideNode1, setUpSideNode2);
                HasOrType(s, key, res);

                return res;

            }


            private void Ors(OrType orType, ITypeReference a, ITypeReference b)
            {
                orTypeComponets[orType] = (a, b);
            }

            private void HasOrType(IScope scope, IKey kay, OrType orType1)
            {
                if (!orTypes.ContainsKey(scope))
                {
                    orTypes[scope] = new Dictionary<IKey, OrType>();
                }
                orTypes[scope][kay] = orType1;
            }


            #endregion


            public Member GetReturns(IScope s)
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

            internal Member GetReturns(Method method)
            {
                return methodReturns[method];
            }


            public Member GetInput(Method method)
            {
                return methodInputs[method];
            }

            public IKey GetKey(ITypeReference type)
            {
                return lookUpTypeKey[type];
            }

            // pretty sure it is not safe to solve more than once 
            public ITypeSolution Solve()
            {
                var realizedGeneric = new Dictionary<GenericTypeKey, IExplicitType>();
                var lookUps = new Dictionary<ILookUpType, IHaveMembers>();

                // create types for everything 
                var toLookUp = typeProblemNodes.OfType<ILookUpType>().ToArray();
                foreach (var node in toLookUp.Where(x => !lookUpTypeKey.ContainsKey(x)))
                {
                    var key = new ImplicitKey();
                    var type = new InferedType(this, $"for {((TypeProblemNode)node).debugName}");
                    lookUps[node] = type;
                }

                // generics register themsleves 
                foreach (var node in typeProblemNodes.OfType<IExplicitType>().Where(x => genericOverlays.TryGetValue(x, out var dict) && dict.Any()))
                {
                    var key = new GenericTypeKey(node, genericOverlays[node].Values.ToArray());
                    realizedGeneric[key] = node;
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
                var defersTo = new Dictionary<IHaveMembers, IHaveMembers>();

                foreach (var item in possibleMembers)
                {
                    foreach (var pair in item.Value)
                    {
                        if (TryGetMember(item.Key, pair.Key, out var member))
                        {
                            defersTo[GetType(pair.Value)] = GetType(member);
                        }
                        else
                        {
                            HasMember(item.Key, pair.Key, pair.Value);
                        }
                    }
                }

                var orTypeMembers = new Dictionary<OrType, Dictionary<IKey, Member>>();

                // hopeful members 

                foreach (var hopeful in hopefulMembers)
                {
                    foreach (var pair in hopeful.Value)
                    {
                        if (GetMembers(GetType(hopeful.Key)).TryGetValue(pair.Key, out var member))
                        {
                            defersTo[GetType(pair.Value)] = GetType(member);
                        }
                        else if (GetType(hopeful.Key) is InferedType infered)
                        {
                            HasMember(infered, pair.Key, pair.Value);
                        }
                        else
                        {
                            throw new Exception("member could not be handled ");
                        }
                    }
                }

                //var flowFroms = assignments.Select(x => x.Item1).ToList();
                //var nextFlowFroms = flowFroms;
                //while (flowFroms.Any()) {
                //    nextFlowFroms = new List<ICanAssignFromMe>();
                //    foreach (var from in flowFroms)
                //    {
                //        if (flowMap.ContainsKey(from))
                //        {
                //            foreach (var to in flowMap[from])
                //            {
                //                if (Flow(GetType(from), GetType(to))) {
                //                    if (!nextFlowFroms.Contains(to)) {
                //                        nextFlowFroms.Add(to);
                //                    }   
                //                }
                //            }
                //        }
                //    }
                //    flowFroms = nextFlowFroms;
                //}

                // very sloppy and slow
                // if I never am worried about speed I am sure this will be a canidate
                bool go;
                do
                {
                    go = false;
                    foreach (var (from, to) in assignments)
                    {
                        go |= Flow(GetType(to), GetType(from));
                    }
                } while (go);


                // we dont flow downstream
                // flow downstream
                // we can't flow through convergences, since it might be an or-type
                //foreach (var (from, to) in assignments.GroupBy(x => x.Item2).Where(x => x.Count() == 1).SelectMany(x => x))
                //{
                //    Flow(GetType(to), GetType(from));
                //}

                #region Result

                var convetCache = new Dictionary<IHaveMembers, OrType<OrSolutionType, ConcreteSolutionType>>();
                var convetCache2 = new Dictionary<(ITypeReference, ITypeReference), OrType<OrSolutionType, ConcreteSolutionType>>();


                /// ok now build the result 
                var resultLookups = new Dictionary<ILookUpType, OrType<OrSolutionType, ConcreteSolutionType>>();

                foreach (var item in lookUps)
                {
                    resultLookups.Add(item.Key, Convert(item.Value));
                }

                var resultExplicitTypes = new Dictionary<IExplicitType, OrType<OrSolutionType, ConcreteSolutionType>>();

                foreach (var values in types.Values)
                {
                    foreach (var item in values)
                    {
                        resultExplicitTypes.Add(item.Value, Convert(item.Value));
                    }
                }

                var resultOrTypes = new Dictionary<OrType, OrType<OrSolutionType, ConcreteSolutionType>>();

                foreach (var item in orTypeComponets)
                {
                    resultOrTypes.Add(item.Key, Convert2(item.Value.Item1, item.Value.Item2));
                }

                var resultScopes = new Dictionary<IScope, OrType<OrSolutionType, ConcreteSolutionType>>();

                foreach (var item in kidParent.Keys)
                {
                    resultScopes.Add(item, Convert(item));
                }
                resultScopes.Add(Root, Convert(Root));

                OrType<OrSolutionType, ConcreteSolutionType> Convert(IHaveMembers haveMembers)
                {

                    if (convetCache.TryGetValue(haveMembers, out var res))
                    {
                        return res;
                    }

                    var diction = new Dictionary<IKey, (bool, OrType<OrSolutionType, ConcreteSolutionType>)>();
                    var conveted = new ConcreteSolutionType(diction);
                    res = new OrType<OrSolutionType, ConcreteSolutionType>(conveted);
                    convetCache[haveMembers] = res;
                    if (members.ContainsKey(haveMembers))
                    {
                        foreach (var member in members[haveMembers])
                        {
                            diction[member.Key] = (member.Value.IsReadonly, Convert(lookUps[member.Value]));
                        }
                    }
                    return res;
                }

                OrType<OrSolutionType, ConcreteSolutionType> Convert2(ITypeReference left, ITypeReference right)
                {
                    if (convetCache2.TryGetValue((left, right), out var res))
                    {
                        return res;
                    }

                    res = new OrType<OrSolutionType, ConcreteSolutionType>(new OrSolutionType(Convert(lookUps[left]), Convert(lookUps[right])));
                    convetCache2[(left, right)] = res;
                    return res;
                }

                return new TypeSolution(resultLookups, resultExplicitTypes, resultOrTypes, resultScopes);

                #endregion


                #region Helpers

                IHaveMembers LookUpOrOverlayOrThrow(ILookUpType node)
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



                IHaveMembers LookUpOrOverlayOrThrow2(IScope from, IKey key)
                {
                    if (!TryLookUpOrOverlay(from, key, out var res))
                    {
                        throw new Exception("could not find type");
                    }
                    return res;
                }

                bool TryLookUpOrOverlay(IScope from, IKey key, out IHaveMembers res)
                {

                    if (key is GenericNameKey genericNameKey)
                    {

                        var types = genericNameKey.Types.Select(typeKey => LookUpOrOverlayOrThrow2(from, typeKey)).ToArray();

                        if (!(LookUpOrOverlayOrThrow2(from, genericNameKey.name) is IExplicitType lookedUp))
                        {
                            throw new Exception();
                        }
                        var genericTypeKey = new GenericTypeKey(lookedUp, types);

                        if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                        {
                            res = res2;
                            return true;
                        }

                        var map = new Dictionary<IHaveMembers, IHaveMembers>();
                        foreach (var (oldType, newType) in types.Zip(genericOverlays[lookedUp], (x, y) => (y.Value, x)))
                        {
                            map[oldType] = newType;
                        }

                        var explict = CopyTree(lookedUp, new Type(this, $"generated-generic-{((TypeProblemNode)lookedUp).debugName}"), map);
                        realizedGeneric.Add(genericTypeKey, explict);
                        res = explict;
                        return true;
                    }
                    else
                    if (TryLookUp(from, key, out res))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                bool TryLookUp(IScope haveTypes, IKey key, out IHaveMembers result)
                {
                    while (true)
                    {
                        {
                            if (types.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = res;
                                return true;
                            }
                        }
                        {
                            if (orTypes.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = res;
                                return true;
                            }
                        }
                        {
                            if (genericOverlays.TryGetValue(haveTypes, out var dict) && dict.TryGetValue(key, out var res))
                            {
                                result = res;
                                return true;
                            }
                        }
                        if (!kidParent.TryGetValue(haveTypes, out haveTypes))
                        {
                            result = null;
                            return false;
                        }
                    }
                }

                IExplicitType CopyTree(IExplicitType from, IExplicitType to, IReadOnlyDictionary<IHaveMembers, IHaveMembers> overlayed)
                {

                    var map = new Dictionary<ITypeProblemNode, ITypeProblemNode>();
                    Copy(from, to);

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
                            Ors(orTo, CopiedToOrSelf(orTypeComponets[orFrom].Item1), CopiedToOrSelf(orTypeComponets[orFrom].Item2));
                        }


                        if (pair.Key is Method methodFrom && pair.Value is Method methodTo)
                        {
                            methodInputs[methodTo] = CopiedToOrSelf(methodInputs[methodFrom]);
                            methodReturns[methodTo] = CopiedToOrSelf(methodReturns[methodFrom]);
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
                                        var newValue = Copy(item, new Value(this, $"copied from {((TypeProblemNode)item).debugName}"));
                                        HasValue(innerScopeTo, newValue);
                                    }
                                }
                            }

                            {
                                if (refs.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var item in dict)
                                    {
                                        var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}"));
                                        HasReference(innerScopeTo, newValue);
                                    }
                                }
                            }

                            {
                                if (members.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var member in dict)
                                    {
                                        var newValue = Copy(member.Value, new Member(this, member.Value.IsReadonly, $"copied from {((TypeProblemNode)member.Value).debugName}"));
                                        HasMember(innerScopeTo, member.Key, newValue);
                                    }
                                }
                            }


                            {
                                if (types.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}"));
                                        HasType(innerScopeTo, type.Key, newValue);
                                    }
                                }
                            }

                            {
                                if (orTypes.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}"));
                                        HasOrType(innerScopeTo, type.Key, newValue);
                                    }
                                }
                            }


                            {
                                if (genericOverlays.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var type in dict)
                                    {
                                        if (overlayed.TryGetValue(type.Value, out var toType))
                                        {
                                            HasPlaceholderType(innerScopeTo, type.Key, toType);
                                        }
                                        else
                                        {
                                            HasPlaceholderType(innerScopeTo, type.Key, type.Value);
                                        }
                                    }
                                }
                            }

                            {
                                if (possibleMembers.TryGetValue(innerFromScope, out var dict))
                                {
                                    foreach (var possible in dict)
                                    {
                                        var newValue = Copy(possible.Value, new Member(this, possible.Value.IsReadonly, $"copied from {((TypeProblemNode)possible.Value).debugName}"));
                                        HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                                    }
                                }
                            }
                        }

                        if (innerFrom is IHaveHopefulMembers innerFromHopeful && innerTo is IHaveHopefulMembers innerToHopeful)
                        {
                            if (hopefulMembers.TryGetValue(innerFromHopeful, out var dict))
                            {
                                foreach (var possible in dict)
                                {
                                    var newValue = Copy(possible.Value, new Member(this, possible.Value.IsReadonly, $"copied from {((TypeProblemNode)possible.Value).debugName}"));
                                    HasHopefulMember(innerToHopeful, possible.Key, newValue);
                                }
                            }
                        }

                        return innerTo;
                    }
                }


                IHaveMembers GetType2(ILookUpType value)
                {
                    var res = lookUps[value];
                    while (true)
                    {
                        if (res is IExplicitType explicitType && defersTo.TryGetValue(explicitType, out var nextRes))
                        {
                            res = nextRes;
                        }
                        else
                        {
                            return res;
                        }
                    }
                }

                IHaveMembers GetType(ITypeProblemNode value)
                {
                    if (value is ILookUpType lookup)
                    {
                        return GetType2(lookup);
                    }
                    if (value is IHaveMembers haveMembers)
                    {
                        return haveMembers;
                    }

                    throw new Exception("flaming pile of piss");
                    // well, I guess I now know that we have a duality
                    // you either are a type, or you have a type
                    // 
                }

                // returns true if the target was modified 
                bool Flow(IHaveMembers from, IHaveMembers to)
                {
                    var res = false;
                    // I think the only thing that "flow" are members
                    // but not all types will accept new members
                    if (to is InferedType infered)
                    {
                        foreach (var memberPair in GetMembers(from))
                        {
                            if (!members.ContainsKey(infered))
                            {
                                members[infered] = new Dictionary<IKey, Member>();
                            }
                            var dict = members[infered];
                            if (dict.TryGetValue(memberPair.Key, out var upstreamMember))
                            {
                                var one = GetType(upstreamMember);
                                var two = GetType(memberPair.Value);
                                if (one is InferedType oneInfered)
                                {
                                    res |= Flow(two, oneInfered);
                                }
                                else if (two is InferedType twoInfered)
                                {
                                    res |= Flow(one, twoInfered);
                                }
                                else
                                {
                                    throw new Exception("these types are not compatible... right?");
                                }
                            }
                            else
                            {

                                var newValue = new Member(this, memberPair.Value.IsReadonly, $"copied from {((TypeProblemNode)memberPair.Value).debugName}");
                                HasMember(infered, memberPair.Key, newValue);
                                lookUps[newValue] = lookUps[memberPair.Value];
                                res = true;
                            }
                        }
                    }
                    return res;
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
                        var (left, right) = orTypeComponets[orType];

                        var rightMembers = GetMembers(GetType(right));
                        foreach (var leftMember in GetMembers(GetType(left)))
                        {
                            if (rightMembers.TryGetValue(leftMember.Key, out var rightMember))
                            {
                                // if they are the same type
                                if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value)))
                                {
                                    var member = new Member(this, leftMember.Value.IsReadonly, $"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}");
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
                    if (members[context].TryGetValue(key, out member))
                    {
                        return true;
                    }
                    if (!kidParent.TryGetValue(context, out context))
                    {
                        return false;
                    }
                }
            }


            public TypeProblem2()
            {
                Root = new Scope(this, "root");
                //CreateGenericType(Root, new NameKey("method"), new IKey[] {
                //    new NameKey("input"),
                //    new NameKey("output")
                //});

                //CreateGenericType(Root, new NameKey("implementation"), new IKey[] {
                //    new NameKey("context"),
                //    new NameKey("input"),
                //    new NameKey("output")
                //});
                //CreateType(Root, new NameKey("number"));
                //CreateType(Root, new NameKey("string"));
                //CreateType(Root, new NameKey("bool"));
                //CreateType(Root, new NameKey("empty"));
            }

            private class GenericTypeKey
            {
                private readonly IExplicitType primary;
                private readonly IHaveMembers[] parameters;

                public GenericTypeKey(IExplicitType primary, IHaveMembers[] parameters)
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
                        parameters.Count() == other.parameters.Count() &&
                        parameters.Zip(other.parameters, (x, y) => x.Equals(y)).All(x => x);
                }

                public override int GetHashCode()
                {
                    return primary.GetHashCode() + parameters.Sum(x => x.GetHashCode());
                }
            }

        }

    }

    public static class TpnExtensions
    {
        //extensions
        public static IKey Key<TType, TScope, TExplictType, TObject, TOrType, TMethod>(this Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.ITypeReference type)
        {
            return type.Problem.GetKey(type);
        }

        public static Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.TypeProblem2.Member Returns<TType, TScope, TExplictType, TObject, TOrType, TMethod>(this Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.IMethod method)
        {
            return method.Problem.GetReturns(method);
        }


        public static Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.TypeProblem2.Member Input<TType, TScope, TExplictType, TObject, TOrType, TMethod>(this Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.IMethod method)
        {
            return method.Problem.GetInput(method);
        }

        public static void AssignTo<TType, TScope, TExplictType, TObject, TOrType, TMethod>(this Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.ICanAssignFromMe from, Tpn<TType, TScope, TExplictType, TObject, TOrType, TMethod>.ICanBeAssignedTo to)
        {
            from.Problem.IsAssignedTo(from, to);
        }
    }
}
