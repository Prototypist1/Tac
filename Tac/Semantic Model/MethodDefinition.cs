using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public class MethodDefinition
    {
        public Referance<TypeDefinition> InputType { get; }
        public Referance<TypeDefinition> OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }
        public MethodBodyDefinition MethodBodyDefinition { get; }
    }
}
