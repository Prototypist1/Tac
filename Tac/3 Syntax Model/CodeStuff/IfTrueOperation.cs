using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class IfTrueOperation : BinaryOperation
    {
        public IfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }

    public class ElseOperation : BinaryOperation
    {
        public ElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}
