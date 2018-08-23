using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AssignOperation : BinaryOperation<ICodeElement, ReferanceOrMemberDef>
    {
        public AssignOperation(ICodeElement left, ReferanceOrMemberDef right) : base(left.TakeReferance(), right)
        {
        }

        public override bool Equals(object obj) => obj is AssignOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope) => left.ReturnType(scope);
    }
}
