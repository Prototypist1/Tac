using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
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

    public struct NumberType : INumberType { }
    public struct AnyType : IAnyType { }
    public struct EmptyType : IEmptyType { }
    public struct BooleanType : IBooleanType { }
    public struct BlockType : IBlockType { }
    public struct StringType : IStringType { }
    public struct ObjectType : IObjectType { }
    public struct ModuleType : IModuleType { }
    
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

        public IVerifiableType InputType { get; private set; }
        public IVerifiableType OutputType { get; private set; }
    }
    
    public interface IMethodTypeBuilder
    {
        void Build(IVerifiableType inputType, IVerifiableType outputType);
    }

    // TODO struct?
    // no, anything that is built can not be a struct
    public class ImplementationType : IImplementationType, IImplementationTypeBuilder
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
        
        public IVerifiableType InputType { get; private set; }
        public IVerifiableType OutputType { get; private set; }
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
