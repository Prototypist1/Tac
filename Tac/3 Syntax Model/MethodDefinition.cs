using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class MethodDefinition : AbstractBlockDefinition,  ITypeDefinition
    {
        public MethodDefinition(ITypeDefinition outputType, MemberDefinition parameterDefinition, ICodeElement[] body, MethodScope scope, IEnumerable<ICodeElement> staticInitializers) : base(scope ?? throw new ArgumentNullException(nameof(scope)), body, staticInitializers)
        {
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        public ITypeDefinition InputType
        {
            get
            {
                return ParameterDefinition.Type;
            }
        }
        public ITypeDefinition OutputType { get; }
        public MemberDefinition ParameterDefinition { get; }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
        }

        public ITypeDefinition GetTypeDefinition(ScopeStack scope)
        {
            return scope.GetGenericType(new GenericExplicitTypeName(RootScope.MethodType.Name, InputType, OutputType));
        }
    }
}