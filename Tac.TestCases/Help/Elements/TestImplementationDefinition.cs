using System;
using System.Collections.Generic;
using Tac.Model.Operations;

namespace Tac.Model.Elements
{
    public class TestImplementationDefinition : IImplementationDefinition
    {
        public TestImplementationDefinition(IVarifiableType outputType, IMemberDefinition contextDefinition, IMemberDefinition parameterDefinition, IFinalizedScope scope, IEnumerable<ICodeElement> methodBody, IEnumerable<ICodeElement> staticInitialzers)
        {
            OutputType = outputType;
            ContextDefinition = contextDefinition;
            ParameterDefinition = parameterDefinition;
            Scope = scope;
            MethodBody = methodBody;
            StaticInitialzers = staticInitialzers;
        }

        public IVarifiableType OutputType { get; set; }
        public IMemberDefinition ContextDefinition { get; set; }
        public IMemberDefinition ParameterDefinition { get; set; }
        public IFinalizedScope Scope { get; set; }
        public IEnumerable<ICodeElement> MethodBody { get; set; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ImplementationDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }
}
