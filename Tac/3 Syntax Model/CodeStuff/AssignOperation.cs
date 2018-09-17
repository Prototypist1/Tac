using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class AssignOperation : BinaryOperation<ICodeElement, MemberDefinition>
    {
        public AssignOperation(ICodeElement left, MemberDefinition right) : base(left, right)
        {
        }
        
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return left.ReturnType(scope);
        }
    }
}
