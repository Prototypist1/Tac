using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;

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


        public static IInterfaceModuleType ToVerifiableType(this (IKey, IOrType<IVerifiableType,IError>)[] scope)
        {
            return InterfaceType.CreateAndBuild(scope.Select(x => MemberDefinition.CreateAndBuild(x.Item1, x.Item2, false)).ToList());
        }
    }

    // 🤫 IInterfaceType and module are they same 
    public class InterfaceType : IInterfaceType, IModuleType, IInterfaceTypeBuilder
    {

        private readonly Buildable<IReadOnlyList<IMemberDefinition>> buildableMembers = new Buildable<IReadOnlyList<IMemberDefinition>>();
        public IReadOnlyList<IMemberDefinition> Members => buildableMembers.Get();

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            throw new NotImplementedException();
            //if (they is IInterfaceType otherObject)
            //{
            //    // they have all our members
            //    return Members.All(member => otherObject.Members.Any(otherMember => otherMember.Type.TheyAreUs(member.Type, false)));
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return they.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            throw new NotImplementedException();
            //if (them is IInterfaceType otherObject)
            //{
            //    // we have all their members
            //    return otherObject.Members.All(otherMember => Members.Any(member => otherMember.Type.WeAreThem(member.Type, false)));
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return this.WeAreThem(this, true);
        }


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

        public IOrType<IVerifiableType, IError> Returns()
        {
            return OrType.Make<IVerifiableType, IError>( this);
        }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.TypeDefinition(this);
        }
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

        private readonly Buildable<IOrType<IVerifiableType, IError>> left = new Buildable<IOrType<IVerifiableType, IError>>();
        private readonly Buildable<IOrType<IVerifiableType, IError>> right = new Buildable<IOrType<IVerifiableType, IError>>();

        public IOrType<IVerifiableType, IError> Left => left.Get();

        public IOrType<IVerifiableType, IError> Right => right.Get();

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            throw new NotImplementedException("how does this work with errors");
            //return Left.TheyAreUs(they, noTagBacks) || Left.TheyAreUs(they, noTagBacks);
        }

        public bool WeAreThem(IVerifiableType them,bool noTagBacks)
        {
            throw new NotImplementedException("how does this work with errors");
            //return Left.WeAreThem(them, noTagBacks) && Left.WeAreThem(them, noTagBacks);
        }


        public static (ITypeOr, ITypeOrBuilder) Create()
        {
            var res = new TypeOr();
            return (res, res);
        }

        public static ITypeOr CreateAndBuild(IOrType<IVerifiableType, IError> left, IOrType<IVerifiableType, IError> right)
        {
            var res = new TypeOr();
            res.Build(left, right);
            return res;
        }


        public void Build(IOrType<IVerifiableType, IError> left, IOrType<IVerifiableType, IError> right)
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
    }

    public interface ITypeOrBuilder
    {
        void Build(IOrType<IVerifiableType,IError> left, IOrType<IVerifiableType, IError> right);
    }

    public struct TypeAnd : ITypeAnd
    {
        public TypeAnd(IOrType<IVerifiableType, IError> left, IOrType<IVerifiableType, IError> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IOrType<IVerifiableType, IError> Left { get; }

        public IOrType<IVerifiableType, IError> Right { get; }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            throw new NotImplementedException("how does this work with errors");
            //return Left.TheyAreUs(they, noTagBacks) && Left.TheyAreUs(they, noTagBacks);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            throw new NotImplementedException("how does this work with errors");
            //return Left.WeAreThem(them, noTagBacks) || Left.WeAreThem(them, noTagBacks);
        }
    }

    public static class SimpleTypeHelp {
        public static bool TheyAreUs<T>(IVerifiableType us, IVerifiableType they, bool noTagBacks)
        {
            if (they is T)
            {
                return true;
            }

            if (noTagBacks)
            {
                return false;
            }

            return they.WeAreThem(us, true);
        }

        public static bool WeAreThem<T>(IVerifiableType us, IVerifiableType them, bool noTagBacks)
        {
            if (them is T)
            {
                return true;
            }

            if (noTagBacks)
            {
                return false;
            }

            return them.TheyAreUs(us, true);
        }
    }

    public struct EntryPointType : IEntryPointType {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<IEntryPointType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IEntryPointType>(this, them, noTagBacks);

    }

    public struct NumberType : INumberType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<INumberType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<INumberType>(this, them, noTagBacks);

    }

    public struct EmptyType : IEmptyType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<IEmptyType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IEmptyType>(this, them, noTagBacks);
    }

    public struct BooleanType : IBooleanType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<IBooleanType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IBooleanType>(this, them, noTagBacks);
    }
    
    // why do I have block type
    // it is just an empty type right?
    public struct BlockType : IBlockType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<IBlockType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IBlockType>(this, them, noTagBacks);
    }

    public struct StringType : IStringType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => SimpleTypeHelp.TheyAreUs<IStringType>(this, they, noTagBacks);

        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IStringType>(this, them, noTagBacks);
    }

    public struct AnyType : IAnyType
    {
        public bool TheyAreUs(IVerifiableType they, bool noTagBacks) => true;
        public bool WeAreThem(IVerifiableType them, bool noTagBacks) => SimpleTypeHelp.WeAreThem<IAnyType>(this, them, noTagBacks);
    }

    public class GemericTypeParameterPlacholder : IVerifiableType, IGemericTypeParameterPlacholderBuilder
    {
        private GemericTypeParameterPlacholder() { }

        public static (IVerifiableType, IGemericTypeParameterPlacholderBuilder) Create()
        {
            var res = new GemericTypeParameterPlacholder();
            return (res, res);
        }

        public static IVerifiableType CreateAndBuild(IKey key) {
            var (x, y) = Create();
            y.Build(key);
            return x;
        }

        public void Build(IKey key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        private IKey? key = null;
        public IKey Key { get=>key?? throw new NullReferenceException(nameof(key)); private set => key = value; }

        public override bool Equals(object obj)
        {
            return obj is GemericTypeParameterPlacholder placholder &&
                   EqualityComparer<IKey>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            if (they is GemericTypeParameterPlacholder otherPlaceHolder)
            {
                return Key.Equals(otherPlaceHolder.Key);
            }

            if (noTagBacks)
            {
                return false;
            }

            return they.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            if (them is GemericTypeParameterPlacholder otherPlaceHolder)
            {
                return Key.Equals(otherPlaceHolder.Key);
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }
    }
    
    public interface IGemericTypeParameterPlacholderBuilder
    {
        void Build(IKey key);

        IKey Key { get; }
    }

    public interface IGenericMethodTypeBuilder {
        void Build(IVerifiableType inputType, IVerifiableType ouputType);
    }

    public class MethodType : IMethodType, IMethodTypeBuilder
    {
        private MethodType() { }

        public void Build(IOrType<IVerifiableType,IError>  inputType, IOrType<IVerifiableType, IError> outputType)
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
        public static IMethodType CreateAndBuild(IOrType<IVerifiableType, IError> inputType, IOrType<IVerifiableType, IError> outputType) {
            var (x, y) = Create();
            y.Build(inputType, outputType);
            return x;
        }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {

            throw new NotImplementedException("how does this work with errors");
            //if (they is IMethodType method) {
            //    return InputType.TheyAreUs(method.InputType, false) && OutputType.WeAreThem(method.OutputType, false);
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return this.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            throw new NotImplementedException("how does this work with errors");
            //if (them is IMethodType method)
            //{
            //    return InputType.WeAreThem(method.InputType, false) && OutputType.TheyAreUs(method.OutputType, false);
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return this.WeAreThem(this, true);
        }

        private IOrType<IVerifiableType, IError>? inputType; 
        public IOrType<IVerifiableType, IError> InputType { get=> inputType?? throw new NullReferenceException(nameof(inputType)); private set=>inputType = value; }
        private IOrType<IVerifiableType, IError>? outputType;
        public IOrType<IVerifiableType, IError> OutputType { get => outputType ?? throw new NullReferenceException(nameof(outputType)); private set => outputType = value; }
    }
    
    public interface IMethodTypeBuilder
    {
        void Build(IOrType<IVerifiableType, IError> inputType, IOrType<IVerifiableType, IError> outputType);
    }

    // TODO struct?
    // no, anything that is built can not be a struct
    //
    // TODO TODO ok so I need this not to exist it is just a method ...
    public class ImplementationType : IImplementationType, IMethodType, IImplementationTypeBuilder
    {
        private ImplementationType() { }

        public void Build(IOrType<IVerifiableType, IError> inputType, IOrType<IVerifiableType, IError> outputType, IOrType<IVerifiableType, IError> contextType)
        {
            ImplementationInputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            ImplementationOutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            MethodInputType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            var (outputMethod, outputMethodBuilder) = MethodType.Create();
            outputMethodBuilder.Build(inputType, outputType);
            MethodOutputType = OrType.Make<IVerifiableType,IError>( outputMethod);

        }
        
        public static (IImplementationType, IImplementationTypeBuilder) Create()
        {
            var res = new ImplementationType();
            return (res, res);
        }

        public static IImplementationType CreateAndBuild(IOrType<IVerifiableType, IError> inputType, IOrType<IVerifiableType, IError> outputType, IOrType<IVerifiableType, IError> contextType)
        {
            var (res, builder) = Create();
            builder.Build(inputType, outputType, contextType);
            return res;
        }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            throw new NotImplementedException("how does it work when these could be errors");
            //if (they is IMethodType method)
            //{
            //    return (this as IMethodType).InputType.TheyAreUs(method.InputType, false) && (this as IMethodType).OutputType.WeAreThem(method.OutputType, false);
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return this.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            throw new NotImplementedException("how does it work when these could be errors");
            //if (them is IMethodType method)
            //{
            //    return (this as IMethodType).InputType.WeAreThem(method.InputType, false) && (this as IMethodType).OutputType.TheyAreUs(method.OutputType, false);
            //}

            //if (noTagBacks)
            //{
            //    return false;
            //}

            //return this.WeAreThem(this, true);
        }

        private IOrType<IVerifiableType, IError>? ImplementationInputType;
        private IOrType<IVerifiableType, IError>? ImplementationOutputType;
        private IOrType<IVerifiableType, IError>? MethodInputType;
        private IOrType<IVerifiableType, IError>? MethodOutputType;

        IOrType<IVerifiableType, IError> IImplementationType.InputType => ImplementationInputType ?? throw new NullReferenceException(nameof(ImplementationInputType));
        IOrType<IVerifiableType, IError> IImplementationType.OutputType => ImplementationOutputType ?? throw new NullReferenceException(nameof(ImplementationOutputType));
        IOrType<IVerifiableType, IError> IMethodType.InputType => MethodInputType ?? throw new NullReferenceException(nameof(MethodInputType));
        IOrType<IVerifiableType, IError> IMethodType.OutputType => MethodOutputType ?? throw new NullReferenceException(nameof(MethodOutputType));
        private IOrType<IVerifiableType, IError>? contextType;
        public IOrType<IVerifiableType, IError> ContextType { get => contextType ?? throw new NullReferenceException(nameof(contextType)); private set => contextType = value ?? throw new NullReferenceException(nameof(value)); }
    }

    public interface IGenericImplementationBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType ouputType, IVerifiableType contextType);
    }

    public interface IImplementationTypeBuilder
    {
        void Build(IOrType<IVerifiableType, IError> inputType, IOrType<IVerifiableType, IError> outputType, IOrType<IVerifiableType, IError> contextType);
    }

}
