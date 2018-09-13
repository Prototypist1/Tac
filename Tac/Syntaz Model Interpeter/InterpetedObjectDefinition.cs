using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedObjectDefinition : ObjectDefinition, IInterpeted
    {
        public InterpetedObjectDefinition(ObjectScope scope, IEnumerable<ICodeElement> codeElements) : base(scope, codeElements)
        {
        }
    }
}