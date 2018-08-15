namespace Tac.Semantic_Model.CodeStuff
{
    public class NoElement : ICodeElement
    {
        public bool ContainsInTree(ICodeElement element) => element.Equals(this);
    }
    
}
