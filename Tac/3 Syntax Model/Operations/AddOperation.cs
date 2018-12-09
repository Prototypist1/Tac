using System;
using System.Collections.Generic;
using System.Linq;
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

    internal class AddSymbols : ISymbols
    {
        public string Symbols=> "+";
    }

    internal class WeakAddOperation : BinaryOperation<ICodeElement, ICodeElement>, IAddOperation
    {
        public WeakAddOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.AddOperation(this);
        }

        public override IVarifiableType Returns() {
            return new NumberType();
        }
    }

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation>
    {
        public AddOperationMaker() : base(new AddSymbols(),(l,r)=>Possibly.Is(new WeakAddOperation(l,r)))
        {
        }
    }
}
