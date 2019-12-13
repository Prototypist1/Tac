using System.Collections.Generic;
using System.Linq;
using Prototypist.LeftToRight;
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

        public IInterpetedScopeTemplate Scope { get; private set; }
        public IEnumerable<IInterpetedAssignOperation<IInterpetedAnyType>> Assignments { get; private set; }
        
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