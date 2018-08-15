using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AddOperation : BinaryOperation
    {
        public AddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override bool Equals(object obj) => obj is AddOperation other &&  base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}
