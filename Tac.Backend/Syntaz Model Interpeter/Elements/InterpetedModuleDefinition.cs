using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedModuleDefinition : IInterpetedOperation<IInterpetedScope>
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpetedOperation<object>> staticInitialization)
        {
            ScopeTemplate = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
        }
        
        public IInterpetedScopeTemplate ScopeTemplate { get; private set; }
        public IEnumerable<IInterpetedOperation<object>> StaticInitialization { get; private set; }

        public IInterpetedResult<IInterpetedMember<IInterpetedScope>> Interpet(InterpetedContext interpetedContext)
        {
            var scope = ScopeTemplate.Create();

            var context = interpetedContext.Child(scope);

            foreach (var line in StaticInitialization)
            {
                line.Interpet(context);
            }

            return InterpetedResult.Create(new InterpetedMember<IInterpetedScope>(scope));
        }
        
        public IInterpetedScope GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedStaticScope.Make();
        }
    }
}