using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class SubtractOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public SubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }
    
    public class SubtractOperationMaker : BinaryOperationMaker<SubtractOperation>
    {
        public SubtractOperationMaker(BinaryOperation.Make<SubtractOperation> make) : base("-", make)
        {
        }
    }
}
