using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model.CodeStuff
{
    public class LessThanOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.BooleanType);
        }
    }
}
