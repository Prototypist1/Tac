using Prototypist.Toolbox;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class TryAssignOperation : ITryAssignOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<IOrType<ICodeElement, IError>> buildableLeft = new Buildable<IOrType<ICodeElement, IError>>();
        private readonly Buildable<IOrType<ICodeElement, IError>> buildableRight = new Buildable<IOrType<ICodeElement, IError>>();

        public void Build(IOrType<ICodeElement, IError> left, IOrType<ICodeElement, IError> right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public IOrType<ICodeElement, IError> Left => buildableLeft.Get();
        public IOrType<ICodeElement, IError> Right => buildableRight.Get();
        public IReadOnlyList<IOrType<ICodeElement, IError>> Operands => new[] { Left, Right };

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

        public IOrType<IVerifiableType, IError> Returns()
        {
            return Left.TransformAndFlatten(x => x.Returns());
        }

        public static ITryAssignOperation CreateAndBuild(IOrType<ICodeElement, IError> left, IOrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

}
