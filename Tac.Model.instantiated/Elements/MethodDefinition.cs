using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class MethodDefinition : IMethodDefinition,
        IMethodDefinitionBuilder
    {
        private readonly Buildable<IEnumerable<ICodeElement>> buildableStaticInitailizers = new Buildable<IEnumerable<ICodeElement>>();
        private readonly Buildable<ICodeElement[]> buildableBody = new Buildable<ICodeElement[]>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<ITypeReferance> buildableInputType = new Buildable<ITypeReferance>();
        private readonly Buildable<ITypeReferance> buildableOutputType = new Buildable<ITypeReferance>();
        private readonly Buildable<IMemberDefinition> buildableParameterDefinition = new Buildable<IMemberDefinition>();

        private MethodDefinition() { }

        #region IMethodDefinition

        public ITypeReferance InputType => buildableInputType.Get();
        public ITypeReferance OutputType => buildableOutputType.Get();
        public IMemberDefinition ParameterDefinition => buildableParameterDefinition.Get();
        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public ICodeElement[] Body { get => buildableBody.Get(); }
        public IEnumerable<ICodeElement> StaticInitailizers { get => buildableStaticInitailizers.Get(); }

        IVarifiableType IMethodType.InputType => InputType;
        IVarifiableType IMethodType.OutputType => OutputType;

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MethodDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
        
        #endregion
        
        public void Build(
    ITypeReferance inputType,
    ITypeReferance outputType,
    IMemberDefinition parameterDefinition,
    IFinalizedScope scope,
    ICodeElement[] body,
    IEnumerable<ICodeElement> staticInitailizers)
        {
            buildableInputType.Set(inputType);
            buildableOutputType.Set(outputType);
            buildableParameterDefinition.Set(parameterDefinition);
            buildableScope.Set(scope);
            buildableBody.Set(body);
            buildableStaticInitailizers.Set(staticInitailizers);
        }

        public static (IMethodDefinition, IMethodDefinitionBuilder) Create()
        {
            var res = new MethodDefinition();
            return (res, res);
        }

    }

    public interface IMethodDefinitionBuilder
    {
        void Build(
            ITypeReferance inputType,
            ITypeReferance outputType,
            IMemberDefinition parameterDefinition,
            IFinalizedScope scope,
            ICodeElement[] body,
            IEnumerable<ICodeElement> staticInitailizers);
    }
}