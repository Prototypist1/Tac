﻿using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    public class WeakAddOperation : BinaryOperation<IWeakCodeElement,IWeakCodeElement>
    {
        public const string Identifier = "+";

        public WeakAddOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns() {
            return new NumberType();
        }
    }

    public class AddOperationMaker : BinaryOperationMaker<WeakAddOperation>
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
