using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class LastCallOperation : ILastCallOperation
    {
        public LastCallOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }
        public ICodeElement[] Operands => new[] { Left, Right };

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.LastCallOperation(this);
        }

        public IVarifiableType Returns()
        {
            return Left.Returns();
        }
    }
}
