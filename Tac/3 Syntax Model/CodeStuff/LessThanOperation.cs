using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    public class LessThanOperation : BinaryOperation
    {
        public LessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}
