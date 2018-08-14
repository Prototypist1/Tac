using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public class MethodDefinition: AbstractBlockDefinition<MethodScope>
    {
        public MethodDefinition(TypeReferance outputType, ParameterDefinition parameterDefinition, CodeElement[] body) : base(new MethodScope(), body)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public TypeReferance InputType { get => ParameterDefinition.Type; }
        public TypeReferance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
    }
}