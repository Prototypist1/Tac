using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;

namespace Tac.Backend.Interpreted.Syntaz_Model_Interpeter.Elements
{
    internal class InterpetedRootScope : IInterpetedOperation
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedAssignOperation> assignments, InterpetedEntryPointDefinition entryPoint)
        {

            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
            EntryPoint = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
        }

        private IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

        private IEnumerable<IInterpetedAssignOperation>? assignments;
        public IEnumerable<IInterpetedAssignOperation> Assignments { get => assignments ?? throw new NullReferenceException(nameof(assignments)); private set => assignments = value ?? throw new NullReferenceException(nameof(value)); }

        private InterpetedEntryPointDefinition? entryPoint;
        public InterpetedEntryPointDefinition EntryPoint { get => entryPoint ?? throw new NullReferenceException(nameof(entryPoint)); private set => entryPoint = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetWithExposedScope(interpetedContext).Item2;
        }

        public (IInterpetedScope, IInterpetedResult<IInterpetedMember>) InterpetWithExposedScope(InterpetedContext interpetedContext)
        {
            var scope = Scope.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Interpet(context);
            }

            return (scope,EntryPoint.Interpet(context));
        }
    }
}
