namespace Tac.Model.Elements
{
    public interface ITypeReferance: ICodeElement
    {
        IVerifiableType VerifiableType { get; }
    }
}
