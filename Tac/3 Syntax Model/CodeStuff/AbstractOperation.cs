using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    public abstract class CodeElement {
        public abstract bool ContainsInTree(CodeElement element);
    }

    public class NoELement: CodeElement
    {

    }

    public abstract class BinaryOperation: CodeElement
    {
        public readonly CodeElement left;
        public readonly CodeElement right;

        public BinaryOperation(CodeElement left, CodeElement right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }
    
}
