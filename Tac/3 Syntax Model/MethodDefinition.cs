using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public class MethodDefinition: AbstractBlockDefinition<MethodScope>
    {
        public MethodDefinition(TypeReferance outputType, ParameterDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public TypeReferance InputType { get => ParameterDefinition.Type; }
        public TypeReferance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }

        public override bool Equals(object obj)
        {
            return obj is MethodDefinition definition && definition != null &&
                   base.Equals(obj) &&
                   EqualityComparer<TypeReferance>.Default.Equals(InputType, definition.InputType) &&
                   EqualityComparer<TypeReferance>.Default.Equals(OutputType, definition.OutputType) &&
                   EqualityComparer<ParameterDefinition>.Default.Equals(ParameterDefinition, definition.ParameterDefinition);
        }

        public override int GetHashCode()
        {
            var hashCode = -814421114;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TypeReferance>.Default.GetHashCode(InputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<TypeReferance>.Default.GetHashCode(OutputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<ParameterDefinition>.Default.GetHashCode(ParameterDefinition);
            return hashCode;
        }

        public override TypeReferance ReturnType() => OutputType;
    }
}