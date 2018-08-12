using System.Collections.Generic;

namespace Tac.Semantic_Model
{

    public class StaticScope : Scope {
        
        public bool TryAddStaticMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }
        
        public bool TryAddStaticType(TypeDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }
    }
    
}