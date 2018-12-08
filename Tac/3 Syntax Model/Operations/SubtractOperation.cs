﻿using System;
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
    internal class SubtractSymbols : ISymbols
    {
        public string Symbols => "-";
    }

    internal class WeakSubtractOperation : BinaryOperation<ICodeElement, ICodeElement>, ISubtractOperation
    {
        public WeakSubtractOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.SubtractOperation(this);
        }
        
        public override IVarifiableType Returns()
        {
            return new NumberType();
        }
    }

    internal class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation>
    {
        public SubtractOperationMaker() : base(new SubtractSymbols(), (l,r)=>
            Possibly.Is(
                new WeakSubtractOperation(l,r))){}
    }
}
