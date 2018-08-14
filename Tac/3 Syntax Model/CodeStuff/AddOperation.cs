using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AddOperation : BinaryOperation
    {
        public AddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }

    public class SubtractOperation : BinaryOperation
    {
        public SubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }

    public class MultiplyOperation : BinaryOperation
    {
        public MultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}
