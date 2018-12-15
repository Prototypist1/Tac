namespace Tac.Model.Elements
{
    public interface ITypeReferance: IVarifiableType, ICodeElement
    {
        IVarifiableType TypeDefinition { get; }
    }
}
