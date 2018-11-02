namespace Tac.Model.Elements
{
    public class TestGenericTypeDefinition : IGenericTypeDefinition
    {
        public IFinalizedScope Scope { get; }
        public IGenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
    }

    public class TestGenericTypeParameterDefinition : IGenericTypeParameterDefinition
    {
        public IKey Key { get; }
    }

}
