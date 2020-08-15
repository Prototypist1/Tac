using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class TryAssignOperation : ITryAssignOperation, IIsOperationBuilder
    {
        private readonly Buildable<ICodeElement> buildableLeft = new Buildable<ICodeElement>();
        private readonly Buildable<ICodeElement> buildableRight = new Buildable<ICodeElement>();
        private readonly Buildable<ICodeElement> buildableBody = new Buildable<ICodeElement>();

        public void Build(ICodeElement left, ICodeElement right, ICodeElement body)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
            buildableBody.Set(body);
        }

        public ICodeElement Left => buildableLeft.Get();
        public ICodeElement Right => buildableRight.Get();
        public ICodeElement Body => buildableBody.Get();
        public IReadOnlyList<ICodeElement> Operands => new[] { Left, Right , Body};

        private TryAssignOperation() { }

        public static (ITryAssignOperation, IIsOperationBuilder) Create()
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

        public static ITryAssignOperation CreateAndBuild(ICodeElement left, ICodeElement right, ICodeElement body)
        {
            var (x, y) = Create();
            y.Build(left, right, body);
            return x;
        }
    }


    public interface IIsOperationBuilder
    {
        void Build(ICodeElement left, ICodeElement right, ICodeElement body);
    }

}
