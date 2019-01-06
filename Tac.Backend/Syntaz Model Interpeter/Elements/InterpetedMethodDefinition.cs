using System;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMethodDefinition : IInterpeted
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
                new InterpetedMemberDefinition().Init(new NameKey("input")),
                new IInterpeted[] { },
                interpetedContext,
                Scope);
        }
    }
}