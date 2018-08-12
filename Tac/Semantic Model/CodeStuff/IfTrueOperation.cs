using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class IfTrueOperation : BinaryOperation
    {
        public IfTrueOperation(CodeElement left, CodeElement right) : base(left, right)
        {
        }
    }
}
