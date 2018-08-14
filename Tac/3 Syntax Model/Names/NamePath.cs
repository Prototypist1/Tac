using System;

namespace Tac.Semantic_Model.Names
{
    // why does this exist???
    // . is an operation! 
    // it can't be for types tho...
    // what a mess! 
    // TODO 
    // TODO
    public class NamePath {
        public readonly AbstractName[] names;

        public NamePath(AbstractName[] names) => this.names = names ?? throw new ArgumentNullException(nameof(names));
    }
}
