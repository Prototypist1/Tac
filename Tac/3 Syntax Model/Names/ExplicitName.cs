using System;
using System.Collections.Generic;

namespace Tac.Semantic_Model.Names
{
    public sealed class ExplicitName: AbstractName
    {
        public ExplicitName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is ExplicitName name &&
                   Name == name.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }
}
