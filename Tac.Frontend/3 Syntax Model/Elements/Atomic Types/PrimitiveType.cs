using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;
using Tac.Type;

namespace Tac.SyntaxModel.Elements.AtomicTypes
{
    internal static class FrontendTypeExtensions{
        public static IOrType<IFrontendType<IVerifiableType>, IError> UnwrapRefrence(this IFrontendType<IVerifiableType> frontendType) 
        {
            if (frontendType is RefType refType) {
                return refType.inner;
            }
            return OrType.Make< IFrontendType<IVerifiableType>,IError >(frontendType);
        }
    }

    internal interface IPrimitiveType: IFrontendType<IVerifiableType>
    {
    }

    internal class FrontEndOrType : IFrontendType<IVerifiableType>
    {
        internal readonly IOrType<IFrontendType<IVerifiableType>,IError> left, right;

        public FrontEndOrType(IOrType<IFrontendType<IVerifiableType>, IError> left, IOrType<IFrontendType<IVerifiableType>, IError> right)
        {
            // TODO this is wrong
            // I think
            // I don't think this should be a validation error
            // it should be an exception at converstion type
            // Idk this design is a little werid
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            var (res, builder) = Tac.Model.Instantiated.TypeOr.Create();


            var inputType = left;
            var outputType = right;
            return new BuildIntention<ITypeOr>(res
                , () =>
                {
                    builder.Build(
                        inputType.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context),
                        outputType.Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context));
                });
        }

        private static IIsPossibly<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)> IsOrType(IFrontendType<IVerifiableType> them) {
            if (them.SafeIs(out FrontEndOrType orType)) {
                return Possibly.Is<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>((orType.left, orType.right));
            }
            return Possibly.IsNot<(IOrType<IFrontendType<IVerifiableType>, IError> fromLeft, IOrType<IFrontendType<IVerifiableType>, IError> fromRight)>();
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return OrTypeLibrary.CanAssign(they, IsOrType, this, left, right, (x, y,list) => x.TheyAreUs(y, list), assumeTrue);
        }

        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return OrTypeLibrary.GetMember(
                key,
                left,
                right,
                (x, key, list) => x.TryGetMember(key,list),
                (x, y) => new FrontEndOrType(OrType.Make<IFrontendType<IVerifiableType>, IError>(x), OrType.Make<IFrontendType<IVerifiableType>, IError>(y)),
                (us, them, list) => us.TheyAreUs(them, list),
                assumeTrue
                );
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn()
        {
            return OrTypeLibrary.TryGetReturn(left, right, (x) => x.TryGetReturn(), (x, y) => new FrontEndOrType(x, y));
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput()
        {
            return OrTypeLibrary.TryGetInput(left, right, (x) => x.TryGetInput(), (x, y) => new FrontEndOrType(x, y));
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var item in left.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return item;
            }
            foreach (var item in right.SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return item;
            }
        }
    }

    internal class HasMembersType : IFrontendType<IInterfaceType> {

        private struct Yes { }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) {
            return HasMembersLibrary.CanAssign(
                they,
                this,
                weakScope.membersList.Select(x => (x.GetValue().Key, x.GetValue().Type.GetValue().TransformInner(y=>(y,x.GetValue().Access)))).ToList(),
                (target,key) => target.TryGetMember(key, assumeTrue),
                (target,other,assumes)=> target.TheyAreUs(other,assumes),
                assumeTrue
                );
        }
        
        public IEnumerable<IError> Validate() => weakScope.membersList.Select(x => x.GetValue().Type.GetValue().Possibly1()).OfType<IIsDefinately<IFrontendType<IVerifiableType>>>().SelectMany(x => x.Value.Validate());

        public IOrType<IOrType<(IFrontendType<IVerifiableType>,Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return HasMembersLibrary.TryGetMember(key, weakScope.membersList.Select(x => (x.GetValue().Key, x.GetValue().Type.GetValue().TransformInner(y => (y, x.GetValue().Access)))).ToList());
        }

        public IBuildIntention<IInterfaceType> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = InterfaceType.Create();
            return new BuildIntention<IInterfaceType>(toBuild, () =>
            {
                maker.Build(weakScope.Convert(context).Members.Values.Select(x => x.Value).ToArray());
            });
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());

        //public readonly IReadOnlyDictionary<IKey, IOrType<IFrontendType<IVerifiableType>, IError>> members;

        public readonly WeakScope weakScope;

        public HasMembersType(WeakScope weakScope)
        {
            this.weakScope = weakScope ?? throw new ArgumentNullException(nameof(weakScope));
        }
    }

    // still bad at structs is this a struct?
    //internal struct HowTypesThinkOfMembers {
    //    public IOrType<IFrontendType<IVerifiableType>, IError> orType;
    //    public IKey key;
    //}

    // reference is a type!
    // but it probably does not mean what you think it means
    // it really means you can assign to it
    // this is what is returned by member and member reference 
    internal class RefType : IFrontendType<IVerifiableType>, IPrimitiveType {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IReferanceType>(new Model.Instantiated.ReferanceType(), () => { });
        }

        public IEnumerable<IError> Validate() => inner.SwitchReturns(x=>x.Validate(), x=>Array.Empty<IError>());

        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            // the method calling this
            // is in charge of unwrapping
            throw new Exception("I don't think this should ever happen");

            // this is a bit of a smell
            // a lot of a smell
        }

        public readonly IOrType< IFrontendType<IVerifiableType>,IError> inner;

        public RefType(IOrType<IFrontendType<IVerifiableType>, IError> inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    // should this really exist?
    // is it a type?
    // it is an error...
    // it is not convertable
    //internal class IndeterminateType : IFrontendType<IVerifiableType>
    //{
    //    private readonly IError error;

    //    public IndeterminateType(IError error)
    //    {
    //        this.error = error ?? throw new ArgumentNullException(nameof(error));
    //    }

    //    public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
    //    {
    //        return OrType.Make<bool, IError>(ReferenceEquals(this, they));
    //    }

    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetMember(IKey key) => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    //    public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());

    //    public IEnumerable<IError> Validate()
    //    {
    //        return new[] { error };
    //    }
    //}

    internal struct BlockType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBlockType>(new Model.Instantiated.BlockType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError> (they is BlockType);

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());


        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    internal struct StringType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IStringType>(new Model.Instantiated.StringType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is StringType);
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }
    internal struct EmptyType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IEmptyType>(new Model.Instantiated.EmptyType(), () => { });
        }
        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is EmptyType);
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    internal struct NumberType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<INumberType>(new Model.Instantiated.NumberType(), () => { });
        }
        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is NumberType);
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    // I don't think this is a type....
    // placeholders are effectively defined by constraints really just metadata
    internal interface IGenericTypeParameterPlacholder //: IFrontendType<IVerifiableType>
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

    internal struct AnyType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IAnyType>(new Model.Instantiated.AnyType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(true);
        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    internal struct BooleanType : IFrontendType<IVerifiableType>, IPrimitiveType
    {
        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return new BuildIntention<IBooleanType>(new Tac.Model.Instantiated.BooleanType(), () => { });
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<bool, IError>(they is BooleanType);

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(new No());
    }

    internal class MethodType : IFrontendType<IVerifiableType>, IPrimitiveType
    {

        private struct Yes { }

        // is the meta-data here worth capturing
        public static MethodType ImplementationType(
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> inputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> outputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> contextType) {
            return new MethodType(
                contextType,
                new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError> (new MethodType(inputType, outputType))));
        }

        public MethodType(
            IBox< IOrType<IFrontendType<IVerifiableType>, IError>> inputType,
            IBox<IOrType<IFrontendType<IVerifiableType>, IError>> outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> InputType { get; }
        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> OutputType { get; }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
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
                        inputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>,IFrontendType<IVerifiableType>>().Convert(context),
                        outputType.GetValue().Is1OrThrow().SafeCastTo<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>().Convert(context));
                });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in InputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
            foreach (var error in OutputType.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return MethodLibrary.CanAssign(
                they,
                this,
                InputType.GetValue(),
                OutputType.GetValue(),
                x => x.TryGetInput(),
                x => x.TryGetReturn(),
                (target, other, list) => target.TheyAreUs(other, list),
                assumeTrue);
        }
        public IOrType<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue) => OrType.Make<IOrType<(IFrontendType<IVerifiableType>, Access), IError>, No, IError>(new No());


        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(OutputType.GetValue());
        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput() => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(InputType.GetValue());
    }

    internal interface IGenericMethodType : IFrontendType<IVerifiableType>, IFrontendGenericType { }
}
