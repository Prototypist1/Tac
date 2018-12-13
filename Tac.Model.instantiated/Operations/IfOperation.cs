using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class IfOperation : IIfOperation
    {
        public IfOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }
        public ICodeElement[] Operands => new[] { Left, Right };

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.IfTrueOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new BooleanType();
        }
    }

}
