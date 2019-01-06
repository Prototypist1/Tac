using System;

namespace Tac.Model.Elements
{

    public interface IGenericType : IVarifiableType
    {
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }
    public class GenericTypeParameterDefinition
    {
        public GenericTypeParameterDefinition(IKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }
    }
}
