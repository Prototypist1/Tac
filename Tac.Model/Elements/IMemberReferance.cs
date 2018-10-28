namespace Tac.Model.Elements
{
    public interface IMemberReferance : ICodeElement, IType
    {
        IMemberDefinition MemberDefinition { get; }
    }
}