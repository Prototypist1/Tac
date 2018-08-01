using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ImplementationDefinition : IReferanced<ImplementationName>
    {
        public ImplementationName Key { get; }

        public Referance<TypeDefinition> ContextType { get; }
        public Referance<TypeDefinition> InputType { get => ParameterDefinition.Type; }
        public Referance<TypeDefinition> OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
        public MethodBodyDefinition MethodBodyDefinition { get; }

    }
}
