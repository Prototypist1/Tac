using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.instantiated
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
        public ICodeElement[] Operands => new[] { Left, Right };

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
