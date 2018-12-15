using System;
using Prototypist.LeftToRight;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Model.Instantiated
{
    public class PathOperation : IPathOperation, IBinaryOperationBuilder
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
        public ICodeElement[] Operands => new[] { Left, Right };

        private PathOperation() { }

        public static (IPathOperation, IBinaryOperationBuilder) Create()
        {
            var res = new PathOperation();
            return (res, res);
        }

        // this two methods Convert and Returns are interesting
        // they could almost be implemented as extensions
        // I mean they are going to look the same in every set of implemenation of the ICodeElements
        // but... I want to ensure they are there so I include them on the interface

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.PathOperation(this);
        }
        
        public IVarifiableType Returns()
        {
            return Right.Cast<IMemberReferance>();
        }
    }
}
