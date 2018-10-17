using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedMethodDefinition : MethodDefinition, IInterpeted
    {
        public InterpetedMethodDefinition(IBox<IReturnable> outputType, IBox<MemberDefinition> parameterDefinition, ICodeElement[] body, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers) : base(outputType, parameterDefinition, body, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMethod(ParameterDefinition.GetValue(), Body, interpetedContext, Scope));
        }

        internal static MethodDefinition MakeNew(IBox<IReturnable> outputType, IBox<MemberDefinition> parameterDefinition, ICodeElement[] body, IResolvableScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            return new InterpetedMethodDefinition(outputType, parameterDefinition, body, scope, staticInitializers);
        }
    }
}