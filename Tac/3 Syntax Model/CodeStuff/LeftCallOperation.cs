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
        public override ITypeDefinition ReturnType(IScope scope) {
            if (right is Referance referance && 
                scope.TryGet<MemberDefinition>(referance.key, out var member) &&
                scope.TryGet<TypeDefinition>(member.Type.key, out var typeDefinition)) {
                
            }
            else if (right is MethodDefinition methodDefinition)
            {
                return right.ReturnType(scope);
            }
            throw new Exception("");
        }
    }
}
