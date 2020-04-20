using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class AssignOperation : IAssignOperation, IBinaryOperationBuilder
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
        public IReadOnlyList< ICodeElement> Operands => new[] { Left, Right };

        private AssignOperation() { }

        public static (IAssignOperation, IBinaryOperationBuilder) Create()
        {
            var res = new AssignOperation();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
                  where TBacking : IBacking
        {
            return context.AssignOperation(this);
        }

        public IVerifiableType Returns()
        {
            return Left.Returns();
        }

        public static IAssignOperation CreateAndBuild(ICodeElement left, ICodeElement right) {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

}
