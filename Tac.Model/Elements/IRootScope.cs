using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IRootScope : ICodeElement
    {

        IFinalizedScope Scope { get; }
        IEntryPointDefinition EntryPoint { get; }
        IReadOnlyList<IAssignOperation> Assignments { get; }
    }
}
