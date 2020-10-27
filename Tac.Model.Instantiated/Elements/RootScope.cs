using System.Collections.Generic;
using System.Linq;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class RootScope : IRootScope, IRootScopeBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IReadOnlyList<IAssignOperation>> buildableAssignments = new Buildable<IReadOnlyList<IAssignOperation>>();
        private readonly Buildable<IEntryPointDefinition> buildableEntryPoint = new Buildable<IEntryPointDefinition>();
        private IVerifiableType type;

        private RootScope() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<IAssignOperation> Assignments => buildableAssignments.Get();
        public IEntryPointDefinition EntryPoint => buildableEntryPoint.Get();



        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.RootScope(this);
        }

        public void Build(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments, IEntryPointDefinition entryPoint)
        {
            buildableScope.Set(scope);
            buildableAssignments.Set(assignments);
            buildableEntryPoint.Set(entryPoint);
            type = InterfaceType.CreateAndBuild(scope.Members.Values.Select(x => MemberDefinition.CreateAndBuild(x.Value.Key, x.Value.Type, x.Value.Access)).ToList());
        }

        public static (IRootScope, IRootScopeBuilder) Create()
        {
            var res = new RootScope();
            return (res, res);
        }

        public static IRootScope CreateAndBuild(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments, IEntryPointDefinition entryPoint)
        {
            var (x, y) = Create();
            y.Build(scope, assignments, entryPoint);
            return x;
        }

        public IVerifiableType Returns() => type;

    }


    public interface IRootScopeBuilder
    {
        void Build(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments, IEntryPointDefinition entryPoint);
    }
}
