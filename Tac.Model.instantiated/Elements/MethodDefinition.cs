using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{

    public class EntryPointDefinition: IEntryPointDefinition,IEntryPointDefinitionBuilder
    {
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableStaticInitailizers = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableBody = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();

        private EntryPointDefinition() { }
        public static (IEntryPointDefinition, IEntryPointDefinitionBuilder) Create()
        {
            var res = new EntryPointDefinition();
            return (res, res);
        }

        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public IReadOnlyList<ICodeElement> Body { get => buildableBody.Get(); }
        public IReadOnlyList<ICodeElement> StaticInitailizers { get => buildableStaticInitailizers.Get(); }


        public void Build(
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers)
        {
            buildableScope.Set(scope);
            buildableBody.Set(body);
            buildableStaticInitailizers.Set(staticInitailizers);
        }

        public static IEntryPointDefinition CreateAndBuild(
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers)
        {
            var (x, y) = Create();
            y.Build( scope, body, staticInitailizers);
            return x;
        }

        //public IVerifiableType VerifiableType() => new EntryPointType();

        public IVerifiableType Returns()
        {
            return new EmptyType();
        }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.EntryPoint(this);
        }

    }

    public interface IEntryPointDefinitionBuilder {

        void Build(
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers);
    }

    public class MethodDefinition : IInternalMethodDefinition,
        IMethodDefinitionBuilder
    {
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableStaticInitailizers = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IReadOnlyList<ICodeElement>> buildableBody = new Buildable<IReadOnlyList<ICodeElement>>();
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IVerifiableType> buildableInputType = new Buildable<IVerifiableType>();
        private readonly Buildable<IVerifiableType> buildableOutputType = new Buildable<IVerifiableType>();
        private readonly Buildable<IMemberDefinition> buildableParameterDefinition = new Buildable<IMemberDefinition>();

        private MethodDefinition() { }

        #region IMethodDefinition

        public IVerifiableType InputType => buildableInputType.Get();
        public IVerifiableType OutputType => buildableOutputType.Get();
        public IMemberDefinition ParameterDefinition => buildableParameterDefinition.Get();
        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public IReadOnlyList<ICodeElement> Body { get => buildableBody.Get(); }
        public IReadOnlyList<ICodeElement> StaticInitailizers { get => buildableStaticInitailizers.Get(); }


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MethodDefinition(this);
        }

        public IVerifiableType Returns()
        {
            return  MethodType.CreateAndBuild(InputType, OutputType);
        }
        
        #endregion
        
        public void Build(
            IVerifiableType inputType,
            IVerifiableType outputType,
            IMemberDefinition parameterDefinition,
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers)
        {
            buildableInputType.Set(inputType);
            buildableOutputType.Set(outputType);
            buildableParameterDefinition.Set(parameterDefinition);
            buildableScope.Set(scope);
            buildableBody.Set(body);
            buildableStaticInitailizers.Set(staticInitailizers);
        }

        public static (IInternalMethodDefinition, IMethodDefinitionBuilder) Create()
        {
            var res = new MethodDefinition();
            return (res, res);
        }

        public static IInternalMethodDefinition CreateAndBuild(
            IVerifiableType inputType,
            IVerifiableType outputType,
            IMemberDefinition parameterDefinition,
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers)
        {
            var (x, y) = Create();
            y.Build(inputType, outputType, parameterDefinition, scope, body, staticInitailizers);
            return x;
        }
    }

    public interface IMethodDefinitionBuilder
    {
        void Build(
            IVerifiableType inputType,
            IVerifiableType outputType,
            IMemberDefinition parameterDefinition,
            IFinalizedScope scope,
            IReadOnlyList<ICodeElement> body,
            IReadOnlyList<ICodeElement> staticInitailizers);
    }
}