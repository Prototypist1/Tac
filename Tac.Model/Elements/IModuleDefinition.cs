using System.Collections.Generic;

namespace Tac.Model.Elements
{
    public interface IModuleDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IEnumerable<ICodeElement> StaticInitialization { get; }
    }
}
