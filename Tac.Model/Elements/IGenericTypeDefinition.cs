namespace Tac.Model.Elements
{

    public interface IGenericType : IVarifiableType
    {
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }

}
