using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // TODO split scopes out in to socpes an scope builders
    public interface IScope
    {
        bool TryGetType(ExplicitTypeName name, out ITypeDefinition type);
        bool TryGetGenericType(ExplicitTypeName name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition);
        bool TryGetMember(ExplicitMemberName name, bool staticOnly, out MemberDefinition member);
    }
}