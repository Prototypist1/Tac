using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class AssignOperation : BinaryOperation<ICodeElement, IMemberSource>
    {
        public AssignOperation(ICodeElement left, IMemberSource right) : base(left, right)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is AssignOperation other && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return left.ReturnType(scope);
        }
    }
}
