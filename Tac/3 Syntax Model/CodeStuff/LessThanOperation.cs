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
        
        public override bool Equals(object obj) => obj is LessThanOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
