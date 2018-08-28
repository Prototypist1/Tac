using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class AssignOperation : BinaryOperation<ICodeElement, Referance>, IScoped<AssignmentScope>
    {
        public AssignOperation(ICodeElement left, Referance right, AssignmentScope assignmentScope) : base(left.TakeReferance(), right)
        {
            Scope = assignmentScope ?? throw new ArgumentNullException(nameof(assignmentScope));
        }

        public AssignmentScope Scope { get; }

        public override bool Equals(object obj) => obj is AssignOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope) => left.ReturnType(scope);
    }
}
