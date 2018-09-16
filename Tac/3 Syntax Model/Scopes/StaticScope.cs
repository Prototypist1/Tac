using System.Collections.Generic;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    // this is really a scope builder
    public class StaticScope : Scope {
        public bool TryAddStaticMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }
        
        public bool TryAddStaticType(NamedTypeDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }

        public bool TryAddStaticGenericType(GenericTypeDefinition definition)
        {
            return TryAddGeneric(DefintionLifetime.Static, definition);
        }
    }
    
}