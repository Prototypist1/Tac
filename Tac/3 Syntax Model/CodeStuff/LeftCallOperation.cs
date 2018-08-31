using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override bool Equals(object obj) => obj is NextCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            if (right is Referance referance &&
                scope.TryGet(referance.key.names, out var member) &&
                member is MemberDefinition memberDefinition)
            {
                if (memberDefinition.Type.Is(out ImplicitTypeReferance implicitTypeReferance) && scope.TryGet(implicitTypeReferance, out var typeDefinition1)){
                    return typeDefinition1.ReturnType(scope);
                }
                if (memberDefinition.Type.Is(out Referance innerReferance) && scope.TryGet(implicitTypeReferance, out var typeDefinition2))
                {
                    return typeDefinition2.ReturnType(scope);
                }

                throw new Exception("could not find the right type");

            }
            else if (right is MethodDefinition methodDefinition)
            {
                return methodDefinition.ReturnType(scope);
            }
            return right.ReturnType(scope);

        }
    }
}
