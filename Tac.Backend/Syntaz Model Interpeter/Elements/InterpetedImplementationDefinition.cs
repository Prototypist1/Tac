using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Elements;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplementationDefinition<TIn, TMethodIn, TMethodOut> : IInterpetedOperation<IInterpetedImplementation<TIn,TMethodIn,TMethodOut>>
        where TIn : class, IInterpetedAnyType
        where TMethodIn : class, IInterpetedAnyType
        where TMethodOut : class, IInterpetedAnyType
    {
        public void Init(
            InterpetedMemberDefinition<TMethodIn> parameterDefinition, 
            InterpetedMemberDefinition<TIn> contextDefinition, 
            IInterpetedOperation<IInterpetedAnyType>[] methodBody,
            IInterpetedScopeTemplate scope,
            IImplementationType implementationType)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            MethodBody = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ImplementationType= implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        }

        private InterpetedMemberDefinition<TMethodIn>? parameterDefinition;
        public InterpetedMemberDefinition<TMethodIn> ParameterDefinition { get => parameterDefinition ?? throw new NullReferenceException(nameof(parameterDefinition)); private set => parameterDefinition = value ?? throw new NullReferenceException(nameof(value)); }


        private InterpetedMemberDefinition<TIn>? contextDefinition;
        public InterpetedMemberDefinition<TIn> ContextDefinition { get => contextDefinition ?? throw new NullReferenceException(nameof(contextDefinition)); private set => contextDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedOperation<IInterpetedAnyType>[]? methodBody;
        public IInterpetedOperation<IInterpetedAnyType>[] MethodBody { get => methodBody ?? throw new NullReferenceException(nameof(methodBody)); private set => methodBody = value ?? throw new NullReferenceException(nameof(value)); }
        
        public IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }

        public IImplementationType? implementationType;
        public IImplementationType ImplementationType { get => implementationType ?? throw new NullReferenceException(nameof(implementationType)); private set => implementationType = value ?? throw new NullReferenceException(nameof(value)); }

        public IInterpetedResult<IInterpetedMember<IInterpetedImplementation<TIn, TMethodIn, TMethodOut>>> Interpet(InterpetedContext interpetedContext)
        {
            var thing = TypeManager.Implementation<TIn, TMethodIn, TMethodOut>(
                ParameterDefinition,
                ContextDefinition,
                MethodBody,
                interpetedContext,
                Scope,
                ImplementationType);

            return InterpetedResult.Create(
                TypeManager.Member(
                    thing.Convert(TransformerExtensions.NewConversionContext()),
                    thing
                ));
        }
        
        //public IInterpetedImplementation<TIn, TMethodIn, TMethodOut> GetDefault(InterpetedContext interpetedContext)
        //{
        //    return TypeManager.Implementation< TIn, TMethodIn, TMethodOut > (
        //            new InterpetedMemberDefinition<TMethodIn>().Init(new NameKey("input")),
        //            new InterpetedMemberDefinition<TIn>().Init(new NameKey("context")),
        //            new IInterpetedOperation<IInterpetedAnyType>[] { },
        //            interpetedContext,
        //            Scope);
        //}
    }
    
}