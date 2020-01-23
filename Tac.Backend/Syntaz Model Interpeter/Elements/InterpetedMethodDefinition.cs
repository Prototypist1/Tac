using System;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
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
            IInterpetedScopeTemplate scope,
            IMethodType methodType )
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
        }


        private InterpetedMemberDefinition<TIn>? parameterDefinition;
        public InterpetedMemberDefinition<TIn> ParameterDefinition { get => parameterDefinition ?? throw new NullReferenceException(nameof(parameterDefinition)); private set => parameterDefinition = value ?? throw new NullReferenceException(nameof(value)); }

        private IInterpetedOperation<IInterpetedAnyType>[]? body;
        public IInterpetedOperation<IInterpetedAnyType>[] Body { get => body ?? throw new NullReferenceException(nameof(body)); private set => body = value ?? throw new NullReferenceException(nameof(value)); }

        private IInterpetedScopeTemplate? scope;
        public IInterpetedScopeTemplate Scope { get => scope ?? throw new NullReferenceException(nameof(scope)); private set => scope = value ?? throw new NullReferenceException(nameof(value)); }


        private IMethodType? methodType;
        public IMethodType MethodType { get => methodType ?? throw new NullReferenceException(nameof(methodType)); private set => methodType = value ?? throw new NullReferenceException(nameof(value)); }


        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TIn,TOut>>> Interpet(InterpetedContext interpetedContext)
        {
            var thing = TypeManager.InternalMethod<TIn, TOut>(
                        ParameterDefinition,
                        Body,
                        interpetedContext,
                        Scope,
                        MethodType);
            return InterpetedResult.Create(
                TypeManager.Member(
                    thing.Convert(TransformerExtensions.NewConversionContext()), 
                    thing));
        }
        
        //public IInterpeted GetDefault(InterpetedContext interpetedContext)
        //{
        //    // here I need to map TIn, TOut to real types
        //    // not sure 
        //    return TypeManager.InternalMethod<TIn, TOut>(
        //        new InterpetedMemberDefinition<TIn> ().Init(new NameKey("input")),
        //        new IInterpetedOperation<IInterpetedAnyType>[] { },
        //        interpetedContext,
        //        Scope);
        //}

    }
}