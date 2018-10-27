using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IIfOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }


    public class WeakIfTrueOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "if";

        // right should have more validation
        public WeakIfTrueOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.BooleanType();
        }
    }

    public class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation>
    {
        public IfTrueOperationMaker(BinaryOperation.Make<WeakIfTrueOperation> make) : base(WeakIfTrueOperation.Identifier, make,new IfConverter())
        {
        }
        
        private class IfConverter : IConverter<WeakIfTrueOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakIfTrueOperation co)
            {
                return context.IfTrueOperation(co);
            }
        }
    }

}
