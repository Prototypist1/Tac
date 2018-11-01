using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedModuleDefinition : IInterpeted
    {
        public void Init(IInterpetedScopeTemplate scope, IEnumerable<IInterpeted> staticInitialization)
        {
            ScopeTemplate = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
        }

        public IInterpetedScopeTemplate ScopeTemplate { get; private set; }
        public IEnumerable<IInterpeted> StaticInitialization { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = ScopeTemplate.Create(interpetedContext);

            var context = interpetedContext.Child(scope);

            foreach (var line in StaticInitialization)
            {
                line.Cast<IInterpeted>().Interpet(context);
            }

            return InterpetedResult.Create(scope);
        }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedStaticScope.Make();
        }
    }
}