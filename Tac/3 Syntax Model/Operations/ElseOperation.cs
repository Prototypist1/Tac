using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    internal class ElseSymbols : ISymbols
    {
        public string Symbols => "else";
    }


    // really an if not
    internal class WeakElseOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement>
    {
        // right should have more validation
        public WeakElseOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ElseOperation(this);
        }
        
        public override IIsPossibly<IVarifiableType> Returns()
        {
            return Possibly.Is(new BooleanType());
        }
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation>
    {
        public ElseOperationMaker() : base(new ElseSymbols(), (l,r)=>Possibly.Is(new WeakElseOperation(l,r)))
        {
        }
    }
    
}
