using System;

namespace Tac.Model.Elements
{
    public class GenericTypeParameter
    {
        public GenericTypeParameter(IVarifiableType type, IGenericTypeParameterDefinition parameter)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public IVarifiableType Type {get; }
        public IGenericTypeParameterDefinition Parameter { get;  }
    }


    public interface IGenericType : IVarifiableType
    {
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }

}
