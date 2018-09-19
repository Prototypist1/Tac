using Prototypist.LeftToRight;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using System.Linq;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedImplementationDefinition : ImplementationDefinition, IInterpeted
    {
        public InterpetedImplementationDefinition(MemberDefinition contextDefinition, ITypeDefinition outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers) : base(contextDefinition, outputType, parameterDefinition, metohdBody, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedImplementation(ParameterDefinition,MethodBody.ToArray(),interpetedContext,Scope));
        }
    }
}