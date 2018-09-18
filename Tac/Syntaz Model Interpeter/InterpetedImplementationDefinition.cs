using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using System.Linq;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedImplementationDefinition : ImplementationDefinition, IInterpeted
    {
        public InterpetedImplementationDefinition(MemberDefinition contextDefinition, ITypeDefinition outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers) : base(contextDefinition, outputType, parameterDefinition, metohdBody, scope, staticInitializers)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            return new InterpetedResult(new InterpetedImplementation(ParameterDefinition,MethodBody.ToArray(),interpetedContext,Scope));
        }
    }

    internal class InterpetedImplementation
    {
        public InterpetedImplementation(MemberDefinition parameterDefinition, ICodeElement[] body, InterpetedContext context, IScope scope)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        private MemberDefinition ParameterDefinition { get; }
        private ICodeElement[] Body { get; }
        private InterpetedContext Context { get; }
        private IScope Scope { get; }
        
        public InterpetedResult Invoke(object input)
        {

            return new InterpetedResult(
                new InterpetedMethod(
                    ParameterDefinition,
                    Body,
                    Context.
                    Child(input.Cast<IInterpetedScope>()), Scope));
        }
    }
}