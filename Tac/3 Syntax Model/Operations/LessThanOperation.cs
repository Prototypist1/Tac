using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public interface ILessThanOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    public class WeakLessThanOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "<?";

        public WeakLessThanOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.BooleanType();
        }
    }
    
    public class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation>
    {
        public LessThanOperationMaker(BinaryOperation.Make<WeakLessThanOperation> make) : base(WeakLessThanOperation.Identifier, make)
        {
        }
    }
}
