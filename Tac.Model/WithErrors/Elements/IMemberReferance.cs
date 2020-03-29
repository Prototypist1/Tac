namespace Tac.Model.WithErrors.Elements
{
    public interface IMemberReferance : ICodeElement
    {
        IMemberDefinition MemberDefinition { get; }
    }
}