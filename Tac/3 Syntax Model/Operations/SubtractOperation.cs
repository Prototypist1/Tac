using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface ISubtractOperation: IBinaryOperation<ICodeElement, ICodeElement>
    {

    }

    public class WeakSubtractOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "-";

        public WeakSubtractOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
        {
            return new NumberType();
        }
    }
    
    public class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation>
    {
        public SubtractOperationMaker() : base(WeakSubtractOperation.Identifier, (l,r)=>new WeakSubtractOperation(l,r), new Converter())
        {
        }

        private class Converter : IConverter<WeakSubtractOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakSubtractOperation co)
            {
                return context.SubtractOperation(co);
            }
        }

    }
}
