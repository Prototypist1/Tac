using Prototypist.LeftToRight;
using Prototypist.TaskChain.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{
    internal struct BlockType : IFrontendType<IBlockType>
    {
        public IBuildIntention<IBlockType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }
    }
    internal struct StringType : IFrontendType<IStringType>
    {
        public IBuildIntention<IStringType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
        }
    }
    internal struct EmptyType : IFrontendType<IEmptyType>
    {
        public IBuildIntention<IEmptyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
        }
    }
    internal struct NumberType : IFrontendType<INumberType>
    {
        public IBuildIntention<INumberType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
        }
    }
    internal struct GemericTypeParameterPlacholder : IFrontendType<IVerifiableType>
    {
        public GemericTypeParameterPlacholder(IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
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
            var (res, maker) = Tac.Model.Instantiated.GemericTypeParameterPlacholder.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var key = Key;
            return new BuildIntention<IVerifiableType>(res, () => { maker.Build(key); });
        }
    }
    internal struct AnyType : IFrontendType<IAnyType>
    {
        public IBuildIntention<IAnyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IAnyType>(new Tac.Model.Instantiated.AnyType(), () => { });
        }
    }
    internal struct BooleanType : IFrontendType<IBooleanType>
    {
        public IBuildIntention<IBooleanType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }
    }
    internal struct ImplementationType : IFrontendType<IImplementationType>
    {
        public ImplementationType(IFrontendType<IVerifiableType> inputType, IFrontendType<IVerifiableType> outputType, IFrontendType<IVerifiableType> contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IFrontendType<IVerifiableType> InputType { get; }
        public IFrontendType<IVerifiableType> OutputType { get; }
        public IFrontendType<IVerifiableType> ContextType { get; }

        public IBuildIntention<IImplementationType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Model.Instantiated.ImplementationType.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var inputType = InputType;
            var outputType = OutputType;
            var contextType = ContextType;
            return new BuildIntention<IImplementationType>(res
                , () =>
                {
                    builder.Build(
                        inputType.Convert(context),
                        outputType.Convert(context),
                        contextType.Convert(context));
                });
        }
    }

    internal struct MethodType : IFrontendType<IMethodType>
    {
        public MethodType(IFrontendType<IVerifiableType> inputType, IFrontendType<IVerifiableType> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IFrontendType<IVerifiableType> InputType { get; }
        public IFrontendType<IVerifiableType> OutputType { get; }

        public IBuildIntention<IMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.MethodType.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var inputType = InputType;
            var outputType = OutputType;
            return new BuildIntention<IMethodType>(res
                , () =>
                {
                    builder.Build(
                        inputType.Convert(context),
                        outputType.Convert(context));
                });
        }
    }

    // TODO TODO TODOD
    // do these need create and build?
    // I don't really think they do
    // if I can pull that out they can be structs as well
    
    internal struct GenericMethodType : IFrontendType<IGenericMethodType>, IFrontendGenericType
    {
        private readonly IFrontendType<IVerifiableType> input;
        private readonly IFrontendType<IVerifiableType> output;

        public static GenericMethodType Create()
        {
            return new GenericMethodType(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")));
        }

        public GenericMethodType(IFrontendType<IVerifiableType> input, IFrontendType<IVerifiableType> output)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            TypeParameterDefinitions = new[] { input, output }.OfType<GemericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
        }

        public IIsPossibly<GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IBuildIntention<IGenericMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.GenericMethodType.Create();
            var myIn = input;
            var myOut = output;
            return new BuildIntention<IGenericMethodType>(res, () => builder.Build(
                myIn.Convert(context),
                myOut.Convert(context)
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

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return GetBuildIntention(context);
        }
    }

    internal struct GenericImplementationType : IFrontendType<IGenericImplementationType>, IFrontendGenericType
    {
        private readonly IFrontendType<IVerifiableType> input;
        private readonly IFrontendType<IVerifiableType> output;
        private readonly IFrontendType<IVerifiableType> context;

        public static GenericImplementationType Create()
        {
            return new GenericImplementationType(new GemericTypeParameterPlacholder(new NameKey("input")), new GemericTypeParameterPlacholder(new NameKey("output")), new GemericTypeParameterPlacholder(new NameKey("context")));
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
            var myContext = this.context;
            var myInput = input;
            var myOutput = output;
            return new BuildIntention<IGenericImplementationType>(toBuild, () =>
            {
                builder.Build(
                    myInput.Convert(context),
                    myOutput.Convert(context),
                    myContext.Convert(context));
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

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return GetBuildIntention(context);
        }
    }
}
