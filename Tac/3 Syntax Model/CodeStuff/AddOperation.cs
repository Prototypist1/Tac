using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AddOperation : BinaryOperation
    {
        public AddOperation(CodeElement left, CodeElement right) : base(left, right)
        {
        }
    }

    public class SubtractOperation : BinaryOperation
    {
        public SubtractOperation(CodeElement left, CodeElement right) : base(left, right)
        {
        }
    }

    public class MultiplyOperation : BinaryOperation
    {
        public MultiplyOperation(CodeElement left, CodeElement right) : base(left, right)
        {
        }
    }
}
