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

    internal interface IHaveMembers: ITypeProblemNode
    {
        // x; 
        // might be in enclosing scope
        // object {x := y;} how do I know to put x in the object and look up y in the enclosing scope???
        IMember LookUpMember(IKey key);
        // int key; or type {a;b;} x;
        void Member(IMember member); 
    }

    internal interface ICanBeAssignedTo : ITypeProblemNode
    {

    }

    internal interface ICanAssignFromMe: ITypeProblemNode
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

    internal interface IType: IHaveMembers
    {

    }

    internal interface IObject : IHaveMembers, ICanAssignFromMe
    {

    }

    internal interface IScope : IHaveMembers
    {
        // type x {a;b;}
        void Type(IType type);
        //T x;
        IType LookUpType(IKey key);
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



    internal interface ITypeProblem {
        // a =: x
        void IsAssignedTo(ICanAssignFromMe from, ICanBeAssignedTo to);

    }

    internal class TypeProblem: ITypeProblem
    {

        public IValue CreateValue(IType type) => new Value(this, type);
        public IMember CreateMember(IKey key, IType type) => new Member(key,this,type);
        public IMember CreateMember(IKey key) => new Member(key,this);
        public IScope CreateScope() => new Scope(this);
        public IType CreateType(IKey key) => new Type(this, key);
        public IObject CreateObject() => new Object(this);
        public IMethod CreateMethod(IMember input, IMember output) => new Method(this, input,output);


        private readonly List<(ICanAssignFromMe, ICanBeAssignedTo)> assignments = new List<(ICanAssignFromMe, ICanBeAssignedTo)>();
        public void IsAssignedTo(ICanAssignFromMe from, ICanBeAssignedTo to) => assignments.Add((from, to));


        private class TypeTracker
        {
            private readonly TypeProblem typeProblem;
            private readonly List<IType> types = new List<IType>();

            public TypeTracker(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem ?? throw new ArgumentNullException(nameof(typeProblem));
            }

            public IType LookUpType(IKey key)
            {
                var res = typeProblem.CreateType(key);
                types.Add(res);
                return res;
            }

            public void Type(IType type)
            {
                types.Add(type);
            }
        }

        private class MemberTracker
        {

            private readonly TypeProblem typeProblem;
            private readonly List<IMember> members = new List<IMember>();

            public MemberTracker(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem ?? throw new ArgumentNullException(nameof(typeProblem));
            }

            public IMember LookUpMember(IKey key)
            {
                var res = typeProblem.CreateMember(key);
                members.Add(res);
                return res;
            }

            public void Member(IMember member)
            {
                members.Add(member);
            }
        }

        private class HopefullMemberTracker {

            private readonly List<IMember> members = new List<IMember>();
            public void HopefullyMember(IMember member) {
                members.Add(member);
            }
        }

        private class Value : IValue
        {
            private TypeProblem typeProblem;
            private IType type;

            public Value(TypeProblem typeProblem, IType type)
            {
                this.typeProblem = typeProblem;
                this.type = type;
            }

            private readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }
        }

        private class Member : IMember
        {
            private readonly TypeProblem typeProblem;
            private readonly IType type;
            public IKey Key { get; }

            public Member(IKey key, TypeProblem typeProblem)
            {
                Key = key;
                this.typeProblem = typeProblem;
            }


            public Member(IKey key, TypeProblem typeProblem, IType type)
            {
                Key = key;
                this.typeProblem = typeProblem;
                this.type = type;
            }

            private readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember( member);
            }
        }

        private class Type : IType
        {
            private readonly TypeProblem typeProblem;
            private readonly MemberTracker memberTracker;
            private readonly IKey key;

            public Type(TypeProblem typeProblem, IKey key)
            {
                this.typeProblem = typeProblem;
                this.key = key;
                memberTracker = new MemberTracker(typeProblem);
            }

            public IMember LookUpMember(IKey key) => memberTracker.LookUpMember(key);
            public void Member(IMember member) =>  memberTracker.Member(member);
        }
        private class Object : IObject
        {
            private readonly MemberTracker memberTracker;
            private readonly TypeProblem typeProblem;


            public Object(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
                memberTracker = new MemberTracker(typeProblem);
            }

            private readonly HopefullMemberTracker hopefullMemberTracker = new HopefullMemberTracker();
            public void HopefullyMember(IMember member)
            {
                hopefullMemberTracker.HopefullyMember(member);
            }


            public IMember LookUpMember(IKey key) => memberTracker.LookUpMember(key);
            public void Member(IMember member) => memberTracker.Member( member);
        }
        private class Scope : IScope
        {
            private readonly TypeProblem typeProblem;
            private readonly MemberTracker memberTracker;
            private readonly TypeTracker TypeTracker;

            public Scope(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
                memberTracker = new MemberTracker(typeProblem);
                TypeTracker = new TypeTracker(typeProblem);
            }

            public IMember LookUpMember(IKey key) => memberTracker.LookUpMember(key);
            public void Member(IMember member) => memberTracker.Member(member);

            public IType LookUpType(IKey key) => TypeTracker.LookUpType(key);
            public void Type(IType type) => TypeTracker.Type(type);
        }
        private class Method : IMethod
        {
            private readonly TypeProblem typeProblem;
            private readonly IMember input;
            private readonly MemberTracker memberTracker;
            private readonly TypeTracker TypeTracker;
            private readonly IMember output;

            public Method(TypeProblem typeProblem, IMember input, IMember output)
            {
                this.typeProblem = typeProblem;
                this.input = input;
                this.output = output;
                memberTracker = new MemberTracker(typeProblem);
                TypeTracker = new TypeTracker(typeProblem);
            }


            public IMember LookUpMember(IKey key) => memberTracker.LookUpMember(key);
            public void Member(IMember member) => memberTracker.Member(member);

            public IType LookUpType(IKey key) => TypeTracker.LookUpType(key);
            public void Type(IType type) => TypeTracker.Type( type);

            public void AssignToInput(ICanAssignFromMe value) => typeProblem.IsAssignedTo(value, input);

            public void AssignToReturns(ICanAssignFromMe value) =>typeProblem.IsAssignedTo(value, output);

            public ICanAssignFromMe Returns() =>output;
        }


        #region Solve Side

        public void Solve()
        {
            // now the impossible part...

            // resolve types

            // I think we need a solution view of members

            // merge the members inside things with members

            // flow members upstream and join them

            // flow members downstream but not pass confluences

        }

        private class SolveSideNode : IReadOnlyDictionary<IKey, SolveSideNode> {
            private SolveSideNode inner;
            private readonly Dictionary<IKey, SolveSideNode> node = new Dictionary<IKey, SolveSideNode>();
            private readonly List<SolveSideNode> assignTo = new List<SolveSideNode>();
            private readonly List<SolveSideNode> assignFrom = new List<SolveSideNode>();
            private bool isInfered;

            #region IReadOnlyDictionary<IKey, SolveSideNode>

            public IEnumerable<IKey> Keys
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).Keys;
                }
            }

            public IEnumerable<SolveSideNode> Values
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).Values;
                }
            }

            public int Count
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).Count;
                }
            }

            public SolveSideNode this[IKey key]
            {
                get
                {
                    return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node)[key];
                }
            }

            public bool ContainsKey(IKey key)
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).ContainsKey(key);
            }

            public bool TryGetValue(IKey key, out SolveSideNode value)
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).TryGetValue(key, out value);
            }

            public IEnumerator<KeyValuePair<IKey, SolveSideNode>> GetEnumerator()
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IReadOnlyDictionary<IKey, SolveSideNode>)inner ?? node).GetEnumerator();
            }
            #endregion

            // YOU ARE HERE
            // TODO add methods to edit 
            // expose assignTo, assignFrom
        }



        #endregion
    }
}
