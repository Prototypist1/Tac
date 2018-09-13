using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class MethodDefinition: AbstractBlockDefinition, ITypeSource, ITypeDefinition
    {
        public MethodDefinition(ITypeSource outputType, MemberDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public ITypeSource InputType { get => ParameterDefinition.Type; }
        public ITypeSource OutputType { get; }
        public MemberDefinition ParameterDefinition { get; }

        public override bool Equals(object obj)
        {
            return obj is MethodDefinition definition && definition != null &&
                   base.Equals(obj) &&
                   EqualityComparer<ITypeSource>.Default.Equals(InputType, definition.InputType) &&
                   EqualityComparer<ITypeSource>.Default.Equals(OutputType, definition.OutputType) &&
                   EqualityComparer<MemberDefinition>.Default.Equals(ParameterDefinition, definition.ParameterDefinition);
        }

        public override int GetHashCode()
        {
            var hashCode = -814421114;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(InputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(OutputType);
            hashCode = hashCode * -1521134295 + EqualityComparer<MemberDefinition>.Default.GetHashCode(ParameterDefinition);
            return hashCode;
        }

        public override ITypeDefinition ReturnType(ScopeStack scope) {
            return scope.GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name,  InputType, OutputType ));
        }

        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition)
        {
            typeDefinition = scope.GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
            return true;
        }
    }
}