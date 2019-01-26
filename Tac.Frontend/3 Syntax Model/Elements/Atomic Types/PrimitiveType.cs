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
    internal class GemericTypeParameterPlacholder : IFrontendType<IVerifiableType>
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

        public IBuildIntention<IVerifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res,maker)= Tac.Model.Instantiated.GemericTypeParameterPlacholder.Create();

            return new BuildIntention<IVerifiableType>(res, () => { maker.Build(Key); });
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
        public ImplementationType(IFrontendType<IVerifiableType> inputType, IFrontendType<IVerifiableType> outputType, IFrontendType<IVerifiableType> contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IFrontendType<IVerifiableType> InputType { get; }
        public IFrontendType<IVerifiableType> OutputType {get;}
        public IFrontendType<IVerifiableType> ContextType {get;}

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
        public MethodType(IFrontendType<IVerifiableType> inputType, IFrontendType<IVerifiableType> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IFrontendType<IVerifiableType> InputType {get;}
        public IFrontendType<IVerifiableType> OutputType {get;}

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

    //TODO
    // these should be overlays too!

    internal class GenericMethodType : IFrontendType<IGenericMethodType>, IFrontendGenericType
    {

        private readonly IFrontendType<IVerifiableType> input;
        private readonly IFrontendType<IVerifiableType> output;

        public GenericMethodType() : this(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")))
        {
        }

        public GenericMethodType(IFrontendType<IVerifiableType> input, IFrontendType<IVerifiableType> output)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            TypeParameterDefinitions = new[] { input, output}.OfType<GemericTypeParameterPlacholder>().Select(x=>Possibly.Is(x)).ToArray();
        }

        public IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IBuildIntention<IGenericMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.GenericMethodType.Create();

            return new BuildIntention<IGenericMethodType>(res, () => builder.Build(
                input.Convert(context),
                output.Convert(context)
                ));
        }
        
        public OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));
            if (typeParameters.All(x => !(x.frontendType is GemericTypeParameterPlacholder)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new GenericMethodType(overlay.Convert(input), overlay.Convert(output)));
            }
            else
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new MethodType(overlay.Convert(input), overlay.Convert(output)));
            }
        }

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    }

    internal class GenericImplementationType : IFrontendType<IGenericImplementationType>, IFrontendGenericType
    {

        private readonly IFrontendType<IVerifiableType> input;
        private readonly IFrontendType<IVerifiableType> output;
        private readonly IFrontendType<IVerifiableType> context;

        public GenericImplementationType() : this(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")), new GemericTypeParameterPlacholder(new NameKey("context")))
        {
        }

        public GenericImplementationType(IFrontendType<IVerifiableType> input, IFrontendType<IVerifiableType> output, IFrontendType<IVerifiableType> context)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            TypeParameterDefinitions = new[] { input, output, context }.OfType<GemericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
        }
        
        public IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IBuildIntention<IGenericImplementationType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, builder) = Tac.Model.Instantiated.GenericImplementationType.Create();
            return new BuildIntention<IGenericImplementationType>(toBuild, () => {
                builder.Build(input.Convert(context), output.Convert(context), this.context.Convert(context));
            });
        }

        public OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));
            if (typeParameters.All(x => !(x.frontendType is GemericTypeParameterPlacholder)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new GenericImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
            }
            else
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new ImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
            }
        }

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    }

    //internal class GenericTypeParameter {
    //    public GenericTypeParameter(IGenericTypeParameterDefinition parameter, IFrontendType<IVerifiableType> type)
    //    {
    //        Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
    //        Type = type ?? throw new ArgumentNullException(nameof(type));
    //    }

    //    public IGenericTypeParameterDefinition Parameter { get; }
    //    public IFrontendType<IVerifiableType> Type { get; }
    //}
}
