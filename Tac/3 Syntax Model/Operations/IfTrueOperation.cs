using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{


    public class WeakIfTrueOperation : BinaryOperation<ICodeElement, ICodeElement>, IIfOperation
    {
        public const string Identifier = "if";

        // right should have more validation
        public WeakIfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return new BooleanType();
        }
    }

    public class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation>
    {
        public IfTrueOperationMaker() : base(WeakIfTrueOperation.Identifier, (l,r)=>new WeakIfTrueOperation(l,r),new IfConverter())
        {
        }
        
        private class IfConverter : IConverter<WeakIfTrueOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakIfTrueOperation co)
            {
                return context.IfTrueOperation(co);
            }
        }
    }

}
