using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public interface ILessThanOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    public class WeakLessThanOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "<?";

        public WeakLessThanOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
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
