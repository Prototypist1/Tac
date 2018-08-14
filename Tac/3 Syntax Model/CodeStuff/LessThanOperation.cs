using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    public class LessThanOperation : BinaryOperation
    {
        public LessThanOperation(CodeElement left, CodeElement right) : base(left, right)
        {
        }
    }
}
