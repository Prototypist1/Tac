using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMethodDefinition : MethodDefinition, IInterpeted
    {
        public InterpetedMethodDefinition(ITypeDefinition outputType, MemberDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(outputType, parameterDefinition, body, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMethod(ParameterDefinition,Body,interpetedContext,Scope));
        }
    }
}