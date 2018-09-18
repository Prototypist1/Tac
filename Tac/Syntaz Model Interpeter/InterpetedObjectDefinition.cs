using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Prototypist.TaskChain.DataTypes;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
using System.Linq;
using Prototypist.LeftToRight;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedObjectDefinition : ObjectDefinition, IInterpeted
    {
        public InterpetedObjectDefinition(ObjectScope scope, IEnumerable<AssignOperation> codeElements) : base(scope, codeElements)
        {
        }

        private InterpetedStaticScope StaticStuff { get; } = InterpetedStaticScope.Empty();

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = InterpetedInstanceScope.Make(StaticStuff, Scope);

            var context = interpetedContext.Child(scope);

            foreach (var line in Assignments)
            {
                line.Cast<IInterpeted>().Interpet(context);
            }

            return InterpetedResult.Create(scope);
        }
    }
}