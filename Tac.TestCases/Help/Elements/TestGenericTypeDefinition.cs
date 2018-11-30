using System;

namespace Tac.Model.Elements
{
    public class TestGenericTypeDefinition : IGenericType
    {
        public TestGenericTypeDefinition(IFinalizedScope scope, IGenericTypeParameterDefinition[] typeParameterDefinitions)
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
        public IKey Key { get; }
    }

}
