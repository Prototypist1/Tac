using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;
using Tac.Syntaz_Model_Interpeter;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Backend.Syntaz_Model_Interpeter.Elements
{
    internal class InterpetedExternalMethodDefinition<TIn,TOut> : IInterpetedOperation<IInterpetedMethod<TIn,TOut>>
    {
        public void Init(InterpetedMemberDefinition<TIn> parameterDefinition, Func<TIn, TOut> backing)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TIn, TOut>>> Interpet(InterpetedContext interpetedContext)
        {
            return InterpetedResult.Create(InterpetedMember.Make<IInterpetedMethod<TIn, TOut>>( new InterpetedExternalMethod<TIn, TOut>(ParameterDefinition,Backing)));
        }

        void IInterpetedOperation.Interpet(InterpetedContext interpetedContext)
        {
            Interpet(interpetedContext);
        }

        public InterpetedMemberDefinition<TIn> ParameterDefinition { get; private set; }
        public Func<TIn, TOut> Backing { get; private set; }

    }

}
