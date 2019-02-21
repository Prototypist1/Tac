using Tac.Model;

namespace Tac.Model.Elements
{

    public interface IInterfaceType : IVerifiableType, ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
    
    public interface IVerifiableType {
    }


    public interface IBlockType : IVerifiableType { }

    public interface INumberType: IVerifiableType { }
    public interface IBooleanType: IVerifiableType { }
    public interface IStringType: IVerifiableType { }

    public interface IAnyType : IVerifiableType { }
    public interface IEmptyType : IVerifiableType { }
    
    public interface IObjectType : IVerifiableType { }
    public interface IModuleType : IVerifiableType { }

    public interface IMethodType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
    }

    public interface IGenericMethodType : IGenericType
    {
    }
    public interface IImplementationType : IVerifiableType {
        IVerifiableType InputType { get; }
        IVerifiableType OutputType { get; }
        IVerifiableType ContextType { get; }
    }

    public interface IGenericImplementationType : IGenericType
    {
    }
}
