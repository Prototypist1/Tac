﻿using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    // really an if not
    public class ElseOperation : BinaryOperation<ICodeElement, ICodeElement>
    {

        public const string Identifier = "else";

        // right should have more validation
        public ElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.EmptyType();
        }
    }


    public class ElseOperationMaker : BinaryOperationMaker<ElseOperation>
    {
        public ElseOperationMaker(BinaryOperation.Make<ElseOperation> make) : base(ElseOperation.Identifier, make)
        {
        }
    }
}
