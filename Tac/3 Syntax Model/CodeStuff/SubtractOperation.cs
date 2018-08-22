﻿using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class SubtractOperation : BinaryOperation
    {
        public SubtractOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right.TakeReferance())
        {
        }
        
        public override bool Equals(object obj) => obj is SubtractOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
