using System.Collections.Generic;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    // this is really a scope builder
    public class StaticScope : Scope {
        public bool TryAddStaticMember(IKey key ,IBox<MemberDefinition> definition)
        {
            return TryAddMember(DefintionLifetime.Static, key, definition);
        }
        
        public bool TryAddStaticType(IKey key, IBox<ITypeDefinition> definition)
        {
            return TryAddType(DefintionLifetime.Static, key, definition);
        }

        public bool TryAddStaticGenericType(IKey key, IBox<GenericTypeDefinition> definition)
        {
            return TryAddGeneric(DefintionLifetime.Static, key, definition);
        }
    }
    
}