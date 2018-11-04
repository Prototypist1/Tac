using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    internal class LessThenSymbols : ISymbols
    {
        public string Symbols => "<?";
    }
    
    internal class WeakLessThanOperation : BinaryOperation<ICodeElement, ICodeElement>, ILessThanOperation
    {
        public const string Identifier = "<?";

        public WeakLessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.LessThanOperation(this);
        }
        
        public override IVarifiableType Returns()
        {
            return new BooleanType();
        }
    }

    internal class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation>
    {
        public LessThanOperationMaker() : base(new LessThenSymbols(), (l,r)=>new WeakLessThanOperation(l,r))
        {
        }
    }
}
