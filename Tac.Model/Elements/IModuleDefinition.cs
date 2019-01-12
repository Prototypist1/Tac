using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public interface IModuleDefinition : ICodeElement, IVerifiableType
    {
        IKey Key { get; }
        IFinalizedScope Scope { get; }
        IReadOnlyList<ICodeElement> StaticInitialization { get; }
    }
}
