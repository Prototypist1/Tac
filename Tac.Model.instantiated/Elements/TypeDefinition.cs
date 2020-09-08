using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Type;

namespace Tac.Model.Instantiated
{
    //public class TypeComparer : ITypeComparer
    //{
    //    public bool IsAssignableTo(IVerifiableType from, IVerifiableType to)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public static class FinalizedScopeExtensions
    {
        public static IInterfaceModuleType ToVerifiableType(this IFinalizedScope scope) => scope.Members.Select(x => (x.Key, x.Value.Value.Type)).ToArray().ToVerifiableType();


        public static IInterfaceModuleType ToVerifiableType(this (IKey, IVerifiableType)[] scope)
        {
            return InterfaceType.CreateAndBuild(scope.Select(x => MemberDefinition.CreateAndBuild(x.Item1, x.Item2, false)).ToList());
        }
    }

    // 🤫 IInterfaceType and module are they same 
    public class InterfaceType : IInterfaceType, IModuleType, IInterfaceTypeBuilder
    {

        private readonly Buildable<IReadOnlyList<IMemberDefinition>> buildableMembers = new Buildable<IReadOnlyList<IMemberDefinition>>();
        public IReadOnlyList<IMemberDefinition> Members => buildableMembers.Get();

        private InterfaceType() { }

        public void Build(IReadOnlyList<IMemberDefinition> scope)
        {
            buildableMembers.Set(scope);
        }

        
        public static (IInterfaceType, IInterfaceTypeBuilder) Create()
        {
            var res = new InterfaceType();
            return (res, res);
        }

        public static IInterfaceType CreateAndBuild(IReadOnlyList<IMemberDefinition> members) {
            var (x, y) = Create();
            y.Build(members);
            return x;
        }

        public IVerifiableType Returns()
        {
            return this;
        }
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeDefinition(this);
        }

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
             return Tac.Type.HasMembersLibrary.CanAssign(
                they,
                this,
                Members.Select(x => (x.Key, (IOrType<IVerifiableType, IError>)OrType.Make<IVerifiableType, IError>(x.Type))).ToList(),
                (type, key) => type.TryGetMember(key).IfElseReturn(
                        x => OrType.Make<IOrType<IVerifiableType, IError>, Tac.Type.No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                        () => OrType.Make<IOrType<IVerifiableType, IError>, Tac.Type.No, IError>(new Tac.Type.No())),
                (target, input, assumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, assumeTrue)), assumeTrue).Is1OrThrow();
        }

        public IIsPossibly<IVerifiableType> TryGetMember(IKey key)
        {

            var res = Tac.Type.HasMembersLibrary.TryGetMember(key, Members.Select(x => (x.Key, (IOrType<IVerifiableType, IError>)OrType.Make<IVerifiableType, IError>(x.Type))).ToList());

            if (res.Is2(out var _)) {
                return Possibly.IsNot<IVerifiableType>();
            }

            return Possibly.Is(res.Is1OrThrow().Is1OrThrow());

            //var member = Members.SingleOrDefault(x => x.Key == key);
            //if (member == null) {
            //    return Possibly.IsNot<IVerifiableType>();
            //}
            //return Possibly.Is(member.Type);
        }

        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
    }

    public interface IInterfaceTypeBuilder
    {
        void Build(IReadOnlyList<IMemberDefinition> scope);
    }

    public class TypeOr : ITypeOr, ITypeOrBuilder
    {
        private TypeOr()
        {
        }

        private readonly Buildable<IVerifiableType> left = new Buildable<IVerifiableType>();
        private readonly Buildable<IVerifiableType> right = new Buildable<IVerifiableType>();

        public IVerifiableType Left => left.Get();

        public IVerifiableType Right => right.Get();

        public static (ITypeOr, ITypeOrBuilder) Create()
        {
            var res = new TypeOr();
            return (res, res);
        }

        public static ITypeOr CreateAndBuild(IVerifiableType left, IVerifiableType right)
        {
            var res = new TypeOr();
            res.Build(left, right);
            return res;
        }


        public void Build(IVerifiableType left, IVerifiableType right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            this.left.Set(left);
            this.right.Set(right);
        }

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return OrTypeLibrary.CanAssign(
                they,
                (them) =>
                {
                    if (them.SafeIs(out TypeOr or))
                    {
                        return Possibly.Is(((IOrType<IVerifiableType, IError>)OrType.Make<IVerifiableType, IError>(or.Left), (IOrType<IVerifiableType, IError>)OrType.Make<IVerifiableType, IError>(or.Right)));
                    }
                    return Possibly.IsNot<(IOrType<IVerifiableType, IError>, IOrType<IVerifiableType, IError>)>();
                },
                this,
                OrType.Make<IVerifiableType, IError>(this.Left),
                OrType.Make<IVerifiableType, IError>(this.Right),
                (target, input, assumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, assumeTrue)),
                assumeTrue).Is1OrThrow();
        }

        public IIsPossibly<IVerifiableType> TryGetMember(IKey key)
        {
            var res = OrTypeLibrary.GetMember(
                key,
                OrType.Make<IVerifiableType, IError>(this.Left),
                OrType.Make<IVerifiableType, IError>(this.Right),
                (target, key) => target.TryGetMember(key).IfElseReturn(
                    x => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                    () => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(new No())
                ),
                (left, right) => CreateAndBuild(left.Is1OrThrow(), right.Is1OrThrow())
                );

            if (res.Is2(out var _))
            {
                return Possibly.IsNot<IVerifiableType>();
            }

            return Possibly.Is(res.Is1OrThrow().Is1OrThrow());
        }

        public IIsPossibly<IVerifiableType> TryGetReturn()
        {
            var res = OrTypeLibrary.TryGetReturn(
                           OrType.Make<IVerifiableType, IError>(this.Left),
                           OrType.Make<IVerifiableType, IError>(this.Right),
                           (target) => target.TryGetReturn().IfElseReturn(
                               x => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                               () => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(new No())
                           ),
                           (left, right) => CreateAndBuild(left.Is1OrThrow(), right.Is1OrThrow())
                           );

            if (res.Is2(out var _))
            {
                return Possibly.IsNot<IVerifiableType>();
            }

            return Possibly.Is(res.Is1OrThrow().Is1OrThrow());
        }

        public IIsPossibly<IVerifiableType> TryGetInput()
        {
            var res = OrTypeLibrary.TryGetReturn(
                           OrType.Make<IVerifiableType, IError>(this.Left),
                           OrType.Make<IVerifiableType, IError>(this.Right),
                           (target) => target.TryGetInput().IfElseReturn(
                               x => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                               () => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(new No())
                           ),
                           (left, right) => CreateAndBuild(left.Is1OrThrow(), right.Is1OrThrow())
                           );

            if (res.Is2(out var _))
            {
                return Possibly.IsNot<IVerifiableType>();
            }

            return Possibly.Is(res.Is1OrThrow().Is1OrThrow());
        }
    }

    public interface ITypeOrBuilder
    {
        void Build(IVerifiableType left, IVerifiableType right);
    }

    public struct TypeAnd : ITypeAnd
    {
        public TypeAnd(IVerifiableType left, IVerifiableType right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IVerifiableType Left { get; }

        public IVerifiableType Right { get; }

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IVerifiableType> TryGetInput()
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IVerifiableType> TryGetMember(IKey key)
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IVerifiableType> TryGetReturn()
        {
            throw new NotImplementedException();
        }
    }

    public struct ReferanceType : IReferanceType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            throw new NotImplementedException();
        }
    }

    public struct EntryPointType : IEntryPointType {
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IVerifiableType> TryGetInput()
        {
            throw new NotImplementedException();
        }

        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();

        public IIsPossibly<IVerifiableType> TryGetReturn()
        {
            throw new NotImplementedException();
        }
    }

    public struct NumberType : INumberType
    {

        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is NumberType;
        }
    }

    public struct EmptyType : IEmptyType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is EmptyType;
        }
    }

    public struct BooleanType : IBooleanType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is BooleanType;
        }
    }
    
    // why do I have block type
    // it is just an empty type right?
    public struct BlockType : IBlockType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is BlockType;
        }
    }

    public struct StringType : IStringType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is StringType;
        }
    }

    public struct AnyType : IAnyType
    {
        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return true;
        }
    }

    public class GemericTypeParameterPlacholder : IVerifiableType, IGemericTypeParameterPlacholderBuilder
    {
        private GemericTypeParameterPlacholder() { }

        public static (IVerifiableType, IGemericTypeParameterPlacholderBuilder) Create()
        {
            var res = new GemericTypeParameterPlacholder();
            return (res, res);
        }

        public static IVerifiableType CreateAndBuild(IOrType<NameKey,ImplicitKey> key) {
            var (x, y) = Create();
            y.Build(key);
            return x;
        }

        public void Build(IOrType<NameKey, ImplicitKey> key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        private IOrType<NameKey, ImplicitKey>? key = null;
        public IOrType<NameKey, ImplicitKey> Key { get=> key ?? throw new NullReferenceException(nameof(key)); private set => key = value; }

        public override bool Equals(object obj)
        {
            return obj is GemericTypeParameterPlacholder placholder &&
                   EqualityComparer<IOrType<NameKey, ImplicitKey>>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }


        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            // 🤷‍ who knows
            return ReferenceEquals(this, they);
        }
    }
    
    public interface IGemericTypeParameterPlacholderBuilder
    {
        void Build(IOrType<NameKey, ImplicitKey> key);

        IOrType<NameKey, ImplicitKey> Key { get; }
    }

    public interface IGenericMethodTypeBuilder {
        void Build(IVerifiableType inputType, IVerifiableType ouputType);
    }

    public class MethodType : IMethodType, IMethodTypeBuilder
    {
        private MethodType() { }

        public void Build(IVerifiableType  inputType, IVerifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }
        
        public static (IMethodType, IMethodTypeBuilder) Create()
        {
            var res = new MethodType();
            return (res, res);
        }

        // TOOD I am not sure I need to create and build any of these types...
        // good old constructors would do
        public static IMethodType CreateAndBuild(IVerifiableType inputType, IVerifiableType outputType) {
            var (x, y) = Create();
            y.Build(inputType, outputType);
            return x;
        }

        private IVerifiableType? inputType; 
        public IVerifiableType InputType { get=> inputType?? throw new NullReferenceException(nameof(inputType)); private set=>inputType = value; }
        private IVerifiableType? outputType;
        public IVerifiableType OutputType { get => outputType ?? throw new NullReferenceException(nameof(outputType)); private set => outputType = value; }


        public IIsPossibly<IVerifiableType> TryGetMember(IKey key) => Possibly.IsNot<IVerifiableType>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return MethodLibrary.CanAssign(
                they,
                this,
                OrType.Make<IVerifiableType, IError>(InputType),
                OrType.Make<IVerifiableType, IError>(OutputType),
                x => x.TryGetInput().IfElseReturn(
                    x => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                    () => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(new No())),
                x => x.TryGetReturn().IfElseReturn(
                    x => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(OrType.Make<IVerifiableType, IError>(x)),
                    () => OrType.Make<IOrType<IVerifiableType, IError>, No, IError>(new No())),
                (target, input, assumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, assumeTrue)),
                assumeTrue
                ).Is1OrThrow();
        }

        public IIsPossibly<IVerifiableType> TryGetReturn()
        {
            return Possibly.Is(OutputType);
        }

        public IIsPossibly<IVerifiableType> TryGetInput()
        {
            return Possibly.Is(InputType);
        }
    }
    
    public interface IMethodTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType);
    }

    // TODO struct?
    // no, anything that is built can not be a struct
    //
    // TODO TODO ok so I need this not to exist it is just a method ...
    // definately
    //public class ImplementationType : IImplementationType, IMethodType, IImplementationTypeBuilder
    //{
    //    private ImplementationType() { }

    //    public void Build(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType)
    //    {
    //        ImplementationInputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
    //        ImplementationOutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
    //        ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
    //        MethodInputType = contextType ?? throw new ArgumentNullException(nameof(contextType));
    //        var (outputMethod, outputMethodBuilder) = MethodType.Create();
    //        outputMethodBuilder.Build(inputType, outputType);
    //        MethodOutputType =  outputMethod;

    //    }
        
    //    public static (IImplementationType, IImplementationTypeBuilder) Create()
    //    {
    //        var res = new ImplementationType();
    //        return (res, res);
    //    }

    //    public static IImplementationType CreateAndBuild(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType)
    //    {
    //        var (res, builder) = Create();
    //        builder.Build(inputType, outputType, contextType);
    //        return res;
    //    }

    //    public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
    //    {
    //        throw new NotImplementedException("how does it work when these could be errors");
    //        //if (they is IMethodType method)
    //        //{
    //        //    return (this as IMethodType).InputType.TheyAreUs(method.InputType, false) && (this as IMethodType).OutputType.WeAreThem(method.OutputType, false);
    //        //}

    //        //if (noTagBacks)
    //        //{
    //        //    return false;
    //        //}

    //        //return this.WeAreThem(this, true);
    //    }

    //    public bool WeAreThem(IVerifiableType them, bool noTagBacks)
    //    {
    //        throw new NotImplementedException("how does it work when these could be errors");
    //        //if (them is IMethodType method)
    //        //{
    //        //    return (this as IMethodType).InputType.WeAreThem(method.InputType, false) && (this as IMethodType).OutputType.TheyAreUs(method.OutputType, false);
    //        //}

    //        //if (noTagBacks)
    //        //{
    //        //    return false;
    //        //}

    //        //return this.WeAreThem(this, true);
    //    }

    //    private IVerifiableType? ImplementationInputType;
    //    private IVerifiableType? ImplementationOutputType;
    //    private IVerifiableType? MethodInputType;
    //    private IVerifiableType? MethodOutputType;

    //    IVerifiableType IImplementationType.InputType => ImplementationInputType ?? throw new NullReferenceException(nameof(ImplementationInputType));
    //    IVerifiableType IImplementationType.OutputType => ImplementationOutputType ?? throw new NullReferenceException(nameof(ImplementationOutputType));
    //    IVerifiableType IMethodType.InputType => MethodInputType ?? throw new NullReferenceException(nameof(MethodInputType));
    //    IVerifiableType IMethodType.OutputType => MethodOutputType ?? throw new NullReferenceException(nameof(MethodOutputType));
    //    private IVerifiableType? contextType;
    //    public IVerifiableType ContextType { get => contextType ?? throw new NullReferenceException(nameof(contextType)); private set => contextType = value ?? throw new NullReferenceException(nameof(value)); }
    //}

    public interface IGenericImplementationBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType ouputType, IVerifiableType contextType);
    }

    public interface IImplementationTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType);
    }

}
