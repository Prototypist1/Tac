using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : ICodeElement
    {
        ITypeDefinition Type { get; }
        bool ReadOnly { get; }
    }
}