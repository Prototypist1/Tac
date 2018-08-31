using System;

namespace Tac.Semantic_Model
{
    public class ImplicitTypeReferance 
    {
        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public ITypeDefinition ReturnType(IScope scope) {
            if (scope.TryGet(this, out var referanced) && referanced is ITypeDefinition typeDefinition) {
                return typeDefinition;
            }

            throw new Exception("ITypeDefinition not found");
        }
    }
}
