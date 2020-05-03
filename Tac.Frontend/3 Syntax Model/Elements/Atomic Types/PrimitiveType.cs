using Prototypist.Toolbox;
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
        public static IOrType<IFrontendType,IError> UnwrapRefrence(this IFrontendType frontendType) 
        {
            if (frontendType is RefType refType) {
                return refType.inner;
            }
            return OrType.Make< IFrontendType,IError >(frontendType);
        }
    }

    internal interface IPrimitiveType: IFrontendType
    {
    }

    internal class FrontEndOrType : IConvertableFrontendType<ITypeOr>
    {
        private IConvertableFrontendType<ITypeOr> left, right;

        public IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsAssignableTo(IFrontendType frontendType)
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key)
        {
            var leftMember = left.TryGetMember(key);
            var rightMember = right.TryGetMember(key);

            // "is" is not safe!
            // sronger it!
            if (leftMember is IIsDefinately<IOrType<IFrontendType, IError>> definateLeft && rightMember is IIsDefinately<IOrType<IFrontendType, IError>> definateRight) {

                // if either is an error pass that up
                // otherwise we pass the less general

                return Possibly.Is(definateLeft.Value.SwitchReturns(
                    x => definateRight.Value.SwitchReturns<IOrType<IFrontendType, IError>>(
                        y => {
                            if (x.IsAssignableTo(y) && y.IsAssignableTo(x)) {
                                // they are the same, pick one
                                return OrType.Make<IFrontendType, IError>(x);
                            }
                            if (x.IsAssignableTo(y) )
                            {
                                // x is less general
                                return OrType.Make<IFrontendType, IError>(x);
                            }
                            if (y.IsAssignableTo(x))
                            {
                                // y is less general
                                return OrType.Make<IFrontendType, IError>(y);
                            }
                            throw new Exception("these types are totally different!");
                        }, 
                        y =>  OrType.Make<IFrontendType, IError>(y)) , 
                    x => definateRight.Value.SwitchReturns<IOrType<IFrontendType, IError>>(
                        y => OrType.Make<IFrontendType, IError>(x), 
                        y => OrType.Make<IFrontendType, IError>(Error.Cascaded("errors all over!",new[] { x, y })))));

            }
            return Possibly.IsNot<IOrType<IFrontendType, IError>>();
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var item in left.Validate())
            {
                yield return item;
            }
            foreach (var item in right.Validate())
            {
                yield return item;
            }
        }
    }

    internal class HasMembersType : IConvertableFrontendType<IInterfaceModuleType> {

        // for now the members all need to be the same type
        // say A is { Human thing; } and B is { Animal thing; }
        // B := A
        // B.thing := () > Method [Empty, Chicken] { .... }
        // breaks A, A.thing is a chicken
        // 
        // now 
        // A := B
        // B.thing := () > Method [Empty, Chicken] { .... }
        // agains breaks A, A.thing is a chicken

        // this is going to be more interesting when things can be read-only 
        // I think readonly means: you never get to changes this but it can change
        // I probably need another work for: this never changes

        // if B is { readonly Animal thing; } then
        // B := A
        // is ok, humnas are always animals and you can't set on be to breaak things
        // A := B is still not since B.thing might not be a Human
        public bool IsAssignableTo(IFrontendType frontendType) {
            
            foreach (var member in weakScope.membersList.Select(x=>x.GetValue()))
            {
                var theirMember = frontendType.TryGetMember(member.Key);
                if (!member.Type.Is1(out var ourTypeBox)) {
                    continue;
                }
                if (!(theirMember is IIsDefinately<IOrType<IFrontendType, IError>> definately)) {
                    return false;
                }
                if (!definately.Value.Is1(out var theirType))
                {
                    continue;
                }
                var ourType = ourTypeBox.GetValue();
                if (!(theirType.IsAssignableTo(ourType) && ourType.IsAssignableTo(theirType))){
                    return false;
                }
            }
            return true;
        }
        
        public IEnumerable<IError> Validate() => weakScope.membersList.Select(x => x.GetValue().Type.Possibly1()).OfType<IIsDefinately<IFrontendType>>().SelectMany(x => x.Value.Validate());

        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key)
        {
            var matches = weakScope.membersList.Where(x => x.GetValue().Key == key);
            if (matches.Count() == 1)
            {
                return Possibly.Is(matches.First().GetValue().Type.TransformInner(x=>x.GetValue()));
            }
            else { 
                return Possibly.IsNot<IOrType<IFrontendType, IError>>();
            }
        }

        public IBuildIntention<IInterfaceModuleType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(weakScope.Convert(context).Members.Values.Select(x => x.Value).ToArray());
            });
        }

        //public readonly IReadOnlyDictionary<IKey, IOrType<IFrontendType, IError>> members;

        private readonly WeakScope weakScope;

        public HasMembersType(WeakScope weakScope)
        {
            this.weakScope = weakScope ?? throw new ArgumentNullException(nameof(weakScope));
        }
    }

    // still bad at structs is this a struct?
    //internal struct HowTypesThinkOfMembers {
    //    public IOrType<IFrontendType, IError> orType;
    //    public IKey key;
    //}

    // reference is a type!
    // but it probably does not mean what you think it means
    // it really means you can assign to it
    // this is what is returned by member and member reference 
    internal class RefType : IConvertableFrontendType<IReferanceType>, IPrimitiveType {
        public IBuildIntention<IReferanceType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IReferanceType>(new Model.Instantiated.ReferanceType(), () => { });
        }

        public IEnumerable<IError> Validate() => inner.SwitchReturns(x=>x.Validate(), x=>Array.Empty<IError>());

        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();

        public bool IsAssignableTo(IFrontendType frontendType)
        {
            // the method calling this
            // is in charge of unwrapping
            throw new Exception("I don't think this should ever happen");

            // this is a bit of a smell
        }

        public readonly IOrType< IFrontendType,IError> inner;

        public RefType(IOrType<IFrontendType, IError> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }

    internal struct BlockType : IConvertableFrontendType<IBlockType>, IPrimitiveType
    {
        public IBuildIntention<IBlockType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }

        public bool IsAssignableTo(IFrontendType frontendType) => frontendType is BlockType;

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
    }

    internal struct StringType : IConvertableFrontendType<IStringType>, IPrimitiveType
    {
        public IBuildIntention<IStringType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
        }

        public bool IsAssignableTo(IFrontendType frontendType) => frontendType is StringType;
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }
    internal struct EmptyType : IConvertableFrontendType<IEmptyType>, IPrimitiveType
    {
        public IBuildIntention<IEmptyType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
        }
        public bool IsAssignableTo(IFrontendType frontendType) => frontendType is EmptyType;
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal struct NumberType : IConvertableFrontendType<INumberType>, IPrimitiveType
    {
        public IBuildIntention<INumberType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
        }
        public bool IsAssignableTo(IFrontendType frontendType) => frontendType is NumberType;
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    // I don't think this is a type....
    // placeholders are effectively defined by constraints really just metadata
    internal interface IGenericTypeParameterPlacholder //: IFrontendType
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

        public bool IsAssignableTo(IFrontendType frontendType) => true;
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
    }

    internal struct BooleanType : IConvertableFrontendType<IBooleanType>, IPrimitiveType
    {
        public IBuildIntention<IBooleanType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }

        public bool IsAssignableTo(IFrontendType frontendType) => frontendType is BooleanType;

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
    }

    // this so is a method....
    //internal struct ImplementationType : IConvertableFrontendType<IImplementationType>, IPrimitiveType
    //{
    //    public ImplementationType(IOrType<IConvertableFrontendType<IVerifiableType>, IError> inputType, IOrType<IConvertableFrontendType<IVerifiableType>, IError> outputType, IOrType<IConvertableFrontendType<IVerifiableType>, IError> contextType)
    //    {
    //        InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
    //        OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
    //        ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
    //    }

    //    public IOrType<IConvertableFrontendType<IVerifiableType>,IError> InputType { get; }
    //    public IOrType<IConvertableFrontendType<IVerifiableType>, IError> OutputType { get; }
    //    public IOrType<IConvertableFrontendType<IVerifiableType>, IError> ContextType { get; }

    //    public IBuildIntention<IImplementationType> GetBuildIntention(IConversionContext context)
    //    {
    //        var (res, builder) = Model.Instantiated.ImplementationType.Create();

    //        // this is stack allocated and might be GC'ed so we need to create locals
    //        // to feed to the lambda
    //        var inputType = InputType;
    //        var outputType = OutputType;
    //        var contextType = ContextType;
    //        return new BuildIntention<IImplementationType>(res
    //            , () =>
    //            {
    //                builder.Build(
    //                    inputType.Is1OrThrow().Convert(context),
    //                    outputType.Is1OrThrow().Convert(context),
    //                    contextType.Is1OrThrow().Convert(context));
    //            });
    //    }

    //    public IEnumerable<IError> Validate()
    //    {
    //        foreach (var error in InputType.SwitchReturns(x => x.Validate(), x => new[]{x})) 
    //        {
    //            yield return error;
    //        }
    //        foreach (var error in OutputType.SwitchReturns(x => x.Validate(), x => new[] { x }))
    //        {
    //            yield return error;
    //        }
    //        foreach (var error in ContextType.SwitchReturns(x => x.Validate(), x => new[] { x }))
    //        {
    //            yield return error;
    //        }
    //    }
    //}
    internal class MethodType : IConvertableFrontendType<IMethodType>, IPrimitiveType
    {
        // is the meta-data here worth capturing
        public static MethodType ImplementationType(
            IOrType<IFrontendType, IError> inputType, 
            IOrType<IFrontendType, IError> outputType, 
            IOrType<IFrontendType, IError> contextType) {
            return new MethodType(
                contextType,
                OrType.Make<IConvertableFrontendType<IVerifiableType>, IError> (new MethodType(inputType, outputType)));
        }

        public MethodType(
            IOrType<IFrontendType, IError> inputType, 
            IOrType<IFrontendType, IError> outputType)
        {

            InputType = inputType?.CastToOr<IFrontendType,IConvertableFrontendType<IVerifiableType>>(Error.Other("")) ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType?.CastToOr<IFrontendType, IConvertableFrontendType<IVerifiableType>>(Error.Other("")) ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IOrType<IConvertableFrontendType<IVerifiableType>, IError> InputType { get; }
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

        public bool IsAssignableTo(IFrontendType frontendType)
        {
            if (!(frontendType is MethodType methodType)) {
                return false;
            }

            // thier input must be assignable to our input
            // if they take a Human and we take an Animal that is fine
            // if they take a Animal and we take a Human that is not fine
            if (methodType.InputType.Is1(out var inThat) && InputType.Is1(out var inThis) && !inThat.IsAssignableTo(inThis)) {
                return false;
            }

            // our output must be assignable to their output
            // if they return a Human and we return an Animal that is not fine
            // if they return a Animal and we return a Human that is fine
            if (methodType.OutputType.Is1(out var outThat) && OutputType.Is1(out var outThis) && !outThis.IsAssignableTo(outThat))
            {
                return false;
            }

            return true;
        }
        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
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

        public bool IsAssignableTo(IFrontendType frontendType)
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IOrType<IFrontendType, IError>> TryGetMember(IKey key) => Possibly.IsNot<IOrType<IFrontendType, IError>>();
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


    //internal interface IGenericImplementationType : IFrontendType, IFrontendGenericType { }

    //internal struct GenericImplementationType : IGenericImplementationType, IPrimitiveType
    //{
    //    private readonly IFrontendType input;
    //    private readonly IFrontendType output;
    //    private readonly IFrontendType context;

    //    public GenericImplementationType(IFrontendType input, IFrontendType output, IFrontendType context)
    //    {
    //        this.input = input ?? throw new ArgumentNullException(nameof(input));
    //        this.output = output ?? throw new ArgumentNullException(nameof(output));
    //        this.context = context ?? throw new ArgumentNullException(nameof(context));
    //        TypeParameterDefinitions = new[] { input, output, context }.OfType<IGenericTypeParameterPlacholder>().Select(x => Possibly.Is(x)).ToArray();
    //    }

    //    public IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }


    //    public IEnumerable<IError> Validate()
    //    {
    //        foreach (var error in input.Validate())
    //        {
    //            yield return error;
    //        }
    //        foreach (var error in output.Validate())
    //        {
    //            yield return error;
    //        }
    //        foreach (var error in context.Validate())
    //        {
    //            yield return error;
    //        }
    //    }
    //    //public IOrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
    //    //{
    //    //    var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));

    //    //    var overlayedInput = overlay.Convert(input);
    //    //    var overlayedOut = overlay.Convert(output);
    //    //    var overlayedContext = overlay.Convert(context);

    //    //    if (overlayedInput is IConvertableFrontendType<IVerifiableType> convertableInput && overlayedOut is IConvertableFrontendType<IVerifiableType> convertableOut && overlayedContext is IConvertableFrontendType<IVerifiableType> convertableContext) {
    //    //        return OrType.Make<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new ImplementationType(convertableInput, convertableOut, convertableContext));
    //    //    }

    //    //    return OrType.Make<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>>(new GenericImplementationType(overlayedInput, overlayedOut, overlayedContext));

    //    //}


    //    // I don't think I need this, WeakTypeOrOperation instead.

    //    //internal struct TypeOr : IFrontendType<ITypeOr>
    //    //{
    //    //    public readonly IFrontendType<ITypeOr> left, right;

    //    //    public TypeOr(IFrontendType<ITypeOr> left, IFrontendType<ITypeOr> right)
    //    //    {
    //    //        this.left = left ?? throw new ArgumentNullException(nameof(left));
    //    //        this.right = right ?? throw new ArgumentNullException(nameof(right));
    //    //    }

    //    //    public IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
    //    //    {
    //    //        var (res, builder) = Tac.Model.Instantiated.TypeOr.Create();
    //    //        var myLeft = left;
    //    //        var myRIght = right;
    //    //        return new BuildIntention<Model.Elements.ITypeOr>(res, () => builder.Build(
    //    //            myLeft.Convert(context),
    //    //            myRIght.Convert(context)
    //    //            ));
    //    //    }
    //    //}
    //}
}
