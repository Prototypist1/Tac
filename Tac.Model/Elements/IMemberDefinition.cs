using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : ICodeElement
    {
        IKey Key { get; }
        ITypeReferance Type { get; }
        bool ReadOnly { get; }
    }
}