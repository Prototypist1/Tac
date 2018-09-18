using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // TODO split scopes out in to socpes an scope builders
    public interface IScope
    {
        IReadOnlyList<MemberDefinition> Members { get; }

        bool TryGetType(NameKey name, out ITypeDefinition type);
        bool TryGetGenericType(NameKey name, IEnumerable<ITypeDefinition> genericTypeParameters, out TypeDefinition typeDefinition);
        bool TryGetMember(NameKey name, bool staticOnly, out MemberDefinition member);
    }
}