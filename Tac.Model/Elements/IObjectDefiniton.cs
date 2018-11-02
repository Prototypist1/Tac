using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public interface IObjectDefiniton : ICodeElement, IVarifiableType
    {
        IFinalizedScope Scope { get; }
        IEnumerable<IAssignOperation> Assignments { get; }
    }
}
