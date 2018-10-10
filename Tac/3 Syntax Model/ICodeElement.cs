using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public interface ICodeElement {
        IReturnable ReturnType(IElementBuilders elementBuilders);
    }
}
