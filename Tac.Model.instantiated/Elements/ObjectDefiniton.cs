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
        private readonly Buildable<IReadOnlyList<IOrType<IAssignOperation, IError>>> buildableAssignments = new Buildable<IReadOnlyList<IOrType<IAssignOperation, IError>>>();

        private ObjectDefiniton() { }

        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<IOrType<IAssignOperation, IError>> Assignments => buildableAssignments.Get();
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ObjectDefinition(this);
        }

        public void Build(IFinalizedScope scope, IReadOnlyList<IOrType<IAssignOperation, IError>> assignments)
        {
            buildableScope.Set(scope);
            buildableAssignments.Set(assignments);
        }
        
        public static (IObjectDefiniton, IObjectDefinitonBuilder) Create()
        {
            var res = new ObjectDefiniton();
            return (res, res);
        }

        public static IObjectDefiniton CreateAndBuild(IFinalizedScope scope, IReadOnlyList<IOrType<IAssignOperation, IError>> assignments) {
            var (x, y) = Create();
            y.Build(scope, assignments);
            return x;
        }

        public IOrType<IVerifiableType, IError> Returns()
        {
            return new OrType<IVerifiableType, IError>( InterfaceType.CreateAndBuild(Scope.Members.Values.Select(x => x.Value).ToList()));
        }
    }

    public interface IObjectDefinitonBuilder
    {
        void Build(IFinalizedScope scope, IReadOnlyList<IOrType<IAssignOperation, IError>> assignments);
    }
}
