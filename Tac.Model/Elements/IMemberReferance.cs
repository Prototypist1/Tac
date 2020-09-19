namespace Tac.Model.Elements
{
    public interface IMemberReference : ICodeElement
    {
        IMemberDefinition MemberDefinition { get; }
    }
}