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

        public override IBox<ITypeDefinition> ReturnType() {
            if (left.ReturnType() == RootScope.NumberType && right.ReturnType() == RootScope.NumberType)
            {
                return scope.GetTypeOrThrow(RootScope.NumberType);
            }
            else if (left.ReturnType() == RootScope.StringType || right.ReturnType() == RootScope.StringType)
            {
                return scope.GetTypeOrThrow(RootScope.StringType);
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
