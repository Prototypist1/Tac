using Prototypist.Toolbox;
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
        private readonly Buildable<IReadOnlyList<IAssignOperation>> buildableAssignments = new Buildable<IReadOnlyList<IAssignOperation>>();
        private IVerifiableType type;

        private ObjectDefiniton() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<IAssignOperation> Assignments => buildableAssignments.Get();
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ObjectDefinition(this);
        }

        public void Build(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments)
        {
            buildableScope.Set(scope);
            buildableAssignments.Set(assignments);
            type = InterfaceType.CreateAndBuild(scope.Members.Values.Select(x => MemberDefinition.CreateAndBuild(x.Value.Key, x.Value.Type, x.Value.Access)).ToList());
        }

        public static (IObjectDefiniton, IObjectDefinitonBuilder) Create()
        {
            var res = new ObjectDefiniton();
            return (res, res);
        }

        public static IObjectDefiniton CreateAndBuild(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments)
        {
            var (x, y) = Create();
            y.Build(scope, assignments);
            return x;
        }

        public IVerifiableType Returns() => type;

    }

    public interface IObjectDefinitonBuilder
    {
        void Build(IFinalizedScope scope, IReadOnlyList<IAssignOperation> assignments);
    }
}
