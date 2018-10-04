using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override IBox<ITypeDefinition> ReturnType(IScope scope)
        {
            return right.ReturnType(scope).GetValue().Scope.GetTypeOrThrow(RootScope.methodOutput.Key);
        }
    }

    public class NextCallOperationMaker : BinaryOperationMaker<NextCallOperation>
    {
        public NextCallOperationMaker(Func<ICodeElement, ICodeElement, NextCallOperation> make) : base(">", make)
        {
        }
    }

    public class LastCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override IBox<ITypeDefinition> ReturnType(IScope scope)
        {
            return left.ReturnType(scope).GetValue().Scope.GetTypeOrThrow(RootScope.methodOutput.Key);
        }
    }

    public class LastCallOperationMaker : BinaryOperationMaker<LastCallOperation>
    {
        public LastCallOperationMaker(Func<ICodeElement, ICodeElement, LastCallOperation> make) : base("<", make)
        {
        }
    }
}
