using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Prototypist.TaskChain.DataTypes;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using System.Linq;
using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedObjectDefinition : WeakObjectDefinition, IInterpeted, IInterpetedPrimitiveType
    {
        public InterpetedObjectDefinition(IWeakFinalizedScope scope, IEnumerable<WeakAssignOperation> assigns, ImplicitKey key) : base(scope, assigns, key)
        {
        }

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

        internal static WeakObjectDefinition MakeNew(IWeakFinalizedScope scope, IEnumerable<WeakAssignOperation> assigns, ImplicitKey key)
        {
            return new InterpetedObjectDefinition(scope, assigns, key);
        }

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedInstanceScope.Make();
        }
    }
}