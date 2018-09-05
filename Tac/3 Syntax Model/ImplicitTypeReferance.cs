using System;

namespace Tac.Semantic_Model
{
    public class ImplicitTypeReferance : ITypeSource
    {
        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public ITypeDefinition<IScope> ReturnType(IScope scope) {
            return RootScope.TypeType;
        }
    }
}
