using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IMemberSource : ICodeElement {
        MemberDefinition GetMemberDefinition(ScopeStack scopeStack);
    }

    public class AssignOperation : BinaryOperation<ICodeElement, IMemberSource>, IScoped<AssignmentScope>
    {
        public AssignOperation(ICodeElement left, IMemberSource right) : base(left, right)
        {
            Scope = new AssignmentScope(left);
        }

        public AssignmentScope Scope { get; } 

        public override bool Equals(object obj) => obj is AssignOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition<IScope> ReturnType(ScopeStack scope) => left.ReturnType(scope);
    }
}
