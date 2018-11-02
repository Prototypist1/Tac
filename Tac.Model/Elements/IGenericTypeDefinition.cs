namespace Tac.Model.Elements
{
    public interface IGenericTypeDefinition : ICodeElement, IVarifiableType
    {
        IFinalizedScope Scope { get; }
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }

}
