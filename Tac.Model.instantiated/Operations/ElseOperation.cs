using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ElseOperation : IElseOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<ICodeElement> buildableLeft = new Buildable<ICodeElement>();
        private readonly Buildable<ICodeElement> buildableRight = new Buildable<ICodeElement>();

        public void Build(ICodeElement left, ICodeElement right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public ICodeElement Left => buildableLeft.Get();
        public ICodeElement Right => buildableRight.Get();
        public ICodeElement[] Operands => new[] { Left, Right };

        private ElseOperation() { }

        public static (IElseOperation, IBinaryOperationBuilder) Create()
        {
            var res = new ElseOperation();
            return (res, res);
        }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ElseOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new BooleanType();
        }
    }
}
