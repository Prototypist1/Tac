using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;

namespace Tac.Frontend.New.Yuck
{

    internal interface IAmWhat
    {
        // z =: (a.x);
        void HopefullyMember(IKey key, IAmWhat what);
    }

    // type, object
    internal interface IHaveMembers : IAmWhat
    {
        // int key;
        void Member(IKey key, IKey typeKey, IAmWhat what);
        // type {a;b;} x;
        void Member(IKey key, IAmType type, IAmWhat what);
    }


    //
    // z is a reference to ... something outside of the object 
    //object {
    //  x = z
    //}

    internal interface IAssign : IHaveMembers
    {

        // a =: x
        void IsAssignedTo(IThingWebEntry typeWebEntry);
    }

    internal interface IAmType : IHaveMembers
    {

    }

    internal interface IAmObject : IHaveMembers
    {

    }

    internal interface IAmScope : IHaveMembers
    {
        // type x {a;b;}
        void Type(IKey key, IAmType type);
        // x;
        void MemberPossiblyInEnclosingScope(IKey key, IAmWhat typeWebEntry);
        // return x;
        void SetReturns(IThingWebEntry returnType);
        // 1 > f =: x
        IThingWebEntry GetReturns();

    }
}
