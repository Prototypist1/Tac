using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ImplementationDefinition : IImplementationDefinition
    {
        public ImplementationDefinition(ITypeReferance outputType, IMemberDefinition contextDefinition, IMemberDefinition parameterDefinition, IFinalizedScope scope, IEnumerable<ICodeElement> methodBody, IEnumerable<ICodeElement> staticInitialzers)
        {
            OutputType = outputType;
            ContextDefinition = contextDefinition;
            ParameterDefinition = parameterDefinition;
            Scope = scope;
            MethodBody = methodBody;
            StaticInitialzers = staticInitialzers;
        }

        public ITypeReferance OutputType { get; set; }
        public IMemberDefinition ContextDefinition { get; set; }
        public IMemberDefinition ParameterDefinition { get; set; }
        public IFinalizedScope Scope { get; set; }
        public IEnumerable<ICodeElement> MethodBody { get; set; }
        public IEnumerable<ICodeElement> StaticInitialzers { get; set; }

        #region IImplementationDefinition

        public IVarifiableType InputType => ParameterDefinition.Type;
        public IVarifiableType ContextType => ContextDefinition;
        IVarifiableType IImplementationType.OutputType => OutputType;

        #endregion

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
