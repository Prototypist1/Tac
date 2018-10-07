namespace Tac.Semantic_Model.CodeStuff
{
    public interface ICodeElement {
        IBox<ITypeDefinition> ReturnType(RootScope rootScope);
    }
}
