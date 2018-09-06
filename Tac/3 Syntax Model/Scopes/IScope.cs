using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IScope
    {
        bool TryGetType(AbstractName name, out TypeDefinition type);
        bool TryGetMember(AbstractName name, bool staticOnly, out MemberDefinition member);
        // this is a weird API, can I just get away with taking in a scopeStack and return a ITypeDefinition???
        bool TryGet(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition<IScope>> item);
    }
}