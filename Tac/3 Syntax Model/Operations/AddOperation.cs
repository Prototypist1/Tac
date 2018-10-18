using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    public class AddOperation : BinaryOperation<ICodeElement,ICodeElement>
    {
        public AddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders) {
            if (left.Returns(elementBuilders) is NumberType && right.Returns(elementBuilders) is NumberType)
            {
                return elementBuilders.NumberType();
            }
            else if (left.Returns(elementBuilders) is StringType || right.Returns(elementBuilders) is StringType)
            {
                return elementBuilders.StringType();
            }
            else
            {
                throw new Exception("add expects string and int");
            }
        }
    }

    public class AddOperationMaker : BinaryOperationMaker<AddOperation>
    {
        public AddOperationMaker(BinaryOperation.Make<AddOperation> make) : base("+", make)
        {
        }
    }
}
