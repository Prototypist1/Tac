using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ImplementationDefinition: IScoped<InstanceScope>, IReferanced
    {
        public ImplementationDefinition(TypeReferance contextType, TypeReferance outputType, ParameterDefinition parameterDefinition, AbstractBlockDefinition<InstanceScope> methodBodyDefinition, AbstractName key)
        {
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            MethodBodyDefinition = methodBodyDefinition ?? throw new ArgumentNullException(nameof(methodBodyDefinition));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public TypeReferance ContextType { get; }
        public TypeReferance InputType { get => ParameterDefinition.Type; }
        public TypeReferance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
        public AbstractBlockDefinition<InstanceScope> MethodBodyDefinition { get; }

        public AbstractName Key { get; }

        public InstanceScope Scope => ((IScoped<InstanceScope>)MethodBodyDefinition).Scope;

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
