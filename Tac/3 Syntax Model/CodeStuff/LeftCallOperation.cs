using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}
