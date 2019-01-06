namespace Tac.Model.Elements
{
    public interface IMemberReferance : ICodeElement, IVerifiableType
    {
        IMemberDefinition MemberDefinition { get; }
    }
}