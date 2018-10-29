using Prototypist.LeftToRight;
using System.Collections.Generic;
using System.Linq;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplementationDefinition : IInterpeted, IInterpetedPrimitiveType
    {
        public void Init(InterpetedMemberDefinition parameterDefinition, InterpetedMemberDefinition contextDefinition, IInterpeted methodBody, IFinalizedScope scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            MethodBody = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public InterpetedMemberDefinition ParameterDefinition { get; private set; }
        public InterpetedMemberDefinition ContextDefinition { get; private set; }
        public IInterpeted MethodBody { get; private set; }
        public IFinalizedScope Scope { get; private set; }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(new InterpetedImplementation(
                ParameterDefinition,
                ContextDefinition,
                MethodBody.ToArray(),
                interpetedContext,
                Scope));
        }
        
        public IRunTime GetDefault(InterpetedContext interpetedContext)
        {
            return new InterpetedImplementation(
                    interpetedContext.elementBuilders.MemberDefinition(false,new NameKey("input"),new Box<IWeakReturnable>(new InterpetedAnyType())),
                    interpetedContext.elementBuilders.MemberDefinition(false, new NameKey("countex"), new Box<IWeakReturnable>(new InterpetedAnyType())),
                    new IWeakCodeElement[] { },
                    interpetedContext,
                    new FinalizedScope(new Dictionary<IKey,IBox<WeakMemberDefinition>>()));
        }
    }
    
}