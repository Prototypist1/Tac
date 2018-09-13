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
    }
}