using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{


    internal class BlockType : IFrontendType<IBlockType>
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.BlockType(), () => { });
        }
    }
    internal class StringType : IFrontendType
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.StringType(),()=> { });
        }
    }
    internal class EmptyType : IFrontendType
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.EmptyType(), () => { });
        }
    }
    internal class NumberType : IFrontendType
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.NumberType(), () => { });
        }
    }
    internal class GemericTypeParameterPlacholder : IFrontendType
    {
        public GemericTypeParameterPlacholder(IKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }

        public override bool Equals(object obj)
        {
            return obj is GemericTypeParameterPlacholder placholder &&
                   EqualityComparer<IKey>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.GemericTypeParameterPlacholder(), () => { });
        }
    }
    internal class AnyType : IFrontendType
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.AnyType(), () => { });
        }
    }
    internal class BooleanType : IFrontendType
    {
        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IVarifiableType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }
    }
    internal class ImplementationType : IFrontendType
    {
        public ImplementationType(IFrontendType inputType, IFrontendType outputType, IFrontendType contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IFrontendType InputType { get; }
        public IFrontendType OutputType {get;}
        public IFrontendType ContextType {get;}

        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res,builder) = Tac.Model.Instantiated.ImplementationType.Create();

            return new BuildIntention<IVarifiableType>(res
                , () => {
                    builder.Build(
                        InputType.Convert(context),
                        OutputType.Convert(context),
                        ContextType.Convert(context)); });
        }
    }
    internal class MethodType : IFrontendType
    {
        public MethodType(IFrontendType inputType, IFrontendType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IFrontendType InputType {get;}
        public IFrontendType OutputType {get;}

        public IBuildIntention<IVarifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.MethodType.Create();

            return new BuildIntention<IVarifiableType>(res
                , () => {
                    builder.Build(
                        InputType.Convert(context),
                        OutputType.Convert(context));
                });
        }
    }

    internal class GenericMethodType : IFrontendType
    {

        private readonly IGenericTypeParameterDefinition input = new GenericTypeParameterDefinition("input");
        private readonly IGenericTypeParameterDefinition output = new GenericTypeParameterDefinition("output");

        public GenericMethodType()
        {
            TypeParameterDefinitions = new[] {input,output,};
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public IFrontendType GetConcreteType(Model.Elements.GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 2) {
                return new MethodType(
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type, 
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type);
            }
            throw new Exception("Exceptions important, why do you always half ass them?");
        }
        
    }

    internal class GenericImplementationType : IFrontendType
    {

        private readonly IGenericTypeParameterDefinition input = new GenericTypeParameterDefinition("input");
        private readonly IGenericTypeParameterDefinition output = new GenericTypeParameterDefinition("output");
        private readonly IGenericTypeParameterDefinition context = new GenericTypeParameterDefinition("context");

        public GenericImplementationType()
        {
            TypeParameterDefinitions = new[] { input, output, context };
        }

        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
        

        public IFrontendType GetConcreteType(Model.Elements.GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 3)
            {
                return new ImplementationType()
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(context)).Type);
            }

            throw new Exception("Exceptions important, why do you always half ass them?");
        }
    }
}
