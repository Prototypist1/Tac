using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedImplementation<TIn, TMethodIn, TMethodOut> : IInterpetedData
        where TIn : IInterpetedData
        where TMethodIn: IInterpetedData
        where TMethodOut : IInterpetedData
    {
        IInterpetedResult<IInterpetedMethod<TMethodIn, TMethodOut>> Invoke(TIn input);
    }

    internal class InterpetedImplementation<TIn, TMethodIn, TMethodOut> : IInterpetedImplementation<TIn, TMethodIn, TMethodOut>
        where TIn : IInterpetedData
        where TOut : IInterpetedData
    {

        public InterpetedImplementation(
            InterpetedMemberDefinition parameterDefinition,
            InterpetedMemberDefinition contextDefinition,
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

        private readonly InterpetedMemberDefinition contextDefinition;
        private InterpetedMemberDefinition ParameterDefinition { get; }
        private IInterpeted[] Body { get; }
        private InterpetedContext Context { get; }
        private IInterpetedScopeTemplate Scope { get; }
        
        public IInterpetedResult<IInterpetedMethod> Invoke(Run_Time_Objects.IInterpeted input)
        {

            var context = Context.Child(InterpetedInstanceScope.Make((contextDefinition.Key, new InterpetedMember(input))));

            return InterpetedResult<IInterpetedMethod>.Create(
                new InterpetedMethod(
                    ParameterDefinition,
                    Body,
                    context,
                    Scope));
        }
    }
}