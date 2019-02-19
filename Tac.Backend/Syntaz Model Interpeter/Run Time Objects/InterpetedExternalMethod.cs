﻿using Prototypist.LeftToRight;
using System;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal class InterpetedExternalMethod<TIn, TOut> : IInterpetedMethod<TIn,TOut>
    {
        public InterpetedExternalMethod(
            InterpetedMemberDefinition<TIn> parameterDefinition,
            Func<TIn, TOut> backing)
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        private InterpetedMemberDefinition<TIn> ParameterDefinition { get; }
        private Func<TIn, TOut> Backing { get; }
        
        public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input)
        {
            return InterpetedResult.Create(new InterpetedMember<TOut>(Backing(input.Value)));
        }
    }
}