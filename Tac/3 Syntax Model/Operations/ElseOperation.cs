using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IElseOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    // really an if not
    public class WeakElseOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {

        public const string Identifier = "else";

        // right should have more validation
        public WeakElseOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.EmptyType();
        }
    }


    public class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation>
    {
        public ElseOperationMaker(BinaryOperation.Make<WeakElseOperation> make) : base(WeakElseOperation.Identifier, make, new ElseConverter())
        {
        }
    }


    public class ElseConverter : IConverter<WeakElseOperation>
    {
        public T Convert<T>(IOpenBoxesContext<T> context, WeakElseOperation co)
        {
            return context.ElseOperation(co);
        }
    }
}
