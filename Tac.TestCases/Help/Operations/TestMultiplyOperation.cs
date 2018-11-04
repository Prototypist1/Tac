using System;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestMultiplyOperation : IMultiplyOperation
    {
        public TestMultiplyOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }
        public ICodeElement[] Operands => new[] { Left, Right };

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MultiplyOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new TestNumberType();
        }
    }
}
