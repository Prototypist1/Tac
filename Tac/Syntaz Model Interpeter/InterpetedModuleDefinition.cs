using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedModuleDefinition : ModuleDefinition, IInterpeted, IInterpetedPrimitiveType
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
            var scope = InterpetedStaticScope.Make(interpetedContext,Scope);

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

        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return InterpetedStaticScope.Make();
        }
    }
}