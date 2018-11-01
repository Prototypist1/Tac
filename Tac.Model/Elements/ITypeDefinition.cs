using Tac.Model;

namespace Tac.Model.Elements
{
    public interface IInterfaceType : IVarifiableType
    {
        IFinalizedScope Scope { get; }
    }

    //public interface IConvertableType
    //{
    //    T Convert<T>(ITypeConverter<T> context);
    //}

    public interface IVarifiableType {
    }

    public interface INumberType: IVarifiableType { }
    public interface IAnyType: IVarifiableType { }
    public interface IEmptyType : IVarifiableType { }
    public interface IBooleanType: IVarifiableType { }
    public interface IStringType: IVarifiableType { }
    public interface IObjectType : IVarifiableType { }

    public interface IModuleType : IVarifiableType { }
    public interface IMethodType : IVarifiableType { }
    public interface IImplementationType : IVarifiableType { }
}
