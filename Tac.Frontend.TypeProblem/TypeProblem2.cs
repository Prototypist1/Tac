using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;

namespace Tac.Frontend.New.CrzayNamespace
{

    public interface ISetUpTypeProblem
    {
        // a =: x

        void IsAssignedTo(Tpn.ICanAssignFromMe assignedFrom, Tpn.ICanBeAssignedTo assignedTo);
        Tpn.IValue CreateValue(Tpn.IScope scope, IKey typeKey);
        Tpn.IMember CreateMember(Tpn.IScope scope, IKey key, IKey typeKey);
        Tpn.IMember CreateMember(Tpn.IScope scope, IKey key);
        Tpn.ITypeReference CreateTypeReference(Tpn.IScope context, IKey typeKey);
        Tpn.IScope CreateScope(Tpn.IScope parent);
        Tpn.IExplicitType CreateType(Tpn.IScope parent, IKey key);
        Tpn.IExplicitType CreateGenericType(Tpn.IScope parent, IKey key, IReadOnlyList<IKey> placeholders);
        Tpn.IObject CreateObject(Tpn.IScope parent, IKey key);
        Tpn.IMethod CreateMethod(Tpn.IScope parent, string inputName);
        Tpn.IMethod CreateMethod(Tpn.IScope parent, Tpn.ITypeReference inputType, Tpn.ITypeReference outputType, string inputName);
        Tpn.IMember GetReturns(Tpn.IScope s);
        Tpn.IMember CreateHopefulMember(Tpn.IHaveHopefulMembers scope, IKey key);
        Tpn.IOrType CreateOrType(Tpn.IScope s, IKey key, Tpn.ITypeReference setUpSideNode1, Tpn.ITypeReference setUpSideNode2);
        IKey GetKey(Tpn.ITypeReference type);
        Tpn.IMember GetInput(Tpn.IMethod method);
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

    public static class TypeProblemNodeExtensions
    {

        public static IKey Key(this Tpn.ITypeReference type)
        {
            return type.Problem.GetKey(type);
        }

        public static Tpn.IMember Returns(this Tpn.IMethod method)
        {
            return method.Problem.GetReturns(method);
        }


        public static Tpn.IMember Input(this Tpn.IMethod method)
        {
            return method.Problem.GetInput(method);
        }

        public static void AssignTo(this Tpn.ICanAssignFromMe from, Tpn.ICanBeAssignedTo to)
        {
            from.Problem.IsAssignedTo(from, to);
        }
    }


    public static class Tpn
    {
        public interface IType { }

        public interface ITypeProblemNode
        {
            ISetUpTypeProblem Problem { get; }
        }
        public interface IHaveMembers : ITypeProblemNode { }
        public interface IHaveHopefulMembers : ITypeProblemNode { }
        public interface ILookUpType : ITypeProblemNode { }

        public interface ICanAssignFromMe : ITypeProblemNode, ILookUpType { }
        public interface ICanBeAssignedTo : ITypeProblemNode, ILookUpType { }


        public interface ITypeReference : ITypeProblemNode, ILookUpType { }
        public interface IValue : ITypeProblemNode, ILookUpType, IHaveHopefulMembers, ICanAssignFromMe { }
        public interface IMember : IValue, ILookUpType, ICanBeAssignedTo { }

        public interface IExplicitType : IHaveMembers, IScope, IType { }
        public interface IOrType : IHaveMembers, IType { }
        public interface IScope : IHaveMembers { }
        public interface IObject : IExplicitType { }
        public interface IMethod : IHaveMembers, IScope { }

    }

    internal class TypeProblem2 : ISetUpTypeProblem
    {

        private abstract class TypeProblemNode : Tpn.ITypeProblemNode
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
        private class TypeReference : TypeProblemNode, Tpn.ITypeReference
        {
            public TypeReference(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Value : TypeProblemNode, Tpn.IValue
        {
            public Value(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Member : TypeProblemNode, Tpn.IMember
        {
            public Member(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Type : TypeProblemNode, Tpn.IExplicitType
        {
            public Type(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }

        private class OrType : TypeProblemNode, Tpn.IOrType
        {
            public OrType(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class InferedType : Type
        {
            public InferedType(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Scope : TypeProblemNode, Tpn.IScope
        {
            public Scope(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Object : TypeProblemNode, Tpn.IObject
        {
            public Object(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }
        private class Method : TypeProblemNode, Tpn.IMethod
        {
            public Method(TypeProblem2 problem, string debugName) : base(problem, debugName)
            {
            }
        }

        // basic stuff
        private readonly HashSet<Tpn.ITypeProblemNode> typeProblemNodes = new HashSet<Tpn.ITypeProblemNode>();

        public Tpn.IScope Root { get; }
        // relationships
        private readonly Dictionary<Tpn.IScope, Tpn.IScope> kidParent = new Dictionary<Tpn.IScope, Tpn.IScope>();

        private readonly Dictionary<Tpn.IMethod, Tpn.IMember> methodReturns = new Dictionary<Tpn.IMethod, Tpn.IMember>();
        private readonly Dictionary<Tpn.IMethod, Tpn.IMember> methodInputs = new Dictionary<Tpn.IMethod, Tpn.IMember>();

        private readonly Dictionary<Tpn.IScope, List<Tpn.IValue>> values = new Dictionary<Tpn.IScope, List<Tpn.IValue>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>> members = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>>();
        private readonly Dictionary<Tpn.IScope, List<Tpn.ITypeReference>> refs = new Dictionary<Tpn.IScope, List<Tpn.ITypeReference>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IOrType>> orTypes = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IOrType>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IExplicitType>> types = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IExplicitType>>();
        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IHaveMembers>> genericOverlays = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IHaveMembers>>();


        private readonly Dictionary<Tpn.IOrType, (Tpn.ITypeReference, Tpn.ITypeReference)> orTypeComponets = new Dictionary<Tpn.IOrType, (Tpn.ITypeReference, Tpn.ITypeReference)>();

        private readonly Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>> possibleMembers = new Dictionary<Tpn.IScope, Dictionary<IKey, Tpn.IMember>>();
        private readonly Dictionary<Tpn.IHaveHopefulMembers, Dictionary<IKey, Tpn.IMember>> hopefulMembers = new Dictionary<Tpn.IHaveHopefulMembers, Dictionary<IKey, Tpn.IMember>>();
        private readonly List<(Tpn.ICanAssignFromMe, Tpn.ICanBeAssignedTo)> assignments = new List<(Tpn.ICanAssignFromMe, Tpn.ICanBeAssignedTo)>();
        // members
        private readonly Dictionary<Tpn.ILookUpType, IKey> lookUpTypeKey = new Dictionary<Tpn.ILookUpType, IKey>();
        private readonly Dictionary<Tpn.ILookUpType, Tpn.IScope> lookUpTypeContext = new Dictionary<Tpn.ILookUpType, Tpn.IScope>();

        private readonly Dictionary<GenericTypeKey, Tpn.IExplicitType> realizedGeneric = new Dictionary<GenericTypeKey, Tpn.IExplicitType>();

        #region Building APIs

        public void IsChildOf(Tpn.IScope parent, Tpn.IScope kid)
        {
            kidParent.Add(kid, parent);
        }
        public void HasValue(Tpn.IScope parent, Tpn.IValue value)
        {
            if (!values.ContainsKey(parent))
            {
                values.Add(parent, new List<Tpn.IValue>());
            }
            values[parent].Add(value);
        }
        public void HasReference(Tpn.IScope parent, Tpn.ITypeReference reference)
        {
            if (!refs.ContainsKey(parent))
            {
                refs.Add(parent, new List<Tpn.ITypeReference>());
            }
            refs[parent].Add(reference);
        }
        public void HasType(Tpn.IScope parent, IKey key, Tpn.IExplicitType type)
        {
            if (!types.ContainsKey(parent))
            {
                types.Add(parent, new Dictionary<IKey, Tpn.IExplicitType>());
            }
            types[parent].Add(key, type);
        }

        public void HasPlaceholderType(Tpn.IScope parent, IKey key, Tpn.IHaveMembers type)
        {
            if (!genericOverlays.ContainsKey(parent))
            {
                genericOverlays.Add(parent, new Dictionary<IKey, Tpn.IHaveMembers>());
            }
            genericOverlays[parent].Add(key, type);
        }
        public void HasMember(Tpn.IScope parent, IKey key, Tpn.IMember member)
        {
            if (!members.ContainsKey(parent))
            {
                members.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            members[parent].Add(key, member);
        }
        public void HasMembersPossiblyOnParent(Tpn.IScope parent, IKey key, Tpn.IMember member)
        {
            if (!possibleMembers.ContainsKey(parent))
            {
                possibleMembers.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            possibleMembers[parent].Add(key, member);
        }
        public void HasHopefulMember(Tpn.IHaveHopefulMembers parent, IKey key, Tpn.IMember member)
        {

            if (!hopefulMembers.ContainsKey(parent))
            {
                hopefulMembers.Add(parent, new Dictionary<IKey, Tpn.IMember>());
            }
            hopefulMembers[parent].Add(key, member);
        }

        private T Register<T>(T typeProblemNode)
            where T : Tpn.ITypeProblemNode
        {
            typeProblemNodes.Add(typeProblemNode);
            return typeProblemNode;
        }

        public void IsAssignedTo(Tpn.ICanAssignFromMe assignedFrom, Tpn.ICanBeAssignedTo assignedTo)
        {
            assignments.Add((assignedFrom, assignedTo));
        }

        public Tpn.IValue CreateValue(Tpn.IScope scope, IKey typeKey)
        {
            var res = new Value(this, typeKey.ToString());
            HasValue(scope, res);
            lookUpTypeContext[res] = scope;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IMember CreateMember(Tpn.IScope scope, IKey key, IKey typeKey)
        {
            var res = new Member(this, key.ToString());
            HasMember(scope, key, res);
            lookUpTypeContext[res] = scope;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IMember CreateMember(Tpn.IScope scope, IKey key)
        {
            var res = new Member(this, key.ToString());
            HasMember(scope, key, res);
            lookUpTypeContext[res] = scope;
            return res;
        }



        public Tpn.ITypeReference CreateTypeReference(Tpn.IScope context, IKey typeKey)
        {
            var res = new TypeReference(this, typeKey.ToString());
            HasReference(context, res);
            lookUpTypeContext[res] = context;
            lookUpTypeKey[res] = typeKey;
            return res;
        }

        public Tpn.IScope CreateScope(Tpn.IScope parent)
        {
            var res = new Scope(this,$"child-of-{((TypeProblemNode)parent).debugName}");
            IsChildOf(parent, res);
            return res;
        }

        public Tpn.IExplicitType CreateType(Tpn.IScope parent, IKey key)
        {
            var res = new Type(this,key.ToString());
            IsChildOf(parent, res);
            HasType(parent, key, res);
            return res;
        }

        public Tpn.IExplicitType CreateGenericType(Tpn.IScope parent, IKey key, IReadOnlyList<IKey> placeholders)
        {
            var res = new Type(this,$"generic-{key.ToString()}-{placeholders.Aggregate("",(x,y)=>x+"-"+y.ToString())}");
            IsChildOf(parent, res);
            HasType(parent, key, res);
            foreach (var placeholder in placeholders)
            {
                var placeholderType = new Type(this,$"generic-parameter-{placeholder.ToString()}");
                HasPlaceholderType(res, placeholder, placeholderType);
            }
            return res;
        }

        public Tpn.IObject CreateObject(Tpn.IScope parent, IKey key)
        {
            var res = new Object(this, key.ToString());
            IsChildOf(parent, res);
            HasType(parent, key, res);
            return res;
        }

        public Tpn.IMethod CreateMethod(Tpn.IScope parent, string inputName)
        {
            var res = new Method(this, $"method{{inputName:{inputName}}}");
            IsChildOf(parent, res);
            var returns = CreateMember(res, new ImplicitKey());
            methodReturns[res] = returns;
            var input= CreateMember(res, new NameKey(inputName));
            methodInputs[res] = input;
            return res;
        }


        public Tpn.IMethod CreateMethod(Tpn.IScope parent, Tpn.ITypeReference inputType, Tpn.ITypeReference outputType, string inputName)
        {

            var res = new Method(this, $"method{{inputName:{inputName},inputType:{((TypeProblemNode)inputType).debugName},outputType:{((TypeProblemNode)outputType).debugName}}}");
            IsChildOf(parent, res);
            var returns = lookUpTypeKey.TryGetValue(inputType, out var outkey) ? CreateMember(res, new ImplicitKey(), outkey) : CreateMember(res, new ImplicitKey());
            methodReturns[res] = returns;
            if (lookUpTypeKey.TryGetValue(inputType, out var inkey)) {
                methodInputs[res] = CreateMember(res, new NameKey(inputName), inkey);
            } else {
                methodInputs[res] = CreateMember(res, new NameKey(inputName));
            }
            return res;
        }


        public Tpn.IMember CreateHopefulMember(Tpn.IHaveHopefulMembers scope, IKey key)
        {
            var res = new Member(this,key.ToString());
            HasHopefulMember(scope, key, res);
            return res;
        }


        public Tpn.IOrType CreateOrType(Tpn.IScope s, IKey key, Tpn.ITypeReference setUpSideNode1, Tpn.ITypeReference setUpSideNode2)
        {
            var res = new OrType(this,$"{((TypeProblemNode)setUpSideNode1).debugName} || {((TypeProblemNode)setUpSideNode2).debugName}");
            Ors(res, setUpSideNode1, setUpSideNode2);
            HasOrType(s, key, res);

            return res;

        }


        private void Ors(Tpn.IOrType orType, Tpn.ITypeReference a, Tpn.ITypeReference b)
        {
            orTypeComponets[orType] = (a, b);
        }

        private void HasOrType(Tpn.IScope scope, IKey kay, Tpn.IOrType orType1)
        {
            if (!orTypes.ContainsKey(scope))
            {
                orTypes[scope] = new Dictionary<IKey, Tpn.IOrType>();
            }
            orTypes[scope][kay] = orType1;
        }


        #endregion


        public Tpn.IMember GetReturns(Tpn.IScope s)
        {
            if (s is Tpn.IMethod method)
            {
                return GetReturns(method);
            }
            else
            {
                return GetReturns(kidParent[s]);
            }
        }

        internal Tpn.IMember GetReturns(Tpn.IMethod method)
        {
            return methodReturns[method];
        }


        public Tpn.IMember GetInput(Tpn.IMethod method)
        {
            return methodInputs[method];
        }

        public IKey GetKey(Tpn.ITypeReference type)
        {
            return lookUpTypeKey[type];
        }


        public void Solve()
        {
            var lookUps =  new Dictionary<Tpn.ILookUpType, Tpn.IHaveMembers>();

            // create types for everything 
            var toLookUp = typeProblemNodes.OfType<Tpn.ILookUpType>().ToArray();
            foreach (var node in toLookUp.Where(x => !lookUpTypeKey.ContainsKey(x)))
            {
                var key = new ImplicitKey();
                var type = new InferedType(this,$"for {((TypeProblemNode)node).debugName}");
                lookUps[node] = type;
            }

            toLookUp = typeProblemNodes.OfType<Tpn.ILookUpType>().Except(lookUps.Keys).ToArray();

            // overlay generics
            while (toLookUp.Any())
            {
                foreach (var node in toLookUp)
                {
                    lookUps[node] = LookUpOrOverlayOrThrow(lookUpTypeContext[node], lookUpTypeKey[node]);
                }
                toLookUp = typeProblemNodes.OfType<Tpn.ILookUpType>().Except(lookUps.Keys).ToArray();
            }

            // members that might be on parents 
            var defersTo = new Dictionary<Tpn.IHaveMembers, Tpn.IHaveMembers>();

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

            var orTypeMembers = new Dictionary<Tpn.IOrType, Dictionary<IKey, Tpn.IMember>>();

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
                    else {
                        throw new Exception("member could not be handled ");
                    }
                }
            }

            // flow upstream
            foreach (var (from, to) in assignments)
            {
                Flow(GetType(from), GetType(to));
            }

            // flow downstream
            // we can't flow through convergences, since it might be an or-type
            foreach (var (from, to) in assignments.GroupBy(x => x.Item2).Where(x => x.Count() == 1).SelectMany(x => x))
            {
                Flow(GetType(to), GetType(from));
            }

            #region Helpers


            Tpn.IHaveMembers GetType2(Tpn.ILookUpType value)
            {
                var res = lookUps[value];
                while (true)
                {
                    if (res is Tpn.IExplicitType explicitType && defersTo.TryGetValue(explicitType, out var nextRes))
                    {
                        res = nextRes;
                    }
                    else
                    {
                        return res;
                    }
                }
            }

            Tpn.IHaveMembers GetType(Tpn.ITypeProblemNode value) {
                if (value is Tpn.ILookUpType lookup) {
                    return GetType2(lookup);
                }
                if (value is Tpn.IHaveMembers haveMembers) {
                    return haveMembers;
                }

                throw new Exception("flaming pile of piss");
                // well, I guess I now know that we have a duality
                // you either are a type, or you have a type
                // 
            }


            void Flow(Tpn.IHaveMembers from, Tpn.IHaveMembers to)
            {
                // I think the only thing that "flow" are members
                // but not all types will accept new members
                if (to is InferedType infered)
                {
                    foreach (var memberPair in GetMembers(from))
                    {
                        if (members[infered].TryGetValue(memberPair.Key, out var upstreamMember))
                        {
                            var one = GetType(upstreamMember);
                            var two = GetType(memberPair.Value);
                            if (one is InferedType oneInfered)
                            {
                                Flow(two, oneInfered);
                            }
                            else if (two is InferedType twoInfered)
                            {
                                Flow(one, twoInfered);
                            }
                            else
                            {
                                throw new Exception("these types are not compatible... right?");
                            }
                        }
                        else
                        {
                            members[infered].Add(memberPair.Key, memberPair.Value);
                        }
                    }
                }
            }


            IReadOnlyDictionary<IKey, Tpn.IMember> GetMembers(Tpn.IHaveMembers type)
            {
                if (type is Tpn.IExplicitType explictType)
                {
                    return members[explictType];
                }

                if (type is Tpn.IOrType orType)
                {
                    if (orTypeMembers.TryGetValue(orType, out var res)) {
                        return res;
                    }

                    res = new Dictionary<IKey, Tpn.IMember>();
                    var (left,right) =  orTypeComponets[orType];

                    var rightMembers = GetMembers(GetType(right));
                    foreach (var leftMember in GetMembers(GetType(left)))
                    {
                        if (rightMembers.TryGetValue(leftMember.Key, out var rightMember)) {
                            // if they are the same type
                            if (ReferenceEquals(GetType(rightMember), GetType(leftMember.Value))) {
                                var member = new Member(this,$"generated or member out of {((TypeProblemNode)leftMember.Key).debugName} and {((TypeProblemNode)rightMember).debugName}");
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


        private bool TryGetMember(Tpn.IScope context, IKey key, out Tpn.IMember member)
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

        private Tpn.IHaveMembers LookUpOrOverlayOrThrow(Tpn.IScope from, IKey key)
        {
            if (!TryLookUpOrOverlay(from, key, out var res))
            {
                throw new Exception("could not find type");
            }
            return res;
        }


        public TypeProblem2()
        {
            Root = new Scope(this,"root");
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

        private bool TryLookUpOrOverlay(Tpn.IScope from, IKey key, out Tpn.IHaveMembers res)
        {

            if (key is GenericNameKey genericNameKey)
            {

                var types = genericNameKey.Types.Select(typeKey => (typeKey, LookUpOrOverlayOrThrow(from, typeKey))).ToArray();

                if (!(LookUpOrOverlayOrThrow(from, genericNameKey.name) is Tpn.IExplicitType lookedUp)){
                    throw new Exception();
                }
                var genericTypeKey = new GenericTypeKey(lookedUp, types.Select(x => x.Item2).ToArray());

                if (realizedGeneric.TryGetValue(genericTypeKey, out var res2))
                {
                    res = res2;
                    return true;
                }

                var to = new Type(this,"generated-generic");
                foreach (var type in types)
                {
                    HasPlaceholderType(to, type.typeKey, type.Item2);
                }

                var explict  = CopyTree(lookedUp, to);
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

        private bool TryLookUp(Tpn.IScope haveTypes, IKey key, out Tpn.IHaveMembers result)
        {
            while (true)
            {
                if (types.TryGetValue(haveTypes, out var dict1) && dict1.TryGetValue(key, out var res))
                {
                    result = res;
                    return true;
                }
                if (orTypes.TryGetValue(haveTypes, out var dict2) && dict2.TryGetValue(key, out var res2))
                {
                    result = res2;
                    return true;
                }
                if (!kidParent.TryGetValue(haveTypes, out haveTypes))
                {
                    result = null;
                    return false;
                }
            }
        }

        private Tpn.IExplicitType CopyTree(Tpn.IExplicitType from, Tpn.IExplicitType to)
        {

            var map = new Dictionary<Tpn.ITypeProblemNode, Tpn.ITypeProblemNode>();
            Copy(from, to);

            foreach (var pair in map)
            {
                if (pair.Key is Tpn.IScope fromScope && to is Tpn.IScope toScope)
                {
                    kidParent[toScope] = kidParent[CopiedToOrSelf(fromScope)];
                }
            }

            var oldAssignments = assignments.ToArray();
            foreach (var pair in map)
            {
                if (pair.Key is Tpn.ICanBeAssignedTo assignedToFrom && pair.Value is Tpn.ICanBeAssignedTo assignedToTo)
                {
                    foreach (var item in oldAssignments)
                    {
                        if (item.Item2 == assignedToFrom)
                        {
                            assignments.Add((CopiedToOrSelf(item.Item1), assignedToTo));
                        }
                    }
                }

                if (pair.Value is Tpn.ICanAssignFromMe assignFromFrom && pair.Value is Tpn.ICanAssignFromMe assignFromTo)
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
                if (pair.Key is Tpn.ILookUpType lookUpFrom && pair.Value is Tpn.ILookUpType lookUpTo)
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
            
                if (pair.Key is Tpn.IOrType orFrom && pair.Value is Tpn.IOrType orTo)
                {
                    Ors(orTo, CopiedToOrSelf(orTypeComponets[orFrom].Item1), CopiedToOrSelf(orTypeComponets[orFrom].Item2));
                }
            

                if (pair.Key is Tpn.IMethod methodFrom && pair.Value is Tpn.IMethod methodTo)
                {
                    methodInputs[methodTo] = CopiedToOrSelf(methodInputs[methodFrom]);
                    methodReturns[methodTo] = CopiedToOrSelf(methodReturns[methodFrom]);
                }
            }

            return to;

            T CopiedToOrSelf<T>(T item)
                where T : Tpn.ITypeProblemNode
            {
                if (map.TryGetValue(item, out var res))
                {
                    return (T)res;
                }
                return item;
            }

            T Copy<T>(T innerFrom, T innerTo)
                where T : Tpn.ITypeProblemNode
            {
                map.Add(innerFrom, innerTo);

                if (innerFrom is Tpn.IScope innerFromScope && innerTo is Tpn.IScope innerScopeTo)
                {

                    foreach (var item in values[innerFromScope])
                    {
                        var newValue = Copy(item, new Value(this,$"copied from {((TypeProblemNode)item).debugName}"));
                        HasValue(innerScopeTo, newValue);
                    }

                    foreach (var item in refs[innerFromScope])
                    {
                        var newValue = Copy(item, new TypeReference(this, $"copied from {((TypeProblemNode)item).debugName}"));
                        HasReference(innerScopeTo, newValue);
                    }

                    foreach (var member in members[innerFromScope])
                    {
                        var newValue = Copy(member.Value, new Member(this, $"copied from {((TypeProblemNode)member.Value).debugName}"));
                        HasMember(innerScopeTo, member.Key, newValue);
                    }

                    foreach (var type in types[innerFromScope])
                    {
                        var newValue = Copy(type.Value, new Type(this, $"copied from {((TypeProblemNode)type.Value).debugName}"));
                        HasType(innerScopeTo, type.Key, newValue);
                    }

                    foreach (var type in orTypes[innerFromScope])
                    {
                        var newValue = Copy(type.Value, new OrType(this, $"copied from {((TypeProblemNode)type.Value).debugName}"));
                        HasOrType(innerScopeTo, type.Key, newValue);
                    }

                    foreach (var type in genericOverlays[innerFromScope])
                    {
                        if (!genericOverlays[innerScopeTo].ContainsKey(type.Key))
                        {
                            HasPlaceholderType(innerScopeTo, type.Key, type.Value);
                        }
                    }
                
                    foreach (var possible in possibleMembers[innerFromScope])
                    {
                        var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}"));
                        HasMembersPossiblyOnParent(innerScopeTo, possible.Key, newValue);
                    }
                }

                if (innerFrom is Tpn.IHaveHopefulMembers innerFromHopeful && innerTo is Tpn.IHaveHopefulMembers innerToHopeful)
                {
                    foreach (var possible in hopefulMembers[innerFromHopeful])
                    {
                        var newValue = Copy(possible.Value, new Member(this, $"copied from {((TypeProblemNode)possible.Value).debugName}"));
                        HasHopefulMember(innerToHopeful, possible.Key, newValue);
                    }
                }

                return innerTo;
            }
        }

        private class GenericTypeKey 
        {
            private readonly Tpn.IExplicitType primary;
            private readonly Tpn.IHaveMembers[] parameters;

            public GenericTypeKey(Tpn.IExplicitType primary, Tpn.IHaveMembers[] parameters)
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
                    parameters.Zip(other.parameters,(x,y)=> !x.Equals(y)).Where(x=>x).Any();
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(primary, parameters);
            }
        }

    }
}
