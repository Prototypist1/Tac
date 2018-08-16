using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class ObjectDefinition: IScoped<ObjectScope>
    {
        public ObjectDefinition(IReadOnlyList<(Referance<MemberDefinition>, Referance)> initializers, IScope enclosingScope) {
            Initializers = initializers ?? throw new ArgumentNullException(nameof(initializers));
            Scope = new ObjectScope(enclosingScope);
        }

        public IReadOnlyList<(Referance<MemberDefinition>, Referance)> Initializers { get; }
        public ObjectScope Scope { get; }

        public override bool Equals(object obj)
        {
            return obj is ObjectDefinition definition &&
                   Initializers.SequenceEqual(definition.Initializers) &&
                   EqualityComparer<ObjectScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = 1953067843;
            hashCode = hashCode * -1521134295 + Initializers.Sum(x=>x.GetHashCode());
            hashCode = hashCode * -1521134295 + EqualityComparer<ObjectScope>.Default.GetHashCode(Scope);
            return hashCode;
        }
    }
}
