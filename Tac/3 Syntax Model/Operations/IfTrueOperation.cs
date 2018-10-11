using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class IfTrueOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        // right should have more validation
        public IfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return elementBuilders.BooleanType();
        }
    }

    public class IfTrueOperationMaker : BinaryOperationMaker<IfTrueOperation>
    {
        public IfTrueOperationMaker(Func<ICodeElement, ICodeElement, IfTrueOperation> make) : base("if", make)
        {
        }
    }
}
