using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class IfOperation : IIfOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<IOrType<ICodeElement,IError>> buildableLeft = new Buildable<IOrType<ICodeElement, IError>>();
        private readonly Buildable<IOrType<ICodeElement, IError>> buildableRight = new Buildable<IOrType<ICodeElement, IError>>();

        public void Build(IOrType<ICodeElement, IError> left, IOrType<ICodeElement, IError> right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public IOrType<ICodeElement, IError> Left => buildableLeft.Get();
        public IOrType<ICodeElement, IError> Right => buildableRight.Get();
        public IReadOnlyList<IOrType<ICodeElement, IError>> Operands => new[] { Left, Right };

        private IfOperation() { }

        public static (IIfOperation, IBinaryOperationBuilder) Create()
        {
            var res = new IfOperation();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.IfTrueOperation(this);
        }

        public IOrType<IVerifiableType, IError> Returns()
        {
            return OrType.Make<IVerifiableType, IError>(new BooleanType());
        }
        
        public static IIfOperation CreateAndBuild(IOrType<ICodeElement, IError> left, IOrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

}
