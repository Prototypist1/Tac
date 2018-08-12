using System;

namespace Tac.Semantic_Model.Names
{
    public class NamePath {
        public readonly AbstractName[] names;

        public NamePath(AbstractName[] names) => this.names = names ?? throw new ArgumentNullException(nameof(names));
    }
}
