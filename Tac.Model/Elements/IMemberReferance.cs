namespace Tac.Model.Elements
{
    public interface IMemberReferance : ICodeElement
    {
        IMemberDefinition MemberDefinition { get; }
    }
}