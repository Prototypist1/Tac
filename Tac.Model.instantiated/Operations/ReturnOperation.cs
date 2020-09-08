using Prototypist.Toolbox;
using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ReturnOperation : IReturnOperation, ITrailingOperationBuilder { 
    
        private readonly Buildable<ICodeElement> buildableResult = new Buildable<ICodeElement>();

        public void Build(ICodeElement result)
        {
            buildableResult.Set(result);
        }

        public ICodeElement Result => buildableResult.Get();
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ReturnOperation(this);
        }

        public IVerifiableType Returns()
        {
            return new EmptyType();
        }

        private ReturnOperation() { }

        public static (IReturnOperation, ITrailingOperationBuilder) Create()
        {
            var res = new ReturnOperation();
            return (res, res);
        }

        public static IReturnOperation CreateAndBuild(ICodeElement result) {
            var (x, y) = Create();
            y.Build(result);
            return x;
        }
    }

    public interface ITrailingOperationBuilder
    {
        void Build(ICodeElement result);
    }
}
