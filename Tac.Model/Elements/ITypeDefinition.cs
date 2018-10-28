using Tac.Model;

namespace Tac.Model.Elements
{
    public interface ITypeDefinition : ICodeElement, IType
    {
        IFinalizedScope Scope { get; }
    }

    public interface IType { }
}
