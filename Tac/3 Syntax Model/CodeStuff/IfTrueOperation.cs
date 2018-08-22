using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class IfTrueOperation : BinaryOperation
    {
        // right should have more validation
        public IfTrueOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right)
        {
        }
        
        public override bool Equals(object obj) => obj is IfTrueOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
