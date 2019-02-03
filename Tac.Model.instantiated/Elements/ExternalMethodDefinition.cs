//using System;
//using System.Collections.Generic;
//using Tac.Model.Elements;

//namespace Tac.Model.Instantiated
//{
//    public class ExternalMethodDefinition : IExternalMethodDefinition, IExternalMethodDefinitionBuilder
//    {

//        private ExternalMethodDefinition() { }

//        private readonly Buildable<IMemberDefinition> buildableParameterDefinition = new Buildable<IMemberDefinition>();
//        private readonly Buildable<IVerifiableType> buildableInputType = new Buildable<IVerifiableType>();
//        private readonly Buildable<IVerifiableType> buildableOutputType = new Buildable<IVerifiableType>();
//        private readonly BuildableValue<Guid> buildableId = new BuildableValue<Guid>();
        
//        public IMemberDefinition ParameterDefinition => buildableParameterDefinition.Get();
//        public IVerifiableType InputType => buildableInputType.Get();
//        public IVerifiableType OutputType => buildableOutputType.Get();
//        public Guid Id => buildableId.Get();

//        public void Build(ITypeReferance inputType, ITypeReferance outputType, IMemberDefinition parameterDefinition, Guid id)
//        {
//            buildableInputType.Set(inputType);
//            buildableOutputType.Set(outputType);
//            buildableParameterDefinition.Set(parameterDefinition);
//            buildableId.Set(id);
//        }

//        public static (IExternalMethodDefinition, IExternalMethodDefinitionBuilder) Create()
//        {
//            var res = new ExternalMethodDefinition();
//            return (res, res);
//        }

//        public static IExternalMethodDefinition CreateAndBuild(
//            ITypeReferance inputType,
//            ITypeReferance outputType,
//            IMemberDefinition parameterDefinition, 
//            Guid id)
//        {
//            var (x, y) = Create();
//            y.Build(inputType, outputType, parameterDefinition, id);
//            return x;
//        }

//        public IVerifiableType Returns()
//        {
//            return this;
//        }

//        public T Convert<T>(IOpenBoxesContext<T> context)
//        {
//            return context.ExternalMethodDefinition(this);
//        }
//    }

//    public interface IExternalMethodDefinitionBuilder
//    {
//        void Build(
//            ITypeReferance inputType,
//            ITypeReferance outputType,
//            IMemberDefinition parameterDefinition, Guid id);
//    }
//}