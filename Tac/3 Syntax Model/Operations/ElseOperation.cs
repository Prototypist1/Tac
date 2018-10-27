using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    // really an if not
    public class WeakElseOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {

        public const string Identifier = "else";

        // right should have more validation
        public WeakElseOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
        {
            return new EmptyType();
        }
    }


    public class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation>
    {
        public ElseOperationMaker() : base(WeakElseOperation.Identifier, (l,r)=>new WeakElseOperation(l,r), new ElseConverter())
        {
        }
    }


    public class ElseConverter : IConverter<WeakElseOperation>
    {
        public T Convert<T>(IOpenBoxesContext<T> context, WeakElseOperation co)
        {
            return context.ElseOperation(co);
        }
    }
}
