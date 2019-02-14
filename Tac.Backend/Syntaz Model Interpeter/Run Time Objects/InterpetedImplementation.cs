﻿using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedImplementation<TIn, TMethodIn, TMethodOut> : IInterpetedCallable<TIn, IInterpetedMethod<TMethodIn,TMethodOut>>
    {
    }

    internal class InterpetedImplementation<TIn, TMethodIn, TMethodOut> : IInterpetedImplementation<TIn, TMethodIn, TMethodOut>
    {

        public InterpetedImplementation(
            InterpetedMemberDefinition<TMethodIn> parameterDefinition,
            InterpetedMemberDefinition<TIn> contextDefinition,
            IInterpeted[] body,
            InterpetedContext context, 
            IInterpetedScopeTemplate scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        private readonly InterpetedMemberDefinition<TIn> contextDefinition;
        private InterpetedMemberDefinition<TMethodIn> ParameterDefinition { get; }
        private IInterpeted[] Body { get; }
        private InterpetedContext Context { get; }
        private IInterpetedScopeTemplate Scope { get; }
        
        public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TMethodIn, TMethodOut>>> Invoke(IInterpetedMember<TIn> input)
        {

            var context = Context.Child(InterpetedInstanceScope.Make((contextDefinition.Key, input)));

            return InterpetedResult.Create(new InterpetedMember< IInterpetedMethod < TMethodIn, TMethodOut >> (
                new InterpetedMethod<TMethodIn, TMethodOut>(
                    ParameterDefinition,
                    Body,
                    context,
                    Scope)));
        }
    }
}