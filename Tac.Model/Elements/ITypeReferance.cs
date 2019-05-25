namespace Tac.Model.Elements
{
    public interface ITypeReferance: IConvertable
    {
        IVerifiableType VerifiableType { get; }
    }
}
