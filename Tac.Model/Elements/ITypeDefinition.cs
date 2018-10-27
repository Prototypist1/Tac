using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ITypeDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
}
