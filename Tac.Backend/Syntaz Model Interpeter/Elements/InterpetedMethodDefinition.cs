using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMethodDefinition : IInterpeted, IInterpetedType
    {
        public void Init(
            InterpetedMemberDefinition parameterDefinition, 
            IInterpeted[] methodBody,
            IInterpetedScopeTemplate scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public InterpetedMemberDefinition ParameterDefinition { get; private set; }
        public IInterpeted[] Body { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }
        
        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedMethod(
                ParameterDefinition,
                Body, 
                interpetedContext,
                Scope));
        }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new InterpetedMethod(
                new InterpetedMemberDefinition().Init(new InterpetedAnyType(), new NameKey("input")),
                new IInterpeted[] { },
                interpetedContext,
                Scope);
        }
    }
}