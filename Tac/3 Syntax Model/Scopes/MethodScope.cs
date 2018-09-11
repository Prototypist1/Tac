﻿namespace Tac.Semantic_Model
{
    public class MethodScope : LocalStaticScope
    {
        public override bool Equals(object obj) => obj is MethodScope && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public bool TryAddParameter(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }
    
}