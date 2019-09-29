using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tac.Model;

namespace Tac.Frontend.New
{


    internal interface ITypeProblemNode
    {
    }


    internal interface IDefineMembers : ITypeProblemNode
    {
        // int key; or type {a;b;} x;
        void Member(IMember member);

        IDefineMembers ParentOrNull { get; }
    }

    internal interface ICanBeAssignedTo : ITypeProblemNode
    {

    }

    internal interface ICanAssignFromMe : ITypeProblemNode
    {
        // z =: (a.x);
        void HopefullyMember(IMember member);
    }

    internal interface IValue : ICanAssignFromMe
    {
    }

    internal interface IMember : ICanAssignFromMe, ICanBeAssignedTo
    {
        IKey Key { get; }
    }

    internal interface IType : IDefineMembers
    {

    }

    internal interface IObject : IDefineMembers, ICanAssignFromMe
    {

    }

    internal interface IScope : IDefineMembers
    {
        // type x {a;b;}
        void Type(IType type);
        // x;
        void MightHaveMember(IKey key);
    }

    internal interface IMethod : IScope
    {
        // 1 > f
        void AssignToInput(ICanAssignFromMe value);
        // 2 return
        void AssignToReturns(ICanAssignFromMe value);
        // 1 > f =: x
        ICanAssignFromMe Returns();
    }



    internal interface ITypeProblem
    {
        // a =: x
        void IsAssignedTo(ICanAssignFromMe from, ICanBeAssignedTo to);

    }

    internal class TypeProblem : ITypeProblem
    {

        public IValue CreateValue(IType type) => new Value(type);

        public IMember CreateMember(IKey key, IType type) => new Member(key, type);

        public IMember CreateMember(IKey key, IKey keyType) => new Member(key, keyType);
        public IMember CreateMember(IKey key) => new Member(key);

        private readonly List<Scope> scopes = new List<Scope>();
        public IScope CreateScope()
        {
            var res = new Scope();
            scopes.Add(res);
            return res;
        }
        public IScope CreateScope(IDefineMembers parent)
        {
            var res = new Scope(parent);
            scopes.Add(res);
            return res;
        }

        private readonly List<Type> types = new List<Type>();

        public IType CreateType(IDefineMembers parent, IKey key)
        {
            var res = new Type(key, parent);
            types.Add(res);
            return res;
        }

        public IType CreateType()
        {
            var res = new Type();
            types.Add(res);
            return res;
        }


        private readonly List<Object> objects = new List<Object>();

        public IObject CreateObject(IDefineMembers parent)
        {
            var res = new Object(this, parent);
            objects.Add(res);
            return res;
        }


        private readonly List<Method> methods = new List<Method>();

        public IMethod CreateMethod(IMember input, IMember output, IDefineMembers parent)
        {
            var method = new Method(this, input, output, parent);
            methods.Add(method);
            return method;
        }


        private readonly List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
        public void IsAssignedTo(ICanAssignFromMe from, ICanBeAssignedTo to) => assignments.Add((from, to));




        //public SolveSideNode Convert(IScope encolsingScope, ITypeProblemNode node)
        //{
        //    {
        //        if (cache.TryGetValue(node, out var res))
        //        {
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Value value)
        //        {
        //            var res = new SolveSideNode(true);

        //            // add all the hopeful members
        //            foreach (var hopefulMember in value.hopefullMemberTracker.members)
        //            {
        //                res.AddMember(hopefulMember.Key, Convert(encolsingScope, hopefulMember));
        //            }

        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Member member)
        //        {
        //            var res = new SolveSideNode(member.type == null);

        //            if (member.type != null)
        //            {
        //                // point to the type
        //                var type = Convert(encolsingScope, member.type);
        //                res.DefersTo(type);
        //            }

        //            if (member.typeKey != null)
        //            {
        //            }

        //            // try to join your hopeful members
        //            foreach (var hopefulMember in member.hopefullMemberTracker.members)
        //            {
        //                res.AddMember(hopefulMember.Key, Convert(encolsingScope, hopefulMember));
        //            }

        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Type type)
        //        {
        //            var res = new SolveSideNode(false);

        //            // add all your members
        //            foreach (var member in type.memberTracker.members)
        //            {
        //                res.AddMember(member.Key, Convert(encolsingScope, member));
        //            }

        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Object @object)
        //        {
        //            var res = new SolveSideNode(false);

        //            // add all your members
        //            foreach (var member in @object.memberTracker.members)
        //            {
        //                res.AddMember(member.Key, Convert(encolsingScope, member));
        //            }

        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Scope scope)
        //        {
        //            var res = new SolveSideNode(false);

        //            // add all your members
        //            // here we deal with mebers that might be on an enclosing scope -  yuk

        //            // so many order problems 
        //            // the parent scope must be fully populated 
        //            foreach (var member in scope.memberTracker.members)
        //            {
        //                if (encolsingScope.TryLookUpMember(member.Key, out var existingMember))
        //                {

        //                }
        //                else
        //                {
        //                    res.AddMember(member.Key, Convert(scope, member));
        //                }
        //            }


        //            foreach (var type in scope.TypeTracker.types)
        //            {
        //                // we convert types just to hit exceptions 
        //                Convert(scope, type);
        //            }


        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    {
        //        if (node is Method method)
        //        {
        //            var res = new SolveSideNode();

        //            // add all your members
        //            // here we deal with mebers that might be on an enclosing scope
        //            // we also have to deal with returns 

        //            cache.Add(node, res);
        //            return res;
        //        }
        //    }

        //    throw new NotImplementedException();
        //}






        private class TypeTracker
        {
            public readonly List<IType> types = new List<IType>();

            public void Type(IType type)
            {
                types.Add(type);
            }
        }

        private class MemberTracker
        {

            public readonly List<Member> members = new List<Member>();

            public void Member(IMember member)
            {

                if (!(member is Member realMember))
                {
                    // we are having a hard time with the internal exteranl view here
                    // 😡
                    throw new Exception("this sucks");
                }

                members.Add(realMember);
            }
        }

        private class HopefullMemberTracker
        {

            public readonly List<IMember> members = new List<IMember>();
            public void HopefullyMember(IMember member)
            {
                members.Add(member);
            }
        }

        private class Value : IValue
        {
            private IType type;

            public Value(IType type)
            {
                this.type = type;
            }

            public readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }
        }

        private class Member : IMember
        {
            public readonly IType type;
            public readonly IKey typeKey;
            public IKey Key { get; }

            public Member(IKey key)
            {
                Key = key;
            }


            public Member(IKey key, IKey typeKey)
            {
                Key = key;
                this.typeKey = typeKey;
            }


            public Member(IKey key, IType type)
            {
                Key = key;
                this.type = type;
            }

            public readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }
        }

        private class Type : IType
        {
            public readonly MemberTracker memberTracker;
            public IDefineMembers ParentOrNull { get; }
            public readonly IKey key;

            public Type()
            {
                memberTracker = new MemberTracker();
            }

            public Type(IKey key, IDefineMembers definedIn) : this()
            {
                this.key = key;
                this.ParentOrNull = definedIn;
            }


            public void Member(IMember member) => memberTracker.Member(member);
        }
        private class Object : IObject
        {
            public readonly MemberTracker memberTracker;
            private readonly TypeProblem typeProblem;
            public IDefineMembers ParentOrNull { get; }


            public Object(TypeProblem typeProblem, IDefineMembers parent)
            {
                this.typeProblem = typeProblem;
                memberTracker = new MemberTracker();
                ParentOrNull = parent;
            }

            private readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }


            public void Member(IMember member) => memberTracker.Member(member);
        }
        private class Scope : IScope
        {
            public readonly MemberTracker memberTracker;
            public readonly TypeTracker TypeTracker;
            public readonly Scope parent;

            public IDefineMembers ParentOrNull { get; }

            public Scope()
            {
                memberTracker = new MemberTracker();
                TypeTracker = new TypeTracker();
            }

            public Scope(IDefineMembers parent) : this()
            {
                this.ParentOrNull = parent;
            }

            public void Member(IMember member) => memberTracker.Member(member);

            public void Type(IType type) => TypeTracker.Type(type);

            public void MightHaveMember(IKey key)
            {
                throw new NotImplementedException();
            }
        }
        private class Method : IMethod
        {
            private readonly TypeProblem typeProblem;
            private readonly IMember input;
            public readonly MemberTracker memberTracker;
            private readonly TypeTracker TypeTracker;
            private readonly IMember output;
            public IDefineMembers ParentOrNull { get; }

            public Method(TypeProblem typeProblem, IMember input, IMember output, IDefineMembers parent)
            {
                this.typeProblem = typeProblem;
                this.input = input;
                this.output = output;
                ParentOrNull = parent;
                memberTracker = new MemberTracker();
                TypeTracker = new TypeTracker();
            }


            public void Member(IMember member) => memberTracker.Member(member);

            public void Type(IType type) => TypeTracker.Type(type);

            public void AssignToInput(ICanAssignFromMe value) => typeProblem.IsAssignedTo(value, input);

            public void AssignToReturns(ICanAssignFromMe value) => typeProblem.IsAssignedTo(value, output);

            public ICanAssignFromMe Returns() => output;

            public void MightHaveMember(IKey key)
            {
                throw new NotImplementedException();
            }
        }


        #region Solve Side

        public void Solve()
        {
            var cache = new Dictionary<ITypeProblemNode, SolveSideNode>();
            var scopeCache = new Dictionary<IDefineMembers, SolveSideScope>();

            // now the impossible part...

            // generate scope tree
            GenerateScopeTree();

            // convert types
            ConvertTypes();

            // then members
            ConvertMembers();

            // then members that might be on parents

            // then hopeful members

            // flow members upstream and merge them

            // flow members downstream but not pass confluences


            void GenerateScopeTree()
            {
                foreach (var scope in scopes)
                {
                    ConvertScope(scope);
                }

                foreach (var @object in objects)
                {
                    ConvertScope(@object);
                }


                foreach (var method in methods)
                {
                    ConvertScope(method);
                }

                foreach (var type in types)
                {
                    ConvertScope(type);
                }

                SolveSideScope ConvertScope(IDefineMembers scope)
                {
                    if (scope == null)
                    {
                        throw new ArgumentNullException(nameof(scope));
                    }

                    {
                        if (scopeCache.TryGetValue(scope, out var res))
                        {
                            return res;
                        }
                    }

                    if (scope.ParentOrNull == null)
                    {
                        var res = new SolveSideScope(null);
                        scopeCache.Add(scope, res);
                        return res;

                    }
                    else
                    {
                        var parent = ConvertScope(scope.ParentOrNull);
                        var res = new SolveSideScope(parent);
                        scopeCache.Add(scope, res);
                        return res;
                    }
                }

            }

            void ConvertTypes()
            {
                foreach (var type in types)
                {
                    ConvertType(type);
                }

                SolveSideNode ConvertType(Type type)
                {
                    {
                        if (cache.TryGetValue(type, out var res))
                        {
                            return res;
                        }
                    }
                    {
                        var res = new SolveSideNode();

                        if (type.key != null)
                        {
                            scopeCache[type.ParentOrNull].AddType(type.key, res);
                        }

                        cache.Add(type, res);
                        return res;
                    }
                }
            }

            void ConvertMembers()
            {
                foreach (var scope in scopes)
                {
                    foreach (var member in scope.memberTracker.members)
                    {
                        ConvertMember(scope,member);
                    }
                }

                foreach (var @object in objects)
                {
                    foreach (var member in @object.memberTracker.members)
                    {
                        ConvertMember(@object,member);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var member in method.memberTracker.members)
                    {
                        ConvertMember(method,member);
                    }
                }

                foreach (var type in types)
                {
                    foreach (var member in type.memberTracker.members)
                    {
                        ConvertMember(type,member);
                    }
                }

                SolveSideNode ConvertMember(IDefineMembers owner , Member member)
                {
                    {
                        if (cache.TryGetValue(member, out var res)) {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode();
                        cache.Add(member, res);

                        if (member.typeKey != null) {
                            if (!(scopeCache[owner].TryGetType(member.typeKey,out SolveSideNode node))) {
                                res.DefersTo(node);
                            }
                        }

                        if (member.type != null) {
                            res.DefersTo(cache[member.type]);
                        }

                        cache[owner].AddMember(member.Key, res);

                        return res;
                    }

                }
            }
        }


        private class SolveSideScope
        {
            public readonly SolveSideNode node = new SolveSideNode();
            public readonly SolveSideScope parentOrNull;

            public SolveSideScope(SolveSideScope parentOrNull)
            {
                this.parentOrNull = parentOrNull;
            }

            public readonly Dictionary<IKey, SolveSideNode> types = new Dictionary<IKey, SolveSideNode>();
            internal void AddType(IKey key, SolveSideNode res)
            {
                types.Add(key, res);
            }

            internal bool TryGetType(IKey typeKey, out SolveSideNode node)
            {
                if (types.TryGetValue(typeKey,out node))
                {
                    return true;
                }
                if (parentOrNull == null) {
                    return false;
                }
                return parentOrNull.TryGetType(typeKey, out node);
            }
        }

        private class SolveSideNode : IReadOnlyDictionary<IKey, SolveSideNode>
        {
            private SolveSideNode inner;
            private readonly Dictionary<IKey, SolveSideNode> members = new Dictionary<IKey, SolveSideNode>();
            private readonly List<SolveSideNode> assignTo = new List<SolveSideNode>();
            private readonly List<SolveSideNode> assignFrom = new List<SolveSideNode>();
            // this is really, do we know the list of member this has...
            private readonly bool isInfered;

            public SolveSideNode()
            {
            }

            #region IReadOnlyDictionary<IKey, SolveSideNode>

            public IEnumerable<IKey> Keys
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).Keys;
                }
            }

            public IEnumerable<SolveSideNode> Values
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).Values;
                }
            }

            public int Count
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).Count;
                }
            }

            public SolveSideNode this[IKey key]
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members)[key];
                }
            }

            public bool ContainsKey(IKey key)
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).ContainsKey(key);
            }

            public bool TryGetValue(IKey key, out SolveSideNode value)
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).TryGetValue(key, out value);
            }

            public IEnumerator<KeyValuePair<IKey, SolveSideNode>> GetEnumerator()
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? members).GetEnumerator();
            }

            #endregion

            internal void AddMember(IKey key, SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    if (inner.TryGetValue(key, out var current))
                    {
                        current.Merge(solveSideNode);
                    }
                    else
                    {
                        members.Add(key, solveSideNode);
                    }
                }
                else
                {
                    inner.AddMember(key, solveSideNode);
                }
            }

            private void Merge(SolveSideNode solveSideNode)
            {
                throw new NotImplementedException();
            }

            internal void DefersTo(SolveSideNode type)
            {
                throw new NotImplementedException();
            }
        }



        #endregion
    }
}
