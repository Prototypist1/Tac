using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AssignOperation : BinaryOperation
    {
        public AssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }

    public class IsStaticOperation : BinaryOperation
    {
        public IsStaticOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
    }
}
