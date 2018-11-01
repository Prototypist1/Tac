using Prototypist.LeftToRight;
using System.Collections.Generic;
using System.Linq;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplementationDefinition : IInterpeted
    {
        public void Init(
            InterpetedMemberDefinition parameterDefinition, 
            InterpetedMemberDefinition contextDefinition, 
            IInterpeted[] methodBody,
            IInterpetedScopeTemplate scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            MethodBody = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public InterpetedMemberDefinition ParameterDefinition { get; private set; }
        public InterpetedMemberDefinition ContextDefinition { get; private set; }
        public IInterpeted[] MethodBody { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedImplementation(
                ParameterDefinition,
                ContextDefinition,
                MethodBody,
                interpetedContext,
                Scope));
        }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new InterpetedImplementation(
                    new InterpetedMemberDefinition().Init(new NameKey("input")),
                    new InterpetedMemberDefinition().Init(new NameKey("context")),
                    new IInterpeted[] { },
                    interpetedContext,
                    Scope);
        }
    }
    
}