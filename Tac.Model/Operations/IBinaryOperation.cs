using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public interface IOperation : ICodeElement
    {
        IReadOnlyList<IOrType<ICodeElement,IError>> Operands { get; }
    }

    public interface IBinaryOperation<TLeft, TRight> : IOperation
        where TLeft : ICodeElement
        where TRight : ICodeElement
    {
        OrType<TLeft,IError> Left { get; }
        OrType<TRight,IError> Right { get; }
    }


    public interface IBinaryTypeOperation<TLeft, TRight> : IOperation
        where TLeft : IVerifiableType
        where TRight : IVerifiableType
    {
        TLeft Left { get; }
        TRight Right { get; }
    }
}
