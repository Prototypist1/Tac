using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface ISubtractOperation: IBinaryOperation<ICodeElement, ICodeElement>
    {

    }

    public class WeakSubtractOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "-";

        public WeakSubtractOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }
    
    public class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation>
    {
        public SubtractOperationMaker(BinaryOperation.Make<WeakSubtractOperation> make) : base(WeakSubtractOperation.Identifier, make, new Converter())
        {
        }

        private class Converter : IConverter<WeakSubtractOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakSubtractOperation co)
            {
                return context.SubtractOperation(co);
            }
        }

    }
}
