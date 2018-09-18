using Prototypist.LeftToRight;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedModuleDefinition : ModuleDefinition, IInterpeted
    {
        public InterpetedModuleDefinition(IScope scope, IEnumerable<ICodeElement> staticInitialization) : base(scope, staticInitialization)
        {
        }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            var scope = InterpetedStaticScope.Make(Scope);

            var context = interpetedContext.Child(scope);

            foreach (var line in StaticInitialization)
            {
                line.Cast<IInterpeted>().Interpet(context);
            }

            return InterpetedResult.Create(scope);
        }
    }
}