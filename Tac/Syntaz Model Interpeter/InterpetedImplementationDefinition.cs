using Prototypist.LeftToRight;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using System.Linq;
using System;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedImplementationDefinition : ImplementationDefinition, IInterpeted
    {
        public InterpetedImplementationDefinition(IBox<MemberDefinition> contextDefinition, IBox<MemberDefinition> parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers) : base(contextDefinition, parameterDefinition, outputType, metohdBody, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedImplementation(ParameterDefinition.GetValue(),MethodBody.ToArray(),interpetedContext,Scope));
        }

        internal static ImplementationDefinition MakeNew(IBox<MemberDefinition> contextDefinition, IBox<MemberDefinition> parameterDefinition, IBox<IReturnable> outputType, IEnumerable<ICodeElement> metohdBody, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            return new InterpetedImplementationDefinition(contextDefinition, parameterDefinition, outputType, metohdBody, scope, staticInitializers);
        }
    }
}