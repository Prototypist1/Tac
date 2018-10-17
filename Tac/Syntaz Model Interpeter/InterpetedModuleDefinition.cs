using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedModuleDefinition : ModuleDefinition, IInterpeted
    {
        public InterpetedModuleDefinition(
            IResolvableScope scope, 
            IEnumerable<ICodeElement> staticInitialization, 
            NameKey Key) : 
            base(scope, staticInitialization, Key)
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

        internal static ModuleDefinition MakeNew(IResolvableScope scope, IEnumerable<ICodeElement> staticInitialization, NameKey key)
        {
            return new InterpetedModuleDefinition(scope, staticInitialization, key);
        }
    }
}