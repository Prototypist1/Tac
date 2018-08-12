using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class ObjectDefinition: IScoped<ObjectScope>
    {
        public ObjectDefinition(IReadOnlyList<(Referance<MemberDefinition>, Referance)> initializers) => Initializers = initializers ?? throw new ArgumentNullException(nameof(initializers));

        public IReadOnlyList<(Referance<MemberDefinition>, Referance)> Initializers { get; }
        public ObjectScope Scope { get; } = new ObjectScope();
    }
}
