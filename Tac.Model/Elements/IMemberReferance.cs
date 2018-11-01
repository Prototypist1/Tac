namespace Tac.Model.Elements
{
    public interface IMemberReferance : ICodeElement, IVarifiableType
    {
        IMemberDefinition MemberDefinition { get; }
    }
}