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
    }
}