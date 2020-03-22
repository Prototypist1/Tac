using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class ElseOperation : IElseOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<OrType<ICodeElement,IError>> buildableLeft = new Buildable<OrType<ICodeElement, IError>>();
        private readonly Buildable<OrType<ICodeElement, IError>> buildableRight = new Buildable<OrType<ICodeElement, IError>>();

        public void Build(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public OrType<ICodeElement, IError> Left => buildableLeft.Get();
        public OrType<ICodeElement, IError> Right => buildableRight.Get();
        public IReadOnlyList<OrType<ICodeElement, IError>> Operands => new[] { Left, Right };

        private ElseOperation() { }

        public static (IElseOperation, IBinaryOperationBuilder) Create()
        {
            var res = new ElseOperation();
            return (res, res);
        }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.ElseOperation(this);
        }

        public OrType< IVerifiableType,IError> Returns()
        {
            return new OrType<IVerifiableType, IError>(new BooleanType());
        }
        
        public static IElseOperation CreateAndBuild(OrType<ICodeElement, IError> left, OrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }
}
