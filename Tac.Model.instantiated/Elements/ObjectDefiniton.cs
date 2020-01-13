using System;
using System.Collections.Generic;
using System.Linq;
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
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ObjectDefinition(this);
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

        public static IObjectDefiniton CreateAndBuild(IFinalizedScope scope, IEnumerable<IAssignOperation> assignments) {
            var (x, y) = Create();
            y.Build(scope, assignments);
            return x;
        }

        public IVerifiableType Returns()
        {
            return InterfaceType.CreateAndBuild(Scope.Members.Values.Select(x => x.Value).ToList());
        }
    }

    public interface IObjectDefinitonBuilder
    {
        void Build(IFinalizedScope scope, IEnumerable<IAssignOperation> assignments);
    }
}
