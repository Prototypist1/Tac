using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class IfTrueOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        // right should have more validation
        public IfTrueOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return RootScope.BooleanType.GetTypeDefinition(scope);
        }
    }
}
