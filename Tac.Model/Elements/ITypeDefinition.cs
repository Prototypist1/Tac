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

    public interface IInterfaceType : IVerifiableType, ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
    
    public interface IVerifiableType {

        bool TheyAreUs(IVerifiableType they, bool noTagBacks);
        bool WeAreThem(IVerifiableType them, bool noTagBacks);
    }

    public interface ILogicalOperationType {
    }

    public interface ITypeOr : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
    }

    public interface ITypeAnd : IVerifiableType {
        IVerifiableType Left { get; }
        IVerifiableType Right { get; }
    }


    public interface IBlockType : IVerifiableType { }

    public interface INumberType: IVerifiableType { }
    public interface IBooleanType: IVerifiableType { }
    public interface IStringType: IVerifiableType { }

    public interface IAnyType : IVerifiableType { }
    public interface IEmptyType : IVerifiableType { }
    
    public interface IObjectType : IVerifiableType {
        // am I sure these need to be ordered?
        // is staticness important here?
        // staticness just controlls access 
        IReadOnlyList<IMemberDefinition> Members { get; }
    }


    public interface IModuleType : IVerifiableType {
        // am I sure these need to be ordered?
        IReadOnlyList<IMemberDefinition> Members { get; }
    }

    public interface IMethodType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
    }

    public interface IGenericMethodType : IGenericType
    {
    }
    // TODO an implementation type is a method type!
    public interface IImplementationType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IVerifiableType ContextType { get; }
    }

    public interface IGenericImplementationType : IGenericType
    {
    }
}
