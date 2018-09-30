using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    public class AddOperation : BinaryOperation<ICodeElement,ICodeElement>
    {
        public AddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IBox<ITypeDefinition> ReturnType(ScopeTree scope) {
            if (left.ReturnType(scope) == RootScope.NumberType && right.ReturnType(scope) == RootScope.NumberType)
            {
                return scope.GetType(RootScope.NumberType);
            }
            else if (left.ReturnType(scope) == RootScope.StringType && right.ReturnType(scope) == RootScope.StringType)
            {
                return scope.GetType(RootScope.NumberType);
            }
            else
            {
                throw new Exception("add expects string and int");
            }
        }
    }

    public class AddOperationMaker : BinaryOperationMaker<AddOperation>
    {
        public AddOperationMaker(Func<ICodeElement, ICodeElement, AddOperation> make) : base("+", make)
        {
        }
    }
}
