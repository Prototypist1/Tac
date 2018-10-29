using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedModuleDefinition : IInterpeted, IInterpetedPrimitiveType
    {
        public void Init(IFinalizedScope scope, IEnumerable<IInterpeted> staticInitialization)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialization = staticInitialization ?? throw new ArgumentNullException(nameof(staticInitialization));
        }

        public IFinalizedScope Scope { get; private set; }
        public IEnumerable<IInterpeted> StaticInitialization { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = InterpetedStaticScope.Make(interpetedContext,Scope);

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