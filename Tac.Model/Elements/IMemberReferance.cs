using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public interface IMemberReferance : ICodeElement
    {
        IMemberDefinition MemberDefinition { get; }
    }
}