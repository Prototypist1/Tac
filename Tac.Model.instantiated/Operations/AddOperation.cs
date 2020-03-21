using Prototypist.Toolbox;
using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class AddOperation : IAddOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<OrType<ICodeElement, IError>> buildableLeft = new Buildable<OrType<ICodeElement, IError>>();
        private readonly Buildable<OrType<ICodeElement, IError>> buildableRight = new Buildable<OrType<ICodeElement, IError>>();
        
        public void Build(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public OrType<ICodeElement, IError> Left => buildableLeft.Get();
        public OrType<ICodeElement, IError> Right => buildableRight.Get();
        public OrType<ICodeElement, IError>[] Operands => new[] { Left, Right };

        private AddOperation() { }

        public static (IAddOperation, IBinaryOperationBuilder) Create()
        {
            var res = new AddOperation();
            return (res, res);
        }
        
        public T Convert<T,TBacking>(IOpenBoxesContext<T,TBacking> context)
            where TBacking:IBacking
        {
            return context.AddOperation(this);
        }

        public IVerifiableType Returns()
        {
            return new NumberType();
        }
        
        public static IAddOperation CreateAndBuild(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

    public interface IBinaryOperationBuilder
    {
        void Build(OrType<ICodeElement,IError> left, OrType<ICodeElement,IError> right);
    }
}
