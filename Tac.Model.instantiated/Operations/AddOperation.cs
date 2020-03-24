using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class AddOperation : IAddOperation, IBinaryOperationBuilder
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

        public IOrType<IVerifiableType,IError> Returns()
        {
            return new OrType<IVerifiableType, IError>(new NumberType());
        }
        
        public static IAddOperation CreateAndBuild(IOrType<ICodeElement, IError> left, IOrType<ICodeElement, IError> right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }
    }

    public interface IBinaryOperationBuilder
    {
        void Build(IOrType<ICodeElement,IError> left, IOrType<ICodeElement,IError> right);
    }
}
