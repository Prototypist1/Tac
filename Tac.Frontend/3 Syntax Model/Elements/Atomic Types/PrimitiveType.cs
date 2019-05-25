﻿using Prototypist.LeftToRight;
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
    internal static class PrimitiveTypes
    {

        public static IConvertableFrontendType<IBlockType> CreateBlockType() {
            return new BlockType();
        }
        private struct BlockType : IConvertableFrontendType<IBlockType>
        {
            public IBuildIntention<IBlockType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
            }
        }

        public static IConvertableFrontendType<IStringType> CreateStringType()
        {
            return new StringType();
        }

        private struct StringType : IConvertableFrontendType<IStringType>
        {
            public IBuildIntention<IStringType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
            }
        }
        public static IConvertableFrontendType<IEmptyType> CreateEmptyType()
        {
            return new EmptyType();
        }

        private struct EmptyType : IConvertableFrontendType<IEmptyType>
        {
            public IBuildIntention<IEmptyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
            }
        }

        public static IConvertableFrontendType<INumberType> CreateNumberType()
        {
            return new NumberType();
        }
        private struct NumberType : IConvertableFrontendType<INumberType>
        {
            public IBuildIntention<INumberType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
            }
        }
        public static IGenericTypeParameterPlacholder CreateGenericTypeParameterPlacholder(IKey key)
        {
            return new GenericTypeParameterPlacholder(key);
        }

        internal interface IGenericTypeParameterPlacholder:  IConvertableFrontendType<IVerifiableType>{
            IKey Key { get; }
        }

        private struct GenericTypeParameterPlacholder : IGenericTypeParameterPlacholder
        {
            public GenericTypeParameterPlacholder(IKey key)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IKey Key { get; }

            public override bool Equals(object obj)
            {
                return obj is GenericTypeParameterPlacholder placholder &&
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
        public static IConvertableFrontendType<IAnyType> CreateAnyType()
        {
            return new AnyType();
        }

        private struct AnyType : IConvertableFrontendType<IAnyType>
        {
            public IBuildIntention<IAnyType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<IAnyType>(new Tac.Model.Instantiated.AnyType(), () => { });
            }
        }

        public static IConvertableFrontendType<IBooleanType> CreateBooleanType()
        {
            return new BooleanType();
        }
        private struct BooleanType : IConvertableFrontendType<IBooleanType>
        {
            public IBuildIntention<IBooleanType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
            }
        }
        public static IConvertableFrontendType<IImplementationType> CreateImplementationType(IConvertableFrontendType<IVerifiableType> inputType, IConvertableFrontendType<IVerifiableType> outputType, IConvertableFrontendType<IVerifiableType> contextType)
        {
            return new ImplementationType(inputType, outputType, contextType);
        }
        private struct ImplementationType : IConvertableFrontendType<IImplementationType>
        {
            public ImplementationType(IConvertableFrontendType<IVerifiableType> inputType, IConvertableFrontendType<IVerifiableType> outputType, IConvertableFrontendType<IVerifiableType> contextType)
            {
                InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
                OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
                ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            }

            public IConvertableFrontendType<IVerifiableType> InputType { get; }
            public IConvertableFrontendType<IVerifiableType> OutputType { get; }
            public IConvertableFrontendType<IVerifiableType> ContextType { get; }

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
        public static IConvertableFrontendType<IMethodType> CreateMethodType(IConvertableFrontendType<IVerifiableType> inputType, IConvertableFrontendType<IVerifiableType> outputType)
        {
            return new MethodType(inputType, outputType);
        }
        private struct MethodType : IConvertableFrontendType<IMethodType>
        {
            public MethodType(IConvertableFrontendType<IVerifiableType> inputType, IConvertableFrontendType<IVerifiableType> outputType)
            {
                InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
                OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            }

            public IConvertableFrontendType<IVerifiableType> InputType { get; }
            public IConvertableFrontendType<IVerifiableType> OutputType { get; }

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

        public static IGenericMethodType CreateGenericMethodType()
        {
            return new GenericMethodType(new GenericTypeParameterPlacholder(new NameKey("input")), new GenericTypeParameterPlacholder(new NameKey("output")));
        }

        internal interface IGenericMethodType : IConvertableFrontendType<Model.Elements.IGenericMethodType>, IFrontendGenericType { }

        private struct GenericMethodType : IGenericMethodType
        {
            private readonly IConvertableFrontendType<IVerifiableType> input;
            private readonly IConvertableFrontendType<IVerifiableType> output;



            public GenericMethodType(IConvertableFrontendType<IVerifiableType> input, IConvertableFrontendType<IVerifiableType> output)
            {
                this.input = input ?? throw new ArgumentNullException(nameof(input));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                TypeParameterDefinitions = new[] { input, output }.OfType<IGenericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
            }

            public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

            public IBuildIntention<Model.Elements.IGenericMethodType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                var (res, builder) = Tac.Model.Instantiated.GenericMethodType.Create();
                var myIn = input;
                var myOut = output;
                return new BuildIntention<Model.Elements.IGenericMethodType>(res, () => builder.Build(
                    myIn.Convert(context),
                    myOut.Convert(context)
                    ));
            }

            public OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
            {
                var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));

                if (typeParameters.All(x => !(x.frontendType is IGenericTypeParameterPlacholder)))
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new GenericMethodType(overlay.Convert(input), overlay.Convert(output)));
                }
                else
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new MethodType(overlay.Convert(input), overlay.Convert(output)));
                }

            }

            IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return GetBuildIntention(context);
            }
        }

        public static IGenericImplementationType CreateGenericImplementationType()
        {
            return new GenericImplementationType(new GenericTypeParameterPlacholder(new NameKey("input")), new GenericTypeParameterPlacholder(new NameKey("output")), new GenericTypeParameterPlacholder(new NameKey("context")));
        }

        internal interface IGenericImplementationType : IConvertableFrontendType<Tac.Model.Elements.IGenericImplementationType>, IFrontendGenericType { }

        private struct GenericImplementationType : IGenericImplementationType
        {
            private readonly IConvertableFrontendType<IVerifiableType> input;
            private readonly IConvertableFrontendType<IVerifiableType> output;
            private readonly IConvertableFrontendType<IVerifiableType> context;
            
            public GenericImplementationType(IConvertableFrontendType<IVerifiableType> input, IConvertableFrontendType<IVerifiableType> output, IConvertableFrontendType<IVerifiableType> context)
            {
                this.input = input ?? throw new ArgumentNullException(nameof(input));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
                this.context = context ?? throw new ArgumentNullException(nameof(context));
                TypeParameterDefinitions = new[] { input, output, context }.OfType<IGenericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
            }

            public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

            public IBuildIntention<Model.Elements.IGenericImplementationType> GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                var (toBuild, builder) = Tac.Model.Instantiated.GenericImplementationType.Create();
                var myContext = this.context;
                var myInput = input;
                var myOutput = output;
                return new BuildIntention<Model.Elements.IGenericImplementationType>(toBuild, () =>
                {
                    builder.Build(
                        myInput.Convert(context),
                        myOutput.Convert(context),
                        myContext.Convert(context));
                });
            }

            public OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
            {
                var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));

                if (typeParameters.All(x => !(x.frontendType is GenericTypeParameterPlacholder)))
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new GenericImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
                }
                else
                {
                    return new OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new ImplementationType(overlay.Convert(input), overlay.Convert(output), overlay.Convert(context)));
                }
            }

            IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(TransformerExtensions.ConversionContext context)
            {
                return GetBuildIntention(context);
            }
        }

        // I don't think I need this, WeakTypeOrOperation instead.

        //internal struct TypeOr : IFrontendType<ITypeOr>
        //{
        //    public readonly IFrontendType<ITypeOr> left, right;

        //    public TypeOr(IFrontendType<ITypeOr> left, IFrontendType<ITypeOr> right)
        //    {
        //        this.left = left ?? throw new ArgumentNullException(nameof(left));
        //        this.right = right ?? throw new ArgumentNullException(nameof(right));
        //    }

        //    public IBuildIntention<ITypeOr> GetBuildIntention(TransformerExtensions.ConversionContext context)
        //    {
        //        var (res, builder) = Tac.Model.Instantiated.TypeOr.Create();
        //        var myLeft = left;
        //        var myRIght = right;
        //        return new BuildIntention<Model.Elements.ITypeOr>(res, () => builder.Build(
        //            myLeft.Convert(context),
        //            myRIght.Convert(context)
        //            ));
        //    }
        //}
    }
}
