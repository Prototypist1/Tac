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

        public InterpetedMemberDefinition<TIn> ParameterDefinition { get; private set; }
        public IInterpetedOperation<IInterpetedAnyType>[] Body { get; private set; }
        public IInterpetedScopeTemplate Scope { get; private set; }
        public IMethodType MethodType { get; private set; }

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