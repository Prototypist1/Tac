using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using System.Linq;

namespace Tac.Syntaz_Model_Interpeter
{
    public class InterpetedImplementation
    {
        public InterpetedImplementation(MemberDefinition parameterDefinition, ICodeElement[] body, InterpetedContext context, IResolvableScope scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        private MemberDefinition ParameterDefinition { get; }
        private ICodeElement[] Body { get; }
        private InterpetedContext Context { get; }
        private IResolvableScope Scope { get; }
        
        public InterpetedResult Invoke(object input)
        {

            return InterpetedResult.Create(
                new InterpetedMethod(
                    ParameterDefinition,
                    Body,
                    Context.Child(input.Cast<IInterpetedScope>()) // what if the input is an int or something... 
                    Scope));
        }
    }
}