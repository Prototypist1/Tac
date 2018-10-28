using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    public class WeakSubtractOperation : BinaryOperation<ICodeElement, ICodeElement>, ISubtractOperation
    {
        public const string Identifier = "-";

        public WeakSubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
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
