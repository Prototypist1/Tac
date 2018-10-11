using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class MultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public MultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }
    
    public class MultiplyOperationMaker : BinaryOperationMaker<MultiplyOperation>
    {
        public MultiplyOperationMaker(Func<ICodeElement, ICodeElement, MultiplyOperation> make) : base("*", make)
        {
        }
    }
}
