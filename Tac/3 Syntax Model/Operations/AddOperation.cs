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

    internal class WeakAddOperation : BinaryOperation<ICodeElement, ICodeElement>, IAddOperation
    {
        public const string Identifier = "+";

        public WeakAddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns() {
            return new NumberType();
        }
    }

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation>
    {
        public AddOperationMaker() : base(WeakAddOperation.Identifier, (l,r)=>new WeakAddOperation(l,r), new AddConverter())
        {
        }

        private class AddConverter : IConverter<WeakAddOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakAddOperation co)
            {
                return context.AddOperation(co);
            }
        }
    }
}
