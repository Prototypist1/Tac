using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : ICodeElement
    {
        IType Type { get; }
        bool ReadOnly { get; }
    }
}