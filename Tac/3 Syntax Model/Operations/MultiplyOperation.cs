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

    public class WeakMultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>, IMultiplyOperation
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return new NumberType();
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
