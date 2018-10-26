using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedBlockDefinition : WeakBlockDefinition, IInterpeted
    {
        public InterpetedBlockDefinition(IWeakCodeElement[] body, IWeakFinalizedScope scope, IEnumerable<IWeakCodeElement> staticInitailizers) : base(body, scope, staticInitailizers)
        {
        }

        private InterpetedStaticScope StaticStuff { get; } = InterpetedStaticScope.Empty();

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var res = InterpetedInstanceScope.Make(interpetedContext,StaticStuff, Scope);

            var scope = interpetedContext.Child(res);

            foreach (var line in Body)
            {
                var result = line.Cast<IInterpeted>().Interpet(scope);
                if (result.IsReturn)
                {
                    if (result.HasValue)
                    {
                        return InterpetedResult.Create(result.Get());
                    }
                    else
                    {
                        return InterpetedResult.Create();
                    }
                }
            }

            return InterpetedResult.Create();
        }

        internal static WeakBlockDefinition MakeNew(IWeakCodeElement[] body, IWeakFinalizedScope scope, IEnumerable<IWeakCodeElement> staticInitailizers)
        {
            return new InterpetedBlockDefinition(body, scope, staticInitailizers);
        }
    }
}