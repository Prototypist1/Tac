﻿using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class SubtractOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public SubtractOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.NumberType);
        }
    }
    
    public class SubtractOperationMaker : BinaryOperationMaker<SubtractOperation>
    {
        public SubtractOperationMaker(Func<ICodeElement, ICodeElement, SubtractOperation> make, IElementBuilders elementBuilders) : base("-", make, elementBuilders)
        {
        }
    }
}