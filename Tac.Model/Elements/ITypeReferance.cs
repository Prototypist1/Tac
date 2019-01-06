namespace Tac.Model.Elements
{
    public interface ITypeReferance: IVerifiableType, ICodeElement
    {
        IVerifiableType TypeDefinition { get; }
    }
}
