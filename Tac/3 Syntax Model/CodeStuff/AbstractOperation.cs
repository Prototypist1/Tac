using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{

    public abstract class BinaryOperation: ICodeElement
    {
        public readonly ICodeElement left;
        public readonly ICodeElement right;

        public BinaryOperation(ICodeElement left, ICodeElement right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public bool ContainsInTree(ICodeElement element) {
            if (element.Equals(this))
            {
                return true;
            }
            else {
                return left.ContainsInTree(element) || right.ContainsInTree(element);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryOperation operation &&
                   EqualityComparer<ICodeElement>.Default.Equals(left, operation.left) &&
                   EqualityComparer<ICodeElement>.Default.Equals(right, operation.right);
        }

        public override int GetHashCode()
        {
            var hashCode = -124503083;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICodeElement>.Default.GetHashCode(left);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICodeElement>.Default.GetHashCode(right);
            return hashCode;
        }
    }
    
}
