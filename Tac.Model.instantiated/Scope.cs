using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class Scope : IFinalizedScope
    {
        private readonly IFinalizedScope parent;

        private class IsStatic {
            public IsStatic(IMemberDefinition value, bool @static)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
                Static = @static;
            }

            public IMemberDefinition Value { get; }
            public bool Static { get; } 
        }

        private readonly IReadOnlyDictionary<IKey, IsStatic> members = new ConcurrentDictionary<IKey, IsStatic>();

        public Scope()
        {
        }

        public Scope(IFinalizedScope parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public IEnumerable<IKey> MemberKeys
        {
            get => members.Keys;
            
        }

        public bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition box) {
            box = default;
            if (!members.TryGetValue(name, out var isStatic)) { return false; }
            if (!isStatic.Static && staticOnly) { return false; }
            box = isStatic.Value;
            return true;
         }
        
        public bool TryGetParent(out IFinalizedScope res)
        {
            res = parent;
            return parent == null;
        }

        public bool TryGetType(IKey name, out IVarifiableType type)
        {
            // ugh generic types 
        }
    }
}
