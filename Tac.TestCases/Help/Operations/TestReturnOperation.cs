using System;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestReturnOperation : IReturnOperation
    {
        public TestReturnOperation(ICodeElement result)
        {
            Result = result;
        }

        public ICodeElement Result { get; set; }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ReturnOperation(this);
        }

        public IVarifiableType Returns()
        {
            return new TestEmptyType();
        }
    }
}
