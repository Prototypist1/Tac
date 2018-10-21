using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public class LessThanOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public const string Identifier = "<?";

        public LessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.BooleanType();
        }
    }
    
    public class LessThanOperationMaker : BinaryOperationMaker<LessThanOperation>
    {
        public LessThanOperationMaker(BinaryOperation.Make<LessThanOperation> make) : base(LessThanOperation.Identifier, make)
        {
        }
    }
}
