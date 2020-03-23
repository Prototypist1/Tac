using Prototypist.Toolbox;
using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ReturnOperation : IReturnOperation, ITrailingOperationBuilder
    {
        private readonly Buildable<OrType<ICodeElement, IError>> buildableResult = new Buildable<OrType<ICodeElement, IError>>();

        public void Build(OrType<ICodeElement, IError> result)
        {
            buildableResult.Set(result);
        }

        public OrType<ICodeElement, IError> Result => buildableResult.Get();
        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ReturnOperation(this);
        }

        public OrType<IVerifiableType, IError> Returns()
        {
            return new OrType<IVerifiableType, IError>(new EmptyType());
        }

        private ReturnOperation() { }

        public static (IReturnOperation, ITrailingOperationBuilder) Create()
        {
            var res = new ReturnOperation();
            return (res, res);
        }

        public static IReturnOperation CreateAndBuild(OrType<ICodeElement, IError> result) {
            var (x, y) = Create();
            y.Build(result);
            return x;
        }
    }

    public interface ITrailingOperationBuilder
    {
        void Build(OrType< ICodeElement,IError> result);
    }
}
