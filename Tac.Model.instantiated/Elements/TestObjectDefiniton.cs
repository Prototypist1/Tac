using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.instantiated
{
    public class TestObjectDefiniton : IObjectDefiniton
    {
        public TestObjectDefiniton(IFinalizedScope scope, IEnumerable<IAssignOperation> assignments)
        {
            Scope = scope;
            Assignments = assignments;
        }

        public IFinalizedScope Scope { get; set; }
        public IEnumerable<IAssignOperation> Assignments { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ObjectDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }
}
