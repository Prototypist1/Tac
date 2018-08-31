using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public class MethodDefinition: AbstractBlockDefinition<MethodScope>
    {
        public MethodDefinition(Referance outputType, ParameterDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public Referance InputType { get => ParameterDefinition.Type; }
        public Referance OutputType { get; }
        public ParameterDefinition ParameterDefinition { get; }

        public override bool Equals(object obj)
        {
            return obj is MethodDefinition definition && definition != null &&
                   base.Equals(obj) &&
                   EqualityComparer<Referance>.Default.Equals(InputType, definition.InputType) &&
                   EqualityComparer<Referance>.Default.Equals(OutputType, definition.OutputType) &&
                   EqualityComparer<ParameterDefinition>.Default.Equals(ParameterDefinition, definition.ParameterDefinition);
        }

        public override int GetHashCode()
        {
            var hashCode = -814421114;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Referance>.Default.GetHashCode(InputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<Referance>.Default.GetHashCode(OutputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<ParameterDefinition>.Default.GetHashCode(ParameterDefinition);
            return hashCode;
        }

        public override ITypeDefinition ReturnType(IScope scope) {
            var inputRef = getOrThrow(InputType);
            var outputRef = getOrThrow(OutputType);

            return RootScope.MethodType(inputRef, outputRef);

            ITypeDefinition getOrThrow(Referance referance) {
                if (scope.TryGet(referance.key.names, out var res) && res is ITypeDefinition typeDefinition)
                {
                    return typeDefinition;
                }
                else {
                    throw new Exception("could not find type");
                }
            }
        }
    }
}