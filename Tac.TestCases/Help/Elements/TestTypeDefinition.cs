using System;
using System.Collections.Generic;
using Tac.Model;

namespace Tac.Model.Elements
{
    public class TestInterfaceType : IInterfaceType
    {
        public TestInterfaceType(IFinalizedScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IFinalizedScope Scope { get; set; }
    }


    public class TestGemericTypeParameterPlacholder : IVarifiableType
    {
        public TestGemericTypeParameterPlacholder(IKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }

        public override bool Equals(object obj)
        {
            return obj is TestGemericTypeParameterPlacholder placholder &&
                   EqualityComparer<IKey>.Default.Equals(Key, placholder.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    public class TestNumberType : INumberType { }
    public class TestAnyType : IAnyType { }
    public class TestEmptyType : IEmptyType { }
    public class TestBooleanType : IBooleanType { }
    public class TestStringType : IStringType { }
    public class TestObjectType : IObjectType { }
    public class TestModuleType : IModuleType { }
    public class TestMethodType : IMethodType
    {
        public TestMethodType(IVarifiableType inputType, IVarifiableType outputType)
        {
            InputType = inputType ?? throw new ArgumentNullException(nameof(inputType));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
        }

        public IVarifiableType InputType { get; set; }
        public IVarifiableType OutputType{ get; set; }
    }
    public class TestImplementationType : IImplementationType
    {
        public TestImplementationType(IVarifiableType inputType, IVarifiableType outputType, IVarifiableType contextType)
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
