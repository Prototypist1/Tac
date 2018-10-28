using Tac.Model;

namespace Tac.Model.Elements
{
    public interface ITypeDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
    }
}
