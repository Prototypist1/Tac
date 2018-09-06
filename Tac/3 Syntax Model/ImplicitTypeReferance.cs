using System;
using System.Linq;

namespace Tac.Semantic_Model
{
    public class ImplicitTypeReferance : ITypeSource
    {
        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public ITypeDefinition<IScope> ReturnType(IScope scope) {
            return RootScope.TypeType;
        }

        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition<IScope> typeDefinition) {
            if (scope.Scopes.First().TryGet(this, out var item)) {
                typeDefinition = item(scope);
                return true;
            }
            typeDefinition = default;
            return false;
        }
    }
}
