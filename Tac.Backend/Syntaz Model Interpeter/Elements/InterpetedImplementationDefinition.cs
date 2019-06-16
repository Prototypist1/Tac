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

        public InterpetedMemberDefinition<TMethodIn> ParameterDefinition { get; private set; }
        public InterpetedMemberDefinition<TIn> ContextDefinition { get; private set; }
        public IInterpetedOperation<IInterpetedAnyType>[] MethodBody { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }
        public IImplementationType ImplementationType { get; private set; }

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