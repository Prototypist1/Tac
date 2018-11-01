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

    // really an if not
    internal class WeakElseOperation : BinaryOperation<ICodeElement, ICodeElement>, IElseOperation
    {

        public const string Identifier = "else";

        // right should have more validation
        public WeakElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ElseOperation(this);
        }
        
        public override IVarifiableType Returns()
        {
            return new EmptyType();
        }
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation>
    {
        public ElseOperationMaker() : base(WeakElseOperation.Identifier, (l,r)=>new WeakElseOperation(l,r))
        {
        }
    }
    
}
