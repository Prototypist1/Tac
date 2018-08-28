using System.Collections.Generic;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    public class StaticScope : Scope {
        public StaticScope(IScope enclosingScope) : base(enclosingScope)
        {
        }

        public override bool Equals(object obj) => obj is StaticScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddStaticMember(MemberDefinition definition)
        {
            if (definition.IsStatic == false) {
                throw new System.Exception("can't add a none static member");
            }

            return TryAdd(DefintionLifetime.Static, definition);
        }
        
        public bool TryAddStaticType(TypeDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }
    }
    
}