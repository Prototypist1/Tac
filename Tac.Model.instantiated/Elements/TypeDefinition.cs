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
            return InterfaceType.CreateAndBuild(scope.Select(x => MemberDefinition.CreateAndBuild(x.Item1, x.Item2, Access.ReadWrite)).ToList());
        }
    }

    // 🤫 IInterfaceType and module are they same 
    public class InterfaceType : IInterfaceType, IModuleType, IInterfaceTypeBuilder
    {

        private readonly Buildable<IReadOnlyList<IMemberDefinition>> buildableMembers = new();
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
                Members.Select(x => (x.Key, (IOrType<(IVerifiableType,Access), IError>)OrType.Make<(IVerifiableType,Access), IError>((x.Type, x.Access)))).ToList(),
                (type, key) => type.TryGetMember(key, assumeTrue).IfElseReturn(
                        x => OrType.Make<IOrType<(IVerifiableType,Access), IError>, Tac.Type.No, IError>(OrType.Make<(IVerifiableType, Access), IError>(x)),
                        () => OrType.Make<IOrType<(IVerifiableType, Access), IError>, Tac.Type.No, IError>(new Tac.Type.No())),
                (target, input, assumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, assumeTrue)), 
                assumeTrue).Is1OrThrow();
        }

        public IIsPossibly<(IVerifiableType,Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {

            var res = Tac.Type.HasMembersLibrary.TryGetMember(key, Members.Select(x => (x.Key, (IOrType<(IVerifiableType, Access), IError>)OrType.Make<(IVerifiableType, Access), IError>((x.Type,x.Access)))).ToList());

            if (res.Is2(out var _)) {
                return Possibly.IsNot<(IVerifiableType, Access)>();
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

        private readonly Buildable<IVerifiableType> left = new();
        private readonly Buildable<IVerifiableType> right = new();
        private readonly Buildable<IReadOnlyList<IMemberDefinition>> members = new();
        private readonly Buildable<IIsPossibly< IVerifiableType>> input = new();
        private readonly Buildable<IIsPossibly<IVerifiableType>> output = new();


        public IVerifiableType Left => left.Get();

        public IVerifiableType Right => right.Get();

        public IReadOnlyList<IMemberDefinition> Members => members.Get();

        public static (ITypeOr, ITypeOrBuilder) Create()
        {
            var res = new TypeOr();
            return (res, res);
        }

        // I am passing stuff around that could be calcuated
        // but I do that because it is calcuated at the start of things
        // and then flows through
        public static ITypeOr CreateAndBuild(IVerifiableType left, IVerifiableType right, IMemberDefinition[] members, IIsPossibly<IVerifiableType> input, IIsPossibly<IVerifiableType> output)
        {
            var res = new TypeOr();
            res.Build(left, right, members,input,output);

            return res;
        }


        public void Build(IVerifiableType left, IVerifiableType right, IMemberDefinition[] members, IIsPossibly<IVerifiableType> input, IIsPossibly<IVerifiableType> output)
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
            this.members.Set(members);
            this.input.Set(input);
            this.output.Set(output);
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
                (target, input, innerAssumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, innerAssumeTrue)),
                assumeTrue).Is1OrThrow();
        }

        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assume)
        {
            var matches = members.Get().Where(x => x.Key.Equals(key)).ToArray();

            if (!matches.Any()) {
                return Possibly.IsNot<(IVerifiableType, Access)>();
            }

            var match = matches.Single();

            return Possibly.Is((match.Type, match.Access));
        }

        public IIsPossibly<IVerifiableType> TryGetReturn() => output.Get();
        public IIsPossibly<IVerifiableType> TryGetInput() => input.Get();
    }

    public interface ITypeOrBuilder
    {
        void Build(IVerifiableType left, IVerifiableType right, IMemberDefinition[] members, IIsPossibly<IVerifiableType> input, IIsPossibly<IVerifiableType> output);
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

        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue)
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
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
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

        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();

        public IIsPossibly<IVerifiableType> TryGetReturn()
        {
            throw new NotImplementedException();
        }
    }

    public struct NumberType : INumberType
    {

        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is NumberType;
        }
    }

    public struct EmptyType : IEmptyType
    {
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is EmptyType;
        }
    }

    public struct BooleanType : IBooleanType
    {
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
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
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is BlockType;
        }
    }

    public struct StringType : IStringType
    {
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return they is StringType;
        }
    }

    public struct AnyType : IAnyType
    {
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
        public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
        public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();
        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            return true;
        }
    }

    //public class GemericTypeParameterPlacholder : IVerifiableType, IGemericTypeParameterPlacholderBuilder
    //{
    //    private GemericTypeParameterPlacholder() { }

    //    public static (IVerifiableType, IGemericTypeParameterPlacholderBuilder) Create()
    //    {
    //        var res = new GemericTypeParameterPlacholder();
    //        return (res, res);
    //    }

    //    public static IVerifiableType CreateAndBuild(IOrType<NameKey,ImplicitKey> key) {
    //        var (x, y) = Create();
    //        y.Build(key);
    //        return x;
    //    }

    //    public void Build(IOrType<NameKey, ImplicitKey> key)
    //    {
    //        this.key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    private IOrType<NameKey, ImplicitKey>? key = null;
    //    public IOrType<NameKey, ImplicitKey> Key { get=> key ?? throw new NullReferenceException(nameof(key)); private set => key = value; }

    //    public override bool Equals(object obj)
    //    {
    //        return obj is GemericTypeParameterPlacholder placholder &&
    //               EqualityComparer<IOrType<NameKey, ImplicitKey>>.Default.Equals(Key, placholder.Key);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return HashCode.Combine(Key);
    //    }


    //    public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();
    //    public IIsPossibly<IVerifiableType> TryGetReturn() => Possibly.IsNot<IVerifiableType>();
    //    public IIsPossibly<IVerifiableType> TryGetInput() => Possibly.IsNot<IVerifiableType>();

    //    public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
    //    {
    //        // 🤷‍ who knows
    //        return ReferenceEquals(this, they);
    //    }
    //}
    
    public interface IGemericTypeParameterPlacholderBuilder
    {
        void Build(IOrType<NameKey, ImplicitKey> key);

        IOrType<NameKey, ImplicitKey> Key { get; }
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


        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();

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

    public interface IGenericImplementationBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType ouputType, IVerifiableType contextType);
    }

    public interface IImplementationTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType);
    }

    public class GenericMethodType : IGenericMethodType, IGenericMethodTypeBuilder
    {

        private Buildable<IGenericTypeParameter[]> buildableParameters = new Buildable<IGenericTypeParameter[]>();
        private Buildable<IVerifiableType> buildableOutputType = new Buildable<IVerifiableType>();
        private Buildable<IVerifiableType> buildableInputType = new Buildable<IVerifiableType>();
        public IGenericTypeParameter[] Parameters => buildableParameters.Get();
        public IVerifiableType OutputType => buildableOutputType.Get();
        public IVerifiableType InputType => buildableInputType.Get();

        public void Build(IVerifiableType inputType, IVerifiableType ouputType, IGenericTypeParameter[] parameters)
        {
            buildableParameters.Set(parameters);
            buildableOutputType.Set(ouputType);
            buildableInputType.Set(inputType);
        }

        private GenericMethodType() { }

        public static (IGenericMethodType, IGenericMethodTypeBuilder) Create()
        {
            var res = new GenericMethodType();
            return (res, res);
        }

        public static IGenericMethodType CreateAndBuild(IVerifiableType inputType, IVerifiableType ouputType, IGenericTypeParameter[] parameters)
        {
            var (x, y) = Create();
            y.Build(inputType, ouputType, parameters);
            return x;
        }

        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => Possibly.IsNot<(IVerifiableType, Access)>();

        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            if (!they.SafeIs(out GenericMethodType genericMethod)) {
                return false;
            }

            if (Parameters.Length != genericMethod.Parameters.Length) {
                return false;
            }

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
                (target, input, innerAssumeTrue) => OrType.Make<bool, IError>(target.TheyAreUs(input, innerAssumeTrue)),
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

    public interface IGenericMethodTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType ouputType, IGenericTypeParameter[] parameters);
    }


    public class GenericTypeParameter : IGenericTypeParameter, IGenericTypeParameterBuilder
    {
        //private Buildable<IVerifiableType> buildableParent = new Buildable<IVerifiableType>();
        private Buildable<IVerifiableType> buildableConstraint = new Buildable<IVerifiableType>();
        private BuildableValue<int> buildableIndex = new BuildableValue<int>();

        public void Build(/*IVerifiableType parent,*/ int index, IVerifiableType constraint)
        {
            //if (parent is null)
            //{
            //    throw new ArgumentNullException(nameof(parent));
            //}

            if (constraint is null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }

            //buildableParent.Set(parent);
            buildableConstraint.Set(constraint);
            buildableIndex.Set(index);
        }

        
        private GenericTypeParameter() { }

        public static (IGenericTypeParameter, IGenericTypeParameterBuilder) Create()
        {
            var res = new GenericTypeParameter();
            return (res, res);
        }

        public static IGenericTypeParameter CreateAndBuild(/*IVerifiableType parent,*/ int index, IVerifiableType constraint)
        {
            var (x, y) = Create();
            y.Build(/*parent,*/ index, constraint);
            return x;
        }


        public bool TheyAreUs(IVerifiableType they, List<(IVerifiableType, IVerifiableType)> assumeTrue)
        {
            if (assumeTrue.Contains((this, they)))
            {
                return true;
            }
            assumeTrue.Add((this, they));

            return buildableConstraint.Get().TheyAreUs(they, assumeTrue);
        }
        public IIsPossibly<(IVerifiableType, Access)> TryGetMember(IKey key, List<(IVerifiableType, IVerifiableType)> assumeTrue) => buildableConstraint.Get().TryGetMember(key, assumeTrue);
        public IIsPossibly<IVerifiableType> TryGetReturn() => buildableConstraint.Get().TryGetReturn();
        public IIsPossibly<IVerifiableType> TryGetInput() => buildableConstraint.Get().TryGetInput();
    }

    public interface IGenericTypeParameterBuilder
    {
        void Build(/*IVerifiableType parent,*/ int index, IVerifiableType constraint);
    }

}
