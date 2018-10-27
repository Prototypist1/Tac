namespace Tac.Semantic_Model.CodeStuff
{
    public interface IBinaryOperation<TLeft, TRight> : ICodeElement
    where TLeft : ICodeElement
    where TRight : ICodeElement
    {
        TLeft Left { get; }
        TLeft Right { get; }
    }
}
