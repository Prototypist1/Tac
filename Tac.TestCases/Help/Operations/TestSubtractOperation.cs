using System;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestSubtractOperation : ISubtractOperation
    {
        public TestSubtractOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.SubtractOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new TestNumberType();
        }
    }
}
