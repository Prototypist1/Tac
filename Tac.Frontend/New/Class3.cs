using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;

namespace Tac.Frontend.New
{


    internal interface IMember {
        // z =: (a.x);
        void HopefullyMember(IKey key, IMember member);
    }

    internal interface IHaveMembers  {
        // x; 
        // might be in enclosing scope
        // object {x := y;} how do I know to put x in the object and look up y in the enclosing scope???
        IMember LookUpMember(IKey key);
        // int key;
        void Member(IKey key, IKey typekey, IMember member); 
        // type {a;b;} x;
        void Member(IKey key, IType type, IMember member);// wierd that IType type is passed in here, should that not be part of the member??
    }

    internal interface IType: IHaveMembers
    {

    }

    internal interface IObject : IHaveMembers
    {

    }

    internal interface IScope : IHaveMembers
    {
        // type x {a;b;}
        void Type(IKey key, IType type);
    }

    internal interface IMethod : IScope
    {
        // 1 > f =: x
        IMember TryGetReturns();
    }



    internal interface ITypeProblem {


        // TODO you are here
        // you can't assign to a lot of these 
        // you can only assign to something that is actually a member

        // maybe interfaces for ICanBeAssignedTO and ICanBeAssignedFROM

        // still not sure I member should be a thing

        // a =: x
        void IsAssignedTo(IMember from, IMember to);

    }

    internal class TypeProblem: ITypeProblem
    {


        public IMember CreateMember() => new Member(this);
        public IScope CreateScope() => new Scope(this);
        public IType CreateType() => new Type(this);
        public IObject CreateObject() => new Object(this);
        public IMethod CreateMethod() => new Method(this);

        public void IsAssignedTo(IMember from, IMember to)
        {
            throw new NotImplementedException();
        }


        private class Member : IMember
        {
            private TypeProblem typeProblem;

            public Member(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
            }
        }

        private class Type : IType
        {
            private readonly TypeProblem typeProblem;

            public Type(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
            }
        }
        private class Object : IObject
        {
            private readonly TypeProblem typeProblem;

            public Object(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
            }
        }
        private class Scope : IScope
        {
            private readonly TypeProblem typeProblem;

            public Scope(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
            }
        }
        private class Method : IMethod
        {
            private readonly TypeProblem typeProblem;

            public Method(TypeProblem typeProblem)
            {
                this.typeProblem = typeProblem;
            }
        }

    }


}
