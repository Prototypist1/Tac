namespace Tac.Model.Elements
{
    public interface IGenericTypeDefinition : ICodeElement
    {
        IFinalizedScope Scope { get; }
        IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public interface IGenericTypeParameterDefinition {
        IKey Key { get; }
    }

}
