using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IObjectDefiniton : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IReadOnlyList<IAssignOperation> Assignments { get; }
    }
}
