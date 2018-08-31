using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    // why is this not generic??
    // this things that use or type should just be two classes
    public class AssignOperation : BinaryOperation<ICodeElement, ICodeElement>, IScoped<AssignmentScope>
    {
        public AssignOperation(ICodeElement left, Referance right) : this(left, (ICodeElement)right) { }

        public AssignOperation(ICodeElement left, MemberDefinition right) : this(left, (ICodeElement)right) { }

        private AssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
            // TODO AssignmentScope needs to know the type of LHS so it can use it to fill var
            Scope = new AssignmentScope();
        }

        public AssignmentScope Scope { get; } 

        public override bool Equals(object obj) => obj is AssignOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope) => left.ReturnType(scope);
    }
}
