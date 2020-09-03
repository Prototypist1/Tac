using System.Collections.Generic;
using System.Linq;
using Prototypist.Toolbox;
using System;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Instantiated;

namespace Tac.Backend.Emit.SyntaxModel
{
    internal class InterpetedObjectDefinition :  IAssembledOperation
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedAssignOperation> assignments)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
        }

        private IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

        private IEnumerable<IInterpetedAssignOperation>? assignments;
        public IEnumerable<IInterpetedAssignOperation> Assignments { get => assignments ?? throw new NullReferenceException(nameof(assignments)); private set => assignments = value ?? throw new NullReferenceException(nameof(value)); }
        
        public IInterpetedResult<IInterpetedMember> Assemble(AssemblyContext interpetedContext)
        {
            var scope = Scope.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Assemble(context);
            }

            return InterpetedResult.Create(TypeManager.Member(scope.Convert(TransformerExtensions.NewConversionContext()), scope));
        }
        
        //public IInterpetedScope GetDefault()
        //{
        //    return TypeManager.InstanceScope();
        //}
    }
}