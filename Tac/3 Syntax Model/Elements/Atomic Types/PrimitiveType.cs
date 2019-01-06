using System;
using System.Collections.Generic;
using System.Linq;
using Prototypist.LeftToRight;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{


    internal class BlockType : IFrontendType<IBlockType>
    {
        public IBuildIntention<IBlockType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Tac.Model.Instantiated.BlockType(), () => { });
        }
    }
    internal class StringType : IFrontendType<IStringType>
    {
        public IBuildIntention<IStringType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IStringType>(new Tac.Model.Instantiated.StringType(),()=> { });
        }
    }
    internal class EmptyType : IFrontendType<IEmptyType>
    {
        public IBuildIntention<IEmptyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Tac.Model.Instantiated.EmptyType(), () => { });
        }
    }
    internal class NumberType : IFrontendType<INumberType>
    {
        public IBuildIntention<INumberType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<INumberType>(new Tac.Model.Instantiated.NumberType(), () => { });
        }
    }
    internal class GemericTypeParameterPlacholder : IFrontendType<IVarifiableType>
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
            var (res,maker)= Tac.Model.Instantiated.GemericTypeParameterPlacholder.Create();

            return new BuildIntention<IVarifiableType>(res, () => { maker.Build(Key); });
        }
    }
    internal class AnyType : IFrontendType<IAnyType>
    {
        public IBuildIntention<IAnyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IAnyType>(new Tac.Model.Instantiated.AnyType(), () => { });
        }
    }
    internal class BooleanType : IFrontendType<IBooleanType>
    {
        public IBuildIntention<IBooleanType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }
    }
    internal class ImplementationType : IFrontendType<IImplementationType>
    {
        public ImplementationType(IFrontendType<IVarifiableType> inputType, IFrontendType<IVarifiableType> outputType, IFrontendType<IVarifiableType> contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IFrontendType<IVarifiableType> InputType { get; }
        public IFrontendType<IVarifiableType> OutputType {get;}
        public IFrontendType<IVarifiableType> ContextType {get;}

        public IBuildIntention<IImplementationType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res,builder) = Tac.Model.Instantiated.ImplementationType.Create();

            return new BuildIntention<IImplementationType>(res
                , () => {
                    builder.Build(
                        InputType.Convert(context),
                        OutputType.Convert(context),
                        ContextType.Convert(context)); });
        }
    }
    internal class MethodType : IFrontendType<IMethodType>
    {
        public MethodType(IFrontendType<IVarifiableType> inputType, IFrontendType<IVarifiableType> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IFrontendType<IVarifiableType> InputType {get;}
        public IFrontendType<IVarifiableType> OutputType {get;}

        public IBuildIntention<IMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.MethodType.Create();

            return new BuildIntention<IMethodType>(res
                , () => {
                    builder.Build(
                        InputType.Convert(context),
                        OutputType.Convert(context));
                });
        }
    }

    // TODO 
    // this is not good either
    // get a not tired brain and think about it
    // 

    internal class GenericMethodType : IFrontendType<IGenericMethodType>, IFrontendGenericType
    {

        private readonly IFrontendType<IVarifiableType> input;
        private readonly IFrontendType<IVarifiableType> output;

        public GenericMethodType() : this(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")))
        {
        }

        public GenericMethodType(IFrontendType<IVarifiableType> input, IFrontendType<IVarifiableType> output)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            TypeParameterDefinitions = new[] { input, output}.OfType<GemericTypeParameterPlacholder>().Select(x=>Possibly.Is(x)).ToArray();
        }

        public IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IBuildIntention<IGenericMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.GenericMethodType.Create();

            return new BuildIntention<IGenericMethodType>(res, () => builder.Build());
        }

        public IFrontendType<IMethodType> GetConcreteType(GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 2) {
                return new MethodType(
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type, 
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type);
            }
            throw new Exception("Exceptions important, why do you always half ass them?");
        }

        public OrType<IFrontendGenericType, IFrontendType<IVarifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));
            if (typeParameters.All(x => !(x is IFrontendGenericType)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new GenericMethodType(overlay.Convert(input), overlay.Convert(output)));
            }
            else
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new MethodType(overlay.Convert(input), overlay.Convert(output)));
            }
        }

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, builder) = Tac.Model.Instantiated.GenericMethodType.Create();
            return new BuildIntention<IVarifiableType>(toBuild, () =>
            {
                builder.Build();
            });
        }
    }

    internal class GenericImplementationType : IFrontendType<IGenericImplementationType>, IFrontendGenericType
    {

        private readonly IFrontendType<IVarifiableType> input;
        private readonly IFrontendType<IVarifiableType> output;
        private readonly IFrontendType<IVarifiableType> context;

        public GenericImplementationType() : this(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")), new GemericTypeParameterPlacholder(new NameKey("context")))
        {
        }

        public GenericImplementationType(IFrontendType<IVarifiableType> input, IFrontendType<IVarifiableType> output, IFrontendType<IVarifiableType> context)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            TypeParameterDefinitions = new[] { input, output, context }.OfType<GemericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
        }
        
        public IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IBuildIntention<IGenericImplementationType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            throw new NotImplementedException();
        }

        public IFrontendType<IVarifiableType> GetConcreteType(GenericTypeParameter[] parameters)
        {
            if (parameters.Length == 3)
            {
                return new ImplementationType(
                    parameters.Single(x => x.Parameter.Key.Equals(input)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(output)).Type,
                    parameters.Single(x => x.Parameter.Key.Equals(context)).Type);
            }

            throw new Exception("Exceptions important, why do you always half ass them?");
        }

        public OrType<IFrontendGenericType, IFrontendType<IVarifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));
            if (typeParameters.All(x => !(x is IFrontendGenericType)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new GenericImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
            }
            else
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVarifiableType>>(new ImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
            }
        }

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, builder) = Tac.Model.Instantiated.GenericMethodType.Create();
            return new BuildIntention<IVarifiableType>(toBuild, () =>
            {
                builder.Build();
            });
        }
    }

    internal class GenericTypeParameter {
        public GenericTypeParameter(IGenericTypeParameterDefinition parameter, IFrontendType<IVarifiableType> type)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IGenericTypeParameterDefinition Parameter { get; }
        public IFrontendType<IVarifiableType> Type { get; }
    }
}
