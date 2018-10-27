using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IMemberDefinition : ICodeElement
    {
        ITypeDefinition Type { get; }
        bool ReadOnly { get; }
    }
}