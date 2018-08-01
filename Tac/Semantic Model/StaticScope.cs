using System.Collections.Generic;

namespace Tac.Semantic_Model
{
    public abstract class StaticScope : AbstractScope {

        public bool TryAddStaticMethod(StaticMethodDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static,definition);
        }
        
        public bool TryAddStaticMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }

        public bool TryAddStaticImplementation(ImplementationDefinition definition)
        {
            return TryAdd(DefintionLifetime.Static, definition);
        }
    }

    public abstract class LocalScope : StaticScope
    {
        public bool TryAddLocalMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Local, definition);
        }
    }

    public abstract class InstanceScope : StaticScope
    {
        public bool TryAddInstanceMember(MemberDefinition definition)
        {
            return TryAdd(DefintionLifetime.Instance, definition);
        }
    }
    
}