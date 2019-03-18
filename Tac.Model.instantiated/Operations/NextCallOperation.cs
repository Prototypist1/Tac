using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class NextCallOperation : INextCallOperation, IBinaryOperationBuilder
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
        public ICodeElement[] Operands=> new[] { Left, Right };

        private NextCallOperation() { }

        public static (INextCallOperation, IBinaryOperationBuilder) Create()
        {
            var res = new NextCallOperation();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.NextCallOperation(this);
        }

        public IVerifiableType Returns()
        {
            return Right.Returns();
        }

        public static INextCallOperation CreateAndBuild(ICodeElement left, ICodeElement right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }
}
