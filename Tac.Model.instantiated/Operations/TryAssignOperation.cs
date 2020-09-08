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
        private readonly Buildable<IFinalizedScope> buildableScope = new Buildable<IFinalizedScope>();

        public void Build(ICodeElement left, ICodeElement right, ICodeElement body, IFinalizedScope scope)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
            buildableBody.Set(body);
            buildableScope.Set(scope);
        }

        public ICodeElement Left => buildableLeft.Get();
        public ICodeElement Right => buildableRight.Get();
        public ICodeElement Block => buildableBody.Get();
        public IFinalizedScope Scope => buildableScope.Get();
        public IReadOnlyList<ICodeElement> Operands => new[] { Left, Right , Block };


        private TryAssignOperation() { }

        public static (ITryAssignOperation, IIsOperationBuilder) Create()
        {
            var res = new TryAssignOperation();
            return (res, res);
        }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.TryAssignOperation(this);
        }

        public IVerifiableType Returns()
        {
            return Left.Returns();
        }

        public static ITryAssignOperation CreateAndBuild(ICodeElement left, ICodeElement right, ICodeElement body, IFinalizedScope scope)
        {
            var (x, y) = Create();
            y.Build(left, right, body, scope);
            return x;
        }
    }


    public interface IIsOperationBuilder
    {
        void Build(ICodeElement left, ICodeElement right, ICodeElement body, IFinalizedScope scope);
    }

}
