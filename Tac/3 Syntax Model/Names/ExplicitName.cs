using System;

namespace Tac.Semantic_Model.Names
{
    public sealed class ExplicitName: AbstractName
    {
        public ExplicitName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }
    }
}
