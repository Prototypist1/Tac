using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedImplementation : IRunTime
    {

        public InterpetedImplementation(WeakMemberDefinition parameterDefinition, WeakMemberDefinition contextDefinition, IWeakCodeElement[] body, InterpetedContext context, IWeakFinalizedScope scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        private readonly WeakMemberDefinition contextDefinition;
        private WeakMemberDefinition ParameterDefinition { get; }
        private IWeakCodeElement[] Body { get; }
        private InterpetedContext Context { get; }
        private IWeakFinalizedScope Scope { get; }
        
        public InterpetedResult Invoke(IRunTime input)
        {

            var context = Context.Child(InterpetedInstanceScope.Make((contextDefinition.Key, new InterpetedMember(input))));

            return InterpetedResult.Create(
                new InterpetedMethod(
                    ParameterDefinition,
                    Body,
                    context,
                    Scope));
        }
    }
}