﻿using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;

namespace Tac.SyntaxModel.Elements.AtomicTypes
{
    internal static class FrontendTypeExtensions{
        public static IFrontendType UnwrapRefrence(this IFrontendType frontendType) 
        {
            if (frontendType is RefType refType) {
                return refType.inner;
            }
            return frontendType;
        }
    }

    internal interface IPrimitiveType: IFrontendType
    {
    }

    // reference is a type! it 
    // but it probably does not mean what you think it means
    // it really means you can assign to it
    // this is what is returned by member and member reference 
    internal class RefType : IConvertableFrontendType<IReferanceType>, IPrimitiveType {
        public IBuildIntention<IBlockType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }

        public IEnumerable<IError> Validate() => Array.Empty<IError>();

        public readonly IFrontendType inner;
    }

    internal struct BlockType : IConvertableFrontendType<IBlockType>, IPrimitiveType
    {
        public IBuildIntention<IBlockType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal struct StringType : IConvertableFrontendType<IStringType>, IPrimitiveType
    {
        public IBuildIntention<IStringType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
        }
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }
    internal struct EmptyType : IConvertableFrontendType<IEmptyType>, IPrimitiveType
    {
        public IBuildIntention<IEmptyType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
        }
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal struct NumberType : IConvertableFrontendType<INumberType>, IPrimitiveType
    {
        public IBuildIntention<INumberType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
        }
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal interface IGenericTypeParameterPlacholder : IFrontendType
    {
        IOrType<NameKey, ImplicitKey> Key { get; }
    }

    internal struct GenericTypeParameterPlacholder : IGenericTypeParameterPlacholder, IEquatable<GenericTypeParameterPlacholder>
    {
        public GenericTypeParameterPlacholder(IOrType<NameKey, ImplicitKey> key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOrType<NameKey, ImplicitKey> Key { get; }

        public override bool Equals(object? obj)
        {
            return obj is GenericTypeParameterPlacholder placholder && Equals(placholder);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext _)
        {
            var (res, maker) = Model.Instantiated.GemericTypeParameterPlacholder.Create();

            // this is stack allocated and might be GC'ed so we need to create locals
            // to feed to the lambda
            var key = Key;
            return new BuildIntention<IVerifiableType>(res, () => { maker.Build(key); });
        }

        public bool Equals(GenericTypeParameterPlacholder placholder)
        {
            return EqualityComparer<IOrType<NameKey, ImplicitKey>>.Default.Equals(Key, placholder.Key);
        }

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal struct AnyType : IConvertableFrontendType<IAnyType>, IPrimitiveType
    {
        public IBuildIntention<IAnyType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IAnyType>(new Model.Instantiated.AnyType(), () => { });
        }

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal struct BooleanType : IConvertableFrontendType<IBooleanType>, IPrimitiveType
    {
        public IBuildIntention<IBooleanType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }
    internal struct ImplementationType : IConvertableFrontendType<IImplementationType>, IPrimitiveType
    {
        public ImplementationType(IOrType<IConvertableFrontendType<IVerifiableType>, IError> inputType, IOrType<IConvertableFrontendType<IVerifiableType>, IError> outputType, IOrType<IConvertableFrontendType<IVerifiableType>, IError> contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IOrType<IConvertableFrontendType<IVerifiableType>,IError> InputType { get; }
        public IOrType<IConvertableFrontendType<IVerifiableType>, IError> OutputType { get; }
        public IOrType<IConvertableFrontendType<IVerifiableType>, IError> ContextType { get; }

        public IBuildIntention<IImplementationType> GetBuildIntention(IConversionContext context)
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
                        inputType.Is1OrThrow().Convert(context),
                        outputType.Is1OrThrow().Convert(context),
                        contextType.Is1OrThrow().Convert(context));
                });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in InputType.SwitchReturns(x => x.Validate(), x => new[]{x})) 
            {
                yield return error;
            }
            foreach (var error in OutputType.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
            foreach (var error in ContextType.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }
    }
    internal class MethodType : IConvertableFrontendType<IMethodType>, IPrimitiveType
    {
        public MethodType(IOrType<IConvertableFrontendType<IVerifiableType>, IError> inputType, IOrType<IConvertableFrontendType<IVerifiableType>, IError> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IOrType< IConvertableFrontendType<IVerifiableType>,IError> InputType { get; }
        public IOrType<IConvertableFrontendType<IVerifiableType>, IError> OutputType { get; }

        public IBuildIntention<IMethodType> GetBuildIntention(IConversionContext context)
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
                        inputType.Is1OrThrow().Convert(context),
                        outputType.Is1OrThrow().Convert(context));
                });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in InputType.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
            foreach (var error in OutputType.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }
    }

    // uhhh do I still need these?? (GenericMethodType, GenericImplementationType, IGenericMethodType, IGenericImplementationType)
    // I hope not.

    internal interface IGenericMethodType : IFrontendType, IFrontendGenericType { }

    internal struct GenericMethodType : IGenericMethodType, IPrimitiveType
    {
        private readonly IFrontendType input;
        private readonly IFrontendType output;

        public GenericMethodType(IFrontendType input, IFrontendType output)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            TypeParameterDefinitions = new[] { input, output }.OfType<IGenericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
        }

        public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in input.Validate())
            {
                yield return error;
            }
            foreach (var error in output.Validate())
            {
                yield return error;
            }
        }
    }


    internal interface IGenericImplementationType : IFrontendType, IFrontendGenericType { }

    internal struct GenericImplementationType : IGenericImplementationType, IPrimitiveType
    {
        private readonly IFrontendType input;
        private readonly IFrontendType output;
        private readonly IFrontendType context;

        public GenericImplementationType(IFrontendType input, IFrontendType output, IFrontendType context)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            TypeParameterDefinitions = new[] { input, output, context }.OfType<IGenericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
        }

        public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }


        public IEnumerable<IError> Validate()
        {
            foreach (var error in input.Validate())
            {
                yield return error;
            }
            foreach (var error in output.Validate())
            {
                yield return error;
            }
            foreach (var error in context.Validate())
            {
                yield return error;
            }
        }
        //public IOrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        //{
        //    var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));

        //    var overlayedInput = overlay.Convert(input);
        //    var overlayedOut = overlay.Convert(output);
        //    var overlayedContext = overlay.Convert(context);

        //    if (overlayedInput is IConvertableFrontendType<IVerifiableType> convertableInput && overlayedOut is IConvertableFrontendType<IVerifiableType> convertableOut && overlayedContext is IConvertableFrontendType<IVerifiableType> convertableContext) {
        //        return OrType.Make<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new ImplementationType(convertableInput, convertableOut, convertableContext));
        //    }

        //    return OrType.Make<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new GenericImplementationType(overlayedInput, overlayedOut, overlayedContext));

        //}


        // I don't think I need this, WeakTypeOrOperation instead.

        //internal struct TypeOr : IFrontendType<ITypeOr>
        //{
        //    public readonly IFrontendType<ITypeOr> left, right;

        //    public TypeOr(IFrontendType<ITypeOr> left, IFrontendType<ITypeOr> right)
        //    {
        //        this.left = left ?? throw new ArgumentNullException(nameof(left));
        //        this.right = right ?? throw new ArgumentNullException(nameof(right));
        //    }

        //    public IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
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
