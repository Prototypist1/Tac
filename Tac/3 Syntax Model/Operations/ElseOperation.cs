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
    public class WeakElseOperation : BinaryOperation<ICodeElement, ICodeElement>, IElseOperation
    {

        public const string Identifier = "else";

        // right should have more validation
        public WeakElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
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
