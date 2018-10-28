namespace Tac.Model.Operations
{
    public interface IBinaryOperation<TLeft, TRight> : ICodeElement
    where TLeft : ICodeElement
    where TRight : ICodeElement
    {
        TLeft Left { get; }
        TLeft Right { get; }
    }
}
