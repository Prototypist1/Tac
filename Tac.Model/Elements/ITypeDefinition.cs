using Tac.Model;

namespace Tac.Model.Elements
{
    public interface ITypeDefinition : ICodeElement, IType
    {
        IFinalizedScope Scope { get; }
    }

    public interface IType {
        T Convert<T>(ITypeConverter<T> context);
    }

    public interface INumberType: IType { }
    public interface IAnyType: IType { }
    public interface IEmptyType : IType { }
    public interface IBooleanType: IType { }
    public interface IStringType: IType { }
}
