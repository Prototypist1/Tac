using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public abstract class AbstractAssignOperation<TRight> : BinaryOperation<ICodeElement, TRight>, IScoped<AssignmentScope>
        where TRight: class, ICodeElement
    {
        public AbstractAssignOperation(ICodeElement left, TRight right) : base(left, right)
        {
            // TODO AssignmentScope needs to know the type of LHS so it can use it to fill var
            Scope = new AssignmentScope();
        }

        public AssignmentScope Scope { get; } 

        public override bool Equals(object obj) => obj is AbstractAssignOperation<TRight> other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope) => left.ReturnType(scope);
    }

    public sealed class AssignReferanceOperation : AbstractAssignOperation<Referance>
    {
        public AssignReferanceOperation(ICodeElement left, Referance right) : base(left, right) { }

    }

    public sealed class AssignImpliciMemberDefinitionOperation : AbstractAssignOperation<ImplicitMemberDefinition> { 

        public AssignImpliciMemberDefinitionOperation(ICodeElement left, ImplicitMemberDefinition right) : base(left, right) { }
    }


    public sealed class AssignMemberDefinitionOperation : AbstractAssignOperation<MemberDefinition>
    {
        public AssignMemberDefinitionOperation(ICodeElement left, MemberDefinition right) : base(left, right) { }
    }
}
