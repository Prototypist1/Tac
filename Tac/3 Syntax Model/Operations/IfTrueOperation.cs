using System;
using System.Collections.Generic;
using System.Text;
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

    internal class IfTrueSymbols : ISymbols
    {
        public string Symbols => "then";
    }

    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement>
    {
        // right should have more validation
        public WeakIfTrueOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.IfTrueOperation(this);
        }

        public override IIsPossibly<IVarifiableType> Returns()
        {
            return Possibly.Is(new BooleanType());
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation>
    {
        public IfTrueOperationMaker() : base(new IfTrueSymbols(), (l,r)=> Possibly.Is(new WeakIfTrueOperation(l,r)))
        {
        }
    }

}
