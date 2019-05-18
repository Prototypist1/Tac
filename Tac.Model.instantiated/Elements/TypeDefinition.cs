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

    public class InterfaceType : IInterfaceType, IInterfaceTypeBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();

        private InterfaceType() { }

        public void Build(IFinalizedScope scope)
        {
            buildableScope.Set(scope);
        }

        public IFinalizedScope Scope => buildableScope.Get();
        
        public static (IInterfaceType, IInterfaceTypeBuilder) Create()
        {
            var res = new InterfaceType();
            return (res, res);
        }

        public static IInterfaceType CreateAndBuild(IFinalizedScope scope) {
            var (x, y) = Create();
            y.Build(scope);
            return x;
        }

        public IVerifiableType Returns()
        {
            return this;
        }
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.TypeDefinition(this);
        }
    }

    public interface IInterfaceTypeBuilder
    {
        void Build(IFinalizedScope scope);
    }

    public struct TypeOr : ITypeOr
    {
        public TypeOr(IVerifiableType left, IVerifiableType right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public IVerifiableType Left { get; }

        public IVerifiableType Right { get; }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            return Left.TheyAreUs(they, noTagBacks) || Left.TheyAreUs(they, noTagBacks);
        }

        public bool WeAreThem(IVerifiableType them,bool noTagBacks)
        {
            return Left.WeAreThem(them, noTagBacks) && Left.WeAreThem(them, noTagBacks);
        }

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

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            return Left.TheyAreUs(they, noTagBacks) && Left.TheyAreUs(they, noTagBacks);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            return Left.WeAreThem(them, noTagBacks) || Left.WeAreThem(them, noTagBacks);
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
    public class ObjectType : IObjectType
    {
        public IReadOnlyList<IMemberDefinition> Members { get; }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            if (they is IObjectType otherObject)
            {
                // they have all our members
                return Members.All(member =>otherObject.Members.Any(otherMember => otherMember.TheyAreUs(member,false) ));
            }

            if (noTagBacks)
            {
                return false;
            }

            return they.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            if (them is IObjectType otherObject)
            {
                // we have all their members
                return otherObject.Members.All(otherMember => Members.Any(member => otherMember.WeAreThem(member, false)));
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }
    }
    public class ModuleType : IModuleType {

        public IReadOnlyList<IMemberDefinition> Members { get; }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            if (they is IModuleType otherModule)
            {
                // they have all our members
                return Members.All(member => otherModule.Members.Any(otherMember => otherMember.TheyAreUs(member, false)));
            }

            if (noTagBacks)
            {
                return false;
            }

            return they.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            if (them is IModuleType otherModule)
            {
                // we have all their members
                return otherModule.Members.All(otherMember => Members.Any(member => otherMember.WeAreThem(member, false)));
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
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

        public static IVerifiableType CreateAndBuild(IKey key) {
            var (x, y) = Create();
            y.Build(key);
            return x;
        }

        public void Build(IKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is GemericTypeParameterPlacholder placholder &&
                   EqualityComparer<IKey>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
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
    
    public class GenericMethodType : IGenericMethodType, IGenericMethodTypeBuilder
    {
        // TODO should be broken apart, buildable
        private readonly Buildable<IVerifiableType[]> buildTypeParameterDefinitions = new Buildable<IVerifiableType[]>();

        public IReadOnlyList<IKey> TypeParameterKeys => buildTypeParameterDefinitions.Get().OfType<GemericTypeParameterPlacholder>().Select(x => x.Key).ToArray();


        private GenericMethodType() { }

        public void Build(IVerifiableType inputType, IVerifiableType ouputType)
        {
            buildTypeParameterDefinitions.Set(new[] { inputType, ouputType });
        }

        public static (IGenericMethodType, IGenericMethodTypeBuilder) Create()
        {
            var res = new GenericMethodType();
            return (res, res);
        }
    }

    // TODO struct?
    // no, anything that is built can not be a struct
    public class MethodType : IMethodType, IMethodTypeBuilder
    {
        private MethodType() { }

        public void Build(IVerifiableType inputType, IVerifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }
        
        public static (IMethodType, IMethodTypeBuilder) Create()
        {
            var res = new MethodType();
            return (res, res);
        }

        public static IMethodType CreateAndBuild(IVerifiableType inputType, IVerifiableType outputType) {
            var (x, y) = Create();
            y.Build(inputType, outputType);
            return x;
        }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            if (they is IMethodType method) {
                return InputType.TheyAreUs(method.InputType, false) && OutputType.WeAreThem(method.OutputType, false);
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            if (them is IMethodType method)
            {
                return InputType.WeAreThem(method.InputType, false) && OutputType.TheyAreUs(method.OutputType, false);
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }

        public IVerifiableType InputType { get; private set; }
        public IVerifiableType OutputType { get; private set; }
    }
    
    public interface IMethodTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType);
    }

    // TODO struct?
    // no, anything that is built can not be a struct
    //
    // TODO TODO ok so I need this not to exist it is just a method ...
    public class ImplementationType : IImplementationType, IMethodType, IImplementationTypeBuilder
    {
        private ImplementationType() { }

        public void Build(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }
        
        public static (IImplementationType, IImplementationTypeBuilder) Create()
        {
            var res = new ImplementationType();
            return (res, res);
        }

        public bool TheyAreUs(IVerifiableType they, bool noTagBacks)
        {
            if (they is IMethodType method)
            {
                return (this as IMethodType).InputType.TheyAreUs(method.InputType, false) && (this as IMethodType).OutputType.WeAreThem(method.OutputType, false);
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }

        public bool WeAreThem(IVerifiableType them, bool noTagBacks)
        {
            if (them is IMethodType method)
            {
                return (this as IMethodType).InputType.WeAreThem(method.InputType, false) && (this as IMethodType).OutputType.TheyAreUs(method.OutputType, false);
            }

            if (noTagBacks)
            {
                return false;
            }

            return this.WeAreThem(this, true);
        }

        IVerifiableType IImplementationType.InputType { get; private set; }
        IVerifiableType IImplementationType.OutputType { get; private set; }
        IVerifiableType IMethodType.InputType { get; private set; }
        IVerifiableType IMethodType.OutputType { get; private set; }
        public IVerifiableType ContextType { get; private set; }
    }

    public class GenericImplementationType : IGenericImplementationType, IGenericImplementationBuilder
    {
        // TODO should be broken apart, buildable
        private readonly Buildable<IVerifiableType[]> buildTypeParameterDefinitions = new Buildable<IVerifiableType[]>();

        public IReadOnlyList<IKey> TypeParameterKeys  => buildTypeParameterDefinitions.Get().OfType<GemericTypeParameterPlacholder>().Select(x => x.Key).ToArray();
        
        private GenericImplementationType() { }

        public void Build(IVerifiableType inputType, IVerifiableType ouputType, IVerifiableType contextType)
        {

            buildTypeParameterDefinitions.Set(new[] { inputType, ouputType, contextType });
        }

        public static (IGenericImplementationType, IGenericImplementationBuilder) Create()
        {
            var res = new GenericImplementationType();
            return (res, res);
        }
    }
    public interface IGenericImplementationBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType ouputType, IVerifiableType contextType);
    }

    public interface IImplementationTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType, IVerifiableType contextType);
    }

}
