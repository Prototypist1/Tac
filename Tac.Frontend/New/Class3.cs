using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //internal interface IGenericType : IDefineMembers
    //{
    //}

    internal interface IObject : IDefineMembers, ICanAssignFromMe
    {
    }

    internal interface IScope : IDefineMembers
    {
        // type x {a;b;}
        void Type(IType type);

        //// type<T> x {T a;}
        //void GenericType(IGenericType type);

        // x;
        void MightHaveMember(IMember member);
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

    internal interface ITypeSolution
    {
    }

    internal class TypeProblem : ITypeProblem
    {
        private readonly List<Value> values = new List<Value>();
        public IValue CreateValue(IType type)
        {
            var res = new Value(type);
            values.Add(res);
            return res;
        }
        private readonly List<Member> members = new List<Member>();
        
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
        // why?
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

            public readonly List<Member> members = new List<Member>();
            public void HopefullyMember(IMember member)
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
        private class GenericType
        {

            public readonly MemberTracker memberTracker;
            public IDefineMembers ParentOrNull { get; }
            public readonly IKey key;

            public GenericType()
            {
                memberTracker = new MemberTracker();
            }
            public GenericType(IKey key, IDefineMembers definedIn) : this()
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

            public readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }


            public void Member(IMember member) => memberTracker.Member(member);
        }
        private class Scope : IScope
        {
            public readonly MemberTracker mightBeOnParentMemberTracker = new MemberTracker();
            public readonly MemberTracker memberTracker = new MemberTracker();
            public readonly TypeTracker TypeTracker = new TypeTracker();
            public readonly Scope parent;

            public IDefineMembers ParentOrNull { get; }

            public Scope()
            {
            }

            public Scope(IDefineMembers parent) : this()
            {
                this.ParentOrNull = parent;
            }

            public void Member(IMember member) => memberTracker.Member(member);

            public void Type(IType type) => TypeTracker.Type(type);

            public void MightHaveMember(IMember member) => mightBeOnParentMemberTracker.Member(member);
        }
        private class Method : IMethod
        {
            public readonly MemberTracker mightBeOnParentMemberTracker = new MemberTracker();
            public readonly MemberTracker memberTracker = new MemberTracker();
            private readonly TypeTracker TypeTracker = new TypeTracker();

            private readonly TypeProblem typeProblem;
            private readonly IMember input;
            private readonly IMember output;
            public IDefineMembers ParentOrNull { get; }

            public Method(TypeProblem typeProblem, IMember input, IMember output, IDefineMembers parent)
            {
                this.typeProblem = typeProblem;
                this.input = input;
                this.output = output;
                ParentOrNull = parent;
            }


            public void Member(IMember member) => memberTracker.Member(member);

            public void Type(IType type) => TypeTracker.Type(type);

            public void AssignToInput(ICanAssignFromMe value) => typeProblem.IsAssignedTo(value, input);

            public void AssignToReturns(ICanAssignFromMe value) => typeProblem.IsAssignedTo(value, output);

            public ICanAssignFromMe Returns() => output;

            public void MightHaveMember(IMember member) => mightBeOnParentMemberTracker.Member(member);
        }


        #region Solve Side

        private class TypeSolution : ITypeSolution { }

        private class OverlayIntention
        {
            public readonly SolveSideNode copyFrom;
            public readonly SolveSideNode copyTo;
            public readonly List<SolveSideNode> list;

            public OverlayIntention(SolveSideNode copyFrom, SolveSideNode copyTo, List<SolveSideNode> list)
            {
                this.copyFrom = copyFrom ?? throw new ArgumentNullException(nameof(copyFrom));
                this.copyTo = copyTo ?? throw new ArgumentNullException(nameof(copyTo));
                this.list = list ?? throw new ArgumentNullException(nameof(list));
            }

        }



        public ITypeSolution Solve()
        {
            var cache = new Dictionary<ITypeProblemNode, SolveSideNode>();

            // now the impossible part...

            // generate scope tree
            GenerateScopeTree();

            // convert types
            ConvertTypes();

            var toOverlay = new List<OverlayIntention>();

            // then members
            ConvertMembers();

            // then members that might be on parents
            ConvertMembersThatMightBeOnParents();

            // then hopeful members
            ConvertHopefulMembers();

            // add the assignments 
            foreach (var (from, to) in assignments)
            {
                cache[from].AddAssignedTo(cache[to]);
                cache[to].AddAssignedFrom(cache[from]);
            }

            // do generic overlaying
            OverlayGenerics();
            // note that this makes the list of ITypeProblemNode incomplete 

            // figure out what types members are
            ResolveTypes();

            // realize hopeful members
            foreach (var item in cache.Values)
            {
                item.RealizeHopefulMembers();
            }

            // flow generic relation ships A<T> -> A<int>
            foreach (var overlay in toOverlay)
            {
                foreach (var (key, value) in overlay.copyFrom.Members)
                {
                    overlay.copyTo.FlowUpStream(key, value);
                }
            }

            // flow members upstream and merge them
            foreach (var node in cache.Values)
            {
                foreach (var (key, value) in node.Members)
                {
                    foreach (var assignedFromNode in node.AssignedFrom)
                    {
                        assignedFromNode.FlowUpStream(key, value);
                    }
                }
            }

            // flow members downstream but not pass confluences
            foreach (var node in cache.Values)
            {
                foreach (var (key, value) in node.Members)
                {
                    foreach (var assignedFromNode in node.AssignedTo)
                    {
                        assignedFromNode.FlowDownStream(key, value);
                    }
                }
            }

            return new TypeSolution();

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

                SolveSideNode ConvertScope(IDefineMembers scope)
                {
                    if (scope == null)
                    {
                        throw new ArgumentNullException(nameof(scope));
                    }

                    {
                        if (cache.TryGetValue(scope, out var res))
                        {
                            return res;
                        }
                    }

                    if (scope.ParentOrNull == null)
                    {
                        var res = new SolveSideNode(true, null,null);
                        cache.Add(scope, res);
                        return res;

                    }
                    else
                    {
                        var parent = ConvertScope(scope.ParentOrNull);
                        var res = new SolveSideNode(true, parent,null);
                        cache.Add(scope, res);
                        return res;
                    }

                }

            }

            SolveSideNode GetParent(IDefineMembers defineMembers)
            {
                if (defineMembers.ParentOrNull == null)
                {
                    return null;
                }
                return cache[defineMembers.ParentOrNull];
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
                        var res = new SolveSideNode(true, GetParent(type),null);

                        if (type.key != null)
                        {
                            cache[type.ParentOrNull].AddType(type.key, res);
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
                        ConvertMember(scope, member);
                    }
                }

                foreach (var @object in objects)
                {
                    foreach (var member in @object.memberTracker.members)
                    {
                        ConvertMember(@object, member);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var member in method.memberTracker.members)
                    {
                        ConvertMember(method, member);
                    }
                }

                foreach (var type in types)
                {
                    foreach (var member in type.memberTracker.members)
                    {
                        ConvertMember(type, member);
                    }
                }

                SolveSideNode ConvertMember(IDefineMembers owner, Member member)
                {
                    {
                        if (cache.TryGetValue(member, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null,member.typeKey);
                        cache.Add(member, res);

                        if (member.typeKey is GenericNameKey genericNameKey)
                        {
                            HandleGenericNameKey(genericNameKey);
                        }

                        cache[owner].AddMember(member.Key, res);

                        return res;
                    }

                    SolveSideNode HandleGenericNameKey(GenericNameKey genericNameKey)
                    {
                        if (cache[owner].TryGetType(new NameKey(genericNameKey.Name), out var copyFrom))
                        {
                            var list = new List<SolveSideNode>();
                            foreach (var inner in genericNameKey.Types)
                            {
                                if (inner is GenericNameKey innerGernericNameKey)
                                {
                                    list.Add(HandleGenericNameKey(innerGernericNameKey));
                                }
                                else if (cache[owner].TryGetType(new NameKey(genericNameKey.Name), out var innerNode))
                                {
                                    list.Add(innerNode);
                                }
                                else
                                {
                                    throw new Exception("uuhhh, we could not find the type..");
                                }
                            }
                            var copyTo = new SolveSideNode(true, copyFrom.parentOrNull,null);
                            toOverlay.Add(new OverlayIntention(copyFrom, copyTo, list));
                            return copyTo;
                        }
                        else
                        {
                            throw new Exception("uuhhh, we could not find the type..");
                        }
                    }

                }
            }

            void OverlayGenerics()
            {
                foreach (var overlay in toOverlay)
                {
                    var mapped = new Dictionary<SolveSideNode, SolveSideNode>();
                    // TODO we need to store members and the like
                    // in the list we 
                    overlay.copyFrom.OverlayTo(overlay.copyTo, overlay.list, mapped);
                    overlay.copyFrom.OverlayToRelationshipsTo(overlay.copyTo, mapped);
                }
            }

            void ResolveTypes()
            {

                foreach (var node in cache.Values)
                {
                    foreach (var member in node.Members.Select(x=>x.Item2))
                    {
                        ConvertMember(node, member);
                    }
                }

                void ConvertMember(SolveSideNode owner, SolveSideNode member)
                {
                    if (member.typeKeyOrNull != null)
                    {
                        if (owner.TryGetType(member.typeKeyOrNull, out var node))
                        {
                            member.DefersTo(node);
                        }
                        else
                        {
                            throw new Exception("uuhhh, we could not find the type..");
                        }
                    }
                }

            }

            void ConvertMembersThatMightBeOnParents()
            {

                foreach (var scope in scopes)
                {
                    foreach (var member in scope.mightBeOnParentMemberTracker.members)
                    {
                        ConvertMemberThatMightBeOnParents(scope, member);
                    }
                }


                foreach (var method in methods)
                {
                    foreach (var member in method.mightBeOnParentMemberTracker.members)
                    {
                        ConvertMemberThatMightBeOnParents(method, member);
                    }
                }


                SolveSideNode ConvertMemberThatMightBeOnParents(IDefineMembers owner, Member member)
                {
                    {
                        if (cache.TryGetValue(member, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, null);
                        cache.Add(member, res);

                        if (!(cache[owner].TryGetMember(member.Key, out SolveSideNode node)))
                        {
                            res.DefersTo(node);
                        }
                        else
                        {
                            // add it to owner
                            cache[owner].AddMember(member.Key, res);
                        }

                        return res;
                    }
                }
            }

            void ConvertHopefulMembers()
            {
                foreach (var value in values)
                {
                    foreach (var hopefullMember in value.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(value, hopefullMember);
                    }
                }


                foreach (var member in members)
                {
                    foreach (var hopefullMember in member.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(member, hopefullMember);
                    }
                }


                foreach (var @object in objects)
                {
                    foreach (var hopefullMember in @object.hopefullMemberTracker.members)
                    {
                        ConvertHopefulMember(@object, hopefullMember);
                    }
                }

                SolveSideNode ConvertHopefulMember(ITypeProblemNode node, Member hopefullMember)
                {
                    {
                        if (cache.TryGetValue(hopefullMember, out var res))
                        {
                            return res;
                        }
                    }

                    {
                        var res = new SolveSideNode(false, null, null);
                        cache.Add(hopefullMember, res);

                        cache[node].AddHopefulMember(hopefullMember.Key, res);

                        return res;
                    }
                }
            }
        }


        private class SolveSideNode
        {
            private SolveSideNode inner;
            internal IKey typeKeyOrNull;

            // placeholderTypes also appear in types
            public readonly List<SolveSideNode> placeholderTypes = new List<SolveSideNode>();
            public readonly Dictionary<IKey, SolveSideNode> types = new Dictionary<IKey, SolveSideNode>();
            private readonly Dictionary<IKey, SolveSideNode> members = new Dictionary<IKey, SolveSideNode>();
            private readonly List<(IKey, SolveSideNode)> hopefulMembers = new List<(IKey, SolveSideNode)>();
            private readonly List<SolveSideNode> assignTos = new List<SolveSideNode>();
            private readonly List<SolveSideNode> assignFroms = new List<SolveSideNode>();

            public readonly SolveSideNode parentOrNull;
            private readonly bool explictMembersOnly;


            public IEnumerable<(IKey, SolveSideNode)> Members => members.Select(x => (x.Key, x.Value));

            public IEnumerable<SolveSideNode> AssignedFrom
            {
                get
                {
                    if (inner == null)
                    {
                        return assignFroms;
                    }
                    else
                    {
                        return inner.AssignedFrom;
                    }
                }
            }
            public IEnumerable<SolveSideNode> AssignedTo
            {
                get
                {

                    if (inner == null)
                    {
                        return assignTos;
                    }
                    else
                    {
                        return inner.AssignedTo;
                    }

                }
            }

            internal void AddType(IKey key, SolveSideNode res)
            {
                types.Add(key, res);
            }

            internal bool TryGetType(IKey typeKey, out SolveSideNode node)
            {
                if (types.TryGetValue(typeKey, out node))
                {
                    return true;
                }
                if (parentOrNull == null)
                {
                    return false;
                }
                return parentOrNull.TryGetType(typeKey, out node);
            }

            internal bool TryGetMember(IKey key, out SolveSideNode resNode)
            {
                if (TryGetValue(key, out resNode))
                {
                    return true;
                }
                if (parentOrNull == null)
                {
                    return false;
                }
                return parentOrNull.TryGetMember(key, out resNode);
            }

            public SolveSideNode(bool explictMembersOnly, SolveSideNode parent, IKey typeKeyOrNull)
            {
                this.typeKeyOrNull = typeKeyOrNull;
                this.explictMembersOnly = explictMembersOnly;
                this.parentOrNull = parent;
            }

            public bool TryGetValue(IKey key, out SolveSideNode value)
            {
                if (inner == null)
                {
                    return members.TryGetValue(key, out value);
                }
                else
                {
                    return inner.TryGetValue(key, out value);
                }
            }

            // what does this do if the member is already there?
            // probably merge?
            internal void AddMember(IKey key, SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    if (members.TryGetValue(key, out var current))
                    {
                        solveSideNode.DefersTo(current);
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

            internal void DefersTo(SolveSideNode node)
            {
                while (node.inner != null)
                {
                    node = node.inner;
                }

                if (inner != null)
                {
                    inner.DefersTo(node);
                    return;
                }

                if (this == node)
                {
                    return;
                }

                // add all the members
                foreach (var member in node.members)
                {
                    AddMergeMember(member.Key, member.Value);
                }
                // do we need to clean house?
                node.members.Clear();


                // add all the assignFroms
                foreach (var assignFrom in node.assignFroms)
                {
                    assignFroms.Add(assignFrom);
                }
                // do we need to clean house?
                node.assignFroms.Clear();

                // add all the assignTos
                foreach (var assignTo in node.assignTos)
                {
                    assignTos.Add(assignTo);
                }
                // do we need to clean house?
                node.assignTos.Clear();
            }

            internal void AddHopefulMember(IKey key, SolveSideNode solveSideNode)
            {
                hopefulMembers.Add((key, solveSideNode));
            }

            internal void RealizeHopefulMembers() {

                foreach (var (key,solveSideNode) in hopefulMembers)
                {
                    if (inner == null)
                    {
                        if (members.TryGetValue(key, out var current))
                        {
                            solveSideNode.DefersTo(current);
                        }
                        else
                        {
                            if (explictMembersOnly)
                            {
                                throw new Exception("this does not accept hopeful members");
                            }
                            members.Add(key, solveSideNode);
                        }
                    }
                    else
                    {
                        inner.AddHopefulMember(key, solveSideNode);
                    }
                }
            }

            internal void AddMergeMember(IKey key, SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    if (members.TryGetValue(key, out var current))
                    {
                        solveSideNode.DefersTo(current);
                    }
                    else
                    {
                        if (explictMembersOnly)
                        {
                            throw new Exception("this does not accept hopeful members");
                        }
                        members.Add(key, solveSideNode);
                    }
                }
                else
                {
                    inner.AddMergeMember(key, solveSideNode);
                }
            }

            internal void AddAssignedTo(SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    assignTos.Add(solveSideNode);
                }
                else
                {
                    inner.AddAssignedTo(solveSideNode);
                }
            }

            internal void AddAssignedFrom(SolveSideNode solveSideNode)
            {
                if (inner == null)
                {
                    assignFroms.Add(solveSideNode);
                }
                else
                {
                    inner.AddAssignedFrom(solveSideNode);
                }
            }

            internal void FlowUpStream(IKey key, SolveSideNode value)
            {

                if (members.TryGetValue(key, out var current))
                {
                    value.DefersTo(current);
                    return;
                }

                if (explictMembersOnly)
                {
                    return;
                }

                members.Add(key, value);
                foreach (var node in AssignedFrom)
                {
                    node.FlowUpStream(key, value);
                }

            }

            internal void FlowDownStream(IKey key, SolveSideNode value)
            {


                // don't flow in to nodes that are assigned from more than one place 
                if (AssignedFrom.Count() > 1)
                {
                    return;
                }

                if (members.TryGetValue(key, out var current))
                {
                    value.DefersTo(current);
                    return;
                }

                if (explictMembersOnly)
                {
                    return;
                }

                members.Add(key, value);
                foreach (var node in AssignedFrom)
                {
                    node.FlowUpStream(key, value);
                }
            }

            internal SolveSideNode OverlayTo(SolveSideNode copyTo, Dictionary<SolveSideNode, SolveSideNode> map, Dictionary<SolveSideNode, SolveSideNode> alreadyMapped)
            {
                if (inner != null)
                {
                    throw new Exception("at the time when this is called inner should be null");
                    // well inner would not be null on a "member possibly on parent" when it is on the parent
                    // but that should not be reached by this method 
                }

                if (alreadyMapped.TryGetValue(this, out var res))
                {
                    return res;
                }

                alreadyMapped[this] = copyTo;

                foreach (var memberPair in members)
                {
                    if (map.TryGetValue(memberPair.Value, out var newValue))
                    {
                        copyTo.AddMember(memberPair.Key, newValue);
                    }
                    else
                    {
                        copyTo.AddMember(memberPair.Key, memberPair.Value.OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), memberPair.Value.typeKeyOrNull), map, alreadyMapped));
                    }
                }

                foreach (var typePair in types)
                {
                    if (map.TryGetValue(typePair.Value, out var newValue))
                    {
                        copyTo.AddType(typePair.Key, newValue);
                    }
                    else
                    {
                        copyTo.AddType(typePair.Key, OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), typePair.Value.typeKeyOrNull), map, alreadyMapped));
                    }
                }


                foreach (var hopefulMember in hopefulMembers)
                {
                    if (map.TryGetValue(hopefulMember.Item2, out var newValue))
                    {
                        copyTo.AddHopefulMember(hopefulMember.Item1, newValue);
                    }
                    else
                    {
                        copyTo.AddHopefulMember(hopefulMember.Item1, OverlayTo(new SolveSideNode(true, OverlayParent(parentOrNull), hopefulMember.Item2.typeKeyOrNull), map, alreadyMapped));
                    }
                }

                foreach (var placeholder in placeholderTypes)
                {
                    copyTo.placeholderTypes.Add(map[placeholder]);
                }

                return copyTo;

                SolveSideNode OverlayParent(SolveSideNode parentOrNull)
                {
                    if (parentOrNull == null)
                    {
                        return null;
                    }
                    if (alreadyMapped.TryGetValue(parentOrNull, out var parent))
                    {
                        return parent;
                    }

                    return parentOrNull;
                }
            }

            internal void OverlayToRelationshipsTo(SolveSideNode copyTo, Dictionary<SolveSideNode, SolveSideNode> alreadyMapped)
            {
                foreach (var assignTo in assignTos) {
                    if (alreadyMapped.TryGetValue(assignTo, out var resplace))
                    {
                        copyTo.AddAssignedTo(resplace);
                    }
                    else {
                        copyTo.AddAssignedTo(assignTo);
                    }
                }

                foreach (var assignFrom in assignFroms)
                {
                    if (alreadyMapped.TryGetValue(assignFrom, out var resplace))
                    {
                        copyTo.AddAssignedFrom(resplace);
                    }
                    else
                    {
                        copyTo.AddAssignedFrom(assignFrom);
                    }
                }
            }
        }

        #endregion
    }

}
