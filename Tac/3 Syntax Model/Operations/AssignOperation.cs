using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class AssignOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public AssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return left.ReturnType(scope);
        }
    }
}
