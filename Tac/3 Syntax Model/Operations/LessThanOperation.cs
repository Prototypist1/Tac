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

    public class WeakLessThanOperation : BinaryOperation<ICodeElement, ICodeElement>, ILessThanOperation
    {
        public const string Identifier = "<?";

        public WeakLessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return new BooleanType();
        }
    }
    
    public class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation>
    {
        public LessThanOperationMaker() : base(WeakLessThanOperation.Identifier, (l,r)=>new WeakLessThanOperation(l,r), new Converter())
        {
        }
        
        private class Converter : IConverter<WeakLessThanOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakLessThanOperation co)
            {
                return context.LessThanOperation(co);
            }
        }
    }
}
