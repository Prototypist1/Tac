using System;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestNextCallOperation : INextCallOperation
    {
        public TestNextCallOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }
        public ICodeElement[] Operands=> new[] { Left, Right };


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.NextCallOperation(this);
        }

        public IVarifiableType Returns()
        {
            return Right.Returns();
        }
    }
}
