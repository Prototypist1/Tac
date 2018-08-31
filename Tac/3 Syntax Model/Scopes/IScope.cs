using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        bool TryGetType(AbstractName name, out TypeDefinition type);
        bool TryGetMember(AbstractName name, bool staticOnly, out MemberDefinition member);
        bool TryGet(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition> item);
    }
}