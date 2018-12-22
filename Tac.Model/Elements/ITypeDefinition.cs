using Tac.Model;

namespace Tac.Model.Elements
{

    public interface IInterfaceType : IVarifiableType, ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
    
    public interface IVarifiableType {
    }


    public interface IBlockType : IVarifiableType { }

    public interface INumberType: IVarifiableType { }
    public interface IBooleanType: IVarifiableType { }
    public interface IStringType: IVarifiableType { }

    public interface IAnyType : IVarifiableType { }
    public interface IEmptyType : IVarifiableType { }
    public interface IObjectType : IVarifiableType { }
    public interface IModuleType : IVarifiableType { }

    public interface IMethodType : IVarifiableType {
        IVarifiableType InputType { get; }
        IVarifiableType OutputType { get; }
    }

    public interface IGenericMethodType : IVarifiableType
    {
    }

    public interface IImplementationType : IVarifiableType {
        IVarifiableType InputType { get; }
        IVarifiableType OutputType { get; }
        IVarifiableType ContextType { get; }
    }

    public interface IGenericImplementationType : IVarifiableType
    {
    }
}
