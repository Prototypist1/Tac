using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.WithErrors.Elements
{
    public interface IObjectDefiniton : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IReadOnlyList<IOrType<IAssignOperation, IError>> Assignments { get; }
    }
}
