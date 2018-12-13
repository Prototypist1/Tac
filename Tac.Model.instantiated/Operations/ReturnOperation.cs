using System;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ReturnOperation : IReturnOperation
    {
        public ReturnOperation(ICodeElement result)
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
            return new EmptyType();
        }
    }
}
