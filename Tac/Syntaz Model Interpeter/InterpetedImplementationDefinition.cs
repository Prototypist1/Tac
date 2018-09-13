using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplementationDefinition : ImplementationDefinition, IInterpeted
    {
        public InterpetedImplementationDefinition(MemberDefinition contextDefinition, ITypeSource outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers) : base(contextDefinition, outputType, parameterDefinition, metohdBody, scope, staticInitializers)
        {
        }
    }
}