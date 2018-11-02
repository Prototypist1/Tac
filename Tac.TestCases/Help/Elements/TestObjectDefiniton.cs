using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public class TestObjectDefiniton : IObjectDefiniton
    {
        public IFinalizedScope Scope { get; }
        public IEnumerable<IAssignOperation> Assignments { get; }
    }
}
