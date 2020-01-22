using System.Collections.Generic;
using System.Linq;
using Prototypist.Toolbox;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Instantiated;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedObjectDefinition :  IInterpetedOperation<IInterpetedScope>
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedAssignOperation<IInterpetedAnyType>> assignments)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
        }

        private IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

        private IEnumerable<IInterpetedAssignOperation<IInterpetedAnyType>>? assignments;
        public IEnumerable<IInterpetedAssignOperation<IInterpetedAnyType>> Assignments { get => assignments ?? throw new NullReferenceException(nameof(assignments)); private set => assignments = value ?? throw new NullReferenceException(nameof(value)); }
        
        public IInterpetedResult<IInterpetedMember<IInterpetedScope>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = Scope.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Interpet(context);
            }

            return InterpetedResult.Create(TypeManager.Member(scope.Convert(TransformerExtensions.NewConversionContext()), scope));
        }
        
        //public IInterpetedScope GetDefault()
        //{
        //    return TypeManager.InstanceScope();
        //}
    }
}