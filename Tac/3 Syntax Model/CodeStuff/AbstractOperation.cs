using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    public interface ICodeElement {
         bool ContainsInTree(ICodeElement element);
    }

    public class NoELement: ICodeElement
    {

    }

    public abstract class BinaryOperation: ICodeElement
    {
        public readonly ICodeElement left;
        public readonly ICodeElement right;

        public BinaryOperation(ICodeElement left, ICodeElement right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }
    
}
