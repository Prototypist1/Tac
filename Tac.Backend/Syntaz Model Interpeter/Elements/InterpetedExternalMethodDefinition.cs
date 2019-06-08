using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Elements
{
    internal class InterpetedExternalMethodDefinition<TIn,TOut> : IInterpetedOperation<IInterpetedMethod<TIn,TOut>>
        where TOut :  IInterpetedAnyType
        where TIn :  IInterpetedAnyType
    {
        public void Init(Func<TIn, TOut> backing)
        {
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TIn, TOut>>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(TypeManager.Member<IInterpetedMethod<TIn, TOut>>(TypeManager.ExternalMethod<TIn, TOut>(Backing)));
        }

        public Func<TIn, TOut> Backing { get; private set; }

    }

}
