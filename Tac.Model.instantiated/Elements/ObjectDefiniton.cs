using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ObjectDefiniton : IObjectDefiniton, IObjectDefinitonBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IEnumerable<IAssignOperation>> buildableAssignments = new Buildable<IEnumerable<IAssignOperation>>();

        private ObjectDefiniton() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IEnumerable<IAssignOperation> Assignments => buildableAssignments.Get();

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ObjectDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }

        public void Build(IFinalizedScope scope, IEnumerable<IAssignOperation> assignments)
        {
            buildableScope.Set(scope);
            buildableAssignments.Set(assignments);
        }


        public static (IObjectDefiniton, IObjectDefinitonBuilder) Create()
        {
            var res = new ObjectDefiniton();
            return (res, res);
        }
    }

    public interface IObjectDefinitonBuilder
    {
        void Build(IFinalizedScope scope, IEnumerable<IAssignOperation> assignments);
    }
}
