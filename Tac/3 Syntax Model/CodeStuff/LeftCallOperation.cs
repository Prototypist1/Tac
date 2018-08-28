using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left.TakeReferance(), right.TakeReferance())
        {
        }

        public override bool Equals(object obj) => obj is NextCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(IScope scope)
        {
            if (right is Referance referance &&
                scope.TryGet(referance, out var member) &&
                member is MemberDefinition memberDefinition &&
                scope.TryGet(memberDefinition.Type, out var typeDefinition))
            {
                return typeDefinition.ReturnType(scope);
            }
            else if (right is MethodDefinition methodDefinition)
            {
                return methodDefinition.ReturnType(scope);
            }
            return right.ReturnType(scope);

        }
    }
}
