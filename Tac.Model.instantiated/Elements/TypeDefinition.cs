using System;
using System.Collections.Generic;
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

        public IVarifiableType Returns()
        {
            return this;
        }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TypeDefinition(this);
        }
    }

    public interface IInterfaceTypeBuilder
    {
        void Build(IFinalizedScope scope);
    }

    public class NumberType : INumberType { }
    public class AnyType : IAnyType { }
    public class EmptyType : IEmptyType { }
    public class BooleanType : IBooleanType { }
    public class BlockType : IBlockType { }
    public class StringType : IStringType { }
    public class ObjectType : IObjectType { }
    public class ModuleType : IModuleType { }
    
    public class GemericTypeParameterPlacholder : IVarifiableType, IGemericTypeParameterPlacholderBuilder
    {
        private GemericTypeParameterPlacholder() { }

        public static (IVarifiableType, IGemericTypeParameterPlacholderBuilder) Create()
        {
            var res = new GemericTypeParameterPlacholder();
            return (res, res);
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
    }

    public interface IGenericMethodTypeBuilder {
        void Build();
    }

    // TODO!
    // how does this work?!
    public class GenericMethodType : IGenericMethodType, IGenericMethodTypeBuilder
    {
        private GenericMethodType() { }

        public void Build()
        {
        }

        public static (IGenericMethodType, IGenericMethodTypeBuilder) Create()
        {
            var res = new GenericMethodType();
            return (res, res);
        }
    }

    public class MethodType : IMethodType, IMethodTypeBuilder
    {
        private MethodType() { }

        public void Build(IVarifiableType inputType, IVarifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }
        
        public static (IMethodType, IMethodTypeBuilder) Create()
        {
            var res = new MethodType();
            return (res, res);
        }
        
        public IVarifiableType InputType { get; private set; }
        public IVarifiableType OutputType { get; private set; }
    }
    
    public interface IMethodTypeBuilder
    {
        void Build(IVarifiableType inputType, IVarifiableType outputType);
    }

    public class ImplementationType : IImplementationType, IImplementationTypeBuilder
    {
        private ImplementationType() { }

        public void Build(IVarifiableType inputType, IVarifiableType outputType, IVarifiableType contextType)
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
        
        public IVarifiableType InputType { get; private set; }
        public IVarifiableType OutputType { get; private set; }
        public IVarifiableType ContextType { get; private set; }
    }

    public interface IImplementationTypeBuilder
    {
        void Build(IVarifiableType inputType, IVarifiableType outputType, IVarifiableType contextType);
    }
}
