using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class TryAssignOperation : ITryAssignOperation, IBinaryOperationBuilder
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
        public IReadOnlyList<ICodeElement> Operands => new[] { Left, Right };

        private TryAssignOperation() { }

        public static (ITryAssignOperation, IBinaryOperationBuilder) Create()
        {
            var res = new TryAssignOperation();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
                  where TBacking : IBacking
        {
            return context.TryAssignOperation(this);
        }

        public IVerifiableType Returns()
        {
            return Left.Returns();
        }

        public static ITryAssignOperation CreateAndBuild(ICodeElement left, ICodeElement right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

}
