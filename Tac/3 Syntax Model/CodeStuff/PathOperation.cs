using Prototypist.LeftToRight;
using System;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class PathOperation : BinaryOperation<ICodeElement, MemberDefinition>
    {
        public PathOperation(ICodeElement left, MemberDefinition right) : base(left, right)
        {
        }

        // TODO I am not so sure everything is value types...
        // what is that getting me?
        public override bool Equals(object obj) => obj is AddOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();

        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            if (!left.Cast<IScoped>().Scope.TryGetMember(right.Key,false,out _)){
                throw new Exception("Member should be defined");
            }

            // TODO you are HERE!
            // we need to return in the scope of the LHS
            return right.ReturnType();
        }
    }
}
