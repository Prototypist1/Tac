using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedModuleDefinition : IInterpetedOperation<IInterpetedScope>
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedOperation<IInterpetedAnyType>> staticInitialization)
        {
            ScopeTemplate = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
        }
        
        public IInterpetedScopeTemplate ScopeTemplate { get; private set; }
        public IEnumerable<IInterpetedOperation<IInterpetedAnyType>> StaticInitialization { get; private set; }

        public IInterpetedResult<IInterpetedMember<IInterpetedScope>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = ScopeTemplate.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in StaticInitialization)
            {
                line.Interpet(context);
            }

            return InterpetedResult.Create(TypeManager.Member(scope.Convert(TransformerExtensions.NewConversionContext()), scope));
        }
        
        //public IInterpetedScope GetDefault()
        //{
        //    return TypeManager.EmptyStaticScope();
        //}
    }
}