using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ImplementationDefinition : IImplementationDefinition, IImplementationDefinitionBuilder
    {
        private readonly Buildable<ITypeReferance> buildableOutputType = new Buildable<ITypeReferance>();
        private readonly Buildable<IMemberDefinition> buildableContextDefinition = new Buildable<IMemberDefinition>();
        private readonly Buildable<IMemberDefinition> buildableParameterDefinition = new Buildable<IMemberDefinition>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IEnumerable<ICodeElement>> buildableMethodBody = new Buildable<IEnumerable<ICodeElement>>();
        private readonly Buildable<IEnumerable<ICodeElement>> buildableStaticInitialzers = new Buildable<IEnumerable<ICodeElement>>();

        private ImplementationDefinition()
        {
        }

        #region IImplementationDefinition
        
        public ITypeReferance OutputType { get => buildableOutputType.Get(); }
        public IMemberDefinition ContextDefinition { get => buildableContextDefinition.Get(); }
        public IMemberDefinition ParameterDefinition { get => buildableParameterDefinition.Get(); }
        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public IEnumerable<ICodeElement> MethodBody { get => buildableMethodBody.Get(); }
        public IEnumerable<ICodeElement> StaticInitialzers { get => buildableStaticInitialzers.Get(); }

        public IVarifiableType InputType => ParameterDefinition.Type;
        public IVarifiableType ContextType => ContextDefinition;
        IVarifiableType IImplementationType.OutputType => OutputType;

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ImplementationDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }

        #endregion
        
        public void Build(ITypeReferance outputType, IMemberDefinition contextDefinition, IMemberDefinition parameterDefinition, IFinalizedScope scope, IEnumerable<ICodeElement> methodBody, IEnumerable<ICodeElement> staticInitialzers) {
            buildableOutputType.Set(outputType);
            buildableContextDefinition.Set(contextDefinition);
            buildableParameterDefinition.Set(parameterDefinition);
            buildableMethodBody.Set(methodBody);
            buildableStaticInitialzers.Set(staticInitialzers);
        }
        
        public static (IImplementationDefinition, IImplementationDefinitionBuilder) Create()
        {
            var res = new ImplementationDefinition();
            return (res, res);
        }
    }

    public interface IImplementationDefinitionBuilder
    {
        void Build(ITypeReferance outputType, IMemberDefinition contextDefinition, IMemberDefinition parameterDefinition, IFinalizedScope scope, IEnumerable<ICodeElement> methodBody, IEnumerable<ICodeElement> staticInitialzers);
    }
}
