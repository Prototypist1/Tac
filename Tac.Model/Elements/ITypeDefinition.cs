using Tac.Model;

namespace Tac.Model.Elements
{
    public interface IConvertableType
    {
        T Convert<T>(ITypeConverter<T> context);
    }

    public interface IVarifiableType {
    }

    public interface INumberType: IVarifiableType, IConvertableType { }
    public interface IAnyType: IVarifiableType, IConvertableType { }
    public interface IEmptyType : IVarifiableType, IConvertableType { }
    public interface IBooleanType: IVarifiableType, IConvertableType { }
    public interface IStringType: IVarifiableType, IConvertableType { }
    public interface IObjectType : IVarifiableType, IConvertableType { }
    public interface IInterfaceType : IVarifiableType, IConvertableType { }
    public interface IModuleType : IVarifiableType, IConvertableType { }
    public interface IMethodType : IVarifiableType, IConvertableType { }
    public interface IImplementationType : IVarifiableType, IConvertableType, ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
}
