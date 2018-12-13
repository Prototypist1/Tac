using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class InterfaceType : IInterfaceType
    {
        public InterfaceType(IFinalizedScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IFinalizedScope Scope { get; set; }
    }


    public class GemericTypeParameterPlacholder : IVarifiableType
    {
        public GemericTypeParameterPlacholder(IKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }

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

    public class NumberType : INumberType { }
    public class AnyType : IAnyType { }
    public class EmptyType : IEmptyType { }
    public class BooleanType : IBooleanType { }
    public class StringType : IStringType { }
    public class ObjectType : IObjectType { }
    public class ModuleType : IModuleType { }
    public class MethodType : IMethodType
    {
        public MethodType(IVarifiableType inputType, IVarifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IVarifiableType InputType { get; set; }
        public IVarifiableType OutputType{ get; set; }
    }
    public class ImplementationType : IImplementationType
    {
        public ImplementationType(IVarifiableType inputType, IVarifiableType outputType, IVarifiableType contextType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
        }

        public IVarifiableType InputType { get; set; }
        public IVarifiableType OutputType { get; set; }
        public IVarifiableType ContextType { get; set; }
    }
}
