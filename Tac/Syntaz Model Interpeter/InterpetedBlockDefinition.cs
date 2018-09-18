using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedBlockDefinition : BlockDefinition, IInterpeted
    {
        public InterpetedBlockDefinition(ICodeElement[] body, IScope scope, IEnumerable<ICodeElement> staticInitailizers) : base(body, scope, staticInitailizers)
        {
        }

        private InterpetedStaticScope StaticStuff { get; } = new InterpetedStaticScope(new ConcurrentIndexed<NameKey, InterpetedMember>());

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = InterpetedInstanceScope.Make(StaticStuff, Scope);

            var scope = interpetedContext.Child(res);

            foreach (var line in Body)
            {
                line.Cast<IInterpeted>().Interpet(scope);
            }

            return new InterpetedResult();
        }
    }
}