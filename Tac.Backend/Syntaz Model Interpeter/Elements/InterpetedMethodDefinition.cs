using System;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedMethodDefinition<TIn, TOut> : IInterpetedOperation<IInterpetedMethod<TIn, TOut>>
        where TOut : class, IInterpetedAnyType
        where TIn : class, IInterpetedAnyType
    {
        public void Init(
            InterpetedMemberDefinition<TIn> parameterDefinition, 
            IInterpetedOperation<IInterpetedAnyType>[] methodBody,
            IInterpetedScopeTemplate scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public InterpetedMemberDefinition<TIn> ParameterDefinition { get; private set; }
        public IInterpetedOperation<IInterpetedAnyType>[] Body { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }
        
        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TIn,TOut>>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(
                TypeManager.Member<IInterpetedMethod<TIn, TOut>>(
                    TypeManager.InternalMethod<TIn, TOut>(
                        ParameterDefinition,
                        Body, 
                        interpetedContext,
                        Scope)));
        }
        
        public IInterpeted GetDefault(InterpetedContext interpetedContext)
        {
            return TypeManager.InternalMethod<TIn, TOut>(
                new InterpetedMemberDefinition<TIn> ().Init(new NameKey("input")),
                new IInterpetedOperation<IInterpetedAnyType>[] { },
                interpetedContext,
                Scope);
        }

    }
}