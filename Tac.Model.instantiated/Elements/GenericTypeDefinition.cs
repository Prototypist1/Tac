using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class GenericInterfaceDefinition : IGenericInterfaceDefinition, IGenericInterfaceDefinitionBuilder
    {
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();
        private readonly Buildable<IGenericTypeParameterDefinition[]> buildableTypeParameterDefinitions = new Buildable<IGenericTypeParameterDefinition[]>();

        public GenericInterfaceDefinition()
        {
        }

        public IFinalizedScope Scope { get => buildableScope.Get(); }
        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get => buildableTypeParameterDefinitions.Get(); }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.GenericTypeDefinition(this);
        }

        public IVarifiableType Returns()
        {
            return this;
        }
        
        public void Build(IFinalizedScope scope, IGenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            buildableScope.Set(scope);
            buildableTypeParameterDefinitions.Set(typeParameterDefinitions);
        }

        public static (IGenericInterfaceDefinition, IGenericInterfaceDefinitionBuilder) Create()
        {
            var res = new GenericInterfaceDefinition();
            return (res, res);
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

    public interface IGenericInterfaceDefinitionBuilder
    {
        void Build(IFinalizedScope scope, IGenericTypeParameterDefinition[] typeParameterDefinitions);
    }
}
