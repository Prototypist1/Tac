using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AssignOperation : BinaryOperation
    {
        public AssignOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right.TakeReferance())
        {
        }

        public override bool Equals(object obj) => obj is AssignOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
