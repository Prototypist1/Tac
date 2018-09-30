using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class AssignOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public AssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IBox<ITypeDefinition> ReturnType(ScopeTree scope)
        {
            return left.ReturnType(scope);
        }
    }


    public class AssignOperationMaker : BinaryOperationMaker<AssignOperation>
    {
        public AssignOperationMaker(Func<ICodeElement, ICodeElement, AssignOperation> make) : base(">", make)
        {
        }
    }
}
