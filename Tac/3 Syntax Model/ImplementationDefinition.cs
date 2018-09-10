using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // really really not sure how these work atm
    // for now they just hold everything you need to ake a method

    public sealed class ImplementationDefinition: ITypeSource, ITypeDefinition
    {
        public ImplementationDefinition(MemberDefinition contextDefinition, ITypeSource outputType, MemberDefinition parameterDefinition, IEnumerable<ICodeElement> metohdBody, IScope scope, IEnumerable<ICodeElement> staticInitializers)
        {
            ContextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBody = metohdBody ?? throw new ArgumentNullException(nameof(metohdBody));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            StaticInitialzers = staticInitializers ?? throw new ArgumentNullException(nameof(staticInitializers));
        }

        // dang! these could also be inline definitions 
        public ITypeSource ContextType { get => ContextDefinition.Type; }
        public ITypeSource InputType { get => ParameterDefinition.Type; }
        public ITypeSource OutputType { get; }
        public MemberDefinition ContextDefinition { get; }
        public MemberDefinition ParameterDefinition { get; }
        public IScope Scope { get; }
        public IEnumerable<ICodeElement> MethodBody { get; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; }
        

        public override bool Equals(object obj)
        {
            return obj is ImplementationDefinition implementation &&
                ContextDefinition.Equals(implementation.ContextDefinition) &&
                OutputType.Equals(implementation.OutputType) &&
                ParameterDefinition.Equals(implementation.ParameterDefinition) &&
                MethodBody.SequenceEqual(implementation.MethodBody) &&
                Scope.Equals(implementation.Scope) &&
                StaticInitialzers.SequenceEqual(implementation.StaticInitialzers);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ContextDefinition.GetHashCode() +
                    OutputType.GetHashCode() +
                    ParameterDefinition.GetHashCode() +
                    MethodBody.GetHashCode() +
                    Scope.GetHashCode() +
                    StaticInitialzers.GetHashCode();
            }
        }

        public ITypeDefinition ReturnType(ScopeStack scope) {
            if (ContextType.TryGetTypeDefinition(scope, out var context) &&
                InputType.TryGetTypeDefinition(scope, out var input) &&
                OutputType.TryGetTypeDefinition(scope, out var output))
            {
                return RootScope.ImplementationType(context, input, output);
            }
            throw new Exception("could not find ");
        }

        // this smells
        // this is not written like a try
        // it is just going to throw
        // I expect I will get rid of all my tires
        // when I try tacking errors
        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition) {
            typeDefinition = RootScope.ImplementationType(ContextType.GetTypeDefinitionOrThrow(scope), InputType.GetTypeDefinitionOrThrow(scope), OutputType.GetTypeDefinitionOrThrow(scope));
            return true;
        }
    }
}
