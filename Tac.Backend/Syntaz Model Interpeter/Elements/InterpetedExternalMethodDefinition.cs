using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Elements
{
    internal class InterpetedExternalMethodDefinition<TIn,TOut> : IInterpetedOperation<IInterpetedMethod<TIn,TOut>>
        where TOut :  IInterpetedAnyType
        where TIn :  IInterpetedAnyType
    {
        private IMethodType? methodType;
        public IMethodType MethodType { get => methodType ?? throw new NullReferenceException(nameof(methodType)); private set => methodType = value ?? throw new NullReferenceException(nameof(value)); }




        public void Init(Func<TIn, TOut> backing, IMethodType methodType)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
        }

        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TIn, TOut>>> Interpet(InterpetedContext interpetedContext)
        {
            var thing = TypeManager.ExternalMethod(Backing, MethodType);

            return InterpetedResult.Create(TypeManager.Member(thing.Convert(TransformerExtensions.NewConversionContext()), thing));
        }

        private Func<TIn, TOut>? backing;
        public Func<TIn, TOut> Backing { get => backing ?? throw new NullReferenceException(nameof(backing)); private set => backing = value ?? throw new NullReferenceException(nameof(value)); }


    }
}