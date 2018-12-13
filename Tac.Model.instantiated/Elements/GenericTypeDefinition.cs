using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class GenericInterfaceDefinition : IGenericInterfaceDefinition
    {
        public GenericInterfaceDefinition(IFinalizedScope scope, IGenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            Scope = scope;
            TypeParameterDefinitions = typeParameterDefinitions;
        }

        public IFinalizedScope Scope { get; set; }
        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.GenericTypeDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
    }

    public class TestGenericTypeParameterDefinition : IGenericTypeParameterDefinition
    {
        public TestGenericTypeParameterDefinition(IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }
    }

}
