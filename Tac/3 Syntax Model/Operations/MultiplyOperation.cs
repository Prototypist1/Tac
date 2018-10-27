using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IMultiplyOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    public class WeakMultiplyOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }
    
    public class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(WeakMultiplyOperation.Identifier, (l,r)=>new WeakMultiplyOperation(l,r), new Converter())
        {
        }
        
        private class Converter : IConverter<WeakMultiplyOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakMultiplyOperation co)
            {
                return context.MultiplyOperation(co);
            }
        }
    }
}
