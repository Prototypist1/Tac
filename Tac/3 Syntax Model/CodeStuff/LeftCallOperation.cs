using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right.TakeReferance())
        {
        }

        public override bool Equals(object obj) => obj is NextCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
