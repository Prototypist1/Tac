﻿using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model;

namespace Tac.Model.Elements
{

    // maybe I don't want to write equals for all these types
    // I hate value type equality
    // so maybe there is a single type that wraps the type
    // and owns equality 

    //public interface ITypeComparer {
    //    bool IsAssignableTo(IVerifiableType from, IVerifiableType to);
    //}

    public interface IVerifiableType {

        //bool TheyAreUs(IVerifiableType they, bool noTagBacks);
        //bool WeAreThem(IVerifiableType them, bool noTagBacks);

        bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue);
        IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue);
        IIsPossibly<IVerifiableType> TryGetReturn();
        IIsPossibly<IVerifiableType> TryGetInput();
    }

    public interface ILogicalOperationType {
    }

    public interface ITypeOr : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
        IReadOnlyList<IMemberDefinition> Members { get; }
    }

    public interface ITypeAnd : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
    }


    public interface IPrimitiveType : IVerifiableType { }

    public interface IBlockType : IVerifiableType { }
    public interface IEntryPointType : IVerifiableType { }
    public interface IReferanceType : IVerifiableType { }

    public interface INumberType: IPrimitiveType { }
    public interface IBooleanType: IPrimitiveType { }
    public interface IStringType: IPrimitiveType { }

    public interface IAnyType : IVerifiableType { }
    public interface IEmptyType : IPrimitiveType { }

    public interface IInterfaceModuleType : IVerifiableType, ICodeElement
    {
        // am I sure these need to be ordered?
        // is staticness important here?
        // staticness just controlls access 
        IReadOnlyList<IMemberDefinition> Members { get; }
    }

    public interface IInterfaceType : IInterfaceModuleType
    {
    }

    public interface IModuleType : IInterfaceModuleType
    {
    }

    public interface IMethodType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
    }

    // what is this??
    // 
    // so Method is system generic type
    // i have a hard time thinking about this
    // 
    // but really it is not so complex 
    // this creates methods with types
    // 
    // in that case this should have an input type and a output type
    // they are either placeholders or real types.. we can expect atleat one is a placeholder
    //
    // 
    //public interface IGenericMethodType : IGenericType
    //{
    //}

    // TODO an implementation type is a method type!
    //public interface IImplementationType : IVerifiableType {
    //    IVerifiableType InputType { get; }
    //    IVerifiableType OutputType { get; }
    //    IVerifiableType ContextType { get; }
    //}

    //public interface IGenericImplementationType : IGenericType
    //{
    //}
}
