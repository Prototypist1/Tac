using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    internal interface IOperation
    {
        ICodeElement[] Operands { get; }
    }

    public abstract class BinaryOperation<TLeft,TRight>: ICodeElement , IOperation
        where TLeft: class, ICodeElement
        where TRight: class, ICodeElement
    {
        public readonly TLeft left;
        public readonly TRight right;
        public ICodeElement[] Operands
        {
            get
            {
                return new ICodeElement[] { left, right };
            }
        }

        public BinaryOperation(TLeft left, TRight right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }


        public override bool Equals(object obj)
        {
            return obj is BinaryOperation<TLeft, TRight> operation &&
                   EqualityComparer<ICodeElement>.Default.Equals(left, operation.left) &&
                   EqualityComparer<ICodeElement>.Default.Equals(right, operation.right);
        }

        public override int GetHashCode()
        {
            var hashCode = -124503083;
            hashCode = (hashCode * -1521134295) + EqualityComparer<ICodeElement>.Default.GetHashCode(left);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ICodeElement>.Default.GetHashCode(right);
            return hashCode;
        }

        public abstract ITypeDefinition ReturnType(ScopeStack scope);
    }
}
