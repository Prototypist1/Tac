namespace Tac.Model.Operations
{
    public interface IOperation : ICodeElement
    {
        ICodeElement[] Operands { get; }
    }

    public interface IBinaryOperation<TLeft, TRight> : IOperation
        where TLeft : ICodeElement
        where TRight : ICodeElement
    {
        TLeft Left { get; }
        TRight Right { get; }
    }
}
