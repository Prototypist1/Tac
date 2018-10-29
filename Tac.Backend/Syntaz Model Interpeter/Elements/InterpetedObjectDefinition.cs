using System.Collections.Generic;
using Prototypist.TaskChain.DataTypes;
using System.Linq;
using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedObjectDefinition :  IInterpeted, IInterpetedPrimitiveType
    {
        public void Init(IFinalizedScope scope, IEnumerable<InterpetedAssignOperation> assignments)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments));
        }

        public IFinalizedScope Scope { get; private set; }
        public IEnumerable<InterpetedAssignOperation> Assignments { get; private set; }

        private InterpetedStaticScope StaticStuff { get; } = InterpetedStaticScope.Empty();

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = InterpetedInstanceScope.Make(interpetedContext,StaticStuff, Scope);

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Cast<IInterpeted>().Interpet(context);
            }

            return InterpetedResult.Create(scope);
        }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedInstanceScope.Make();
        }
    }
}