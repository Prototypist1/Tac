using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class MultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public const string Identifier = "*";

        public MultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }
    
    public class MultiplyOperationMaker : BinaryOperationMaker<MultiplyOperation>
    {
        public MultiplyOperationMaker(BinaryOperation.Make<MultiplyOperation> make) : base(MultiplyOperation.Identifier, make)
        {
        }
    }
}
