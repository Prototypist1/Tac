using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ImplementationDefinition: IScoped<InstanceScope>, IReferanced, ITypeSource, ITypeDefinition<InstanceScope>
    {
        public ImplementationDefinition(ITypeSource contextType, ITypeSource outputType, MemberDefinition parameterDefinition, AbstractBlockDefinition<InstanceScope> methodBodyDefinition, AbstractName key)
        {
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBodyDefinition = methodBodyDefinition ?? throw new ArgumentNullException(nameof(methodBodyDefinition));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        // dang! these could also be inline definitions 
        public ITypeSource ContextType { get; }
        public ITypeSource InputType { get => ParameterDefinition.Type; }
        public ITypeSource OutputType { get; }
        public MemberDefinition ParameterDefinition { get; }
        public AbstractBlockDefinition<InstanceScope> MethodBodyDefinition { get; }

        public AbstractName Key { get; }

        public InstanceScope Scope => ((IScoped<InstanceScope>)MethodBodyDefinition).Scope;

        public override bool Equals(object obj)
        {
            return obj is ImplementationDefinition implementation &&
                ContextType.Equals(implementation.ContextType) &&
                OutputType.Equals(implementation.OutputType) &&
                ParameterDefinition.Equals(implementation.ParameterDefinition) &&
                MethodBodyDefinition.Equals(implementation.MethodBodyDefinition);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ContextType.GetHashCode() +
                    OutputType.GetHashCode() +
                    ParameterDefinition.GetHashCode() +
                    MethodBodyDefinition.GetHashCode();
            }
        }

        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) {
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
        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition<IScope> typeDefinition) {
            typeDefinition = RootScope.ImplementationType(ContextType.GetTypeDefinitionOrThrow(scope), InputType.GetTypeDefinitionOrThrow(scope), OutputType.GetTypeDefinitionOrThrow(scope));
            return true;
        }
    }
}
