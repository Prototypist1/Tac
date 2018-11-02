namespace Tac.Model.Operations
{
    public class TestBinaryOperation<TLeft, TRight> : IBinaryOperation<TLeft, TRight>
        where TLeft : ICodeElement
        where TRight : ICodeElement
    {
        public TLeft Left { get; }
        public TLeft Right { get; }
    }
}
