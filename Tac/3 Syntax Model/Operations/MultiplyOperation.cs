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
    internal class MultiplySymbols : ISymbols
    {
        public string Symbols => "*";
    }

    internal class WeakMultiplyOperation : BinaryOperation<ICodeElement, ICodeElement>, IMultiplyOperation
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MultiplyOperation(this);
        }
        
        public override IVarifiableType Returns()
        {
            return new NumberType();
        }
    }

    internal class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(new MultiplySymbols(), (l,r)=>new WeakMultiplyOperation(l,r))
        {
        }
    }
}
