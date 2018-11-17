namespace Tac.Model.Elements
{
    public interface ITypeReferance: IVarifiableType
    {
        IVarifiableType TypeDefinition { get; }
    }
}
