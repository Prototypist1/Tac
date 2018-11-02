using System;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestLessThanOperation : ILessThanOperation
    {
        public TestLessThanOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.LessThanOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new TestBooleanType();
        }
    }
}
