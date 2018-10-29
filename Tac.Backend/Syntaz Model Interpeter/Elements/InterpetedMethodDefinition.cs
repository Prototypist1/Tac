using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMethodDefinition : IInterpeted, IInterpetedPrimitiveType
    {
        public void Init(InterpetedMemberDefinition parameterDefinition, InterpetedMemberDefinition contextDefinition, IInterpeted methodBody, IFinalizedScope scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public InterpetedMemberDefinition ParameterDefinition { get; private set; }
        public InterpetedMemberDefinition ContextDefinition { get; private set; }
        public IInterpeted Body { get; private set; }
        public IFinalizedScope Scope { get; private set; }
        
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
                interpetedContext.elementBuilders.MemberDefinition(false, new NameKey("input"), new Box<IWeakReturnable>(new InterpetedAnyType())),
                new IWeakCodeElement[] { },
                interpetedContext,
                new FinalizedScope(new Dictionary<IKey,IBox<WeakMemberDefinition>>()));
        }
    }
}