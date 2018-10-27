using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IModuleDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IEnumerable<ICodeElement> StaticInitialization { get; }
    }
}
