using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class SubtractOperation : ISubtractOperation, IBinaryOperationBuilder
    {
        private readonly Buildable<ICodeElement> buildableLeft = new Buildable<ICodeElement>();
        private readonly Buildable<ICodeElement> buildableRight = new Buildable<ICodeElement>();

        public void Build(ICodeElement left, ICodeElement right)
        {
            buildableLeft.Set(left);
            buildableRight.Set(right);
        }

        public ICodeElement Left => buildableLeft.Get();
        public ICodeElement Right => buildableRight.Get();
        public IReadOnlyList<ICodeElement> Operands => new[] { Left, Right };
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.SubtractOperation(this);
        }

        public IVerifiableType Returns()
        {
            return new NumberType();
        }

        private SubtractOperation() { }

        public static (ISubtractOperation, IBinaryOperationBuilder) Create()
        {
            var res = new SubtractOperation();
            return (res, res);
        }
        
        public static ISubtractOperation CreateAndBuild(ICodeElement left, ICodeElement right)
        {
            var (x, y) = Create();
            y.Build(left, right);
            return x;
        }

    }
}
