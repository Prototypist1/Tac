using System;
using Prototypist.LeftToRight;
using Tac.Model.Elements;

namespace Tac.Model.Operations
{
    public class TestPathOperation : IPathOperation
    {
        public TestPathOperation(ICodeElement left, ICodeElement right)
        {
            Left = left;
            Right = right;
        }

        public ICodeElement Left { get; set; }
        public ICodeElement Right { get; set; }
        public ICodeElement[] Operands => new[] { Left, Right };

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
