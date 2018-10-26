using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{


    public interface IAddOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {

    }

    public class WeakAddOperation : BinaryOperation<IWeakCodeElement,IWeakCodeElement>
    {
        public const string Identifier = "+";

        public WeakAddOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders) {
            return elementBuilders.NumberType();
        }
    }

    public class AddOperationMaker : BinaryOperationMaker<WeakAddOperation>
    {
        public AddOperationMaker(BinaryOperation.Make<WeakAddOperation> make) : base(WeakAddOperation.Identifier, make)
        {
        }
    }
}
